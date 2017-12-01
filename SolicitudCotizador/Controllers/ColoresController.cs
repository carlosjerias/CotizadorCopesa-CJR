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
    public class ColoresController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Colores
        public ActionResult Index()
        {
            var colores = db.Colores;
            return View(colores.ToList());
        }

        // GET: Colores/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Colores colores = db.Colores.Find(id);
            if (colores == null)
            {
                return HttpNotFound();
            }
            return View(colores);
        }

        // GET: Colores/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Colores/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdColores,NumeroColor")] Colores colores)
        {
            if (ModelState.IsValid)
            {
                db.Colores.Add(colores);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            
            return View(colores);
        }

        // GET: Colores/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Colores colores = db.Colores.Find(id);
            if (colores == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProcesoId = new SelectList(db.Proceso, "IdProceso", "NombreProceso", colores.ProcesoId);
            return View(colores);
        }

        // POST: Colores/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdColores,NumeroColor,ProcesoId")] Colores colores)
        {
            if (ModelState.IsValid)
            {
                db.Entry(colores).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProcesoId = new SelectList(db.Proceso, "IdProceso", "NombreProceso", colores.ProcesoId);
            return View(colores);
        }

        // GET: Colores/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Colores colores = db.Colores.Find(id);
            if (colores == null)
            {
                return HttpNotFound();
            }
            return View(colores);
        }

        // POST: Colores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Colores colores = db.Colores.Find(id);
            db.Colores.Remove(colores);
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
