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
        private List<cTicket> sessionCartTickets = new List<cTicket>();
        private cPanierBase panierBdd;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // Vérifiez si l'utilisateur est connecté
                var isUserLoggedIn = await SessionService.ValidateUserSessionAsync();
                sessionCartTickets = await PanierService.GetCartFromSessionAsync(); // Récupérer les tickets de session

                if (isUserLoggedIn)
                {
                    var utilisateur = await UserService.GetAuthenticatedUserAsync();
                    if (utilisateur != null)
                    {
                        // Récupérez le panier de l'utilisateur à partir de la base de données
                        panierBdd = await PanierService.GetPanierByUserIdAsync(utilisateur.IDClient);

                        // Si le panier existe, ajoutez ses tickets à cartTickets
                        if (panierBdd != null)
                        {
                            cartTickets.AddRange(panierBdd.Tickets);
                        }
                    }
                }

                else
                {
                    // Si l'utilisateur n'est pas connecté, utilisez uniquement les tickets de session
                    cartTickets = sessionCartTickets;
                }

                TotalPriceSum(); // Calculez le total des prix
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

            if (panierBdd?.Tickets != null)
            {
                totalPrice += panierBdd.Tickets.Sum(ticket => ticket.Quantity * ticket.Price);
            }
        }


        private async Task OnCheckoutClicked()
        {
            // Vérifiez si l'utilisateur est connecté
            var isUserLoggedIn = await SessionService.ValidateUserSessionAsync();

            if (isUserLoggedIn)
            {

                // Récupérez l'utilisateur connecté et les informations sur le panier
                var utilisateur = await UserService.GetAuthenticatedUserAsync();
                var panier = await PanierService.GetPanierByUserIdAsync(utilisateur.IDClient); // Récupérez le panier de l'utilisateur
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
            // Essayer de récupérer l'utilisateur authentifié (peut être null si non connecté)
            var utilisateur = await UserService.GetAuthenticatedUserAsync();

            // Confirmer l'action de vider le panier
            var confirmation = await DialogService.Confirm("Êtes-vous sûr de vouloir vider le panier ?", "Confirmation");
            if (confirmation.HasValue && confirmation.Value)
            {
                // Vider le panier en session (cela peut être fait sans authentification)
                await PanierService.ClearCartFromSessionAsync();
                cartTickets = new List<cTicket>();
                totalPrice = 0;

                // Si l'utilisateur est connecté, vider également le panier de la base de données
                if (utilisateur != null)
                {
                    var panier = await PanierService.GetPanierByUserIdAsync(utilisateur.IDClient);
                    if (panier != null)
                    {
                        await PanierService.DeletePanierAsync(panier.IDPanier);
                    }
                }

                // Envoyer une notification à l'utilisateur
                NotificationService.Notify(NotificationSeverity.Info, "Panier vidé", "Votre panier a été vidé avec succès.");
                StateHasChanged();
            }
        }




    }
}

