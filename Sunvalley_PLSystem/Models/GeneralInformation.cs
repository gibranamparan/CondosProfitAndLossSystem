using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sunvalley_PLSystem.Models
{
    public class GeneralInformation
    {
        [Key]
        public int InfoID { get; set; }


        [Display(Name = "Informacion")]
        public string InformacionGen { get; set; }


    }



}