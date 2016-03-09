using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sunvalley_PLSystem.Models
{
    public class House
    {
        [Key]
        public int houseID { get; set; }


        public virtual RegisterViewModel owner { get; set; }
        public virtual ICollection<Movement> movimientos { get; set; }
    }
}