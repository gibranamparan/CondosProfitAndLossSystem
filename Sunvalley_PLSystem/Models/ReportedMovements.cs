using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static Sunvalley_PLSystem.Models.Movement;

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
            this.RegisterBy = m.createBy;
            this.TransactionDate = m.transactionDate;
            this.Type = m.typeOfMovement;
            this.Description = m.description;
            this.Amount = m.amount;
            this.Balance = m.balance;
            this.service = m.services.name;
            this.accountStatusReportID = reportID;
        }

        public ReportedMovements(){}

        public class VMReportedMovementes
        {
            [Display(Name = "TransactionDate")]
            public string TransactionDate { get; set; }

            [Display(Name = "Type")]
            public string Type { get; set; }

            [Display(Name = "Service")]
            public String service { get; set; }

            [Display(Name = "Destription")]
            public string Description { get; set; }

            [Display(Name = "Withdrawl")]
            public decimal Withdrawl { get; set; } =0;

            [Display(Name = "Deposit")]
            public decimal Deposit { get; set; } = 0;

            [Display(Name = "Balance")]
            public decimal Balance { get; set; }

            public VMReportedMovementes() { }
            public VMReportedMovementes(ReportedMovements item)
            {
                this.TransactionDate = item.TransactionDate.ToString("MM/dd/yyyy");
                this.Type = item.Type;
                this.service = item.service;
                this.Description = item.Description;
                if (item.Type == TypeOfMovements.EXPENSE)
                    this.Withdrawl = item.Amount;
                if (item.Type == TypeOfMovements.INCOME || item.Type == TypeOfMovements.CONTRIBUTION
                    || item.Type == TypeOfMovements.TAX || item.Type == TypeOfMovements.OWINGPAY)
                    this.Deposit = item.Amount;
                this.Balance = item.Balance;
            }

            public VMReportedMovementes(Movement item)
            {
                this.TransactionDate = item.transactionDate.ToString("MM/dd/yyyy");
                this.Type = item.typeOfMovement;
                this.service = item.services.name;
                this.Description = item.description;
                if (this.Type == TypeOfMovements.EXPENSE)
                    this.Withdrawl = item.amount;
                if (this.Type == TypeOfMovements.INCOME || this.Type == TypeOfMovements.CONTRIBUTION
                    || this.Type == TypeOfMovements.TAX || this.Type == TypeOfMovements.OWINGPAY)
                    this.Deposit = item.amount;
                this.Balance = item.balance;
            }

            /// <summary>
            /// Genera una lista de vistas VMReportedMovementes a partir de un reporte de elementos ReportedMovements
            /// </summary>
            /// <param name="list"></param>
            /// <returns></returns>
            public static List<VMReportedMovementes> listToVMReportedMovements(List<ReportedMovements> list)
            {
                List<VMReportedMovementes> res = new List<VMReportedMovementes>();
                res = (from item in list select new VMReportedMovementes(item)).ToList();
                return res;
            }

            /// <summary>
            /// Genera una lista de vistas VMReportedMovementes a partir de un reporte de elementos Movementes
            /// </summary>
            /// <param name="list"></param>
            /// <returns></returns>
            public static List<VMReportedMovementes> listToVMReportedMovements(List<Movement> list)
            {
                List<VMReportedMovementes> res = new List<VMReportedMovementes>();
                res = (from item in list select new VMReportedMovementes(item)).ToList();
                return res;
            }
        }
    }
}