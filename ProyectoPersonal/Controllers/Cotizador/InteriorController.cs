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
    public class InteriorController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Interior
        public ActionResult Index()
        {
            return View(db.Interior.ToList());
        }

        // GET: Interior/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Interior interior = db.Interior.Find(id);
            if (interior == null)
            {
                return HttpNotFound();
            }
            return View(interior);
        }

        // GET: Interior/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Interior/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdInterior,CantidadPaginas,PapelId")] Interior interior)
        {
            if (ModelState.IsValid)
            {
                db.Interior.Add(interior);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(interior);
        }

        // GET: Interior/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Interior interior = db.Interior.Find(id);
            if (interior == null)
            {
                return HttpNotFound();
            }
            return View(interior);
        }

        // POST: Interior/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdInterior,CantidadPaginas,PapelId")] Interior interior)
        {
            if (ModelState.IsValid)
            {
                db.Entry(interior).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(interior);
        }

        // GET: Interior/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Interior interior = db.Interior.Find(id);
            if (interior == null)
            {
                return HttpNotFound();
            }
            return View(interior);
        }

        // POST: Interior/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Interior interior = db.Interior.Find(id);
            db.Interior.Remove(interior);
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
