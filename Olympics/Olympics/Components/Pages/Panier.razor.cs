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

        [Inject]
        private SessionService SessionService { get; set; }


        private decimal totalPrice;
        private List<cTicket> cartTickets = new List<cTicket>();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Vérifiez si l'utilisateur est connecté
                var isUserLoggedIn = await SessionService.ValidateUserSessionAsync();

                if (isUserLoggedIn)
                {
                    // Si connecté, récupérez le panier de la BDD
                    var panier = PanierService.ObtenirPanier();

                    if (panier != null)
                    {
                        cartTickets = panier.Tickets;
                    }
                }
                else
                {
                    // Si non connecté, récupérez le panier en sessionStorage
                    cartTickets = await PanierService.GetCartFromSessionAsync();
                }

                TotalPriceSum(); 
                StateHasChanged(); 
            }
        }


        private void TotalPriceSum()
        {
            totalPrice = 0; // Réinitialiser le total à chaque appel
            if (cartTickets != null)
            {
                totalPrice += cartTickets.Sum(ticket => ticket.Quantity * ticket.Price);
            }
        }


        private async Task OnCheckoutClicked()
        {
            // Vérifiez si l'utilisateur est connecté
            var isUserLoggedIn = await SessionService.ValidateUserSessionAsync();

            if (isUserLoggedIn)
            {
                var panier = PanierService.ObtenirPanier();

                // Vérifier si le panier n'est pas vide avant de procéder au payement
                if (panier.Tickets == null || !panier.Tickets.Any()) //vérifier si la liste des tickets est vide.
                {
                    NotificationService.Notify(NotificationSeverity.Warning, "Panier vide", "Votre panier ne contient aucun billet.");
                    return; // Sortir de la méthode si le panier est vide
                }

                var utilisateur = await UserService.GetUserByIdAsync(panier.IDClient);
                var montant = totalPrice;

                // Effectuez le paiement
                var payementResult = await PayementService.MockPayementAsync(utilisateur, panier, montant);

                if (payementResult.IsSuccess)
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
                NotificationService.Notify(NotificationSeverity.Error, "Une connexion est nécessaire pour poursuivre vos achats.");
            }
        }




        private async Task ViderPanierCompletAsync()
        {
            // On récupérer l'utilisateur authentifié grâce a son panier
            var panier = PanierService.ObtenirPanier();

            // Confirmer l'action de vider le panier
            var confirmation = await DialogService.Confirm("Êtes-vous sûr de vouloir vider le panier ?", "Confirmation");
            if (confirmation.HasValue && confirmation.Value)
            {
                // Vider le panier en session (cela peut être fait sans authentification)
                await PanierService.ClearCartFromSessionAsync();
                cartTickets = new List<cTicket>();
                totalPrice = 0;

                // Si l'utilisateur est connecté, vider également le panier de la base de données
                if (panier != null)
                {
                   await PanierService.RemoveTicketsFromPanierAsync(panier.IDPanier);
                }

                // Envoyer une notification à l'utilisateur
                NotificationService.Notify(NotificationSeverity.Info, "Panier vidé", "Votre panier a été vidé avec succès.");
                StateHasChanged();
            }
        }




    }
}

