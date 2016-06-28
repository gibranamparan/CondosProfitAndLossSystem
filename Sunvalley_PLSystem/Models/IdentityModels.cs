using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sunvalley_PLSystem.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        [Display(Name="Owner Name")]
        public string firstName { get; set; }
        [Display(Name = "Last Name")]
        public string lastName { get; set; }



      
       
     
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

        [Display(Name = "E-mail")]
        public string Email1 { get; set; }

        [Display(Name = "E-mail2")]
        public string Email2 { get; set; }

        [Display(Name = "Status")]
        public string status { get; set; }

        public virtual ICollection<Movement> movimientos { get; set; }
        public virtual ICollection<House> Houses { get; set; }


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