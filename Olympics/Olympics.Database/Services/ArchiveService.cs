using Microsoft.EntityFrameworkCore;
using Olympics.Metier.Models;


namespace Olympics.Database.Services
{
    public class ArchiveService
    {

        private readonly ApplicationDbContext _context;

        public ArchiveService (ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<cPanierArchive>> GetAllPaniersArchivésAsync()
        {
            return await _context.PanierArchive
                .Include(p => p.TicketsArchive)
                .ToListAsync();
        }


        public async Task<List<cArchiveBase>> GetVentesParOffreAsync()
        {
            var paniers = await GetAllPaniersArchivésAsync();

            var ventes = paniers.SelectMany(p => p.TicketsArchive)
                .GroupBy(t => new { t.SportName}) // Groupement par type d'offre
                .Select(g => new cArchiveBase
                {
                    NameOffre = g.Key.SportName, // Nom de l'offre
                    NombreTickets = g.Sum(t => t.Quantity), // Somme des quantités vendues
                    VentesTotales = g.Sum(t => t.Price * t.Quantity) // Somme des montants totaux vendus
                }).ToList();

            return ventes;
        }


    }
}
