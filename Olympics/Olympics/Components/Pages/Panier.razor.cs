using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Olympics.Database.Services;
using Olympics.Metier.Business;
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

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender)
        //    {
        //        cartTickets = await PanierService.GetCartFromSessionAsync();
        //        TotalPriceSum();
        //        StateHasChanged(); // Mettre à jour l'interface utilisateur
        //    }
        //}

        private void TotalPriceSum()
        {
            if (cartTickets != null)
            {
                totalPrice = cartTickets.Sum(ticket => ticket.Quantity * ticket.Price);
            }
        }

        //private async Task OnCheckoutClicked()
        //{
        //    var utilisateur = await UserService.GetCurrentUserAsync(); 

        //    var panier = await PanierService.GetCurrentCartAsync(); 

        //    var montant = (int)totalPrice; // Convertir le total en montant

        //    var payementResult = await PayementService.MockPayementAsync(utilisateur, panier, montant);

        //    if (payementResult.IsSuccess)
        //    {
        //        // Rediriger vers le QR Code
        //        NavigationManager.NavigateTo(payementResult.QrCodeUrl);
        //    }
        //    else
        //    {
        //        // Gérer l'échec du paiement, par exemple afficher un message d'erreur
        //        NotificationService.Notify(new NotificationMessage { Severity = NotificationSeverity.Error, Summary = "Échec du paiement", Detail = "Une erreur s'est produite lors du paiement.", Duration = 4000 });
        //    }
        //}





    }
}
