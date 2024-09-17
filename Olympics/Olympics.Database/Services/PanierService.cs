using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Olympics.Database;
using Olympics.Metier.Business;

namespace Olympics.Services
{
    public class PanierService
    {
        private readonly ApplicationDbContext _context;

        public PanierService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<cPanierBase> GetPanierByIdAsync(int id)
        {
            return await _context.Panier
                .Include(p => p.Tickets)
                .FirstOrDefaultAsync(p => p.IDPanier == id);
        }

        public async Task CreatePanierAsync(cPanierBase panier)
        {
            _context.Panier.Add(panier);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePanierAsync(cPanierBase panier)
        {
            _context.Panier.Update(panier);
            await _context.SaveChangesAsync();
        }
    }
}
