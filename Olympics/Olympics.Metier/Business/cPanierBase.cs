using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Olympics.Metier.Utils;



namespace Olympics.Metier.Business
{
    public class cPanierBase
    {
        public int IDPanier { get; set; }
        public int IDClient { get; set; } // Pour lier le panier à l'utilisateur
        public List<cTicket> Tickets { get; set; } = new List<cTicket>();
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }

        // Propriété pour calculer le prix total du panier
        public decimal TotalPrice => Tickets.Sum(ticket => ticket.Quantity * ticket.Price);
    }


    public class cTicket
    {
        public int IDTicket { get; set; }
        public int IDPanier { get; set; }
        public string SportName { get; set; }
        public TicketTypeManager.TicketType TicketType { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
