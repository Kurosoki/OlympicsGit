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
        public int IDOffre { get; set; }

        [Required(ErrorMessage = "Le nom du sport est requis.")]
        [Column("sportname")]
        public string SportName { get; set; }

        [Required(ErrorMessage = "La description est requise.")]
        [Column("description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Le prix solo est requis.")]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif.")]
        [Column("prixsolo")]
        public decimal PriceSolo { get; set; }

        [Required(ErrorMessage = "Le prix duo est requis.")]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif.")]
        [Column("prixduo")]
        public decimal PriceDuo { get; set; }

        [Required(ErrorMessage = "Le prix famille est requis.")]
        [Range(0, double.MaxValue, ErrorMessage = "Le prix doit être positif.")]
        [Column("prixfamily")]
        public decimal PriceFamily { get; set; }

        [Column("imageurl")]
        public string ImageUrl { get; set; } 

    }

}
