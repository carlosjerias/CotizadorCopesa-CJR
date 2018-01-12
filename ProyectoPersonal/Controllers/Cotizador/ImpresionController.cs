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
    public class ImpresionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Impresion
        public ActionResult Index()
        {
            var impresion = db.Impresion.Include(i => i.Maquina).Include(i => i.TipoMoneda);
            return View(impresion.ToList());
        }

        
        
        // GET: Impresion/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Impresion impresion = db.Impresion.Find(id);
            if (impresion == null)
            {
                return HttpNotFound();
            }
            return View(impresion);
        }

        // GET: Impresion/Create
        public ActionResult Create()
        {
            ViewBag.MaquinaId = new SelectList(db.Maquina, "IdMaquina", "NombreMaquina");
            ViewBag.TipoMonedaId = new SelectList(db.TipoMoneda, "IdTipoMoneda", "NombreTipoMoneda");
            return View();
        }

        // POST: Impresion/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdImpresion,NombreImpresion,ValorFijoImpresion,valorvariableImpresion,MaquinaId,TipoMonedaId")] Impresion impresion)
        {
            if (ModelState.IsValid)
            {
                db.Impresion.Add(impresion);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.MaquinaId = new SelectList(db.Maquina, "IdMaquina", "NombreMaquina", impresion.MaquinaId);
            ViewBag.TipoMonedaId = new SelectList(db.TipoMoneda, "IdTipoMoneda", "NombreTipoMoneda", impresion.TipoMonedaId);
            return View(impresion);
        }

        // GET: Impresion/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Impresion impresion = db.Impresion.Find(id);
            if (impresion == null)
            {
                return HttpNotFound();
            }
            ViewBag.MaquinaId = new SelectList(db.Maquina, "IdMaquina", "NombreMaquina", impresion.MaquinaId);
            ViewBag.TipoMonedaId = new SelectList(db.TipoMoneda, "IdTipoMoneda", "NombreTipoMoneda", impresion.TipoMonedaId);
            return View(impresion);
        }

        // POST: Impresion/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdImpresion,NombreImpresion,ValorFijoImpresion,valorvariableImpresion,MaquinaId,TipoMonedaId")] Impresion impresion)
        {
            if (ModelState.IsValid)
            {
                db.Entry(impresion).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.MaquinaId = new SelectList(db.Maquina, "IdMaquina", "NombreMaquina", impresion.MaquinaId);
            ViewBag.TipoMonedaId = new SelectList(db.TipoMoneda, "IdTipoMoneda", "NombreTipoMoneda", impresion.TipoMonedaId);
            return View(impresion);
        }

        // GET: Impresion/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Impresion impresion = db.Impresion.Find(id);
            if (impresion == null)
            {
                return HttpNotFound();
            }
            return View(impresion);
        }

        // POST: Impresion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Impresion impresion = db.Impresion.Find(id);
            db.Impresion.Remove(impresion);
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
