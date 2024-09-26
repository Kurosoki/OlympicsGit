using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Olympics.Metier.Models
{
    [Table("utilisateurs")]
    public class cUtilisateurBase 
    {
        [Column("idclient")] 
        public int IDClient { get; set; }

        [Required]
        [Column("nomclient")] 
        public string NomClient { get; set; }

        [Required]
        [Column("prenomclient")] 
        public string PrenomClient { get; set; }

        [Required]
        [EmailAddress]
        [Column("emailclient")] 
        public string EmailClient { get; set; }

        [Required]
        [Column("shamotdepasse")] 
        public string ShaMotDePasse { get; set; }

        [Column("salt")] 
        public string Salt { get; set; }

        [Column("key")] 
        public string Key { get; set; }

        [Column("roleutilisateur")] 
        public RoleUtilisateur RoleUtilisateur { get; set; }
    }

    public enum RoleUtilisateur
    {
        Utilisateur,
        Administrateur,
    }

}
