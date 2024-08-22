using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Olympics.Metier.Utils
{
    public static class SecurityManager
    {
        public static string HashPasswordSHA512(string password)
        {
            //On crypte le mot de passe en SHA512
            //On utilise un salt pour rendre le hash plus sécurisé
            //On utilise UTF8 pour encoder le mot de passe en bytes
            //On utilise un StringBuilder pour convertir le hash en hexadécimal
            //On retourne le hash en hexadécimal                   
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(password);
                byte[] hash = sha512.ComputeHash(bytes);

                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hash)
                {
                    stringBuilder.Append(b.ToString("x2")); // Convertir en hexadécimal
                }

                return stringBuilder.ToString();
            }
        }
    }
}
