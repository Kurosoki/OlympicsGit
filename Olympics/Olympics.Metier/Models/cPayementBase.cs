using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Olympics.Metier.Models
{
    [Table("payement")] 
    public class cPayementBase
    {
        [Key]
        [Column("idpayement")] 
        public int IDPayement { get; set; }

        [ForeignKey("Panier")]
        [Column("idpanier")] 
        public int IDPanier { get; set; }

        public cPanierBase Panier { get; set; }

        [Column("dateachat")] 
        public DateTime DateAchat { get; set; }

        [Column("montant")] 
        public decimal Montant { get; set; }

        [Column("issuccess")] 
        public bool IsSuccess { get; set; }

        [Column("qrcodeurl")] 
        public string QrCodeUrl { get; set; }
    }

}
