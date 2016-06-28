using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sunvalley_PLSystem.Models
{
    public class ReportedMovements
    {
        [Key]
        public int MovementID { get; set; }


        [Display(Name = "RegisterBy")]
        public string RegisterBy { get; set; }

        [Display(Name = "TransactionDate")]
        public DateTime TransactionDate { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Destription")]
        public string Destription  { get; set; }

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Balance")]
        public decimal Balance { get; set; }

        [Display(Name = "Status")]
        public bool Status { get; set; }



        public int accountStatusReportID { get; set; }
        public virtual AccountStatusReport accountStatusReport { get; set; }
    }
}