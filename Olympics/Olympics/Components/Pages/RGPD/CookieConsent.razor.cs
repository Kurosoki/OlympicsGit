using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;

namespace Olympics.Presentation.Components.Pages.RGPD
{
    public partial class CookieConsent
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


        private bool isConsentGiven = false;

        protected override async Task OnInitializedAsync()
        {
            // Vérifie si l'utilisateur a déjà donné son consentement
            isConsentGiven = await JSRuntime.InvokeAsync<bool>("getCookie", "cookieConsent");
        }

        private async Task AcceptCookies()
        {
            // Stocke le consentement dans les cookies
            await JSRuntime.InvokeVoidAsync("setCookie", "cookieConsent", "true", 365);
            isConsentGiven = true;
        }

        private async Task DeclineCookies()
        {
            // Si l'utilisateur refuse, le consentement n'est pas stocké
            await JSRuntime.InvokeVoidAsync("deleteCookie", "cookieConsent");
            isConsentGiven = false;
        }



    }
}