using ProyectoPersonal.Models;
using ProyectoPersonal.Models.Cotizador;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoPersonal.Controllers.Cotizador
{
    public class DoblezController : Controller
    {
        // GET: Doblez
        public ActionResult Index()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return View(db.Doblez.ToList());
            }
        }

        // GET: Doblez/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Doblez/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Doblez/Create
        [HttpPost]
        public ActionResult Create(Doblez doblez)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    if (ModelState.IsValid)
                    {
                        db.Doblez.Add(doblez);
                        db.SaveChanges();
                        return RedirectToAction("Index");
                    }
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        // GET: Doblez/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Doblez/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Doblez/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Doblez/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
