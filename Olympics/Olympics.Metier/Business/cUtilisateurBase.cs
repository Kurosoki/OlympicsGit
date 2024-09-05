using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympics.Metier.Business
{
    public class cUtilisateurBase 
    {
        public int IDClient { get; set; }

        [Required]
        public string NomClient { get; set; }

        [Required]
        public string PrenomClient { get; set; }

        [Required]
        [EmailAddress]
        public string EmailClient { get; set; }

        [Required]
        public string ShaMotDePasse { get; set; }

        [Required]
        public string ShaMotDePasseVerif { get; set; }

        public string Salt { get; set; }

        public string Key { get; set; }

        public RoleUtilisateur RoleUtilisateur { get; set; }

    }

    public enum RoleUtilisateur
    {
        Utilisateur,
        Administrateur,
    }
}
