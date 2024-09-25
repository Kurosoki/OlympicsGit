using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Olympics.Database.Services;
using Olympics.Services;

namespace Olympics.Presentation.Components.Layout
{
    public partial class Navigation
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
        private UserService UserService { get; set; }


        private bool isLoggedIn = false;

        //protected override async Task OnInitializedAsync()
        //{
        //    var userId = await UserService.GetAuthenticatedUserIdAsync();
        //    isLoggedIn = userId.HasValue;
        //}

        //private async Task Logout()
        //{
        //    await UserService.LogoutUserAsync();
        //    isLoggedIn = false; // Mettre à jour l'état après la déconnexion
        //}

        private void NavigateToAccueil()
        {
            NavigationManager.NavigateTo("/");
        }

        private void NavigateToLogin()
        {
            NavigationManager.NavigateTo("/login");
        }

        private void NavigateToBilletterie()
        {
            NavigationManager.NavigateTo("/billetterie");
        }

        private void NavigateToPanier()
        {
            NavigationManager.NavigateTo("/panier");
        }



    }
}