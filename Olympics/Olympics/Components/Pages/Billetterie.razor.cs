using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using Olympics.Database;
using Olympics.Database.Services;
using Olympics.Metier.Models;
using Olympics.Metier.Utils;
using Olympics.Services;
using Radzen;


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
        private OffreService OffreService { get; set; }

        [Inject]
        private SessionService SessionService { get; set; }

        [Inject]
        private ILogger<Billetterie> _logger { get; set; }

        [Inject]
        private ApplicationDbContext Context { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await CheckIfUserIsAdminAsync();
        }

        // Méthode de soumission du formulaire
        private async Task CreateSportTicket()
        {
            await OffreService.SaveSportTicketAsync(newSportTicket);
            newSportTicket = new SportTicket(); // Réinitialiser le modèle
        }


        private bool onlineUser = false;
        private bool isAdmin = false;

        private async Task CheckIfUserIsAdminAsync()
        {

            var (isConnected, isAdminStatus) = SessionService.GetUserStatus();

            // Mettre à jour les variables membres
            onlineUser = isConnected;
            isAdmin = isAdminStatus;

            StateHasChanged(); 
        }

        private SportTicket newSportTicket = new SportTicket();

        List<SportTicket> sportTickets = new List<SportTicket>();


        private async Task Image()
        { 

        }

        private async Task LoadImage(InputFileChangeEventArgs e)
        {
            // Récupère le fichier téléchargé
            var file = e.GetMultipleFiles(1).FirstOrDefault(); // Prend le premier fichier téléchargé

            // Vérifiez que le fichier existe
            if (file != null)
            {
                // Lire le fichier et l'afficher
                var buffer = new byte[file.Size];
                await file.OpenReadStream().ReadAsync(buffer);

                // Convertir le fichier en URL Base64
                newSportTicket.ImageUrl = $"data:{file.ContentType};base64,{Convert.ToBase64String(buffer)}";
            }
        }



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
            bool isUserLoggedIn = await SessionService.ValidateUserSessionAsync();

            // On récupère le panier actuel pour obtenir l'ID de l'utilisateur
            var panier = PanierService.ObtenirPanier();

            if (panier == null || panier.IDClient == 0)
            {
                // Utilisateur non authentifié ou ID non trouvé : stocker dans sessionStorage
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
                // Utilisateur authentifié : stocker dans la base de données
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

                PanierService.MettreAJourPanier(panier);
            }

            NotificationService.Notify(NotificationSeverity.Info,"Ticket ajouté au panier avec succès.");
        }


    }
}