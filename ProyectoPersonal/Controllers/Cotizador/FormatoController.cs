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
    public class FormatoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Formato
        public ActionResult Index()
        {
            var formato = db.Formato.Include(f => f.Doblez);
            return View(formato.ToList());
        }

        // GET: Formato/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formato formato = db.Formato.Find(id);
            if (formato == null)
            {
                return HttpNotFound();
            }
            return View(formato);
        }

        // GET: Formato/Create
        public ActionResult Create()
        {
            ViewBag.DoblezId = new SelectList(db.Doblez, "IdDoblez", "NombreDoblez");
            return View();
        }

        // POST: Formato/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdFormato,FormatoCerradoX,FormatoCerradoY,FormatoExtendidoX,FormatoExtendidoY,EntradasxFormatos,Interior_Ancho,Interior_Alto,TapaDiptica_Ancho,TapaDiptica_Alto,TapaTriptica_Ancho_,TapaTriptica_Alto,DoblezId")] Formato formato)
        {
            if (ModelState.IsValid)
            {
                db.Formato.Add(formato);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.DoblezId = new SelectList(db.Doblez, "IdDoblez", "NombreDoblez", formato.DoblezId);
            return View(formato);
        }

        // GET: Formato/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formato formato = db.Formato.Find(id);
            if (formato == null)
            {
                return HttpNotFound();
            }
            ViewBag.DoblezId = new SelectList(db.Doblez, "IdDoblez", "NombreDoblez", formato.DoblezId);
            return View(formato);
        }

        // POST: Formato/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdFormato,FormatoCerradoX,FormatoCerradoY,FormatoExtendidoX,FormatoExtendidoY,EntradasxFormatos,Interior_Ancho,Interior_Alto,TapaDiptica_Ancho,TapaDiptica_Alto,TapaTriptica_Ancho_,TapaTriptica_Alto,DoblezId")] Formato formato)
        {
            if (ModelState.IsValid)
            {
                db.Entry(formato).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.DoblezId = new SelectList(db.Doblez, "IdDoblez", "NombreDoblez", formato.DoblezId);
            return View(formato);
        }

        // GET: Formato/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Formato formato = db.Formato.Find(id);
            if (formato == null)
            {
                return HttpNotFound();
            }
            return View(formato);
        }

        // POST: Formato/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Formato formato = db.Formato.Find(id);
            db.Formato.Remove(formato);
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
