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
    [Authorize]
    public class MovementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Movements
        [Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {

            var movements = db.Movements.Include(m => m.house);
            return View(movements.ToList());
        }

        public ActionResult IndexReport(int fecha=0, int houseID = 0)
        {
            String ID = User.Identity.GetUserId();
            var house = db.Houses.Find(houseID);
            if (house == null || String.IsNullOrEmpty(ID))
            {
                return RedirectToAction("Index", "Houses");
            }
            ViewBag.house = house;
            DateTime fechaArgumentos;
            if (fecha == 0)
            {
                fechaArgumentos = DateTime.Now;
                ViewBag.fechaA = fechaArgumentos;
                fecha = fechaArgumentos.Year;
            }
            else
            {
                fechaArgumentos = new DateTime(fecha,1,1);
                ViewBag.fechaA = fechaArgumentos;
            }

            var Reports = db.AccountStatusReport.Where(a=> a.dateMonth.Year == fechaArgumentos.Year && a.houseID == houseID);
            if (!User.IsInRole("Administrador"))
            {
                Reports = Reports.Where(a => a.UserID == ID);
            }
            ViewBag.ID = ID;
            return View("Reports", Reports.ToList());
        }

        // GET: Movements/Details/5
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
        [Authorize(Roles = "Administrador")]
        public ActionResult Create(DateTime fechaConArgumentos, int id=0)
        {
            var house = db.Houses.Find(id);
            if (house == null)
            {
                return RedirectToAction("Index", "Account");
            }
            ViewBag.house = house;
            SelectList lista = new SelectList(db.Services, "serviceID", "name");
            ViewBag.Services = lista;
            Movement newMov = new Movement();
            newMov.transactionDate = fechaConArgumentos;
            return View(newMov);
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

                DateTime rightNow = DateTime.Now;
                DateTime transactionDate = movement.transactionDate;
                movement.transactionDate = transactionDate.AddHours(rightNow.Hour).
                    AddMinutes(rightNow.Minute).AddSeconds(rightNow.Second);
                if (movement.typeOfMovement == Movement.TypeOfMovements.INCOME)
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
                else if (movement.typeOfMovement == Movement.TypeOfMovements.CONTRIBUTION)
                {
                    Services contri = db.Services.SingleOrDefault(ser => ser.name == Movement.TypeOfMovements.CONTRIBUTION);
                    if (contri == null || contri.serviceID == 0)
                    {
                        contri = new Services();
                        contri.name = Movement.TypeOfMovements.CONTRIBUTION;
                        db.Services.Add(contri);
                        db.SaveChanges();
                    }

                    movement.serviceID = contri.serviceID;
                }
                try
                {
                    var movimientosAscendentes = db.Movements.Where(mov => mov.houseID == movement.houseID).
                        OrderByDescending(mov => mov.transactionDate);
                    int cant = movimientosAscendentes.Count();
                    var ultimoMov = movimientosAscendentes.First();
                    balanceAnterior = ultimoMov.balance;
                }
                catch { }

                //Tipos de movimientos que incrementan el balance
                if (movement.typeOfMovement == Movement.TypeOfMovements.INCOME
                    ||movement.typeOfMovement == Movement.TypeOfMovements.CONTRIBUTION
                    || movement.typeOfMovement == Movement.TypeOfMovements.TAX
                    || movement.typeOfMovement == Movement.TypeOfMovements.OWINGPAY)
                {
                    movement.balance = balanceAnterior + movement.amount;
                }
                //Tipos de movimientos que restan al balance
                else if (movement.typeOfMovement == Movement.TypeOfMovements.EXPENSE)
                {
                    movement.balance = balanceAnterior - movement.amount;
                }
                //movement.transactionDate = DateTime.Now;
                db.Movements.Add(movement);
                db.SaveChanges();
                int id = movement.houseID;
                //return RedirectToAction("Details", "Houses", new { id = id });
                return RedirectToAction("Recalculate", new { id = movement.houseID, fechaConArgumentos = movement.transactionDate });
            }

            ViewBag.houseID = new SelectList(db.Houses, "houseID", "name", movement.houseID);
            return View(movement);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult Index(DateTime fecha, int houseID, String Accion)
        {
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
            var movements2 = db.Movements.Where(mov => mov.transactionDate.Month == fecha.Month && mov.transactionDate.Year == fecha.Year && mov.houseID == houseID);
            var reporte = db.AccountStatusReport.FirstOrDefault(r => r.dateMonth.Month == fecha.Month && r.UserID == IdUser);
            if (Accion == "Autorizar")
            {
                if (reporte != null)
                {
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
                var Reports = db.AccountStatusReport.Where(A => A.dateMonth.Month == fecha.Month && A.UserID == IdUser);
                if (Reports.Count() > 0)
                {
                    AccountStatusReport Report = Reports.First();
                    db.AccountStatusReport.Remove(Report);
                    foreach (var i in movements2)
                    {
                        i.state = false;
                        db.Entry(i).State = EntityState.Modified;

                    }
                    db.SaveChanges();
                    ViewBag.Mensaje = "the movements are properly authorized";
                }
            }

            return RedirectToAction("Details", "Houses", new { id = houseID });
        }

        [Authorize]
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult generarReporte(int houseID, DateTime fecha)
        {
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

            var movements2 = db.Movements.Where(mov => mov.transactionDate.Month == fecha.Month && mov.transactionDate.Year == fecha.Year && mov.houseID == houseID).OrderBy(move => move.transactionDate);
            var reporte = db.AccountStatusReport.FirstOrDefault(r => r.dateMonth.Month == fecha.Month && r.UserID == IdUser);
            if (reporte == null)
            {
                AccountStatusReport Report = new AccountStatusReport();
                Report.houseID = houseID;
                Report.dateMonth = fecha;
                Report.UserID = IdUser;
                db.AccountStatusReport.Add(Report);

                reporte = Report;
            }
            else
            {
                db.ReportedMovements.RemoveRange(reporte.ReportedMovements.ToList());
            }

            foreach (var i in movements2)
            {
                i.state = true;
                db.Entry(i).State = EntityState.Modified;

            }
            //db.SaveChanges();

            int reportID = reporte.accountStatusReportID;
            /*List<ReportedMovements> movimientosReportados = (from mov in movements2.ToList()
                                                             select
                                                                 new ReportedMovements(mov, reportID)).ToList();*/
            List<ReportedMovements> movimientosReportados = new List<Models.ReportedMovements>();
            int orden = 1;
            foreach (Movement mov in movements2.ToList())
            {
                movimientosReportados.Add(new ReportedMovements(mov, reportID, orden));
                orden++;
            }
            db.ReportedMovements.AddRange(movimientosReportados);
            db.SaveChanges();

            return RedirectToAction("Details", "Houses", new { id = houseID });
        }
        
        [Authorize]
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult eliminarReporte(int houseID, DateTime fecha)
        {

            //Buscar el reporte en la tabla de reportes filtrando por fecha
            House house = db.Houses.Find(houseID);
            String IdUser = house.ApplicationUser.Id;

            var movements2 = db.Movements.Where(mov => mov.transactionDate.Month == fecha.Month && mov.transactionDate.Year == fecha.Year && mov.houseID == houseID);
            var reporte = db.AccountStatusReport.FirstOrDefault(r => r.dateMonth.Month == fecha.Month && r.UserID == IdUser);

            //Marcar el estatus de los movimientos como falso
            foreach (var i in movements2)
            {
                i.state = false;
                db.Entry(i).State = EntityState.Modified;

            }

            //Eliminar reporte
            if (reporte != null)
            {
                db.AccountStatusReport.Remove(reporte);
            }
            db.SaveChanges();

            return RedirectToAction("Details", "Houses", new { id = houseID });
        }

        [Authorize]
        public ActionResult ReportedMovements(int accountStatusReportID) {


            String mensaje = db.GeneralInformations.Find(1).InformacionGen;
            ViewBag.mensaje = mensaje;
            String Nombre = db.AccountStatusReport.Find(accountStatusReportID).house.ApplicationUser.firstName;
            String Apellido = db.AccountStatusReport.Find(accountStatusReportID).house.ApplicationUser.lastName;
            String HAD = db.AccountStatusReport.Find(accountStatusReportID).house.name + ", " + db.AccountStatusReport.Find(accountStatusReportID).house.area + ", " + db.AccountStatusReport.Find(accountStatusReportID).house.adress;
            String CCSP = db.AccountStatusReport.Find(accountStatusReportID).house.cityArea + ", " + db.AccountStatusReport.Find(accountStatusReportID).house.country + ", " + db.AccountStatusReport.Find(accountStatusReportID).house.stateProvince + ", " + db.AccountStatusReport.Find(accountStatusReportID).house.postalCode;
            ViewBag.HAD = HAD;
            ViewBag.CCSP = CCSP;
            ViewBag.Nombre = Nombre;
            ViewBag.Apellido = Apellido;
            var Reporte = db.AccountStatusReport.Find(accountStatusReportID);

            return View (Reporte);
        }

        [Authorize]
        [Authorize(Roles = "Administrador")]
        public ActionResult Recalculate(int id, DateTime fechaConArgumentos)
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

                if (Movemen.typeOfMovement == Movement.TypeOfMovements.INCOME||Movemen.typeOfMovement == Movement.TypeOfMovements.CONTRIBUTION||
                    Movemen.typeOfMovement == Movement.TypeOfMovements.TAX || Movemen.typeOfMovement == Movement.TypeOfMovements.OWINGPAY)
                {
                    Movemen.balance = balanceAnterior + Movemen.amount;
                }
                else if (Movemen.typeOfMovement == Movement.TypeOfMovements.EXPENSE)
                {
                    Movemen.balance = balanceAnterior - Movemen.amount;
                }
                db.Entry(Movemen).State = EntityState.Modified;
            }
            db.SaveChanges();
            return RedirectToAction("Details", "Houses", new { id = id, fecha = fechaConArgumentos.Date });
        }

        // GET: Movements/Edit/5
        [Authorize(Roles = "Administrador")]
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
            DateTime rightNow = DateTime.Now;

            DateTime transactionDate = movement.transactionDate;
            movement.transactionDate = transactionDate.AddHours(rightNow.Hour).
                AddMinutes(rightNow.Minute).AddSeconds(rightNow.Second);

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
