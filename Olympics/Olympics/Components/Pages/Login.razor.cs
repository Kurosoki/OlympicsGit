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
using Olympics.Database.Services;

namespace Olympics.Presentation.Components.Pages
{
    public partial class Login
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


        private cUtilisateurConnexionBase loginUser = new cUtilisateurConnexionBase();

        private bool loginFailed = false;

        private async Task LoginUser()
        {
            // Valider que l'e-mail a un format valide
            if (!EstEmailValide(loginUser.EmailClient))
            {
                loginFailed = true;
                NotificationService.Notify(NotificationSeverity.Error, "Erreur", "Veuillez entrer une adresse e-mail valide.");
                return;
            }

            // Essayer de se connecter avec les informations fournies
            var result = await userService.LoginUserAsync(loginUser);

            if (result)
            {
                NotificationService.Notify(NotificationSeverity.Success, "Succès", "Connexion réussie.");
                NavigationManager.NavigateTo("/");
            }
            else
            {
                loginFailed = true;
                NotificationService.Notify(NotificationSeverity.Error, "Erreur", "Le mot de passe n'est pas correct.");

            }
        }

        private bool EstEmailValide(string emailUtilisateur)
        {
            string emailEnMinuscules = emailUtilisateur.ToLower();
            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return System.Text.RegularExpressions.Regex.IsMatch(emailEnMinuscules, pattern);
        }
    

    private void NavigateToForgotPassword() //Idée de rajout
        {
            // Rediriger vers la page de réinitialisation de mot de passe
            NavigationManager.NavigateTo("/forgot-password");
        }


    }
}