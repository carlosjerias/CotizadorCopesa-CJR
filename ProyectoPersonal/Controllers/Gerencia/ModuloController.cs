
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ProyectoPersonal.Models;
using ProyectoPersonal.Models.Gerencia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ProyectoPersonal.Controllers.Gerencia
{
    public class ModuloController : Controller
    {
        // GET: Modulo
        //[Authorize(Roles = "Administrador")]
        public ActionResult Index()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                return View(db.MenuPrincipal.ToList());
            }
        }
        //[Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult ListarMenuPrincipal()
        {
            List<Modulo> lista = new List<Modulo>();
            if (User.Identity.IsAuthenticated)
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));
                    var roles = userManager.GetRoles(User.Identity.GetUserId());
                    if (db.MenuPrincipal.ToList() != null)
                    {
                        if (roles.FirstOrDefault() == "Administrador")
                        {
                            lista = db.MenuPrincipal.OrderBy(a => a.IdSeccion).OrderBy(b => b.Orden).ToList();
                        }
                        else if(roles.FirstOrDefault() == "SuperUser")
                        {
                            lista = db.MenuPrincipal.Where(x => (x.NombreModulo != "Maquina" && x.NombreModulo != "Empresa" && x.NombreModulo != "Modulo" && x.NombreModulo != "TipoMoneda")).OrderBy(a => a.IdSeccion).OrderBy(b => b.Orden).ToList();
                        }
                        else
                        {
                            lista = db.MenuPrincipal.Where(x => x.NombreModulo == "Presupuesto").OrderBy(a => a.IdSeccion).OrderBy(b => b.Orden).ToList();
                        }
                    }

                }
            }
            return View("_MenuPrincipal", (lista.Count() > 0) ? lista : null);
        }

        // GET: Modulo/Create
        [Authorize(Roles = "Administrador")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Modulo/Create
        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public ActionResult Create(Modulo modulo)
        {
            try
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    if (ModelState.IsValid)
                    {
                        db.MenuPrincipal.Add(modulo);
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

        // GET: Modulo/Edit/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Modulo/Edit/5
        [HttpPost]
        [Authorize(Roles = "Administrador")]
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

        // GET: Modulo/Delete/5
        [Authorize(Roles = "Administrador")]
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Modulo/Delete/5
        [HttpPost]
        [Authorize(Roles = "Administrador")]
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
