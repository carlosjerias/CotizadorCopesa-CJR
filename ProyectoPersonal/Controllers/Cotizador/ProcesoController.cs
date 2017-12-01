using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoPersonal.Models;
using ProyectoPersonal.Models.Cotizador;

namespace ProyectoPersonal.Controllers.Cotizador
{
    public class ProcesoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Proceso
        public ActionResult Index()
        {
            var proceso = db.Proceso.Include(p => p.TipoProceso);
            return View(proceso.ToList());
        }

        // GET: Proceso/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proceso proceso = db.Proceso.Find(id);
            if (proceso == null)
            {
                return HttpNotFound();
            }
            return View(proceso);
        }

        // GET: Proceso/Create
        public ActionResult Create()
        {
            ViewBag.TipoProcesoId = new SelectList(db.TipoProceso, "IdTipoProceso", "NombreTipoProceso");
            return View();
        }

        // POST: Proceso/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdProceso,NombreProceso,TipoProcesoId")] Proceso proceso)
        {
            if (ModelState.IsValid)
            {
                db.Proceso.Add(proceso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TipoProcesoId = new SelectList(db.TipoProceso, "IdTipoProceso", "NombreTipoProceso", proceso.TipoProcesoId);
            return View(proceso);
        }

        // GET: Proceso/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proceso proceso = db.Proceso.Find(id);
            if (proceso == null)
            {
                return HttpNotFound();
            }
            ViewBag.TipoProcesoId = new SelectList(db.TipoProceso, "IdTipoProceso", "NombreTipoProceso", proceso.TipoProcesoId);
            return View(proceso);
        }

        // POST: Proceso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdProceso,NombreProceso,TipoProcesoId")] Proceso proceso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(proceso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TipoProcesoId = new SelectList(db.TipoProceso, "IdTipoProceso", "NombreTipoProceso", proceso.TipoProcesoId);
            return View(proceso);
        }

        // GET: Proceso/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Proceso proceso = db.Proceso.Find(id);
            if (proceso == null)
            {
                return HttpNotFound();
            }
            return View(proceso);
        }

        // POST: Proceso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Proceso proceso = db.Proceso.Find(id);
            db.Proceso.Remove(proceso);
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
