using Olympics.Metier.Models;
using System.Drawing;
using ZXing;

namespace Olympics.Database.Services
{
    public class PayementService
    {
        private readonly UserService _userService;

        public PayementService(UserService userService)
        {
            _userService = userService;
        }

        public async Task<cPayementBase> MockPayementAsync(cUtilisateurBase utilisateur, cPanierBase panier, decimal montant)
        {
            // Simule un délai pour le traitement du paiement
            await Task.Delay(1000);

            // Vérifier si l'utilisateur a une clé valide
            if (utilisateur == null || string.IsNullOrEmpty(utilisateur.Key))
            {
                throw new ArgumentException("L'utilisateur doit être valide et avoir une clé.");
            }

            // Générer une nouvelle clé pour le paiement
            string keyPayement = _userService.GenerateUniqueKey();

            // Concaténer la clé existante avec la nouvelle clé
            string finalKey = $"{utilisateur.Key}-{keyPayement}";

            // Créer un QR Code à partir de la clé finale
            string qrCodeFilePath = GenerateQrCode(finalKey);

            // Construire l'URL pour accéder à l'image QR code
            string qrCodeUrl = $"/qrcodes/{Path.GetFileName(qrCodeFilePath)}"; 

            // Retourner un résultat de paiement
            return new cPayementBase
            {
                IsSuccess = true,
                IDPanier = panier.IDPanier, // Lier le paiement au panier correspondant
                DateAchat = DateTime.Now,
                Montant = montant,
                QrCodeUrl = qrCodeUrl, // Inclure l'URL du QR code dans le résultat
            };
        }


        public string GenerateQrCode(string finalKey)
        {
            // Définir le répertoire de stockage du QR code
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "qrcodes");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Définir le chemin de fichier unique
            string filePath = Path.Combine(directoryPath, $"QRCode_{Guid.NewGuid()}.png");


            var qrCodeWriter = new BarcodeWriter<Bitmap>
            {
                Format = BarcodeFormat.QR_CODE, // Spécifie que l'on génère un QR code
                Options = new ZXing.Common.EncodingOptions
                {
                    Width = 300,   // Largeur de l'image du QR code
                    Height = 300   // Hauteur de l'image du QR code
                }
            };

            // Générer l'image du QR code à partir de la clé finale
            using (Bitmap bitmap = qrCodeWriter.Write(finalKey))
            {
                string filePathQrCode = $"QRCode_{Guid.NewGuid()}.png"; 

                // Sauvegarder l'image au format PNG
                bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);

            

                return filePath; // Retourner le chemin de l'image sauvegardée
            }
        }
















    }
}

