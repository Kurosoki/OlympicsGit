using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympics.Metier.Business
{
    public class cUtilisateurBase
    {
        public int IDClient { get; set; }
        public string NomClient { get; set; }

        public string PrenomClient { get; set; }

        public string EmailClient { get; set; }

        public string ShaMotDePasse { get; set; }

        public string ShaMotDePasseVerif { get; set; }

        public RoleUtilisateur RoleUtilisateur { get; set; }

    }

    public enum RoleUtilisateur
    {
        Utilisateur,
        Administrateur,
    }
}
