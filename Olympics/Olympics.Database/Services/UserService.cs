using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Olympics.Metier.Models;
using Olympics.Metier.Utils;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;
using Olympics.Services;
using System.Text;
using System.Net.Http.Json;
using AuthentificationServer.Classes; 



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

        public UserService(HttpClient httpClient, ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger, AuthenticationStateProvider authenticationStateProvider, IJSRuntime jsRuntime, PanierService panierService)
        {
            _httpClient = httpClient;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _authenticationStateProvider = authenticationStateProvider;
            _jsRuntime = jsRuntime;
            _panierService = panierService;
        }

        public string GenerateUniqueKey()
        {
            return Guid.NewGuid().ToString();
        }

        public async Task<int?> GetAuthenticatedUserIdAsync()
        {
            var authState = await _authenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity.IsAuthenticated)
            {
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                {
                    return userId;
                }
            }

            return null; // Utilisateur non authentifié
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
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.EmailClient == loginUser.EmailClient);

            if (utilisateur == null)
            {
                return (false, false); // L'utilisateur n'existe pas
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


                    ////  ////  ////  ROLE DE L' UTILISATEUR   ////  ////  ////
                    
                    bool isAdmin = utilisateur.RoleUtilisateur == RoleUtilisateur.Administrateur;

                    ////  ////  ////  RECUPERER LE PANIER   ////  ////  ////

                    // Récupérer le panier depuis le session storage
                    var cartTickets = await _panierService.GetCartFromSessionAsync();

                    if (cartTickets != null && cartTickets.Count > 0)
                    {
                        var existingPanier = await _context.Panier
                            .Include(p => p.Tickets)
                            .FirstOrDefaultAsync(p => p.IDClient == utilisateur.IDClient);

                        if (existingPanier != null)
                        {
                            existingPanier.Tickets.AddRange(cartTickets);
                            existingPanier.DateUpdated = DateTime.Now; 
                            await _panierService.UpdatePanierAsync(existingPanier);
                        }
                        else
                        {
                            var newPanier = new cPanierBase
                            {
                                IDClient = utilisateur.IDClient,
                                Tickets = cartTickets,
                                DateCreated = DateTime.Now,
                                DateUpdated = DateTime.Now
                            };
                            await _panierService.CreatePanierAsync(newPanier);
                        }
                        await _panierService.ClearCartFromSessionAsync();
                    }

                    return (true, isAdmin); // Utilisateur connecté et retourne si c'est un admin
                }
            }

            // Si on arrive ici, cela signifie que le mot de passe est invalide ou la réponse de l'API n'est pas un succès
            return (false, false); // Échec de la connexion
        }





    }
}

