using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Olympics.Database;
using Olympics.Metier.Business;

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
        public async Task<cPanierBase> GetPanierByIdAsync(int idPanier)
        {
            return await _context.Panier
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync(p => p.IDPanier == idPanier);
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


        //Supprimer un panier et ses tickets en bdd
        public async Task DeletePanierAsync(int idPanier)
        {
            var panier = _context.Panier
                                 .Include(p => p.Tickets)
                                 .FirstOrDefault(p => p.IDPanier == idPanier);

            if (panier != null)
            {
                _context.Panier.Remove(panier);
                await _context.SaveChangesAsync();
            }
        }



    }
}
