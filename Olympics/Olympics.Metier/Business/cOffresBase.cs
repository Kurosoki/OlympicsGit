using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Olympics.Metier.Business
{

    [Table("offres")]
    public class cOffresBase
    {
        [Key]
        [Column("idoffre")]
        public int IDOffre {  get; set; }





    }




}
