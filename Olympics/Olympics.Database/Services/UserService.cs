using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Olympics.Metier.Business;
using Olympics.Metier.Utils;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;



namespace Olympics.Database.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<UserService> _logger;
        private readonly AuthenticationStateProvider _authenticationStateProvider;

        public UserService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, ILogger<UserService> logger, AuthenticationStateProvider authenticationStateProvider)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _authenticationStateProvider = authenticationStateProvider;
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
                var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier); // Assurez-vous que vous avez un claim pour l'ID de l'utilisateur
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
                .FirstOrDefaultAsync(u => u.EmailClient == user.EmailClient);

            if (existingUser != null)
            {
                // L'utilisateur existe déjà
                return false;
            }

            // Générer un salt pour cet utilisateur
            string salt = SecurityManager.GenerateSalt();

            // Hashage du mot de passe
            user.ShaMotDePasse = SecurityManager.HashPasswordSHA512(user.ShaMotDePasse, salt);
            user.ShaMotDePasseVerif = SecurityManager.HashPasswordSHA512(user.ShaMotDePasseVerif, salt);

            // Enregistrez le salt avec l'utilisateur pour la vérification future
            user.Salt = salt;

            // Générer la clé unique lors de la création de l'utilisateur
            user.Key = GenerateUniqueKey();

            // Ajouter l'utilisateur à la base de données
            _context.Utilisateurs.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> LoginUserAsync(cUtilisateurConnexionBase loginUser)
        {
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.EmailClient == loginUser.EmailClient);

            if (utilisateur == null)
            {
                return false;
            }

            bool isPasswordValid = SecurityManager.VerifyPassword(loginUser.ShaMotDePasse, utilisateur.ShaMotDePasse, utilisateur.Salt);

            if (isPasswordValid)
            {
                // Si le mot de passe est correct, connecter l'utilisateur
                await SignInUserAsync(utilisateur, loginUser.RememberMe);  // Appeler la méthode de session

                return true;  // L'utilisateur est connecté
            }

            return false;
        }


        // Méthode pour créer une session utilisateur
        public async Task SignInUserAsync(cUtilisateurBase utilisateur, bool rememberMe)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                throw new InvalidOperationException("HttpContext is null.");
            }

            // Vérifier si la réponse a déjà commencé
            if (!httpContext.Response.HasStarted)
            {
                var claims = ClaimsManager.GenerateUserClaims(utilisateur.EmailClient, utilisateur.RoleUtilisateur.ToString());


                // Créer une identité avec ces claims
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Options d'authentification
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = rememberMe,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
                };

                // Connexion de l'utilisateur
                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties
                );
            }
            //else
            //{
            //    // Logger l'erreur si la réponse a déjà commencé
            //    _logger.LogWarning("Impossible de définir le cookie, la réponse a déjà commencé.");
            //}
        }



    }
}
