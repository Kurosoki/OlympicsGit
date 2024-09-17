using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympics.Metier.Business
{
    public class cUtilisateurConnexionBase
    {

        [Required]
        [EmailAddress]
        public string EmailClient { get; set; }

        [Required]
        public string ShaMotDePasse { get; set; }

        public bool RememberMe { get; set; }
    }
}
