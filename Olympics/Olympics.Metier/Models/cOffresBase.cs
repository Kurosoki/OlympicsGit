using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympics.Metier.Models
{

    [Table("offres")]
    public class cOffresBase
    {
        [Key]
        [Column("idoffre")]
        public int IDOffre {  get; set; }

        [Column("nomoffre")]
        public string NomOffre { get; set; }

        [Column("description")]
        public string Description { get; set; }

        [Column("prixsolo")]
        public decimal PriceSolo { get; set; }

        [Column("prixduo")]
        public decimal PriceDuo { get; set; }

        [Column("prixfamily")]
        public decimal PriceFamily { get; set; }





    }




}
