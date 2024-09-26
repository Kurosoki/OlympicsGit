using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Olympics.Database.Services;
using Olympics.Metier.Models;
using Olympics.Services;
using Radzen;

namespace Olympics.Presentation.Components.Pages
{
    public partial class Panier
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
        private PayementService PayementService { get; set; }

        [Inject]
        private UserService UserService { get; set; }


        private decimal totalPrice;
        private List<cTicket> cartTickets;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                cartTickets = await PanierService.GetCartFromSessionAsync();
                TotalPriceSum();
                StateHasChanged(); // Mettre à jour l'interface utilisateur
            }
        }

        private void TotalPriceSum()
        {
            if (cartTickets != null)
            {
                totalPrice = cartTickets.Sum(ticket => ticket.Quantity * ticket.Price);
            }
        }

        private async Task OnCheckoutClicked()
        {
            // Vérifiez si l'utilisateur est connecté
            var isUserLoggedIn = await SessionService.ValidateUserSessionAsync(); // Implémentez cette méthode pour vérifier la session

            if (isUserLoggedIn)
            {
                // Récupérez l'utilisateur connecté et les informations sur le panier
                var utilisateur = await UserService.GetCurrentUserAsync(); // Implémentez cette méthode pour obtenir l'utilisateur actuel
                var panier = await PanierService.GetPanierByIdAsync(utilisateur.idPanier); // Récupérez le panier de l'utilisateur
                var montant = totalPrice; // Ou calculez le montant total selon vos besoins

                // Effectuez le paiement
                var payementResult = await PayementService.MockPayementAsync(utilisateur, panier, montant);

                if (payementResult)
                {
                    NotificationService.Notify(NotificationSeverity.Success, "Succès", "Paiement réussi.");
                    NavigationManager.NavigateTo("/confirmation"); // Redirigez vers une page de confirmation
                }
                else
                {
                    NotificationService.Notify(NotificationSeverity.Error, "Erreur", "Échec du paiement.");
                }
            }
            else
            {
                // Redirigez l'utilisateur vers la page de connexion
                NavigationManager.NavigateTo("/login");
            }
        }





    }
}
