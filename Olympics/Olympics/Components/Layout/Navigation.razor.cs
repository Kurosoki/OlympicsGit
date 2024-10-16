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
using Olympics.Presentation.Components.Pages;

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

        [Inject]
        private SessionService SessionService { get; set; }

        private bool isAdmin = false;
        private bool isUserLoggedIn = false;

        protected override async Task OnInitializedAsync()
        {
            isUserLoggedIn = await SessionService.ValidateUserSessionAsync();

            if (isUserLoggedIn)
            {
                CheckIfUserIsAdminAsync();
            }
        }


        private void CheckIfUserIsAdminAsync()
        {
            isAdmin = SessionService.GetUserStatus();
        }

        private async Task Logout()
        {
            
            if (await SessionService.ValidateUserSessionAsync())
            {
                await UserService.LogoutUserAsync(); 
            }

            isUserLoggedIn = false; 
            NavigationManager.NavigateTo("/", forceLoad: true);
        }


        //private async Task ScrollToTop()
        //{
        //    await JSRuntime.InvokeVoidAsync("scrollToElement", "topOfPageId"); 
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


        private void NavigateTableauDeBord()
        {
            NavigationManager.NavigateTo("/tableau-de-bord");
        }

    }
}