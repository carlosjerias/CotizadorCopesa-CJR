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
    public class TipoMonedaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: TipoMoneda
        public ActionResult Index()
        {
            return View(db.TipoMoneda.ToList());
        }

        // GET: TipoMoneda/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoMoneda tipoMoneda = db.TipoMoneda.Find(id);
            if (tipoMoneda == null)
            {
                return HttpNotFound();
            }
            return View(tipoMoneda);
        }

        // GET: TipoMoneda/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TipoMoneda/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdTipoMoneda,NombreTipoMoneda,SiglasTipoMoneda")] TipoMoneda tipoMoneda)
        {
            if (ModelState.IsValid)
            {
                db.TipoMoneda.Add(tipoMoneda);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tipoMoneda);
        }

        // GET: TipoMoneda/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoMoneda tipoMoneda = db.TipoMoneda.Find(id);
            if (tipoMoneda == null)
            {
                return HttpNotFound();
            }
            return View(tipoMoneda);
        }

        // POST: TipoMoneda/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdTipoMoneda,NombreTipoMoneda,SiglasTipoMoneda")] TipoMoneda tipoMoneda)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tipoMoneda).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tipoMoneda);
        }

        // GET: TipoMoneda/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TipoMoneda tipoMoneda = db.TipoMoneda.Find(id);
            if (tipoMoneda == null)
            {
                return HttpNotFound();
            }
            return View(tipoMoneda);
        }

        // POST: TipoMoneda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TipoMoneda tipoMoneda = db.TipoMoneda.Find(id);
            db.TipoMoneda.Remove(tipoMoneda);
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
