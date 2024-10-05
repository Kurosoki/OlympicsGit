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
using System.Web;

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
        private string dateAchat;
        private decimal montant;

        protected override void OnInitialized()
        {
            // Récupérer les paramètres de l'URL
            var uri = new Uri(NavigationManager.Uri);
            var queryParams = HttpUtility.ParseQueryString(uri.Query);

            // Récupérer et décoder chaque paramètre
            qrCodeBase64 = queryParams["qrCodeBase64"];
            dateAchat = HttpUtility.UrlDecode(queryParams["dateAchat"]);
            montant = decimal.Parse(HttpUtility.UrlDecode(queryParams["montant"])); 
        }

        private void NavigateToHome()
        {
            NavigationManager.NavigateTo("/");
        }

    }
}