using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Olympics.Database.Services;
using Olympics.Metier.Business;
using Olympics.Metier.Utils;
using System.Security.Claims;

namespace Olympics.Presentation.ServicesAuthen
{
    public class AuthentificationService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserService _userService;

        public AuthentificationService(IHttpContextAccessor httpContextAccessor, UserService userService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
        }

        public async Task<bool> LoginAsync(string email, string password, bool rememberMe)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
            {
                return false;
            }

            var loginUser = new cUtilisateurConnexionBase
            {
                EmailClient = email,
                ShaMotDePasse = password,
                RememberMe = rememberMe
            };

            // Valider l'utilisateur via UserService
            bool isLoginSuccessful = await _userService.LoginUserAsync(loginUser);
            if (!isLoginSuccessful)
            {
                return false;
            }

            // Créer les claims et l'authentifier
            var claims = ClaimsManager.GenerateUserClaims(email, "Utilisateur");

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = rememberMe
            };

            await httpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), authProperties);

            return true;
        }

        public async Task LogoutAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext != null)
            {
                await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            }
        }

        public bool IsAuthenticated()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.User.Identity != null && httpContext.User.Identity.IsAuthenticated;
        }
    }
}
