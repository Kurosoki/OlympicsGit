using Microsoft.EntityFrameworkCore;
using Olympics.Database;
using Olympics.Metier.Models;
using Olympics.Metier.Utils;

public class OffreService
{
    private readonly ApplicationDbContext _context;


    public OffreService(ApplicationDbContext context)
    {
        _context = context;
    }

    // Sauvegarde un billet sportif dans la base de données
    public async Task SaveSportTicketAsync(SportTicket sportTicket)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(sportTicket.SportName) ||
                string.IsNullOrWhiteSpace(sportTicket.Description) ||
                sportTicket.PriceSolo < 0 ||
                sportTicket.PriceDuo < 0 ||
                sportTicket.PriceFamily < 0)
            {
                throw new ArgumentException("Les données du sportTicket ne sont pas valides.");
            }

            var newOffer = new cOffresBase
            {
                SportName = sportTicket.SportName,
                Description = sportTicket.Description,
                PriceSolo = sportTicket.PriceSolo,
                PriceDuo = sportTicket.PriceDuo,
                PriceFamily = sportTicket.PriceFamily
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


}
