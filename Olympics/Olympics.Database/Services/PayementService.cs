using Microsoft.AspNetCore.Components;
using Olympics.Metier.Models;
using QRCoder;
using System.Drawing;
using ZXing.QrCode.Internal;
using System.Drawing.Imaging;
using System.IO;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using System.Web;
using Olympics.Services;



namespace Olympics.Database.Services
{
    public class PayementService
    {
        private readonly UserService _userService;
        private readonly NavigationManager _navigationManager;
        private readonly PanierService _panierService;

        public PayementService(UserService userService, NavigationManager navigationManager, PanierService panierService)
        {
            _userService = userService;
            _navigationManager = navigationManager;
            _panierService = panierService;
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
             string qrCodeBase64 = GenerateQRCode(finalKey);

            // Retourner un résultat de paiement
            var paiementResult = new cPayementBase
            {
                IsSuccess = true,
                IDPanier = panier.IDPanier, // Lier le paiement au panier correspondant
                DateAchat = DateTime.Now,
                Montant = montant,
            };

            // Encoder les valeurs avant de les passer dans l'URL
            string encodedDateAchat = HttpUtility.UrlEncode(paiementResult.DateAchat.ToString("yyyy-MM-dd HH:mm:ss"));
            string encodedMontant = HttpUtility.UrlEncode(paiementResult.Montant.ToString("F2")); // Format standard avec 2 décimales

            // Rediriger vers la page de confirmation avec les valeurs encodées
            _navigationManager.NavigateTo($"/confirmation?qrCodeBase64={HttpUtility.UrlEncode(qrCodeBase64)}&dateAchat={encodedDateAchat}&montant={encodedMontant}");

            // Vider le panier après le paiement réussi
            await _panierService.RemoveTicketsFromPanierAsync(panier.IDPanier);

            return paiementResult;
        }

        public Bitmap GenerateQRCodeBitmap(string finalKey)
        {
            var qrCodeWriter = new BarcodeWriterPixelData // Cela génère un objet contenant les données de pixel.
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 300,
                    Width = 300,
                    Margin = 1
                }
            };

            // Générer les données de pixel pour le QR code
            var pixelData = qrCodeWriter.Write(finalKey);

            // Créer un Bitmap basé sur les données de pixel
            var bitmap = new Bitmap(pixelData.Width, pixelData.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, bitmap.PixelFormat);

            try
            {
                // Copier les données de pixel dans le bitmap
                System.Runtime.InteropServices.Marshal.Copy(pixelData.Pixels, 0, bitmapData.Scan0, pixelData.Pixels.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }

            return bitmap;
        }


        public string ConvertBitmapToBase64(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                // Enregistrer le Bitmap au format PNG dans le MemoryStream
                bitmap.Save(ms, ImageFormat.Png);
                var byteArray = ms.ToArray();

                // Convertir le tableau de bytes en chaîne Base64
                return Convert.ToBase64String(byteArray);
            }
        }

        public string GenerateQRCode(string finalKey)
        {
            // Étape 1 : Générer le Bitmap
            var qrCodeBitmap = GenerateQRCodeBitmap(finalKey);

            // Étape 2 : Convertir le Bitmap en chaîne Base64
            string base64String = ConvertBitmapToBase64(qrCodeBitmap);

            // Retourner la chaîne Base64
            return base64String;
        }



    }
}

