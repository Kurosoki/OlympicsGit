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


        // Déclarez une instance de cUtilisateurBase
        private cUtilisateurBase user = new cUtilisateurBase();

        private bool isRegistered = false;
        private bool registrationFailed = false;

        // Méthode pour gérer la soumission du formulaire
        public async Task RegisterUser()
        {
            // Récuperer les propriétés du modèle
            string nomUtilisateur = user.NomClient;
            string prenomUtilisateur = user.PrenomClient;
            string emailUtilisateur = user.EmailClient;
            string motDePasseUtilisateur = user.ShaMotDePasse;
            string motDePasseConfirmation = user.ShaMotDePasseVerif;

            // Vérifiez que le mot de passe possède les caractéristiques attendues
            if (!EstMotDePasseValide(motDePasseUtilisateur))
            {
                registrationFailed = true;
                NotificationService.Notify(NotificationSeverity.Error, "Erreur", "Le mot de passe doit contenir au moins une majuscule, une minuscule, un chiffre, un caractère spécial et être d'une longueur minimale de 8 caractères.");
                return;
            }

            // Vérifiez que les mots de passe correspondent
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

            // Créer un nouvel utilisateur avec les informations fournies dans le formulaire
            var result = await userService.RegisterUserAsync(user);

            if (result)
            {
                isRegistered = true;
                NotificationService.Notify(NotificationSeverity.Success, "Succès", "Inscription réussie.");
                NavigationManager.NavigateTo("/login");
            }
            else
            {
                registrationFailed = true;
                NotificationService.Notify(NotificationSeverity.Error, "Erreur", "Cette adresse e-mail est déjà utilisée.");
            }
        }

        private bool EstMotDePasseValide(string motDePasseUtilisateur)
        {
            // Vérifier la longueur minimale
            if (motDePasseUtilisateur.Length < 8)
            {
                return false;
            }

            // Vérifier la présence de majuscules, minuscules, chiffres et symboles
            if (!motDePasseUtilisateur.Any(char.IsUpper) ||      // Au moins une majuscule
                !motDePasseUtilisateur.Any(char.IsLower) ||      // Au moins une minuscule
                !motDePasseUtilisateur.Any(char.IsDigit) ||      // Au moins un chiffre
                !motDePasseUtilisateur.Any(ch => !char.IsLetterOrDigit(ch))) // Au moins un caractère spécial
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