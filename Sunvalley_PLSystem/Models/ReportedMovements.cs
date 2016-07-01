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

        public int orden { get; set; }

        [Display(Name = "RegisterBy")]
        public string RegisterBy { get; set; }

        [Display(Name = "TransactionDate")]
        public DateTime TransactionDate { get; set; }

        [Display(Name = "Type")]
        public string Type { get; set; }

        [Display(Name = "Destription")]
        public string Description  { get; set; }

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }

        [Display(Name = "Balance")]
        public decimal Balance { get; set; }

        [Display(Name = "Service")]
        public String service { get; set; }

        public int accountStatusReportID { get; set; }
        public virtual AccountStatusReport accountStatusReport { get; set; }

        public ReportedMovements(Movement m, int reportID, int orden)
        {
            this.MovementID = m.movementID;
            this.orden = orden;
            this.RegisterBy = m.createBy;
            this.TransactionDate = m.transactionDate;
            this.Type = m.typeOfMovement;
            this.Description = m.description;
            this.Amount = m.amount;
            this.Balance = m.balance;
            this.service = m.services.name;
            this.accountStatusReportID = reportID;
        }

        public ReportedMovements()
        {

        }
    }
}