﻿using System;
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
    public class MaquinaController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Maquina
        public ActionResult Index()
        {
            return View(db.Maquina.ToList());
        }

        // GET: Maquina/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Maquina maquina = db.Maquina.Find(id);
            if (maquina == null)
            {
                return HttpNotFound();
            }
            return View(maquina);
        }

        // GET: Maquina/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Maquina/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdMaquina,NombreMaquina,MermaFija,MermaVariable,Desarrollo")] Maquina maquina)
        {
            if (ModelState.IsValid)
            {
                db.Maquina.Add(maquina);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(maquina);
        }

        // GET: Maquina/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Maquina maquina = db.Maquina.Find(id);
            if (maquina == null)
            {
                return HttpNotFound();
            }
            return View(maquina);
        }

        // POST: Maquina/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "IdMaquina,NombreMaquina,MermaFija,MermaVariable,Desarrollo")] Maquina maquina)
        {
            if (ModelState.IsValid)
            {
                db.Entry(maquina).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(maquina);
        }

        // GET: Maquina/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Maquina maquina = db.Maquina.Find(id);
            if (maquina == null)
            {
                return HttpNotFound();
            }
            return View(maquina);
        }

        // POST: Maquina/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Maquina maquina = db.Maquina.Find(id);
            db.Maquina.Remove(maquina);
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
