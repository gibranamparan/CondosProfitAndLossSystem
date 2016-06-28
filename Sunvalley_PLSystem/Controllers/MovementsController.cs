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
    public class MovementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Movements
        //[Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {
            
            var movements = db.Movements.Include(m => m.house);
            return View(movements.ToList());
        }

        public ActionResult IndexReport(String ID)
        {
            if (ID == null)
            {
                var Reports = db.AccountStatusReport.ToList();
                return View("Reports", Reports.ToList());
            }
            else {
                var Reports = db.AccountStatusReport.Where(a => a.UserID == ID);
                return View("Reports", Reports.ToList());
            }
        }

        // GET: Movements/Details/5
        //[Authorize(Roles = "Administrador")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movement movement = db.Movements.Find(id);
            if (movement == null)
            {
                return HttpNotFound();
            }
            return View(movement);
        }

        // GET: Movements/Create
        //[Authorize(Roles = "Administrador")]
        public ActionResult Create(int id)
        {
            ViewBag.houseID = new SelectList(db.Houses, "houseID", "name");
            ViewBag.houseID = id;
            SelectList lista = new SelectList(db.Services,"serviceID","name");
            ViewBag.Services = lista;
            return View();
        }

        [HttpPost]
        public ActionResult Index(DateTime fecha, int houseID,String Accion)
        {
            //var Movimientos = from movement in db.Movements
            //                  where (movement.transactionDate >= fechaInicio && movement.transactionDate <= fechaFin && movement.UserID == User.Identity.GetUserId())
            //                  select movement;
            //var movements = db.Movements.Where(mov => mov.transactionDate >= fechaInicio && mov.transactionDate <= fechaFin && mov.houseID == houseID);
            if (houseID == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            House house = db.Houses.Find(houseID);
            if (house == null)
            {
                return HttpNotFound();
            }
            String IdUser = house.ApplicationUser.Id;
            var movements2 = db.Movements.Where(mov => mov.transactionDate.Month == fecha.Month &&mov.transactionDate.Year==fecha.Year&& mov.houseID == houseID);
            var reporte = db.AccountStatusReport.FirstOrDefault(r =>r.dateMonth.Month == fecha.Month  &&  r.UserID == IdUser);
            if(Accion == "Autorizar")
            {

                if (reporte != null)
                {

                    //AccountStatusReport Report = db.AccountStatusReport.Find(fecha);
                    db.AccountStatusReport.Remove(reporte);
                    AccountStatusReport Report = new AccountStatusReport();
                    Report.houseID = houseID;
                    Report.dateMonth = fecha;
                    Report.UserID = IdUser;
                    db.AccountStatusReport.Add(Report);
                    foreach (var i in movements2)
                    {
                        i.state = true;
                        db.Entry(i).State = EntityState.Modified;

                    }
                    db.SaveChanges();
                }
                else {
                    AccountStatusReport Report = new AccountStatusReport();
                    Report.houseID = houseID;
                    Report.dateMonth = fecha;
                    Report.UserID = IdUser;
                    db.AccountStatusReport.Add(Report);
                    foreach (var i in movements2)
                    {
                        i.state = true;
                        db.Entry(i).State = EntityState.Modified;
                    }
                    db.SaveChanges();

                }
            }
            else
            {
                //AccountStatusReport Report = db.AccountStatusReport.Where(A=>A.dateMonth.Month==fecha.Month&&A.UserID== IdUser).First();
                var Reports = db.AccountStatusReport.Where(A => A.dateMonth.Month == fecha.Month && A.UserID == IdUser);
                if(Reports.Count()>0)
                {
                    AccountStatusReport Report = Reports.First();
                    db.AccountStatusReport.Remove(Report);
                    foreach (var i in movements2)
                    {
                        i.state = false;
                        db.Entry(i).State = EntityState.Modified;

                    }
                    db.SaveChanges();
                    //return View(Movimientos.ToList());
                    ViewBag.Mensaje = "the movements are properly authorized";
                }
            }

            return RedirectToAction("Details", "Houses", new { id = houseID});
        }

        /*
        [Authorize]
        [HttpPost]
        public ActionResult generarReporte(int id)
        {

        }

        [Authorize]
        [HttpPost]
        public ActionResult eliminarReporte(int id)
        {

        }*/

        [Authorize]
        public ActionResult Recalculate(int id)
        {
            decimal balanceAnterior = 0;
            var Movements = db.Movements.Where(m => m.houseID == id).OrderBy(mov => mov.transactionDate);
            for(int m =0;m<Movements.Count();m++)
            {
                var Movemen = Movements.ToList().ElementAt(m);
                try
                {
                    balanceAnterior = Movements.ToList().ElementAt(m-1).balance;
                }
                catch { }

                if (Movemen.typeOfMovement == "Income"||Movemen.typeOfMovement == "Contribution")
                {
                    Movemen.balance = balanceAnterior + Movemen.amount;
                }
                else if (Movemen.typeOfMovement == "Expense")
                {
                    Movemen.balance = balanceAnterior - Movemen.amount;
                }
                db.Entry(Movemen).State = EntityState.Modified;
            }
            db.SaveChanges();
            return RedirectToAction("Details", "Houses", new { id = id });
        }
        // POST: Movements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Movement movement)
        {
            decimal balanceAnterior = 0;
            if (ModelState.IsValid)
            {
                movement.createBy = User.Identity.GetUserName();
                movement.UserID = User.Identity.GetUserId();
                if (movement.typeOfMovement == "Income")
                {
                    Services rent = db.Services.SingleOrDefault(ser => ser.name == "RENT");
                    if (rent == null || rent.serviceID == 0)
                    {
                        rent = new Services();
                        rent.name = "RENT";
                        db.Services.Add(rent);
                        db.SaveChanges();
                    }
                    movement.serviceID = rent.serviceID;
                }
                else if (movement.typeOfMovement == "Contribution")
                {
                    Services contri = db.Services.SingleOrDefault(ser => ser.name == "Contribution");
                    if (contri == null || contri.serviceID == 0)
                    {
                        contri = new Services();
                        contri.name = "Contribution";
                        db.Services.Add(contri);
                        db.SaveChanges();
                    }
                    
                    movement.serviceID = contri.serviceID;
                }
                //var balances = from movi in db.Movements
                //               where movi.houseID == movement.houseID
                //               orderby movi.transactionDate descending
                //               select movi;
                //decimal b = balances.Take(1).s;
                try {
                    var movimientosAscendentes = db.Movements.Where(mov => mov.houseID == movement.houseID).OrderByDescending(mov => mov.transactionDate);
                    //var movimientosAscendentes = db.Movements.Where(mov => mov.houseID == movement.houseID).OrderBy(mov => mov.transactionDate);
                    int cant = movimientosAscendentes.Count();
                    var ultimoMov = movimientosAscendentes.First();
                    balanceAnterior = ultimoMov.balance;
                }
                catch { }
                if (movement.typeOfMovement == "Income" || movement.typeOfMovement == "Contribution")
                {
                    movement.balance = balanceAnterior+movement.amount;
                }
                else if(movement.typeOfMovement== "Expense")
                {
                    movement.balance = balanceAnterior - movement.amount;
                }
                //movement.transactionDate = DateTime.Now;
                db.Movements.Add(movement);
                db.SaveChanges();
                int id = movement.houseID;
                //return RedirectToAction("Details", "Houses", new { id = id });
                return RedirectToAction("Recalculate", new { id = movement.houseID });
            }

            ViewBag.houseID = new SelectList(db.Houses, "houseID", "name", movement.houseID);
            return View(movement);
        }

        // GET: Movements/Edit/5
        //[Authorize(Roles = "Administrador")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movement movement = db.Movements.Find(id);
            if (movement == null)
            {
                return HttpNotFound();
            }
            ViewBag.houseID = new SelectList(db.Houses, "houseID", "name", movement.houseID);
            SelectList lista = new SelectList(db.Services, "serviceID", "name");
            ViewBag.Services = lista;
            return View(movement);
        }

        // POST: Movements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Movement movement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Recalculate", new { id = movement.houseID});
            }
            ViewBag.houseID = new SelectList(db.Houses, "houseID", "name", movement.houseID);
            SelectList lista = new SelectList(db.Services, "serviceID", "name");
            ViewBag.Services = lista;
            return View(movement);
        }

        // GET: Movements/Delete/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movement movement = db.Movements.Find(id);
            if (movement == null)
            {
                return HttpNotFound();
            }
            return View(movement);
        }

        // POST: Movements/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movement movement = db.Movements.Find(id);
            db.Movements.Remove(movement);
            db.SaveChanges();
            return RedirectToAction("Recalculate", new { id = movement.houseID});
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
