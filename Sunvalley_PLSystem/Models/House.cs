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
        [Display(Name = "Is Active")]
        public Boolean status { get; set; }
        [Display(Name = "Created")]
        public DateTime created { get; set; }
        [Display(Name = "Area")]
        public string area { get; set; }
        [Display(Name = "Address")]
        public string adress { get; set; }
        [Display(Name = "City/Area")]
        public string cityArea { get; set; }
        [Display(Name = "Country")]
        public string country { get; set; }
        [Display(Name = "State/Province")]
        public string stateProvince { get; set; }

        [Display(Name = "Postal Code")]
        public String postalCode { get; set; }

        public String Id { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public virtual ICollection<Movement> movimientos { get; set; }

        internal VMHouse getVM()
        {
            return new VMHouse(this);
        }

        public class VMHouse
        {
            [Display(Name = "Home Name")]
            public string OwnerName { get; set; }

            [Display(Name = "Home Name")]
            public string HomeName { get; set; }

            [Display(Name = "Area")]
            public string Area { get; set; }

            [Display(Name = "Address")]
            public string Address { get; set; }

            [Display(Name = "City/Area")]
            public string City { get; set; }

            [Display(Name = "Country")]
            public string Country { get; set; }

            [Display(Name = "State/Province")]
            public string State { get; set; }

            [Display(Name = "Postal Code")]
            public String PostalCode { get; set; }

            public VMHouse(House h)
            {
                this.OwnerName = h.ApplicationUser.OwnerName;
                this.HomeName = h.name;
                this.Area = h.area;
                this.Address = h.adress;
                this.City = h.cityArea;
                this.Country = h.country;
                this.State = h.stateProvince;
                this.PostalCode = h.postalCode;
            }
        }
    }
}