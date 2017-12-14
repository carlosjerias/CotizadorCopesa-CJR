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
    public class ProcesoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Proceso
        public ActionResult Index()
        {
            var proceso = db.Proceso.Include(p => p.Solicitud);
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
            ViewBag.SolicitudId = new SelectList(db.Solicitud, "IdSolicitud", "NombreProducto");

            ViewBag.Coloress = new SelectList(db.Colores, "NumeroColor", "NumeroColor");
            ViewBag.ColoresID2 = new SelectList(db.Colores, "NumeroColor", "NumeroColor");
            return View();
        }

        // POST: Proceso/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public string Create(Proceso proceso, int ColoresID1, int ColoresID2)
        {
            if (ColoresID1 != 0 || ColoresID2 != 0)
            {
                var Color1 = new Colores(){ NumeroColor = ColoresID1, ProcesoId = proceso.IdProceso };
                var Color2 = new Colores() { NumeroColor = ColoresID2, ProcesoId = proceso.IdProceso };
                List<Colores> colores = new List<Colores>();
                

                if (ModelState.IsValid)
                {
                    db.Proceso.Add(proceso);
                    db.Colores.AddRange(colores);
                    db.SaveChanges();
                    return "OK";
                }
            }
            return "Error";
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
            ViewBag.SolicitudId = new SelectList(db.Solicitud, "IdSolicitud", "NombreProducto", proceso.SolicitudId);
            return View(proceso);
        }

        // POST: Proceso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdProceso,NombreProceso,CantidadProceso,Papel,Gramaje,Observacion,SolicitudId")] Proceso proceso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(proceso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.SolicitudId = new SelectList(db.Solicitud, "IdSolicitud", "NombreProducto", proceso.SolicitudId);
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
