using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Olympics.Presentation.Components.Pages
{
    public partial class Billetterie
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }

        [Inject]
        protected NotificationService NotificationService { get; set; }


        enum TicketType
        {
            Solo,
            Family,
            Duo
        }

        public class SportTicket
        {
            public string SportName { get; set; }

            public int QuantitySolo { get; set; }
            public int QuantityDuo { get; set; }
            public int QuantityFamily { get; set; }

            public decimal PriceSolo { get; set; }
            public decimal PriceDuo { get; set; }
            public decimal PriceFamily { get; set; }
        }

        List<SportTicket> sportTickets = new List<SportTicket>
{
    new SportTicket
    {
        SportName = "Athlétisme",
        QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 50.0m, PriceDuo = 120.0m, PriceFamily = 80.0m
    },
    new SportTicket
    {
        SportName = "Tir à L'Arc",
        QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 50.0m, PriceDuo = 100.0m, PriceFamily = 200.0m
    },

      new SportTicket
    {
        SportName = "Judo",
        QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 50.0m, PriceDuo = 120.0m, PriceFamily = 80.0m
    },
    new SportTicket
    {
        SportName = "Gymnastique Artistique",
        QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 50.0m, PriceDuo = 100.0m, PriceFamily = 200.0m
    },

      new SportTicket
    {
        SportName = "Natation",
        QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 50.0m, PriceDuo = 120.0m, PriceFamily = 80.0m
    },
    new SportTicket
    {
        SportName = "Sports Équestres",
        QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 50.0m, PriceDuo = 100.0m, PriceFamily = 200.0m
    },
      new SportTicket
    {
        SportName = "Escrime",
        QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 50.0m, PriceDuo = 120.0m, PriceFamily = 80.0m
    },
    new SportTicket
    {
        SportName = "Tir",
        QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 50.0m, PriceDuo = 100.0m, PriceFamily = 200.0m
    },
        new SportTicket
    {
        SportName = "Volleyball de plage",
        QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 50.0m, PriceDuo = 100.0m, PriceFamily = 200.0m
    },

};


        private void DecreaseQuantity(SportTicket sport, TicketType ticketType)
        {
            switch (ticketType)
            {
                case TicketType.Solo:
                    if (sport.QuantitySolo > 0)
                        sport.QuantitySolo--;
                    break;

                case TicketType.Duo:
                    if (sport.QuantityDuo > 0)
                        sport.QuantityDuo--;
                    break;

                case TicketType.Family:
                    if (sport.QuantityFamily > 0)
                        sport.QuantityFamily--;
                    break;

                default:
                    break;
            }
        }

        private void IncreaseQuantity(SportTicket sport, TicketType ticketType)
        {
            switch (ticketType)
            {
                case TicketType.Solo:
                    sport.QuantitySolo++;
                    break;

                case TicketType.Duo:
                    sport.QuantityDuo++;
                    break;
                case TicketType.Family:
                    sport.QuantityFamily++;
                    break;

                default:
                    break;
            }
        }

    }
}