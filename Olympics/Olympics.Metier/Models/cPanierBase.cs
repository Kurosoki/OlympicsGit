using Olympics.Metier.Utils;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Olympics.Metier.Models
{
    [Table("panier")] 
    public class cPanierBase
    {
        [Key]
        [Column("idpanier")] 
        public int IDPanier { get; set; }

        [Column("idclient")] 
        public int IDClient { get; set; }

        [Column("datecreated")]
        public DateTime DateCreated { get; set; } 

        [Column("dateupdated")]
        public DateTime DateUpdated { get; set; }

        // Relation avec les tickets
        public List<cTicket> Tickets { get; set; } = new List<cTicket>();
    }


    [Table("tickets")] 
    public class cTicket
    {
        [Key]
        [Column("idticket")] 
        public int IDTicket { get; set; }

        [ForeignKey("Panier")]  // Indique que IDPanier est une clé étrangère
        [Column("idpanier")] 
        public int IDPanier { get; set; }

        public cPanierBase Panier { get; set; }  // Propriété de navigation vers cPanierBase

        [Column("sportname")] 
        public string SportName { get; set; }

        [Column("tickettype")] 
        public TicketTypeManager.TicketType TicketType { get; set; }

        [Column("quantity")] 
        public int Quantity { get; set; }

        [Column("price")] 
        public decimal Price { get; set; }
    }


}
