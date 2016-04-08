using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sunvalley_PLSystem.Models
{
    public class Services
    {
        [Key]
        public int serviceID { get; set; }
        public string name { get; set; }

        public virtual ICollection<Movement> Movements { get; set; }
    }
}