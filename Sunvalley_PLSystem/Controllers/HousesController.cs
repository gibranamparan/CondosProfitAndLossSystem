using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Sunvalley_PLSystem.Models;
using Microsoft.AspNet.Identity;

namespace Sunvalley_PLSystem.Controllers
{
    public class HousesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Houses
        [Authorize]
        public ActionResult Index()
        {

            if (User.IsInRole("Administrador"))
            {
                var houses = db.Houses.Where(h => h.status==true);
                return View(houses.ToList());
            }
            String userID = User.Identity.GetUserId();
            var Casas = from usu in db.Houses where usu.Id == userID select usu;
            return View(Casas.ToList());


        }

        // GET: Houses/Details/5
        [Authorize]
        [HttpGet]
        public ActionResult Details(DateTime? fecha, int? id)
        {
            String mensaje = db.GeneralInformations.Find(1).InformacionGen;
            String NombreCompleto = db.Houses.Find(id).ApplicationUser.firstName + " " + db.Houses.Find(id).ApplicationUser.lastName;
            String HAD = db.Houses.Find(id).name + ", " + db.Houses.Find(id).area + ", " + db.Houses.Find(id).adress;
            String CCSP = db.Houses.Find(id).cityArea + ", " + db.Houses.Find(id).country + ", " + db.Houses.Find(id).stateProvince + ", " + db.Houses.Find(id).postalCode;
            ViewBag.HAD = HAD;
            ViewBag.CCSP = CCSP;
            ViewBag.NombreCompleto = NombreCompleto;
            ViewBag.mensaje = mensaje;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = db.Houses.Find(id);
            DateTime fechaConArgumentos = new DateTime();
            if(fecha == null)
            {
                //Si no viene fecha, se establece por defecto el mes actual
                fechaConArgumentos = DateTime.Now;
                fechaConArgumentos = new DateTime(fechaConArgumentos.Year, fechaConArgumentos.Month, 1);
            }
            else
            {
                //Si dentro de la transaccion viene con fecha, se ignora la hora
                fechaConArgumentos = fecha.Value;
            }

            if (User.IsInRole("Administrador"))
            {

                var m = db.Movements.Where(mov => mov.houseID == id && mov.transactionDate.Month == fechaConArgumentos.Month && mov.transactionDate.Year == fechaConArgumentos.Year).OrderBy(move=>move.transactionDate);
                ViewBag.Movements1 = m.ToList();
            }
            else
            {
                var m = db.Movements.Where(mov => mov.houseID == id &&mov.state== true&& mov.transactionDate.Year== fechaConArgumentos.Year && mov.transactionDate.Month == fechaConArgumentos.Month).OrderBy(move => move.transactionDate);
                ViewBag.Movements1 = m.ToList();
            }

            ViewBag.fechaConArgumentos = fechaConArgumentos;
            if (house == null)
            {
                return HttpNotFound();
            }
            return View(house);
        }

        // GET: Houses/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult Create(String id)
        {
            ViewBag.UserID =id;
            return View();
        }


        // POST: Houses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(House house)
        {
            if (ModelState.IsValid)
            {
                house.status = true;
                house.created = DateTime.Today;
                db.Houses.Add(house);
                db.SaveChanges();
                //return RedirectToAction("Index");
                return RedirectToAction("Details","Account",new {id=house.Id});
            }

            return View(house);
        }
        // GET: Houses/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult CreateReportForHouse(int id)
        {
            Decimal totalBalance = 0;

            House house = db.Houses.Find(id);
            foreach (Movement mov in house.movimientos)
            {
                totalBalance = totalBalance + mov.balance;
            }
            return View();
        }
        // GET: Houses/Edit/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = db.Houses.Find(id);
            if (house == null)
            {
                return HttpNotFound();
            }
            return View(house);
        }

        // POST: Houses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "houseID,name,status,created,area,adress,cityArea,country,stateProvince,postalCode,UserID")] House house)
        {
            if (ModelState.IsValid)
            {
                db.Entry(house).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(house);
        }

        // GET: Houses/Delete/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = db.Houses.Find(id);
            if (house == null)
            {
                return HttpNotFound();
            }
            return View(house);
        }

        // POST: Houses/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            House house = db.Houses.Find(id);
            db.Houses.Remove(house);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
