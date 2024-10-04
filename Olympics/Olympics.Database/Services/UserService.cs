using AuthentificationServer.Classes;
using AuthentificationServer.DAL;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Olympics.Metier.Models;
using Olympics.Metier.Utils;
using Olympics.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Cryptography;


namespace Olympics.Database.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        private readonly IJSRuntime _jsRuntime;
        private readonly PanierService _panierService;
        private readonly HttpClient _httpClient;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly SessionService _sessionService;
        private cUtilisateurBase _currentUser;
        private readonly ILocalStorageService _localStorage;


        public UserService(HttpClient httpClient, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor,
        ILogger<UserService> logger, AuthenticationStateProvider authenticationStateProvider, IJSRuntime jsRuntime,
        PanierService panierService, IDataProtectionProvider dataProtectionProvider, SessionService sessionService, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _authenticationStateProvider = authenticationStateProvider;
            _jsRuntime = jsRuntime;
            _panierService = panierService;
            _dataProtectionProvider = dataProtectionProvider;
            _sessionService = sessionService;
            _localStorage = localStorage;
        }

        public string GenerateUniqueKey()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<cUtilisateurBase> GetUserByIdAsync(int userId)
        {
            return await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.IDClient == userId);
        }

        public async Task<bool> RegisterUserAsync(cUtilisateurBase user)
        {
            // Vérifiez si l'utilisateur existe déjà dans la base de données
            var existingUser = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.EmailClient == user.EmailClient.ToLower());

            if (existingUser != null)
            {
                // L'utilisateur existe déjà
                return false;
            }

            // Générer un salt pour cet utilisateur
            string salt = SecurityManager.GenerateSalt();

            // Hashage du mot de passe
            user.ShaMotDePasse = SecurityManager.HashPasswordSHA512(user.ShaMotDePasse, salt);

            // Enregistrez le salt avec l'utilisateur pour la vérification future
            user.Salt = salt;

            // Générer la clé unique lors de la création de l'utilisateur
            user.Key = GenerateUniqueKey();

            // Ajouter l'utilisateur à la base de données
            _context.Utilisateurs.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<(bool IsConnected, bool IsAdmin)> LoginUserAsync(cUtilisateurConnexionBase loginUser)
        {
            // Appeler la méthode pour nettoyer les sessions expirées avant de créer une nouvelle session
            SessionManager.CleanupExpiredSessions();

            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.EmailClient == loginUser.EmailClient);

            if (utilisateur == null)
            {
                return (false, false);
            }

            bool isPasswordValid = SecurityManager.VerifyPassword(loginUser.ShaMotDePasse, utilisateur.ShaMotDePasse, utilisateur.Salt);

            if (isPasswordValid)
            {
                ////  ////  ////  INTERROGER L'API SUR L'AUTHENTIFICATION   ////  ////  ////

                using var httpClient = new HttpClient();

                var loginRequest = new LoginRequest
                {
                    Username = utilisateur.PrenomClient,
                    Email = utilisateur.EmailClient,
                    Ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString()
                };

                var response = await httpClient.PostAsJsonAsync("https://localhost:7187/api/auth/login", loginRequest);

                if (response.IsSuccessStatusCode)
                {
                    var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                    var token = loginResponse.Token;

                    // Chiffrer le token avec Data Protection
                    var protector = _dataProtectionProvider.CreateProtector("AuthTokenProtector");
                    var encryptedToken = protector.Protect(token);

                    // Chiffrer le token et la date d'expiration avec AES
                    await _localStorage.SetItemAsync(SecurityManager.EncryptAES("Token"), SecurityManager.EncryptAES(encryptedToken));
                    await _localStorage.SetItemAsync(SecurityManager.EncryptAES("ExpireDate"), SecurityManager.EncryptAES(loginResponse.Expiration.ToString("o")));


                    ////  ////  ////  IDENTIFIER LE ROLE DE L'UTILISATEUR  ////  ////  ////

                    bool isAdmin = utilisateur.RoleUtilisateur == RoleUtilisateur.Administrateur;


                    ////  ////  ////  RECUPERER LE PANIER  ////  ////  ////

                    var cartTickets = await _panierService.GetCartFromSessionAsync();

                    // Vérifier si l'utilisateur a déjà un panier existant
                    var existingPanier = await _context.Panier
                        .Include(p => p.Tickets)
                        .FirstOrDefaultAsync(p => p.IDClient == utilisateur.IDClient);

                    if (existingPanier != null)
                    {
                        // Si des tickets de session existent, les ajouter au panier existant
                        if (cartTickets != null && cartTickets.Count > 0)
                        {
                            foreach (var ticket in cartTickets)
                            {
                                ticket.IDPanier = existingPanier.IDPanier; // Lier chaque ticket au panier
                            }
                            existingPanier.Tickets.AddRange(cartTickets);
                            existingPanier.DateUpdated = DateTime.Now;

                            // Mettre à jour le panier existant dans la base de données
                            await _panierService.UpdatePanierAsync(existingPanier);
                        }
                    }
                    else
                    {
                        // Sinon, créer un nouveau panier
                        var newPanier = new cPanierBase
                        {
                            IDClient = utilisateur.IDClient,
                            Tickets = new List<cTicket>(), // Initialement vide
                            DateCreated = DateTime.Now,
                            DateUpdated = DateTime.Now,
                        };

                        // Créer le panier dans la base de données pour obtenir un ID
                        await _panierService.CreatePanierAsync(newPanier);

                        // Lier les tickets de session au nouveau panier
                        if (cartTickets != null && cartTickets.Count > 0)
                        {
                            foreach (var ticket in cartTickets)
                            {
                                ticket.IDPanier = newPanier.IDPanier; // Lier le ticket au nouveau panier
                                newPanier.Tickets.Add(ticket); // Ajouter le ticket à la liste
                            }
                        }

                        // Mettre à jour le panier avec les tickets
                        await _panierService.UpdatePanierAsync(newPanier);
                    }

                    var panierUtilisateur = await _panierService.GetPanierByUserIdAsync(utilisateur.IDClient);
                    // Mettre à jour le panier dans le service pour qu'il soit accessible partout
                    _panierService.MettreAJourPanier(panierUtilisateur);

                    return (true, isAdmin);
                }
            }

            // Si on arrive ici, cela signifie que le mot de passe est invalide ou la réponse de l'API n'est pas un succès
            return (false, false);
        }

        public async Task<bool> LogoutUserAsync()
        {
            // Récupérer le token d'authentification chiffré depuis le localStorage
            string encryptedToken = await _localStorage.GetItemAsync<string>(SecurityManager.EncryptAES("Token"));
            string encryptedExpirationDate = await _localStorage.GetItemAsync<string>(SecurityManager.EncryptAES("ExpireDate"));

            if (string.IsNullOrEmpty(encryptedToken) || string.IsNullOrEmpty(encryptedExpirationDate))
            {
                return false;   // Pas de token, rien à supprimer
            }

            try
            {
                //  Déchiffrer avec AES
                var aesDecryptedToken = SecurityManager.DecryptAES(encryptedToken);
                var aesDecryptedExpiration = SecurityManager.DecryptAES(encryptedExpirationDate);

                // Déchiffrer avec Data Protection
                var protector = _dataProtectionProvider.CreateProtector("AuthTokenProtector");
                var token = protector.Unprotect(aesDecryptedToken);
                DateTime expirationDate = Convert.ToDateTime(aesDecryptedExpiration);

                using var httpClient = new HttpClient();

                // Préparer la requête de déconnexion en envoyant un objet ValidateSessionRequest
                var logoutRequest = new ValidateSessionRequest
                {
                    Token = token,
                    Ip = _httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString(),
                    UserAgent = _httpContextAccessor.HttpContext.Request.Headers["User-Agent"].ToString(),
                    Expiration = expirationDate
                };

                var response = await httpClient.PostAsJsonAsync("https://localhost:7187/api/auth/logout", logoutRequest);

                if (response.IsSuccessStatusCode)
                {
                    // Suppression des données dans le localStorage
                    await _localStorage.RemoveItemAsync(SecurityManager.EncryptAES("Token"));
                    await _localStorage.RemoveItemAsync(SecurityManager.EncryptAES("ExpireDate"));

                    // Méthode pour nettoyer le panier en sessionStorage
                    await _panierService.ClearCartFromSessionAsync();

                    // On ajoute un logging ici pour vérifier que le localStorage est bien vidé
                    Console.WriteLine("Déconnexion réussie et token supprimé du localStorage.");
                    return true;
                }
            }
            catch (Exception ex)
            {

            }
            return false;
        }
    }
}

