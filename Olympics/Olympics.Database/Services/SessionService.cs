using AuthentificationServer.Classes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;

namespace Olympics.Database.Services
{
    public class SessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDataProtectionProvider _dataProtectionProvider;

        public SessionService(IHttpContextAccessor httpContextAccessor, IDataProtectionProvider dataProtectionProvider)
        {
            _httpContextAccessor = httpContextAccessor;
            _dataProtectionProvider = dataProtectionProvider;
        }

        public async Task<bool> ValidateUserSessionAsync()
        {

            // Récupérer le token chiffré depuis les cookies
            var encryptedToken = _httpContextAccessor.HttpContext.Request.Cookies["AuthToken"];


            if (string.IsNullOrEmpty(encryptedToken))
            {
                return false; // Si le token est absent, la session n'est pas valide
            }


            // Déchiffrez le token avant de l'utiliser
            var protector = _dataProtectionProvider.CreateProtector("AuthTokenProtector");

            string token;

            try
            {
                token = protector.Unprotect(encryptedToken); // Déchiffrez le token
            }
            catch
            {
                return false; // Si la déchiffrement échoue, la session n'est pas valide
            }

            // Créez le client HTTP
            using var httpClient = new HttpClient();

            // Préparez la requête de validation
            var validateRequest = new ValidateSessionRequest
            {
                Token = token,
                Ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                UserAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString()
            };

            // Envoyer la requête POST à l'API de validation
            var response = await httpClient.PostAsJsonAsync("https://localhost:7187/api/auth/validate", validateRequest);

            // Gérer la réponse de l'API
            if (response.IsSuccessStatusCode)
            {
                var validateResponse = await response.Content.ReadFromJsonAsync<ValidateSessionResponse>();
                return validateResponse.IsValid; // Retourne true si la session est valide
            }

            return false; // Retourne false si la validation échoue
        }







    }

}
