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
using Sunvalley_PLSystem.GeneralTools;
using static Sunvalley_PLSystem.Models.ReportedMovements;
using OfficeOpenXml;
using static Sunvalley_PLSystem.GeneralTools.ExcelTools;
using static Sunvalley_PLSystem.Models.House;

namespace Sunvalley_PLSystem.Controllers
{
    [Authorize]
    public class MovementsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Movements
        [Authorize(Roles = ApplicationUser.RoleNames.ADMINISTRADOR)]
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
            if (!User.IsInRole(ApplicationUser.RoleNames.ADMINISTRADOR))
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
            House house = movement.house;
            String mensaje = db.GeneralInformations.Find(1).InformacionGen;
            String NombreCompleto = house.ApplicationUser.firstName + " " + house.ApplicationUser.lastName;
            String HAD = house.name + ", " + house.area + ", " + house.adress;
            String CCSP = house.cityArea + ", " + house.country + ", " + house.stateProvince + ", " + house.postalCode;
            ViewBag.HAD = HAD;
            ViewBag.CCSP = CCSP;
            ViewBag.NombreCompleto = NombreCompleto;
            ViewBag.mensaje = mensaje;

            return View(movement);
        }

        // GET: Movements/Create
        [Authorize(Roles = ApplicationUser.RoleNames.ADMINISTRADOR)]
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
                //Se toma la fecha de transaccion introducida y se le agrega la hora de la introduccion del movimiento
                DateTime transactionDate = movement.transactionDate;
                movement.transactionDate = transactionDate.AddHours(rightNow.Hour).
                    AddMinutes(rightNow.Minute).AddSeconds(rightNow.Second);
                if (movement.typeOfMovement == Movement.TypeOfMovements.INCOME)
                {
                    String serviceName = "RENT";
                    Services rent = db.Services.SingleOrDefault(ser => ser.name == serviceName);
                    //Si es un income de servicio de renta, se busca y si no existe, se crea el servicio
                    if (rent == null || rent.serviceID == 0)
                    {
                        rent = new Services();
                        rent.name = serviceName;
                        db.Services.Add(rent);
                        db.SaveChanges();
                    }
                    //Se asigna al movimiento
                    movement.serviceID = rent.serviceID;
                }
                else if (movement.typeOfMovement == Movement.TypeOfMovements.CONTRIBUTION)
                {
                    //Si es un tipo de movimiento de contribucion y servicio del mismo nombre, se busca
                    Services contri = db.Services.SingleOrDefault(ser => ser.name == Movement.TypeOfMovements.CONTRIBUTION);
                    //Si no existe se crea
                    if (contri == null || contri.serviceID == 0)
                    {
                        contri = new Services();
                        contri.name = Movement.TypeOfMovements.CONTRIBUTION;
                        db.Services.Add(contri);
                        db.SaveChanges();
                    }
                    //Y se asigna al movimiento que se va a crear
                    movement.serviceID = contri.serviceID;
                }
                else if( movement.typeOfMovement == Movement.TypeOfMovements.OWINGPAY)
                {
                    String serviceName = "Owing Balance";
                    Services serv = db.Services.SingleOrDefault(ser => ser.name == serviceName);
                    //Si es un income de servicio de renta, se busca y si no existe, se crea el servicio
                    if (serv == null || serv.serviceID == 0)
                    {
                        serv = new Services();
                        serv.name = serviceName;
                        db.Services.Add(serv);
                        db.SaveChanges();
                    }
                    //Se asigna al movimiento
                    movement.serviceID = serv.serviceID;
                }
                try
                {
                    //Se determina la cantidad de balance del ultimo movimiento para darle continuidad
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
        [Authorize(Roles = ApplicationUser.RoleNames.ADMINISTRADOR)]
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
        [Authorize(Roles = ApplicationUser.RoleNames.ADMINISTRADOR)]
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
            var reporte = db.AccountStatusReport.FirstOrDefault(r => r.dateMonth.Month == fecha.Month && r.dateMonth.Year == fecha.Year && r.UserID == IdUser && r.houseID == houseID);
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

            return RedirectToAction("Details", "Houses", new { id = houseID, fecha = fecha });
        }
        
        [Authorize]
        [HttpPost]
        [Authorize(Roles = ApplicationUser.RoleNames.ADMINISTRADOR)]
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
        public ActionResult ReportedMovements(int accountStatusReportID)
        {


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

            return View(Reporte);
        }

        [Authorize]
        public FileResult ReportedMovementsToExcel(int accountStatusReportID)
        {
            AccountStatusReport Reporte = db.AccountStatusReport.Find(accountStatusReportID);
            List<TableToExportExcel> datatables = new List<TableToExportExcel>();

            //Getting house data to exportr
            VMHouse vmHouse = Reporte.house.getVM();
            var houseList = new List<VMHouse>();
            houseList.Add(vmHouse);
            DataTable dtHOuse = ExcelTools.listToDatatable<VMHouse>(houseList);

            //Exporting movements data
            List<VMReportedMovementes> vmReporte = VMReportedMovementes.listToVMReportedMovements(Reporte.ReportedMovements.ToList());
            DataTable dtReporte = ExcelTools.listToDatatable<VMReportedMovementes>(vmReporte);

            //Generating excel
            string heading = "Monthly Statement " + Reporte.dateMonth.ToString("MMMM-yyyy");
            datatables.Add(new TableToExportExcel(dtHOuse,"House"));
            datatables.Add(new TableToExportExcel(dtReporte, heading));
            ExcelPackage package = ExcelTools.exportToExcel(datatables, heading);
            byte[] bytesFile = package.GetAsByteArray();

            //Preparing download file
            string usrName = Reporte.house.ApplicationUser.UserName;
            usrName = usrName.Substring(0, usrName.IndexOf('@'));
            return File(bytesFile, ExcelTools.EXCEL_MIME_TYPE,String.Format("{0}_{1}_{2}{3}",heading,usrName,Reporte.house.name,ExcelTools.EXCEL_FORMAT));
        }

        private DateTime today = DateTime.Today;
        [Authorize]
        [Authorize(Roles = "Administrador")]
        public ActionResult Recalculate(int id, DateTime? fechaConArgumentos)
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
                    Movemen.typeOfMovement == Movement.TypeOfMovements.TAX)
                {
                    Movemen.balance = balanceAnterior + Movemen.amount;
                }
                else if (Movemen.typeOfMovement == Movement.TypeOfMovements.EXPENSE || Movemen.typeOfMovement == Movement.TypeOfMovements.OWINGPAY)
                {
                    Movemen.balance = balanceAnterior - Movemen.amount;
                }
                db.Entry(Movemen).State = EntityState.Modified;
            }
            db.SaveChanges();

            fechaConArgumentos = fechaConArgumentos == null ? DateTime.Today : fechaConArgumentos;
            return RedirectToAction("Details", "Houses", new { id = id, fecha = fechaConArgumentos.Value.Date });
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
                return RedirectToAction("Recalculate", new { id = movement.houseID, fechaConArgumentos = movement.transactionDate });
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
            return RedirectToAction("Recalculate", new { id = movement.houseID, fechaConArgumentos = movement.transactionDate });
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
