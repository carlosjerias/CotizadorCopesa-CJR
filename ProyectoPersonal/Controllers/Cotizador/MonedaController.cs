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
    public class MonedaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Moneda
        public ActionResult Index()
        {
            var moneda = db.Moneda.Include(m => m.TipoMoneda);
            return View(moneda.ToList());
        }
        [HttpPost]
        public ActionResult Intento(string Id)
        {
            ViewBag.algo = Id;
            return PartialView("_View");
        }
        // GET: Moneda/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Moneda moneda = db.Moneda.Find(id);
            if (moneda == null)
            {
                return HttpNotFound();
            }
            return View(moneda);
        }

        // GET: Moneda/Create
        public ActionResult Create()
        {
            ViewBag.TipoMonedaId = new SelectList(db.TipoMoneda, "IdTipoMoneda", "NombreTipoMoneda");
            return View();
        }

        // POST: Moneda/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdMoneda,TipoMonedaId,Valor,FechaValor")] Moneda moneda)
        {
            if (ModelState.IsValid)
            {
                db.Moneda.Add(moneda);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TipoMonedaId = new SelectList(db.TipoMoneda, "IdTipoMoneda", "NombreTipoMoneda", moneda.TipoMonedaId);
            return View(moneda);
        }

        // GET: Moneda/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Moneda moneda = db.Moneda.Find(id);
            if (moneda == null)
            {
                return HttpNotFound();
            }
            ViewBag.TipoMonedaId = new SelectList(db.TipoMoneda, "IdTipoMoneda", "NombreTipoMoneda", moneda.TipoMonedaId);
            return View(moneda);
        }

        // POST: Moneda/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdMoneda,TipoMonedaId,Valor,FechaValor")] Moneda moneda)
        {
            if (ModelState.IsValid)
            {
                db.Entry(moneda).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TipoMonedaId = new SelectList(db.TipoMoneda, "IdTipoMoneda", "NombreTipoMoneda", moneda.TipoMonedaId);
            return View(moneda);
        }

        // GET: Moneda/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Moneda moneda = db.Moneda.Find(id);
            if (moneda == null)
            {
                return HttpNotFound();
            }
            return View(moneda);
        }

        // POST: Moneda/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Moneda moneda = db.Moneda.Find(id);
            db.Moneda.Remove(moneda);
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
