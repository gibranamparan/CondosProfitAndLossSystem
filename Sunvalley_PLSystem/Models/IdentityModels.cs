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

        public string firstName { get; set; }
        public string lastName { get; set; }
        public DateTime createAt { get; set; }
        public string company { get; set; }
        public string adress1 { get; set; }
        public string adress2 { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public int postalCode { get; set; }
        public string homePhone { get; set; }
        public string businessPhone { get; set; }
        public string businesFax { get; set; }
        public string mobilePhone { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }

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

        ///public System.Data.Entity.DbSet<Sunvalley_PLSystem.Models.ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<Sunvalley_PLSystem.Models.ApplicationUser> ApplicationUsers { get; set; }
        //public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<Sunvalley_PLSystem.Models.ApplicationUser> ApplicationUsers { get; set; }

        //public System.Data.Entity.DbSet<Sunvalley_PLSystem.Models.ApplicationUser> ApplicationUsers { get; set; }
    }
}