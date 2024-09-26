using Microsoft.AspNetCore.Mvc;
using Olympics.Database.Services;
using Olympics.Metier.Models;

namespace Olympics.Presentation.Controllers
{
    public class PayementController : Controller
    {
        private readonly PayementService _payementService;

        public PayementController(PayementService payementService)
        {
            _payementService = payementService;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessPayement(cUtilisateurBase utilisateur, cPanierBase panier, int montant)
        {
            // Traitement du paiement (mocké ici pour l'exemple)
            var payementResult = await _payementService.MockPayementAsync(utilisateur, panier, montant);

            if (payementResult.IsSuccess)
            {
                // Rediriger vers l'URL du QR Code
                return Redirect(payementResult.QrCodeUrl);
            }

            // Gérer l'échec du paiement (exemple : retourner une vue d'erreur)
            return View("PayementFailed");
        }
    }
}
