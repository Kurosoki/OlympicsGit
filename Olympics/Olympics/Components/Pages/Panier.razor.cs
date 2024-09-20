using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
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




    }

}