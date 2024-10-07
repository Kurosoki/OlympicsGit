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

    public class SportTicketModel
    {
        private List<SportTicket> MapToSportTickets(List<cOffresBase> offres)
        {
            return offres.Select(offre => new SportTicket
            {
                SportName = offre.SportName,
                Description = offre.Description,
                PriceSolo = offre.PriceSolo,
                PriceDuo = offre.PriceDuo,
                PriceFamily = offre.PriceFamily,
                ImageUrl = "default-image.jpg", // Valeur par défaut ou récupérée dynamiquement
                QuantitySolo = 0,
                QuantityDuo = 0,
                QuantityFamily = 0
            }).ToList();
        }

    }
}
