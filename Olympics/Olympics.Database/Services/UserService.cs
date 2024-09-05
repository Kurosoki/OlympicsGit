using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Olympics.Metier.Business;
using Olympics.Metier.Utils;
using System.Security.Claims;



namespace Olympics.Database.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
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

            // Ajouter l'utilisateur à la base de données
            _context.Utilisateurs.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> LoginUserAsync(cUtilisateurConnexionBase loginUser)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var utilisateur = await _context.Utilisateurs
                .FirstOrDefaultAsync(u => u.EmailClient == loginUser.EmailClient);

            if (utilisateur == null)
            {
                return false;
            }

            bool isPasswordValid = SecurityManager.VerifyPassword(loginUser.ShaMotDePasse, utilisateur.ShaMotDePasse, utilisateur.Salt);
            if (isPasswordValid)
            {
                var claims = ClaimsManager.GenerateUserClaims(utilisateur.EmailClient, utilisateur.RoleUtilisateur.ToString());

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await httpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    new AuthenticationProperties
                    {
                        IsPersistent = loginUser.RememberMe, // Gère la persistance du cookie
                        ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60) // Expire après 60 minutes
                    });

                return true;
            }

            return false;


        }
    }
}
