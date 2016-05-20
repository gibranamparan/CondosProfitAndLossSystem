using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Sunvalley_PLSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sunvalley_PLSystem.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public HomeController() { }
        public HomeController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        public ActionResult Index()
        {
            //try
            //{
            //    String nombre = UserManager.FindById(User.Identity.GetUserId()).firstName;
            //    String apellido = UserManager.FindById(User.Identity.GetUserId()).lastName;
            //    ViewBag.NombreCompleto = nombre + " " + apellido;
            //}
            //catch { }
            if(!User.Identity.IsAuthenticated){
                return RedirectToAction("LogIn","Account");
            }
            else if (User.IsInRole("Administrador"))
            {
                return RedirectToAction("Index", "Account");
            }
            else if(User.IsInRole("Cliente"))
            {
                return RedirectToAction("Index", "Houses");
            }
            else
                return View();
        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}