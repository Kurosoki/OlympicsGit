using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Olympics.Metier.Business;

namespace Olympics.Presentation.Components.Pages
{
    public partial class Register
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



        // Déclarez une instance de cUtilisateurBase
        private cUtilisateurBase user = new cUtilisateurBase();


        // Méthode pour gérer la soumission du formulaire
        private void HandleSubmit()
        {
            // Logique de soumission, par exemple, enregistrer l'utilisateur
        }










    }

}