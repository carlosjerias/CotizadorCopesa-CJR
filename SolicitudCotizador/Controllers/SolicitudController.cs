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
    public class SolicitudController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Solicitud
        public ActionResult Index()
        {
            var solicitud = db.Solicitud.Include(s => s.Encuadernacion).Include(s => s.Formato);
            return View(solicitud.ToList());
        }

        // GET: Solicitud/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitud solicitud = db.Solicitud.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }
            return View(solicitud);
        }

        // GET: Solicitud/Create
        public ActionResult Create()
        {
            ViewBag.EncuadernacionId = new SelectList(db.Encuadernacion, "IdEncuadernacion", "NombreEncuadernacion");
            ViewBag.FormatoId = new SelectList(db.Formato, "IdFormato", "IdFormato");
            return View();
        }

        // POST: Solicitud/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdSolicitud,NombreProducto,FormatoId,EncuadernacionId,FechaProduccion,Embalaje,Despacho,CantidadPaginasTotales,ColoresTotales")] Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                db.Solicitud.Add(solicitud);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EncuadernacionId = new SelectList(db.Encuadernacion, "IdEncuadernacion", "NombreEncuadernacion", solicitud.EncuadernacionId);
            ViewBag.FormatoId = new SelectList(db.Formato, "IdFormato", "IdFormato", solicitud.FormatoId);
            return View(solicitud);
        }

        // GET: Solicitud/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitud solicitud = db.Solicitud.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }
            ViewBag.EncuadernacionId = new SelectList(db.Encuadernacion, "IdEncuadernacion", "NombreEncuadernacion", solicitud.EncuadernacionId);
            ViewBag.FormatoId = new SelectList(db.Formato, "IdFormato", "IdFormato", solicitud.FormatoId);
            return View(solicitud);
        }

        // POST: Solicitud/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdSolicitud,NombreProducto,FormatoId,EncuadernacionId,FechaProduccion,Embalaje,Despacho,CantidadPaginasTotales,ColoresTotales")] Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                db.Entry(solicitud).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EncuadernacionId = new SelectList(db.Encuadernacion, "IdEncuadernacion", "NombreEncuadernacion", solicitud.EncuadernacionId);
            ViewBag.FormatoId = new SelectList(db.Formato, "IdFormato", "IdFormato", solicitud.FormatoId);
            return View(solicitud);
        }

        // GET: Solicitud/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Solicitud solicitud = db.Solicitud.Find(id);
            if (solicitud == null)
            {
                return HttpNotFound();
            }
            return View(solicitud);
        }

        // POST: Solicitud/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Solicitud solicitud = db.Solicitud.Find(id);
            db.Solicitud.Remove(solicitud);
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
