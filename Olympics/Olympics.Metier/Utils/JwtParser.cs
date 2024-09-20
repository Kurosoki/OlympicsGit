using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Olympics.Metier.Utils
{
    public static class JwtParser
    {
        public static IEnumerable<Claim> ParseClaimsFromJwt(string jwt)
        {
            var claims = new List<Claim>();
            var jwtHandler = new JwtSecurityTokenHandler();
            var tokenS = jwtHandler.ReadToken(jwt) as JwtSecurityToken;
            claims.AddRange(tokenS.Claims);
            return claims;
        }
    }
}
