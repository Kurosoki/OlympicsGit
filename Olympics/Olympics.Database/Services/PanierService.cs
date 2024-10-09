using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Olympics.Database;
using Olympics.Metier.Models;

namespace Olympics.Services
{
    public class PanierService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJSRuntime _jsRuntime;

        public PanierService(ApplicationDbContext context, IJSRuntime jsRuntime)
        {
            _context = context;
            _jsRuntime = jsRuntime;
        }

        // Méthode pour récupérer les tickets dans le panier sessionStorage
        public async Task<List<cTicket>> GetCartFromSessionAsync()
        {
            var cartJson = await _jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "cart");
            if (string.IsNullOrEmpty(cartJson))
            {
                return new List<cTicket>();
            }
            return System.Text.Json.JsonSerializer.Deserialize<List<cTicket>>(cartJson);
        }

        // Méthode pour sauvegarder les tickets dans le panier sessionStorage
        public async Task SetCartInSessionAsync(List<cTicket> cartTickets)
        {
            var cartJson = System.Text.Json.JsonSerializer.Serialize(cartTickets);
            await _jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "cart", cartJson);
        }


        // Méthode pour effacer le panier stocké en sessionStorage
        public async Task ClearCartFromSessionAsync()
        {
            await _jsRuntime.InvokeVoidAsync("sessionStorage.removeItem", "cart");
        }


        //Récupérer un panier spécifique avec tous ses tickets en bdd

        public async Task<cPanierBase> GetPanierByUserIdAsync(int idClient)
        {
            return await _context.Panier
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync(p => p.IDClient == idClient);
        }

        public cPanierBase PanierActuel { get; set; }

        public void MettreAJourPanier(cPanierBase panierUtilisateur)
        {
            PanierActuel = panierUtilisateur;
        }

        public cPanierBase ObtenirPanier()
        {
            return PanierActuel;
        }

        //Création d'un nouveau panier en bdd
        public async Task CreatePanierAsync(cPanierBase newPanier)
        {
            _context.Panier.Add(newPanier);
            await _context.SaveChangesAsync();
        }


        //Sauvegarder les modifications du panier en bdd
        public async Task UpdatePanierAsync(cPanierBase panier)
        {
            _context.Panier.Update(panier);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveTicketsFromPanierAsync(int panierId)
        {
            // Récupérez le panier
            var panier = await _context.Panier.Include(p => p.Tickets)
                                                  .FirstOrDefaultAsync(p => p.IDPanier == panierId);

            if (panier == null)
            {
                throw new InvalidOperationException("Le panier n'existe pas.");
            }

            // Supprimez tous les tickets du panier
            _context.Tickets.RemoveRange(panier.Tickets);
            await _context.SaveChangesAsync();
        }


    }
}
