using Microsoft.EntityFrameworkCore;
using Olympics.Database;
using Olympics.Metier.Models;
using Olympics.Metier.Utils;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

public class OffreService
{
    private readonly ApplicationDbContext _context;


    public OffreService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Sauvegarde un billet sportif dans la base de données
    public async Task SaveSportTicketAsync(SportTicket currentSportTicket)
    {
        try
        {          
            var newOffer = new cOffresBase
            {
                SportName = currentSportTicket.SportName,
                Description = currentSportTicket.Description,
                PriceSolo = currentSportTicket.PriceSolo,
                PriceDuo = currentSportTicket.PriceDuo,
                PriceFamily = currentSportTicket.PriceFamily,
                ImageUrl = "images/sports/cloche-gold.png"
            };

            _context.Offres.Add(newOffer);
            await _context.SaveChangesAsync(); // Sauvegarde dans la base de données

            Console.WriteLine("L'offre a été enregistrée avec succès.");

        }
        catch (Exception ex)
        {
            Console.WriteLine($"Une erreur s'est produite : {ex.Message}");
        }
    }



    public async Task<List<cOffresBase>> GetAllOffersAsync()
    {
        var offres = await _context.Offres.ToListAsync();

        Console.WriteLine($"Nombre d'offres récupérées : {offres?.Count ?? 0}");

        return offres;
    }


    public async Task<cOffresBase> GetOfferByIdAsync(int idOffre)
    {
        return await _context.Offres.FirstOrDefaultAsync(o => o.IDOffre == idOffre);
    }



    public async Task UpdateOffreAsync(SportTicket currentSportTicket)
    {
        var offre = await _context.Offres.FirstOrDefaultAsync(o => o.IDOffre == currentSportTicket.IDOffre);

        if (offre != null)
        {
            offre.SportName = currentSportTicket.SportName;
            offre.Description = currentSportTicket.Description;
            offre.PriceSolo = currentSportTicket.PriceSolo;
            offre.PriceDuo = currentSportTicket.PriceDuo;
            offre.PriceFamily = currentSportTicket.PriceFamily;

            await _context.SaveChangesAsync();
            Console.WriteLine("Offre modifiée avec succès.");
        }
        else
        {
            Console.WriteLine("Offre non trouvée.");
        }
    }



    public async Task DeleteOffreAsync(int idoffre)
    {
        var offre = await _context.Offres.FirstOrDefaultAsync(o => o.IDOffre == idoffre);
        if (offre != null)
        {
            _context.Offres.Remove(offre);
            await _context.SaveChangesAsync();
            Console.WriteLine("Offre supprimée avec succès.");
        }
        else
        {
            Console.WriteLine("Offre non trouvée.");
        }
    }



}


