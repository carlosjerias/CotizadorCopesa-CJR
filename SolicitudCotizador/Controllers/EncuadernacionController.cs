using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SolicitudCotizador.Models;

namespace SolicitudCotizador.Controllers
{
    public class EncuadernacionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Encuadernacion
        public ActionResult Index()
        {
            return View(db.Encuadernacion.ToList());
        }

        // GET: Encuadernacion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Encuadernacion encuadernacion = db.Encuadernacion.Find(id);
            if (encuadernacion == null)
            {
                return HttpNotFound();
            }
            return View(encuadernacion);
        }

        // GET: Encuadernacion/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Encuadernacion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdEncuadernacion,NombreEncuadernacion")] Encuadernacion encuadernacion)
        {
            if (ModelState.IsValid)
            {
                db.Encuadernacion.Add(encuadernacion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(encuadernacion);
        }

        // GET: Encuadernacion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Encuadernacion encuadernacion = db.Encuadernacion.Find(id);
            if (encuadernacion == null)
            {
                return HttpNotFound();
            }
            return View(encuadernacion);
        }

        // POST: Encuadernacion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdEncuadernacion,NombreEncuadernacion")] Encuadernacion encuadernacion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(encuadernacion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(encuadernacion);
        }

        // GET: Encuadernacion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Encuadernacion encuadernacion = db.Encuadernacion.Find(id);
            if (encuadernacion == null)
            {
                return HttpNotFound();
            }
            return View(encuadernacion);
        }

        // POST: Encuadernacion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Encuadernacion encuadernacion = db.Encuadernacion.Find(id);
            db.Encuadernacion.Remove(encuadernacion);
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
