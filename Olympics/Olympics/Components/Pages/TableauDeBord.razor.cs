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
using Olympics.Metier.Models;
using Olympics.Services;

namespace Olympics.Presentation.Components.Pages
{
    public partial class TableauDeBord
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
        protected SessionService SessionService { get; set; }

        [Inject]
        protected PanierService PanierService { get; set; }

        [Inject]
        protected ArchiveService TicketSalesInfoService { get; set; }



        private bool isUserLoggedIn = false;

        private List<cArchiveBase> ventesParOffre;


        protected override async Task OnInitializedAsync()
        {
            isUserLoggedIn = await SessionService.ValidateUserSessionAsync();

            if (isUserLoggedIn)
            {
                ventesParOffre = await TicketSalesInfoService.GetVentesParOffreAsync();
            }
        }
    }
}