using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

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