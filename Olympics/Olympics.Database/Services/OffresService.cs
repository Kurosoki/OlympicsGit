using Microsoft.EntityFrameworkCore;
using Olympics.Metier.Models;

namespace Olympics.Database.Services
{
    public class OffresService
    {
        private readonly ApplicationDbContext _context;


        public OffresService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Ajouter une nouvelle offre
        public async Task<bool> AjouterOffreAsync(cOffresBase nouvelleOffre)
        {
            try
            {
                await _context.Offres.AddAsync(nouvelleOffre);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de l'ajout de l'offre : {ex.Message}");
                return false;
            }
        }

        // Modifier une offre existante
        public async Task<bool> ModifierOffreAsync(cOffresBase offreModifiee)
        {
            try
            {
                var offreExistante = await _context.Offres
                    .FirstOrDefaultAsync(o => o.IDOffre == offreModifiee.IDOffre);

                if (offreExistante == null)
                {
                    Console.WriteLine("Offre non trouvée.");
                    return false;
                }

                // Mise à jour des propriétés
                offreExistante.NomOffre = offreModifiee.NomOffre;
                offreExistante.Description = offreModifiee.Description;
                offreExistante.PriceSolo = offreModifiee.PriceSolo;
                offreExistante.PriceDuo = offreModifiee.PriceDuo;
                offreExistante.PriceFamily = offreModifiee.PriceFamily;


                _context.Offres.Update(offreExistante);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la modification de l'offre : {ex.Message}");
                return false;
            }
        }

        // Supprimer une offre
        public async Task<bool> SupprimerOffreAsync(int idOffre)
        {
            try
            {
                var offre = await _context.Offres.FirstOrDefaultAsync(o => o.IDOffre == idOffre);

                if (offre == null)
                {
                    Console.WriteLine("Offre non trouvée.");
                    return false;
                }

                _context.Offres.Remove(offre);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la suppression de l'offre : {ex.Message}");
                return false;
            }
        }

        // Récupérer une offre par ID (optionnel)
        public async Task<cOffresBase> GetOffreByIdAsync(int idOffre)
        {
            return await _context.Offres.FirstOrDefaultAsync(o => o.IDOffre == idOffre);
        }

        // Récupérer toutes les offres (optionnel)
        public async Task<List<cOffresBase>> GetAllOffresAsync()
        {
            return await _context.Offres.ToListAsync();
        }
    }
}
