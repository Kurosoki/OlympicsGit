using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Olympics.Metier.Business;
using Olympics.Metier.Utils;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Text.Json;
using Olympics.Services;


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

        public UserService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger, AuthenticationStateProvider authenticationStateProvider, IJSRuntime jsRuntime, PanierService panierService)
        {
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
                return (false, false);
            }

            bool isPasswordValid = SecurityManager.VerifyPassword(loginUser.ShaMotDePasse, utilisateur.ShaMotDePasse, utilisateur.Salt);

            if (isPasswordValid)
            {
                ////  ////  ////  INTERROGER L'API SUR L'AUTHENTIFICATION   ////  ////  ////

                // Connexion de l'utilisateur
                //await SignInUserAsync(utilisateur, loginUser.RememberMe);  


                ////  ////  ////  ROLE DE L' UTILISATEUR   ////  ////  ////

                // Vérifiez le rôle de l'utilisateur
                bool isAdmin = utilisateur.RoleUtilisateur == RoleUtilisateur.Administrateur;


                ////  ////  ////  RECUPERER LE PANIER   ////  ////  ////


                // Récupérer le panier depuis le session storage
                var cartTickets = await _panierService.GetCartFromSessionAsync();

                if (cartTickets != null && cartTickets.Count > 0)
                {
                    // Vérifier si l'utilisateur a déjà un panier dans la base de données
                    var existingPanier = await _context.Panier
                        .Include(p => p.Tickets)  // Inclure les tickets du panier existant
                        .FirstOrDefaultAsync(p => p.IDClient == utilisateur.IDClient);

                    if (existingPanier != null )
                    {
                        // Si un panier existe, ajouter les tickets du session storage au panier existant
                        existingPanier.Tickets.AddRange(cartTickets);
                        existingPanier.DateUpdated = DateTime.Now;  // Mise à jour de la date de modification
                        await _panierService.UpdatePanierAsync(existingPanier);
                    }
                    else
                    {
                        // Si l'utilisateur n'a pas encore de panier, en créer un nouveau
                        var newPanier = new cPanierBase
                        {
                            IDClient = utilisateur.IDClient,  // Associer le panier à l'utilisateur
                            Tickets = cartTickets,  // Ajouter les tickets du session storage
                            DateCreated = DateTime.Now,
                            DateUpdated = DateTime.Now
                        };
                        await _panierService.CreatePanierAsync(newPanier);
                    }
                    // Effacer le panier du session storage après avoir été associé
                    await _panierService.ClearCartFromSessionAsync();
                }

                return (true, isAdmin);  // Utilisateur connecté et retourne si c'est un admin
            }

            return (false, false); // Échec de la connexion
        }


    }
}
