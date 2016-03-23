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
        public ActionResult Create()
        {
            ViewBag.houseID = new SelectList(db.Houses, "houseID", "name");
            return View();
        }

        // POST: Movements/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]

        public ActionResult Create([Bind(Include = "movementID,createBy,transactionDate,code,description,value,qty,amount,balance,houseID,UserID")] Movement movement)
        {
            if (ModelState.IsValid)
            {
                movement.createBy = User.Identity.GetUserName();
                movement.UserID = User.Identity.GetUserId(); ;
                db.Movements.Add(movement);
                db.SaveChanges();
                return RedirectToAction("Index");
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
            return View(movement);
        }

        // POST: Movements/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "movementID,createBy,transactionDate,code,description,value,qty,amount,balance,houseID,UserID")] Movement movement)
        {
            if (ModelState.IsValid)
            {
                db.Entry(movement).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.houseID = new SelectList(db.Houses, "houseID", "name", movement.houseID);
            return View(movement);
        }

        // GET: Movements/Delete/5
        //[Authorize(Roles = "Administrador")]
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
        //[Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Movement movement = db.Movements.Find(id);
            db.Movements.Remove(movement);
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
