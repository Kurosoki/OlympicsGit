using Olympics.Metier.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympics.Metier.Models
{
    [Table("panierarchive")]
    public class cPanierArchive
    {
        [Key]
        [Column("idpanierarchive")]
        public int IDPanierArchive { get; set; }

        [Column("idclient")]
        public int IDClient { get; set; }

        [Column("dateachat")]
        public DateTime DateAchat { get; set; }

        public List<cTicketArchive> TicketsArchive { get; set; }
    }

    [Table("ticketsarchive")]
    public class cTicketArchive
    {
        [Key]
        [Column("idticketarchive")]
        public int IDTicketArchive { get; set; }

        [ForeignKey("PanierArchive")]
        [Column("idpanierarchive")]
        public int IDPanierArchive { get; set; }

        public cPanierArchive PanierArchive { get; set; }

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
