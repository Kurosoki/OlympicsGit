using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using Radzen;
using Olympics.Presentation.ServicesAuthen;



namespace Olympics.Presentation.Components.Layout
{
    public partial class MainLayout
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
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        [Inject]
        private AuthenticationService AuthenticationService { get; set; }



        private bool isLoggedIn;
        private string userEmail;

        protected override async Task OnInitializedAsync()
        {
            // Vérifier si l'utilisateur est authentifié
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            isLoggedIn = user.Identity.IsAuthenticated;

            // Si connecté, obtenir l'email de l'utilisateur ( si on veut personaliser et afficher l'email usr ex bjr dia..)
            if (isLoggedIn)
            {
                userEmail = user.Identity.Name; // Par exemple, récupérer l'email
            }
        }

        private async Task OnLogout()
        {
            await AuthenticationService.LogoutAsync();
            NavigationManager.NavigateTo("/");
        }






        private void NavigateToLogin()
        {
            NavigationManager.NavigateTo("/login");
        }

        private void NavigateToAccueil()
        {
            NavigationManager.NavigateTo("/");
        }

        private void NavigateToBilletterie()
        {
            NavigationManager.NavigateTo("/billetterie");
        }

        private void NavigateToPanier()
        {
            NavigationManager.NavigateTo("/panier"); //lorsque l'user clique sur btn payer rediriger vers la page de connexion
        }

    }
}
