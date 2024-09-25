using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Olympics.Metier.Utils;
using Olympics.Metier.Business;
using Olympics.Services;
using Blazorise;
using Olympics.Database.Services;


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

        [Inject]
        private PanierService PanierService { get; set; }

        [Inject]
        private UserService UserService { get; set; }

        [Inject]
        private OffresService OffresService { get; set; }



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
         SportName = "Athl�tisme",
         QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 20.0m, PriceDuo = 35.0m, PriceFamily = 150.0m
    },
    new SportTicket
    {
         SportName = "Tir � L'Arc",
         QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 20.0m, PriceDuo = 35.0m, PriceFamily = 150.0m
    },

      new SportTicket
    {
         SportName = "Judo",
         QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 20.0m, PriceDuo = 35.0m, PriceFamily = 150.0m
    },
    new SportTicket
    {
         SportName = "Gymnastique Artistique",
         QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 20.0m, PriceDuo = 35.0m, PriceFamily = 150.0m
    },

      new SportTicket
    {
         SportName = "Natation",
         QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 20.0m, PriceDuo = 35.0m, PriceFamily = 150.0m
    },
    new SportTicket
    {
         SportName = "Sports �questres",
         QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 20.0m, PriceDuo = 35.0m, PriceFamily = 150.0m
    },
      new SportTicket
    {
         SportName = "Escrime",
         QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 20.0m, PriceDuo = 35.0m, PriceFamily = 150.0m
    },
    new SportTicket
    {
         SportName = "Tir",
         QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 20.0m, PriceDuo = 35.0m, PriceFamily = 150.0m
    },
        new SportTicket
    {
         SportName = "Volleyball de plage",
         QuantitySolo = 0, QuantityDuo = 0, QuantityFamily = 0,
         PriceSolo = 20.0m, PriceDuo = 35.0m, PriceFamily = 150.0m
    },

};


        private void DecreaseQuantity(SportTicket sport, TicketTypeManager.TicketType ticketType)
        {
            switch (ticketType)
            {
                case TicketTypeManager.TicketType.Solo:
                    if (sport.QuantitySolo > 0)
                        sport.QuantitySolo--;
                    break;

                case TicketTypeManager.TicketType.Duo:
                    if (sport.QuantityDuo > 0)
                        sport.QuantityDuo--;
                    break;

                case TicketTypeManager.TicketType.Family:
                    if (sport.QuantityFamily > 0)
                        sport.QuantityFamily--;
                    break;

                default:
                    break;
            }
        }

        private void IncreaseQuantity(SportTicket sport, TicketTypeManager.TicketType ticketType)
        {
            switch (ticketType)
            {
                case TicketTypeManager.TicketType.Solo:
                    sport.QuantitySolo++;
                    break;

                case TicketTypeManager.TicketType.Duo:
                    sport.QuantityDuo++;
                    break;

                case TicketTypeManager.TicketType.Family:
                    sport.QuantityFamily++;
                    break;

                default:
                    break;
            }
        }


        private async Task AddTicketToPanier(SportTicket sportTicket)
        {

            if (sportTicket.QuantitySolo > 0)
            {
                await AjouterBilletAuPanier(sportTicket, TicketTypeManager.TicketType.Solo, sportTicket.QuantitySolo, sportTicket.PriceSolo);
            }

            if (sportTicket.QuantityDuo > 0)
            {
                await AjouterBilletAuPanier(sportTicket, TicketTypeManager.TicketType.Duo, sportTicket.QuantityDuo, sportTicket.PriceDuo);
            }

            if (sportTicket.QuantityFamily > 0)
            {
                await AjouterBilletAuPanier(sportTicket, TicketTypeManager.TicketType.Family, sportTicket.QuantityFamily, sportTicket.PriceFamily);
            }
        }

        private async Task AjouterBilletAuPanier(SportTicket sportTicket, TicketTypeManager.TicketType ticketType, int quantity, decimal price)
        {
            var userId = await UserService.GetAuthenticatedUserIdAsync();

            if (userId == null)
            {
                // Utilisateur non authentifi� : stocker dans sessionStorage
                var cartTickets = await PanierService.GetCartFromSessionAsync();

                var ticket = new cTicket
                {
                    SportName = sportTicket.SportName,
                    TicketType = ticketType,
                    Quantity = quantity,
                    Price = price
                };

                cartTickets.Add(ticket);
                await PanierService.SetCartInSessionAsync(cartTickets);
            }
            else
            {
                // Utilisateur authentifi� : stocker dans la base de donn�es
                var panier = await PanierService.GetPanierByIdAsync(userId.Value);

                if (panier == null)
                {
                    panier = new cPanierBase
                    {
                        IDClient = userId.Value, // Associer le panier � l'utilisateur
                    };
                }

                var ticket = new cTicket
                {
                    IDPanier = panier.IDPanier, // Lier le ticket au panier
                    SportName = sportTicket.SportName,
                    TicketType = ticketType,
                    Quantity = quantity,
                    Price = price
                };

                panier.Tickets.Add(ticket);
                await PanierService.UpdatePanierAsync(panier);
            }
        }



        private bool isAdmin = false; // Remplacez ceci par votre logique de vérification d'admin
        private bool isConnected = false;
       // private Offer newOffer = new Offer(); // Remplacez par votre modèle d'offre



        private async Task IsAdmin(bool isAdmin)
        {
            if (isConnected)
            {
                if (isAdmin)
                {
                    // Logique pour les administrateurs
                }
                else
                {
                    // Logique pour les utilisateurs normaux
                }
            }
            else
            {
                // Logique pour un échec de connexion
            }

        }



        //private async Task SaveOffer()
        //{
        //    // Logique pour ajouter ou modifier une offre dans la base de données
        //    if (newOffer.ID == 0)
        //    {
        //        // Ajouter une nouvelle offre
        //        await OffresService.AddOffer(newOffer);
        //    }
        //    else
        //    {
        //        // Modifier une offre existante
        //        await OffresService.UpdateOffer(newOffer);
        //    }

        //    // Réinitialiser le formulaire
        //    newOffer = new Offer();
        //}

        //private async Task DeleteOffer(int offerId)
        //{
        //    // Logique pour supprimer l'offre de la base de données
        //    await OffresService.DeleteOffer(offerId);
        //}









    }
}