    using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sunvalley_PLSystem.Models
{
    public class Movement
    {
        [Key]
        public int movementID { get; set; }

        [Display(Name = "Registered By")]
        public string createBy { get; set; }

        [Display(Name = "Transaction Date")]
        [DataType(DataType.Date)]

        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime transactionDate { get; set; }

        [Display(Name = "Type of movement")]
        public string typeOfMovement { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

        [Display(Name = "Amount")]
        public decimal amount { get; set; }

        [DisplayFormat(DataFormatString = "{0:c}", ApplyFormatInEditMode = true)]
        [Display(Name = "Balance")]
        public decimal balance { get; set; }

        [Display(Name = "Status")]
        public Boolean state { get; set; }

        //To one movement correspond one house
        public int houseID { get; set; }
        public virtual House house { get; set; }

        public int serviceID { get; set; }
        public virtual Services services { get;set;}

        //To one movement correspond one user
        public virtual String UserID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }

        public static Object[] getTypeOfMovements()
        {
            Object[] typeOfMovementes = new Object[]{
                new {Text = TypeOfMovements.EXPENSE, Value=TypeOfMovements.EXPENSE},
                new {Text = TypeOfMovements.INCOME, Value=TypeOfMovements.INCOME},
                new {Text = TypeOfMovements.CONTRIBUTION, Value=TypeOfMovements.CONTRIBUTION},
                new {Text = TypeOfMovements.TAX, Value=TypeOfMovements.TAX},
                new {Text = TypeOfMovements.OWINGPAY, Value=TypeOfMovements.OWINGPAY}
            };
            return typeOfMovementes;
        }


        //Default validation error messages
        public static class TypeOfMovements
        {
            public const string EXPENSE = "Expense";
            public const string INCOME = "Income";
            public const string CONTRIBUTION = "Contribution";
            public const string TAX = "TAX";
            public const string OWINGPAY = "Owing Pay";
        }

    }
}