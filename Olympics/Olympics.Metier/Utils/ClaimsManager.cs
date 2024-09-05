using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Olympics.Metier.Utils
{
    public static class ClaimsManager
    {

        public static List<Claim> GenerateUserClaims(string email, string role)
        {
            return new List<Claim>
            {
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, role)
            };
        }
    }
}
