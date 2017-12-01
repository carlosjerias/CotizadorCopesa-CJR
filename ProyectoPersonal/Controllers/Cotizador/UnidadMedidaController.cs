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
    public class UnidadMedidaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: UnidadMedida
        public ActionResult Index()
        {
            return View(db.UnidadMedida.ToList());
        }

        // GET: UnidadMedida/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnidadMedida unidadMedida = db.UnidadMedida.Find(id);
            if (unidadMedida == null)
            {
                return HttpNotFound();
            }
            return View(unidadMedida);
        }

        // GET: UnidadMedida/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UnidadMedida/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdUnidadMedida,NombreUnidadMedida,SiglaUnidad")] UnidadMedida unidadMedida)
        {
            if (ModelState.IsValid)
            {
                db.UnidadMedida.Add(unidadMedida);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(unidadMedida);
        }

        // GET: UnidadMedida/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnidadMedida unidadMedida = db.UnidadMedida.Find(id);
            if (unidadMedida == null)
            {
                return HttpNotFound();
            }
            return View(unidadMedida);
        }

        // POST: UnidadMedida/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdUnidadMedida,NombreUnidadMedida,SiglaUnidad")] UnidadMedida unidadMedida)
        {
            if (ModelState.IsValid)
            {
                db.Entry(unidadMedida).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(unidadMedida);
        }

        // GET: UnidadMedida/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UnidadMedida unidadMedida = db.UnidadMedida.Find(id);
            if (unidadMedida == null)
            {
                return HttpNotFound();
            }
            return View(unidadMedida);
        }

        // POST: UnidadMedida/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UnidadMedida unidadMedida = db.UnidadMedida.Find(id);
            db.UnidadMedida.Remove(unidadMedida);
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
