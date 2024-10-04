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

namespace Olympics.Presentation.Components.Pages
{
    public partial class Confirmation
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



        private string qrCodeBase64;
        private string dateachat;
        private string montant;

        protected override void OnInitialized()
        {
            // Récupérer les paramètres de l'URL
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            qrCodeBase64 = query["qrCodeBase64"];
            dateachat = query["dateAchat"];
            montant = query["montant"];
        }



    }
}