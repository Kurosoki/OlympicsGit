using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.EntityFrameworkCore;
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
        private OffreService OffreService { get; set; }

        [Inject]
        private SessionService SessionService { get; set; }

        [Inject]
        private SportTicketManager SportTicketManager { get; set; }


        private SportTicket currentSportTicket = new SportTicket();
        private List<SportTicket> sportTickets; // Utiliser SportTicket comme modèle de vue
        private bool isUserLoggedIn = false;


        protected override async Task OnInitializedAsync()
        {
            await LoadSportTickets();

            isUserLoggedIn = await SessionService.ValidateUserSessionAsync();

            if (isUserLoggedIn)
            {
                CheckIfUserIsAdminAsync();
            }
        }


        private async Task LoadSportTickets()
        {
            sportTickets = SportTicketManager.MapToSportTickets(await OffreService.GetAllOffersAsync());
        }

        private bool isAdmin = false;
        bool isEditMode = false; // false = création, true = modification

        private void CheckIfUserIsAdminAsync()
        {
            isAdmin = SessionService.GetUserStatus();
        }


        // Méthode pour basculer en mode modification
        private async void EditOffer(SportTicket sportTicket)
        {
            currentSportTicket = sportTicket;
            isEditMode = true;
            BeginEdit(sportTicket);
        }


        // Méthode pour basculer en mode création
        private void CreateNewOffer()
        {
            currentSportTicket = new SportTicket();  // Réinitialise le modèle
            isEditMode = false;
            StateHasChanged();
        }

        // Méthode de gestion des soumissions
        private async Task HandleSubmit()
        {
            if (isEditMode)
            {
                // Appeler la méthode de modification
                await UpdateBilletOffre(currentSportTicket);
            }
            else
            {
                // Appeler la méthode de création
                await CreateBilletOffre();
            }
        }

        // Création d'une nouvelle offre
        private async Task CreateBilletOffre()
        {
            // S'assurer que les champs obligatoires sont remplis
            if (string.IsNullOrWhiteSpace(currentSportTicket.SportName) ||
                string.IsNullOrWhiteSpace(currentSportTicket.Description) ||
                currentSportTicket.PriceSolo <= 0 ||
                currentSportTicket.PriceDuo <= 0 ||
                currentSportTicket.PriceFamily <= 0)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erreur", "Veuillez remplir tous les champs obligatoires avec des valeurs valides.");
                return;
            }

            try
            {
                // Enregistrement de l'offre si les champs sont valides
                await OffreService.SaveSportTicketAsync(currentSportTicket);
                NotificationService.Notify(NotificationSeverity.Success, "Succès", "L'offre a été enregistrée avec succès.");

                await LoadSportTickets(); // Recharge les tickets
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erreur", $"Échec de l'enregistrement de l'offre : {ex.Message}");
            }

            // Réinitialise le modèle après l'enregistrement ou l'échec
            currentSportTicket = new SportTicket();
        }


        // Mise à jour d'une offre existante
        private async Task UpdateBilletOffre(SportTicket currentSportTicket)
        {
            try
            {
                // Comparer les valeurs de l'objet original avec l'objet actuel
                if (!HasChanges(currentSportTicket, originalSportTicket))
                {
                    NotificationService.Notify(NotificationSeverity.Warning, "Aucune modification", "Aucune modification n'a été détectée.");
                    return;
                }

                await OffreService.UpdateOffreAsync(currentSportTicket);
                NotificationService.Notify(NotificationSeverity.Success, "Succès", "L'offre a été mise à jour avec succès.");

                // Réinitialiser ou synchroniser avec le nouvel état
                originalSportTicket = new SportTicket
                {
                    Description = currentSportTicket.Description,
                    PriceSolo = currentSportTicket.PriceSolo,
                    PriceDuo = currentSportTicket.PriceDuo,
                    PriceFamily = currentSportTicket.PriceFamily,
                    SportName = currentSportTicket.SportName
                };
              
                await LoadSportTickets();

                CreateNewOffer();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erreur", $"Échec de la mise à jour de l'offre : {ex.Message}");
            }
        }

        // Méthode pour comparer les valeurs initiales et modifiées
        private bool HasChanges(SportTicket current, SportTicket original)
        {
            return current.Description != original.Description ||
                   current.PriceSolo != original.PriceSolo ||
                   current.PriceDuo != original.PriceDuo ||
                   current.PriceFamily != original.PriceFamily ||
                   current.SportName != original.SportName;
        }

        private SportTicket originalSportTicket;

        private void BeginEdit(SportTicket ticket)
        {
            // Créer une copie du ticket original avant les modifications
            originalSportTicket = new SportTicket
            {
                Description = ticket.Description,
                ImageUrl = ticket.ImageUrl,
                PriceSolo = ticket.PriceSolo,
                PriceDuo = ticket.PriceDuo,
                PriceFamily = ticket.PriceFamily,
                SportName = ticket.SportName
            };

            currentSportTicket = ticket;
            isEditMode = true;
        }


        // Suppression d'une offre
        private async Task DeleteBilletOffre(int idoffre)
        {
            try
            {
                await OffreService.DeleteOffreAsync(idoffre);
                NotificationService.Notify(NotificationSeverity.Success, "Succès", "L'offre a été supprimée avec succès.");

                await LoadSportTickets();
            }
            catch (Exception ex)
            {
                NotificationService.Notify(NotificationSeverity.Error, "Erreur", $"Échec de la suppression de l'offre : {ex.Message}");
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

            NotificationService.Notify(NotificationSeverity.Info, "Ticket ajouté au panier avec succès.");
        }


    }
}