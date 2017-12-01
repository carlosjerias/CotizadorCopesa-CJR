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
    public class TipoProcesoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TipoProceso
        public ActionResult Index()
        {
            return View(db.TipoProceso.ToList());
        }

        // GET: TipoProceso/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoProceso tipoProceso = db.TipoProceso.Find(id);
            if (tipoProceso == null)
            {
                return HttpNotFound();
            }
            return View(tipoProceso);
        }

        // GET: TipoProceso/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoProceso/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdTipoProceso,NombreTipoProceso")] TipoProceso tipoProceso)
        {
            if (ModelState.IsValid)
            {
                db.TipoProceso.Add(tipoProceso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoProceso);
        }

        // GET: TipoProceso/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoProceso tipoProceso = db.TipoProceso.Find(id);
            if (tipoProceso == null)
            {
                return HttpNotFound();
            }
            return View(tipoProceso);
        }

        // POST: TipoProceso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdTipoProceso,NombreTipoProceso")] TipoProceso tipoProceso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoProceso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoProceso);
        }

        // GET: TipoProceso/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoProceso tipoProceso = db.TipoProceso.Find(id);
            if (tipoProceso == null)
            {
                return HttpNotFound();
            }
            return View(tipoProceso);
        }

        // POST: TipoProceso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoProceso tipoProceso = db.TipoProceso.Find(id);
            db.TipoProceso.Remove(tipoProceso);
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
