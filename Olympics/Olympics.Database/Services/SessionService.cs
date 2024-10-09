using AuthentificationServer.Classes;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using System.Net.Http.Json;
using System.Reflection.Metadata.Ecma335;
using SecurityManager = Olympics.Metier.Utils.SecurityManager;

namespace Olympics.Database.Services
{
    public class SessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly ILocalStorageService _localStorage;

        public SessionService(IHttpContextAccessor httpContextAccessor, IDataProtectionProvider dataProtectionProvider, ILocalStorageService localStorage)
        {
            _httpContextAccessor = httpContextAccessor;
            _dataProtectionProvider = dataProtectionProvider;
            _localStorage = localStorage;
        }

        private static bool _isAdmin = false;

        public void SetUserStatus(bool isAdmin)
        {
            _isAdmin = isAdmin;
        }

        public bool GetUserStatus()
        {
            return _isAdmin; // Retourne l'état stocké
        }

        // Appel de cette méthode lors de la déconnexion
        public void ClearUserStatus()
        {
            _isAdmin = false;
        }


        public async Task<bool> ValidateUserSessionAsync()
        {
            try
            {
                // Récupérer le token d'authentification chiffré depuis le localStorage
                string sEncryptedToken = await _localStorage.GetItemAsync<string>(SecurityManager.EncryptAES("Token"));
                string sExpirationDate = await _localStorage.GetItemAsync<string>(SecurityManager.EncryptAES("ExpireDate"));

                if (string.IsNullOrEmpty(sEncryptedToken) || string.IsNullOrEmpty(sExpirationDate))
                {
                    return false;  // Si le token est absent, la session n'est pas valide
                }

                DateTime expirationDate = Convert.ToDateTime(SecurityManager.DecryptAES(sExpirationDate));

                // Vérification de l'expiration du token
                if (expirationDate < DateTime.UtcNow)
                {
                    return false; // Si le token est expiré, la session n'est pas valide
                }
                else if (expirationDate < DateTime.UtcNow.AddMinutes(5))
                {
                    // Si le token expire bientôt, tenter de le renouveler
                    bool renewed = await RenewSessionAsync(sEncryptedToken, expirationDate);
                    return renewed;
                }

                // Déchiffrer le token avec AES
                var decryptedToken = SecurityManager.DecryptAES(sEncryptedToken);

                // Déchiffrer le token pour pouvoir l'envoyer à l'API de validation
                var protector = _dataProtectionProvider.CreateProtector("AuthTokenProtector");
                var token = protector.Unprotect(decryptedToken);

                // Créez le client HTTP
                using var httpClient = new HttpClient();

                // Préparez la requête de validation
                var validateRequest = new ValidateSessionRequest
                {
                    Token = token,
                    Expiration = expirationDate,
                    Ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString()
                };

                // Envoyer la requête POST à l'API de validation
                var response = await httpClient.PostAsJsonAsync("https://localhost:7187/api/auth/validate", validateRequest);

                if (response.IsSuccessStatusCode)
                {
                    var validateResponse = await response.Content.ReadFromJsonAsync<ValidateSessionResponse>();

                    if (validateResponse.IsValid)
                    {
                        return true; // Retourne true si la session est valide
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                // Loggez l'erreur (implémentation spécifique à votre système de logging)
                Console.WriteLine($"Erreur lors de la validation de la session : {ex.Message}");
                return false;
            }
        }


        public async Task<bool> RenewSessionAsync(string encryptedToken, DateTime dateExpiration)
        {
            try
            {
                // Déchiffrer le token
                var decryptedToken = SecurityManager.DecryptAES(encryptedToken);
                var protector = _dataProtectionProvider.CreateProtector("AuthTokenProtector");
                var token = protector.Unprotect(decryptedToken);

                // Créez le client HTTP
                using var httpClient = new HttpClient();

                // Préparez la requête de renouvellement
                var renewRequest = new RenewSessionRequest
                {
                    Token = token,
                    Expiration = dateExpiration,
                    Ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString()
                };

                // Envoyer la requête POST à l'API de renouvellement
                var response = await httpClient.PostAsJsonAsync("https://localhost:7187/api/auth/renew", renewRequest);

                if (response.IsSuccessStatusCode)
                {
                    var renewResponse = await response.Content.ReadFromJsonAsync<RenewSessionResponse>();

                    // Chiffrer à nouveau le nouveau token avec Data Protection avant de le stocker
                    var newToken = renewResponse.NewToken;
                    var protectedNewToken = protector.Protect(newToken);

                    // Récupérer la nouvelle date d'expiration
                    var newExpiration = renewResponse.NewExpiration;

                    // Mettre à jour le local storage avec le nouveau token chiffré et la nouvelle date d'expiration
                    await _localStorage.SetItemAsync(SecurityManager.EncryptAES("Token"), SecurityManager.EncryptAES(protectedNewToken));
                    await _localStorage.SetItemAsync(SecurityManager.EncryptAES("ExpireDate"), SecurityManager.EncryptAES(newExpiration.ToString("o")));

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                // Loggez l'erreur (implémentation spécifique à votre système de logging)
                Console.WriteLine($"Erreur lors du renouvellement de la session : {ex.Message}");
                return false;
            }
        }
    }
}
