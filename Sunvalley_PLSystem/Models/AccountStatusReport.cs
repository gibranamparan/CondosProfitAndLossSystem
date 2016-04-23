using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sunvalley_PLSystem.Models
{
    public class AccountStatusReport
    {
        [Key]
        [DisplayFormat(DataFormatString="{0:MMMM yyyy}")]
        public DateTime dateMonth { get; set; }

        public int houseID { get; set; }
        public virtual House house { get; set; }

        public virtual String UserID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }




    }
}