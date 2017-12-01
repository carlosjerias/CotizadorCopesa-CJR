using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using ProyectoPersonal.Models;
using ProyectoPersonal.Models.Cotizador;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ProyectoPersonal.Controllers
{
    public class HomeController : Controller
    {

        public RedirectToRouteResult Index()
        {
            //RedirectToRouteResult ActionResult
            //return RedirectToAction("Index", "Presupuesto");
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                Moneda a = db.Moneda.Where(x => x.Estado == true).FirstOrDefault();


                if (db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).Select(x => x.FechaValor).FirstOrDefault().ToString("dd-MM-yyyy") != DateTime.Now.ToString("dd-MM-yyyy"))
                {
                    Moneda modificable = db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).FirstOrDefault();
                    if (modificable != null)
                    {
                        modificable.Estado = false;
                        db.Entry(modificable).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    string FechaHoy = DateTime.Now.ToString("dd-MM-yyyy");
                    string apiUrl = "http://www.mindicador.cl/api";
                    string jsonString = "{}";
                    WebClient http = new WebClient();
                    JavaScriptSerializer jss = new JavaScriptSerializer();

                    http.Headers.Add(HttpRequestHeader.Accept, "application/json");
                    jsonString = http.DownloadString(apiUrl);
                    var indicatorsObject = jss.Deserialize<Dictionary<string, object>>(jsonString);

                    Dictionary<string, Dictionary<string, string>> dailyIndicators = new Dictionary<string, Dictionary<string, string>>();

                    int i = 0;
                    foreach (var key in indicatorsObject.Keys.ToArray())
                    {
                        var item = indicatorsObject[key];

                        if (item.GetType().FullName.Contains("System.Collections.Generic.Dictionary"))
                        {
                            Dictionary<string, object> itemObject = (Dictionary<string, object>)item;
                            Dictionary<string, string> indicatorProp = new Dictionary<string, string>();

                            int j = 0;
                            foreach (var key2 in itemObject.Keys.ToArray())
                            {
                                indicatorProp.Add(key2, itemObject[key2].ToString());
                                j++;
                            }

                            dailyIndicators.Add(key, indicatorProp);
                        }
                        i++;
                    }
                    Moneda moneda = new Moneda();
                    moneda.TipoMonedaId = 2;
                    moneda.Estado = true;
                    moneda.FechaValor = DateTime.Now;
                    moneda.Valor = Convert.ToDouble(dailyIndicators["uf"]["valor"]);
                    if (ModelState.IsValid)
                    {
                        db.Moneda.Add(moneda);
                        db.SaveChanges();
                        return RedirectToAction("Index", "Presupuesto");
                    }

                    else
                    {
                        return RedirectToAction("Index", "Presupuesto");
                    }
                }
                else
                {
                    return RedirectToAction("Index", "Presupuesto");
                }
            }


        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            //if (User.Identity.IsAuthenticated)
            //{
            //using (ApplicationDbContext db = new ApplicationDbContext())
            //{
            //    var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(db));
            //    //Crear un Rol
            //    var resultado = roleManager.Create(new IdentityRole("SuperUser"));
            //    resultado = roleManager.Create(new IdentityRole("Administrador"));
            //    resultado = roleManager.Create(new IdentityRole("User"));
            //    //        //var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(db));

            //    //    }
            //}
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}