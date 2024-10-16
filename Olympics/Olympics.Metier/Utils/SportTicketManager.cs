using Olympics.Metier.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympics.Metier.Utils
{

    public class SportTicket
    {
        public int IDOffre { get; set; }

        public string SportName { get; set; }

        public string Description { get; set; }
        public string ImageUrl { get; set; }

        public int QuantitySolo { get; set; }
        public int QuantityDuo { get; set; }
        public int QuantityFamily { get; set; }

        public decimal PriceSolo { get; set; }
        public decimal PriceDuo { get; set; }
        public decimal PriceFamily { get; set; }
    }

    public class SportTicketManager
    {
        public List<SportTicket> MapToSportTickets(List<cOffresBase> offres)
        {
            return offres.Select(offre => new SportTicket
            {
                IDOffre = offre.IDOffre,
                SportName = offre?.SportName ?? "Inconnu", // Valeur par défaut si null
                Description = offre?.Description ?? "Pas de description", // Valeur par défaut si null
                PriceSolo = offre?.PriceSolo ?? 0,
                PriceDuo = offre?.PriceDuo ?? 0,
                PriceFamily = offre?.PriceFamily ?? 0,

                // Toujours définir l'image par défaut
                ImageUrl = offre?.ImageUrl ?? "/images/sports/cloche-gold.png", 

                QuantitySolo = 0,
                QuantityDuo = 0,
                QuantityFamily = 0
            }).ToList();
        }

    }
}
