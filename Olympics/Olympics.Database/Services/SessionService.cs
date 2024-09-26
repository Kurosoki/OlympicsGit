using AuthentificationServer.Classes;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace Olympics.Database.Services
{
    public class SessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> ValidateUserSessionAsync(string token)
        {
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
