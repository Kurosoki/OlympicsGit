using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Olympics.Metier.Utils
{
    public class SecurityManager
    {

        // Clé et IV AES générés au lancement de l'application
        private readonly byte[] aesKey;
        private readonly byte[] aesIV;

        public SecurityManager()
        {
            // Générer la clé et l'IV AES au démarrage de l'application
            using (Aes aes = Aes.Create())
            {
                aesKey = aes.Key;
                aesIV = aes.IV;
            }
        }

        public string GenerateSalt(int size = 16)
        {
            // Génère un salt aléatoire
            byte[] saltBytes = new byte[size];
            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }


        public string HashPasswordSHA512(string password, string salt)
        {   
            // Concaténer le mot de passe et le salt
            string saltedPassword = password + salt;

            // Hacher le mot de passe salé avec SHA-512
            using (SHA512 sha512 = SHA512.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(saltedPassword); // On encode le mot de passe en UTF-8 en une séquence d'octets.
                byte[] hash = sha512.ComputeHash(bytes); // On hache les octets du mot de passe en utilisant SHA-512.

                StringBuilder stringBuilder = new StringBuilder();
                foreach (byte b in hash)
                {
                    stringBuilder.Append(b.ToString("x2")); // On convertit chaque octet en une chaîne hexadécimale et l'ajoute au StringBuilder.
                }

                return stringBuilder.ToString(); // On retourne la chaîne hexadécimale représentant le hash.
            }

        }

        public virtual bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            // Hachez le mot de passe entré par l'utilisateur en utilisant le même salt
            string hashOfEnteredPassword = HashPasswordSHA512(enteredPassword, storedSalt);

            // Comparez le hash du mot de passe entré avec le hash stocké
            return hashOfEnteredPassword.Equals(storedHash);
        }

        // Méthode pour chiffrer une chaîne de caractères en utilisant AES et retourner une chaîne Base64
        public string EncryptAES(string plainText)
        {
            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.IV = aesIV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        // Écrire les données à chiffrer dans le flux
                        swEncrypt.Write(plainText);
                    }
                    byte[] encryptedBytes = msEncrypt.ToArray();
                    // Convertir les bytes chiffrés en chaîne Base64
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
        }

        // Méthode pour déchiffrer une chaîne Base64 en utilisant AES
        public string DecryptAES(string cipherTextBase64)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(cipherTextBase64);

            using (Aes aes = Aes.Create())
            {
                aes.Key = aesKey;
                aes.IV = aesIV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherTextBytes))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    // Lire les données déchiffrées du flux
                    return srDecrypt.ReadToEnd();
                }
            }
        }


    }
}
