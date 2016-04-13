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
        public ActionResult Index()
        {

            if (User.IsInRole("Administrador"))
            {
                return View(db.Houses.ToList());
            }
            String userID = User.Identity.GetUserId();
            var Casas = from usu in db.Houses where usu.Id == userID select usu;
            return View(Casas.ToList());


        }

        // GET: Houses/Details/5
        [Authorize]
        public ActionResult Details(/*DateTime? fechaInicio, DateTime? fechaFin, */int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = db.Houses.Find(id);

            if (User.IsInRole("Administrador"))
            {
                var m = db.Movements.Where(mov => mov.houseID== id);
                ViewBag.Movements1 = m.ToList();
            }
            else
            {
               var m = db.Movements.Where(mov => mov.houseID == id && mov.state == true);
                ViewBag.Movements1 = m.ToList();
            }

            //if(fechaInicio==null && fechaFin == null)
            //{
                

            //    //var movements = from movement in db.Movements where movement.houseID == id select movement;
            //    //ViewBag.movements = movements.ToList();
            //}
            //else
            //{
            //    //house.movimientos.Where(mov => mov.transactionDate >= fechaInicio && mov.transactionDate <= fechaFin && mov.houseID == id);
            //    var movementsFiltro = db.Movements.Where(mov => mov.transactionDate >= fechaInicio && mov.transactionDate <= fechaFin && mov.houseID == id);
            //    ViewBag.movements = movementsFiltro.ToList();
            //}
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
                house.created = DateTime.Today;
                db.Houses.Add(house);
                db.SaveChanges();
                return RedirectToAction("Index");
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
