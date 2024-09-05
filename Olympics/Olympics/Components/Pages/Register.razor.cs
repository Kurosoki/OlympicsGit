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


        // D�clarez une instance de cUtilisateurBase
        private cUtilisateurBase user = new cUtilisateurBase();

        private bool isRegistered = false;
        private bool registrationFailed = false;

        // M�thode pour g�rer la soumission du formulaire
        public async Task RegisterUser()
        {
            // R�cuperer les propri�t�s du mod�le
            string nomUtilisateur = user.NomClient;
            string prenomUtilisateur = user.PrenomClient;
            string emailUtilisateur = user.EmailClient;
            string motDePasseUtilisateur = user.ShaMotDePasse;
            string motDePasseConfirmation = user.ShaMotDePasseVerif;

            // V�rifiez que le mot de passe poss�de les caract�ristiques attendues
            if (!EstMotDePasseValide(motDePasseUtilisateur))
            {
                registrationFailed = true;
                NotificationService.Notify(NotificationSeverity.Error, "Erreur", "Le mot de passe doit contenir au moins une majuscule, une minuscule, un chiffre, un caract�re sp�cial et �tre d'une longueur minimale de 8 caract�res.");
                return;
            }

            // V�rifiez que les mots de passe correspondent
            if (motDePasseUtilisateur != motDePasseConfirmation)
            {
                registrationFailed = true;
                return;
            }

            // Valider que l'e-mail a un format valide
            if (!EstEmailValide(emailUtilisateur))
            {
                registrationFailed = true;
                NotificationService.Notify(NotificationSeverity.Error, "Erreur", "Veuillez entrer une adresse e-mail valide.");
                return;
            }

            // Cr�er un nouvel utilisateur avec les informations fournies dans le formulaire
            var result = await userService.RegisterUserAsync(user);

            if (result)
            {
                isRegistered = true;
                NotificationService.Notify(NotificationSeverity.Success, "Succ�s", "Inscription r�ussie.");
                NavigationManager.NavigateTo("/login");
            }
            else
            {
                registrationFailed = true;
                NotificationService.Notify(NotificationSeverity.Error, "Erreur", "Cette adresse e-mail est d�j� utilis�e.");
            }
        }

        private bool EstMotDePasseValide(string motDePasseUtilisateur)
        {
            // V�rifier la longueur minimale
            if (motDePasseUtilisateur.Length < 8)
            {
                return false;
            }

            // V�rifier la pr�sence de majuscules, minuscules, chiffres et symboles
            if (!motDePasseUtilisateur.Any(char.IsUpper) ||      // Au moins une majuscule
                !motDePasseUtilisateur.Any(char.IsLower) ||      // Au moins une minuscule
                !motDePasseUtilisateur.Any(char.IsDigit) ||      // Au moins un chiffre
                !motDePasseUtilisateur.Any(ch => !char.IsLetterOrDigit(ch))) // Au moins un caract�re sp�cial
            {
                return false;
            }

            return true;
        }

        private bool EstEmailValide(string emailUtilisateur)
        {
            // Convertir l'e-mail en minuscules avant la validation
            string emailEnMinuscules = emailUtilisateur.ToLower();

            string pattern = @"^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            return System.Text.RegularExpressions.Regex.IsMatch(emailEnMinuscules, pattern);
        }
    }


}