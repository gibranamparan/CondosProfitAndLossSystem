using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using static Sunvalley_PLSystem.Models.Movement;

namespace Sunvalley_PLSystem.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser() { }
        public ApplicationUser(RegisterViewModel model)
        {
            this.UserName = model.Email.Trim();
            this.Email = model.Email.Trim();
            this.firstName = model.firstName;
            this.lastName = model.lastName;
            this.createAt = DateTime.Today;
            this.company = model.company;
            this.adress1 = model.adress1;
            this.adress2 = model.adress2;
            this.city = model.city;
            this.country = model.country;
            this.state = model.state;
            this.postalCode = model.postalCode;
            this.mobilePhone = model.mobilePhone;
            this.homePhone = model.homePhone;
            this.businesFax = model.businesFax;
            this.businessPhone = model.businessPhone;
            this.otrosEmail = model.otrosEmails;
            this.status = "Activate";
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [Display(Name="First Name")]
        public string firstName { get; set; }
        [Display(Name = "Last Name")]
        public string lastName { get; set; }

        [Display(Name = "Full Name")]
        public string OwnerName { get {
                return String.Format("{0} {1}", this.firstName, this.lastName);
            } }

        [Display(Name = "Created At")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime createAt { get; set; }

        [Display(Name = "Company")]
        public string company { get; set; }

        [Display(Name = "Address")]
        public string adress1 { get; set; }

        [Display(Name = "Address 2")]
        public string adress2 { get; set; }

        [Display(Name = "City")]
        public string city { get; set; }
        [Display(Name = "Country")]
        public string country { get; set; }

        [Display(Name = "State")]
        public string state { get; set; }

        [Display(Name = "Postal Code")]
        public string postalCode { get; set; }

        [Display(Name = "Home Phone")]
        public string homePhone { get; set; }

        [Display(Name = "BusinessPhone")]
        public string businessPhone { get; set;}

        [Display(Name = "BusinessFax")]
        public string businesFax { get; set; }

        [Display(Name = "MobilePhone")]
        public string mobilePhone { get; set; }

        [Display(Name = "Other Emails")]
        public string otrosEmail { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }

        public virtual ICollection<Movement> movimientos { get; set; }
        public virtual ICollection<House> Houses { get; set; }

        public ProfitAndLossReport generateProfitAndLossReport(DateTime fecha1, DateTime fecha2, int houseID = 0)
        {
            return new ProfitAndLossReport(this, fecha1, fecha2,houseID);
        }

        public static class RoleNames
        {
            public const string ADMINISTRADOR = "Administrador";
            public const string CLIENTE = "Cliente";
        }

        public class ProfitAndLossReport
        {
            public ApplicationUser owner { get;}
            public DateTime fecha1{ get; }
            public DateTime fecha2 { get; }
            public IEnumerable<Movement> lossReport { get; set; }
            public IEnumerable<Movement> profitReport { get; set; }
            public IEnumerable<Movement> contributionReport { get; set; }
            public List<ProfitAndLossTotals> totals { get; set; }

            public ProfitAndLossReport(ApplicationUser owner, DateTime fecha1, DateTime fecha2, int houseID = 0)
            {
                this.owner = owner;
                this.fecha1 = fecha1;
                this.fecha2 = fecha2;
                List<Movement> listMovement = new List<Movement>();

                if(houseID == 0) //House not specified, showing all movements done by owner
                    foreach (var h in owner.Houses)
                        listMovement.AddRange(h.movimientos);
                else //If house is specified, the movementes to be shown will be only from that house
                    listMovement.AddRange(owner.Houses.FirstOrDefault(h => h.houseID == houseID).movimientos);

                this.lossReport = listMovement.Where(m => m.typeOfMovement == Movement.TypeOfMovements.EXPENSE &&
                    m.transactionDate >= fecha1 && m.transactionDate <= fecha2);
                this.profitReport = listMovement.Where(m => (m.typeOfMovement == Movement.TypeOfMovements.INCOME || m.typeOfMovement == Movement.TypeOfMovements.TAX)
                    && m.transactionDate >= fecha1 && m.transactionDate <= fecha2);
                this.contributionReport = listMovement.Where(m => m.typeOfMovement == Movement.TypeOfMovements.CONTRIBUTION &&
                    m.transactionDate >= fecha1 && m.transactionDate <= fecha2);

                //Calculate totals in profit and loss report
                decimal totalProfit = this.profitReport.Sum(m => m.amount);
                decimal totalLoss = this.lossReport.Sum(m => m.amount);
                decimal totalContribution = this.contributionReport.Sum(m => m.amount);
                var vmTotalProfit = new ProfitAndLossTotals(ProfitAndLossTotals.TotalConcepts.TotalProfit, totalProfit);
                var vmTotalLoss = new ProfitAndLossTotals(ProfitAndLossTotals.TotalConcepts.TotalLoss, totalLoss);
                var vmTotalContribution = new ProfitAndLossTotals(ProfitAndLossTotals.TotalConcepts.TotalContribution, totalContribution);
                var vmTotal = new ProfitAndLossTotals(ProfitAndLossTotals.TotalConcepts.Total, totalProfit - totalLoss);
                this.totals = new List<ProfitAndLossTotals>();
                this.totals.Add(vmTotalProfit);
                this.totals.Add(vmTotalLoss);
                this.totals.Add(vmTotalContribution);
                this.totals.Add(vmTotal);
            }

            public string ToString(bool onlyContributions = false)
            {
                string userName = this.owner.UserName;
                userName = userName.Substring(0, userName.IndexOf('@'));
                return String.Format((onlyContributions?"Contributions": "ProfitAndLoss") + "_{0}_{1}_{2}",userName, fecha1.ToString("yyyyMMMdd"), fecha2.ToString("yyyyMMMdd"));
            }

            public class ProfitAndLossTotals
            {
                public TotalConcepts concept { get; set; }
                public string conceptName { get {
                        return ConceptsNames[this.concept];
                    } }

                [DisplayFormat(DataFormatString = "{0:C}")]
                public decimal total { get; set; }

                public ProfitAndLossTotals(TotalConcepts concept, decimal total)
                {
                    this.concept = concept;
                    this.total = total;
                }

                private Dictionary<TotalConcepts, string> ConceptsNames = new Dictionary<TotalConcepts, string> {
                    { TotalConcepts.TotalLoss, "Loss"},
                    { TotalConcepts.TotalProfit, "Profit"},
                    { TotalConcepts.TotalContribution, "Contributions"},
                    { TotalConcepts.Total, "Total Profit"},
                };

                public enum TotalConcepts
                {
                    TotalLoss, TotalProfit,TotalContribution, Total
                }
            }

            public class VMProfitAndLossRow
            {
                [Display(Name = "Home Name")]
                public string Home { get; set; }

                public string Service { get; set; }

                [Display(Name = "Transaction Date")]
                public string TransactionDate { get; set; }

                [Display(Name = "Type of Movement")]
                public string TypeOfMovement { get; set; }

                [Display(Name = "Description")]
                public string Description { get; set; }

                [Display(Name = "Amount")]
                public decimal Amount { get; set; } = 0;

                public VMProfitAndLossRow(Movement item)
                {
                    this.Home = item.house.name;
                    this.Service = item.services.name;
                    this.TransactionDate = item.transactionDate.ToString("MM/dd/yyyy");
                    this.TypeOfMovement = item.typeOfMovement;
                    this.Description = item.description;
                    this.Amount = Math.Abs(item.amount);
                }

                /// <summary>
                /// Genera una lista de vistas VMProfitAndLossRow a partir de un reporte de elementos Movement
                /// </summary>
                /// <param name="list"></param>
                /// <returns></returns>
                public static List<VMProfitAndLossRow> listToVMMovements(List<Movement> list)
                {
                    List<VMProfitAndLossRow> res = new List<VMProfitAndLossRow>();
                    res = (from item in list select new VMProfitAndLossRow(item)).ToList();
                    return res;
                }
            }
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("ConexionSunValley", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }


        //Entidades definidas para atender requerimientos de logica de negocio
        public DbSet<House> Houses { get; set; }
        public DbSet<Movement> Movements { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<AccountStatusReport> AccountStatusReport { get; set; }
        public DbSet<ReportedMovements> ReportedMovements { get; set; }

        public System.Data.Entity.DbSet<Sunvalley_PLSystem.Models.GeneralInformation> GeneralInformations { get; set; }

        ///public System.Data.Entity.DbSet<Sunvalley_PLSystem.Models.ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<Sunvalley_PLSystem.Models.ApplicationUser> ApplicationUsers { get; set; }
        //public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<Sunvalley_PLSystem.Models.ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<Sunvalley_PLSystem.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}