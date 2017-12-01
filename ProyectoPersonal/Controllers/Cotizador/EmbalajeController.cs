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
    public class EmbalajeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Embalaje
        public ActionResult Index()
        {
            return View(db.Embalaje.ToList());
        }

        // GET: Embalaje/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Embalaje embalaje = db.Embalaje.Find(id);
            if (embalaje == null)
            {
                return HttpNotFound();
            }
            return View(embalaje);
        }

        // GET: Embalaje/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Embalaje/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idEmbalaje,Base,AltoCaja,CajaEstandar,EncajadoxCaja,Estado")] Embalaje embalaje)
        {
            if (ModelState.IsValid)
            {
                db.Embalaje.Add(embalaje);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(embalaje);
        }

        // GET: Embalaje/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Embalaje embalaje = db.Embalaje.Find(id);
            if (embalaje == null)
            {
                return HttpNotFound();
            }
            return View(embalaje);
        }

        // POST: Embalaje/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idEmbalaje,Base,AltoCaja,CajaEstandar,EncajadoxCaja,Estado")] Embalaje embalaje)
        {
            if (ModelState.IsValid)
            {
                db.Entry(embalaje).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(embalaje);
        }

        // GET: Embalaje/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Embalaje embalaje = db.Embalaje.Find(id);
            if (embalaje == null)
            {
                return HttpNotFound();
            }
            return View(embalaje);
        }

        // POST: Embalaje/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Embalaje embalaje = db.Embalaje.Find(id);
            db.Embalaje.Remove(embalaje);
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
