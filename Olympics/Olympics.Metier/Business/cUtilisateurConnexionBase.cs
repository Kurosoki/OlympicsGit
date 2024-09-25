using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Olympics.Metier.Business
{
    [Table("utilisateurconnexion")] 
    public class cUtilisateurConnexionBase
    {
        [Required]
        [EmailAddress]
        [Column("emailclient")] 
        public string EmailClient { get; set; }

        [Required]
        [Column("shamotdepasse")] 
        public string ShaMotDePasse { get; set; }

        [Column("rememberme")] 
        public bool RememberMe { get; set; }
    }
}
