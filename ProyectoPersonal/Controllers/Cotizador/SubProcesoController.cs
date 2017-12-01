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
    public class SubProcesoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: SubProceso
        public ActionResult Index()
        {
            var subProceso = db.SubProceso.Include(s => s.Doblez).Include(s => s.Proceso).Include(s => s.TipoMoneda).Include(s => s.UnidadMedida);
            return View(subProceso.ToList());
        }

        // GET: SubProceso/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubProceso subProceso = db.SubProceso.Find(id);
            if (subProceso == null)
            {
                return HttpNotFound();
            }
            return View(subProceso);
        }

        // GET: SubProceso/Create
        public ActionResult Create()
        {
            ViewBag.DoblezId = new SelectList(db.Doblez, "IdDoblez", "NombreDoblez");
            ViewBag.ProcesoId = new SelectList(db.Proceso, "IdProceso", "NombreProceso");
            ViewBag.TipoMonedaId = new SelectList(db.TipoMoneda, "IdTipoMoneda", "NombreTipoMoneda");
            ViewBag.UnidadMedidaId = new SelectList(db.UnidadMedida, "IdUnidadMedida", "NombreUnidadMedida");
            return View();
        }

        // POST: SubProceso/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "idSubProceso,NombreSubProceso,CostoFijoSubProceso,CostoVariableSubProceso,Observacion,ProcesoId,DoblezId,UnidadMedidaId,TipoMonedaId")] SubProceso subProceso)
        {
            if (ModelState.IsValid)
            {
                db.SubProceso.Add(subProceso);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DoblezId = new SelectList(db.Doblez, "IdDoblez", "NombreDoblez", subProceso.DoblezId);
            ViewBag.ProcesoId = new SelectList(db.Proceso, "IdProceso", "NombreProceso", subProceso.ProcesoId);
            ViewBag.TipoMonedaId = new SelectList(db.TipoMoneda, "IdTipoMoneda", "NombreTipoMoneda", subProceso.TipoMonedaId);
            ViewBag.UnidadMedidaId = new SelectList(db.UnidadMedida, "IdUnidadMedida", "NombreUnidadMedida", subProceso.UnidadMedidaId);
            return View(subProceso);
        }

        // GET: SubProceso/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubProceso subProceso = db.SubProceso.Find(id);
            if (subProceso == null)
            {
                return HttpNotFound();
            }
            ViewBag.DoblezId = new SelectList(db.Doblez, "IdDoblez", "NombreDoblez", subProceso.DoblezId);
            ViewBag.ProcesoId = new SelectList(db.Proceso, "IdProceso", "NombreProceso", subProceso.ProcesoId);
            ViewBag.TipoMonedaId = new SelectList(db.TipoMoneda, "IdTipoMoneda", "NombreTipoMoneda", subProceso.TipoMonedaId);
            ViewBag.UnidadMedidaId = new SelectList(db.UnidadMedida, "IdUnidadMedida", "NombreUnidadMedida", subProceso.UnidadMedidaId);
            return View(subProceso);
        }

        // POST: SubProceso/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "idSubProceso,NombreSubProceso,CostoFijoSubProceso,CostoVariableSubProceso,Observacion,ProcesoId,DoblezId,UnidadMedidaId,TipoMonedaId")] SubProceso subProceso)
        {
            if (ModelState.IsValid)
            {
                db.Entry(subProceso).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DoblezId = new SelectList(db.Doblez, "IdDoblez", "NombreDoblez", subProceso.DoblezId);
            ViewBag.ProcesoId = new SelectList(db.Proceso, "IdProceso", "NombreProceso", subProceso.ProcesoId);
            ViewBag.TipoMonedaId = new SelectList(db.TipoMoneda, "IdTipoMoneda", "NombreTipoMoneda", subProceso.TipoMonedaId);
            ViewBag.UnidadMedidaId = new SelectList(db.UnidadMedida, "IdUnidadMedida", "NombreUnidadMedida", subProceso.UnidadMedidaId);
            return View(subProceso);
        }

        // GET: SubProceso/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubProceso subProceso = db.SubProceso.Find(id);
            if (subProceso == null)
            {
                return HttpNotFound();
            }
            return View(subProceso);
        }

        // POST: SubProceso/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SubProceso subProceso = db.SubProceso.Find(id);
            db.SubProceso.Remove(subProceso);
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
