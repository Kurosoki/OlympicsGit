using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Olympics.Metier.Business
{
    [Table("payement")] 
    public class cPayementBase
    {
        [Key]
        [Column("idpayement")] 
        public int IDPayement { get; set; }

        [ForeignKey("cPanierBase")]
        [Column("idpanier")] 
        public int IDPanier { get; set; }

        [Column("dateachat")] 
        public DateTime DateAchat { get; set; }

        [Column("montant")] 
        public int Montant { get; set; }

        [Column("issuccess")] 
        public bool IsSuccess { get; set; }

        [Column("qrcodeurl")] 
        public string QrCodeUrl { get; set; }
    }


    //public class cTicketPayé
    //{
    //    public int IDTicket { get; set; }
    //    public int IDClient { get; set; }
    //    public string KeyFinal {  get; set; }
    //    public DateTime DateAchat { get; set; }

    //}
}
