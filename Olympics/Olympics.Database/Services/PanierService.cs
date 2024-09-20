using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Olympics.Database;
using Olympics.Metier.Business;
using Microsoft.JSInterop;

namespace Olympics.Services
{
    public class PanierService
    {
        private readonly ApplicationDbContext _context;
        private readonly IJSRuntime jsRuntime;

        public PanierService(ApplicationDbContext context, IJSRuntime jsRuntime)
        {
            _context = context;
            this.jsRuntime = jsRuntime;
        }

        // Méthode pour récupérer les tickets dans le panier sessionStorage
        public async Task<List<cTicket>> GetCartFromSessionAsync()
        {
            var cartJson = await jsRuntime.InvokeAsync<string>("sessionStorage.getItem", "cart");
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
            await jsRuntime.InvokeVoidAsync("sessionStorage.setItem", "cart", cartJson);
        }



        //Récupérer un panier spécifique avec tous ses tickets 
        public async Task<cPanierBase> GetPanierByIdAsync(int idPanier)
        {
            return await _context.Panier
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync(p => p.IDPanier == idPanier);
        }


        //Création d'un nouveau panier
        public async Task CreatePanierAsync(cPanierBase panier)
        {
            _context.Panier.Add(panier);
            await _context.SaveChangesAsync();
        }


        //Sauvegarder les modifications du panier
        public async Task UpdatePanierAsync(cPanierBase panier)
        {
            _context.Panier.Update(panier);
            await _context.SaveChangesAsync();
        }


        //Supprimer un panier et ses tickets 
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
