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
        var newOffer = new cOffresBase
        {
            SportName = sportTicket.SportName,
            Description = sportTicket.Description,
            PriceSolo = sportTicket.PriceSolo,
            PriceDuo = sportTicket.PriceDuo,
            PriceFamily = sportTicket.PriceFamily,
            ImageUrl = sportTicket.ImageUrl
        };

        _context.Offres.Add(newOffer);
        await _context.SaveChangesAsync(); // Sauvegarde dans la base de données
    }



}
