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
        [Display(Name ="Home Name")]
        public string name { get; set; }
        [Display(Name = "Status")]
        public Boolean status { get; set; }
        [Display(Name = "Created")]
        public DateTime created { get; set; }
        [Display(Name = "Area")]
        public string area { get; set; }
        [Display(Name = "Adress")]
        public string adress { get; set; }
        [Display(Name = "City/Area")]
        public string cityArea { get; set; }
        [Display(Name = "Country")]
        public string country { get; set; }
        [Display(Name = "State/Province")]
        public string stateProvince { get; set; }
        [Display(Name = "Postal Code")]
        public int postalCode { get; set; }

        public virtual String UserID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Movement> movimientos { get; set; }
    }
}