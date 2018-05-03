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
using Rotativa;
using Microsoft.AspNet.Identity;
using MvcRazorToPdf;
using System.Net.Mail;
using System.IO;
using ProyectoPersonal.Models.Gerencia;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using System.Web.Services;
using System.Data.SqlClient;

namespace ProyectoPersonal.Controllers.Cotizador
{
    public class PresupuestoController : Controller
    {
        private Alertas alertas = new Alertas();
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Presupuesto
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Index()
        {
            var presupuesto = db.Presupuesto.Include(p => p.Formato).Include(p => p.Interior).Include(p => p.Tapa).Include(p => p.Interior.Papel).Include(p => p.Tapa.Papel).Include(p => p.TipoCatalogo);
            return View(presupuesto.ToList());

        }

        public ActionResult OfertaComercial(int id)
        {
            var presupuesto = db.Presupuesto.Include(p => p.Formato).Include(p => p.Interior).Include(p => p.Tapa).Include(p => p.Interior.Papel).Include(p => p.Interior.Papel.Empresa)
                                                    .Include(p => p.Tapa.Papel).Include(p => p.Tapa.Papel.Empresa).Include(p => p.Encuadernacion).Include(p => p.Moneda).Include(p => p.Usuario)
                                                    .Include(p => p.TipoCatalogo).Where(x => x.IdPresupuesto == id).FirstOrDefault();

            return View("OfertaComercial", presupuesto);
        }

        // GET: Presupuesto/Details/5
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Presupuesto presupuesto = db.Presupuesto.Find(id);
            if (presupuesto == null)
            {
                return HttpNotFound();
            }
            return View(presupuesto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public JsonResult Calcular(string NombrePresupuesto, int Tiraje, string SelectFormato, int? SelectEnc, int? CantidadInt, int? SelectPapelIntId, int? CantidadTapa,
                                        int? TapaPapel, int? ddlQuintoColor, int? ddlBarnizAcuoso, int? ddlEmbolsado, int? ddlLaminado, int? ddlBarnizUV, int? ddlAlzadoPlano, int? ddlEmbolsadoManual,
                                        int? ddlPegadoSticker, int? ddlFajado, int? ddlPegado, int? ddlInsercion, int? ddlAlzado, int? ddlDesembolsado, int? ddlAdhesivo, //int? ddlAdhesivoCms,int CantidadCajas,
                                        int CantidadModelos, int ddlQuintoColorPasadas, int? CatalogoId, string NombreCatalogo, int CantidadAlzadoPlano, int CantidadDesembolsado, int CantidadAlzado, int CantidadInsercion,
                                        int CantidadPegado, int CantidadFajado, int CantidadPegadoSticker, int CantidadEnCajas, int CantidadEnZuncho, int CantidadEnBolsa, string ddlTroquel, int CantidadTerminacionEmbolsado)
        {
            Presupuesto pres = ProcesarCalculo(SelectFormato, SelectEnc, (CantidadInt != null) ? Convert.ToInt32(CantidadInt) : 0, (CantidadTapa != null) ? Convert.ToInt32(CantidadTapa) : 0, "Plana", Tiraje, ddlQuintoColor, TapaPapel, SelectPapelIntId, ddlBarnizAcuoso, ddlEmbolsado, ddlLaminado, ddlBarnizUV
                                            , ddlAlzadoPlano, ddlEmbolsadoManual, ddlPegadoSticker, ddlFajado, ddlPegado, ddlInsercion, ddlAlzado, ddlDesembolsado, ddlAdhesivo, 4//, CantidadCajas
                                            , CantidadModelos, ddlQuintoColorPasadas, CantidadAlzadoPlano, CantidadDesembolsado, CantidadAlzado, CantidadInsercion, CantidadPegado, CantidadFajado, CantidadPegadoSticker, CatalogoId
                                            , CantidadEnCajas, CantidadEnZuncho, CantidadEnBolsa, ddlTroquel, CantidadTerminacionEmbolsado);
            if (CatalogoId != null)
            {
                string TipoCatalogo = db.Catalogo.Where(x => x.IdTipoCatalogo == (int)CatalogoId).Select(x => x.NombreTipoCatalogo).FirstOrDefault();
                pres.NombrePresupuesto = TipoCatalogo + " " + NombrePresupuesto;
            }
            else
            {
                pres.NombrePresupuesto = NombreCatalogo + " " + NombrePresupuesto;
            }

            return Json(pres, JsonRequestBehavior.AllowGet);
        }

        public Presupuesto ProcesarCalculo(string FormatoId, int? EncuadernacionId, int CantidadPaginasInt, int CantidadPaginasTap, string MaquinaTap, int Tiraje, int? IDQuintoColor
            , int? idPapelTap, int? idPapelInterior, int? BarnizAcuoso, int? Embolsado, int? Laminado, int? UV, int? AlzadoPlano, int? EmbolsadoManual, int? Sticker, int? Fajado, int? Pegado, int? Insercion, int? Alzado, int? Desembolsado,
            int? Adhesivo, int? cmsAdhesivo, int CantidadModelos, int CantidadPasadaQuintoColor, int CantidadAlzadoPlano, int CantidadDesembolsado, int CantidadAlzado, int CantidadInsercion,
                                        int CantidadPegado, int CantidadFajado, int CantidadPegadoSticker, int? CatalogoId, int CantidadEnCajas, int CantidadEnZuncho, int CantidadEnBolsa, string ddlTroquel, int CantidadTerminacionEmbolsado)
        {

            Presupuesto detalle = new Presupuesto();
            detalle.MonedaId = db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).Select(x => x.IdMoneda).FirstOrDefault();
            detalle.Tiraje = Tiraje;
            if (idPapelInterior > 0)
            {
                #region nreEntradasxFormato
                //la produccion es la tabla de pliegos totales impreso por tipo doblez.
                Produccion produccion = db.Produccion.Where(x => x.Paginas == CantidadPaginasInt).FirstOrDefault();

                int NumeroDoblez = 4;
                if (db.Papel.Where(x => x.IdPapel == (int)idPapelInterior).Select(x => x.NombrePapel).Contains("Bond"))
                {
                    if ((CantidadPaginasInt / 32) > 0)
                    {
                        produccion.Entradas16 = 0;
                        produccion.Entradas48 = 0;
                        produccion.Entradas64 = 0;
                        detalle.EntradasPag32 = (CantidadPaginasInt / 32);
                        produccion.Impresion32 = detalle.EntradasPag32;
                        NumeroDoblez = 32;
                        switch (CantidadPaginasInt - (detalle.EntradasPag32 * 32))
                        {
                            case 28:
                                detalle.EntradasPag16 = 1; produccion.Entradas16 = 1;
                                produccion.Impresion08 = 1; detalle.EntradasPag8 = 1;
                                detalle.EntradasPag4 = 1; produccion.Impresion04 = 1;
                                break;
                            case 24:
                                detalle.EntradasPag16 = 1; produccion.Entradas16 = 1;
                                detalle.EntradasPag8 = 1; produccion.Impresion08 = 1;
                                break;
                            case 20:
                                detalle.EntradasPag16 = 1; produccion.Entradas16 = 1;
                                detalle.EntradasPag4 = 1; produccion.Impresion04 = 1;
                                break;
                            case 16:
                                detalle.EntradasPag16 = 1; produccion.Entradas16 = 1;
                                break;
                            case 12:
                                detalle.EntradasPag8 = 1; produccion.Impresion08 = 1;
                                detalle.EntradasPag4 = 1; produccion.Impresion04 = 1;
                                break;
                            case 8:
                                detalle.EntradasPag8 = 1; produccion.Impresion08 = 1;
                                break;
                            case 4:
                                detalle.EntradasPag4 = 1; produccion.Impresion04 = 1;
                                break;
                        }
                        produccion.Entradas64 = (detalle.EntradasPag32);
                        produccion.Entradas16 = (detalle.EntradasPag16 + detalle.EntradasPag8 + detalle.EntradasPag4);
                    }
                    else if ((CantidadPaginasInt / 16) > 0)
                    {
                        detalle.EntradasPag16 = (CantidadPaginasInt / 16);
                        produccion.Impresion16 = detalle.EntradasPag16;
                        NumeroDoblez = 16;
                        switch (CantidadPaginasInt - (detalle.EntradasPag16 * 16))
                        {
                            case 12:
                                detalle.EntradasPag8 = 1;
                                produccion.Impresion08 = 1;
                                detalle.EntradasPag4 = 1;
                                produccion.Impresion04 = 1;
                                break;
                            case 8:
                                detalle.EntradasPag8 = 1;
                                produccion.Impresion08 = 1;
                                break;
                            case 4:
                                detalle.EntradasPag4 = 1;
                                produccion.Impresion04 = 1;
                                break;
                        }
                        produccion.Entradas16 = (detalle.EntradasPag16 + detalle.EntradasPag8 + detalle.EntradasPag4);
                    }
                    else if ((CantidadPaginasInt / 8) > 0)
                    {
                        detalle.EntradasPag8 = (CantidadPaginasInt / 8);
                        produccion.Impresion08 = detalle.EntradasPag8;
                        NumeroDoblez = 8;
                        switch (CantidadPaginasInt - (detalle.EntradasPag8 * 8))
                        {
                            case 4:
                                detalle.EntradasPag4 = 1;
                                produccion.Impresion04 = 1;
                                break;
                        }
                        produccion.Entradas16 = (detalle.EntradasPag8 + detalle.EntradasPag4);
                    }
                    produccion.Web88cms = Convert.ToDouble(CantidadPaginasInt) / Convert.ToDouble(NumeroDoblez);
                }
                else if (FormatoId != "230 x 300")
                {
                    detalle.EntradasPag64 = produccion.Impresion64;
                    detalle.EntradasPag48 = produccion.Impresion48;
                    detalle.EntradasPag32 = produccion.Impresion32;
                    detalle.EntradasPag24 = produccion.Impresion24;
                    detalle.EntradasPag16 = produccion.Impresion16;
                    detalle.EntradasPag8 = produccion.Impresion08;
                    detalle.EntradasPag4 = produccion.Impresion04;
                    if (produccion.Entradas64 > 0)
                    {
                        NumeroDoblez = 64;
                    }
                    else if (produccion.Entradas48 > 0)
                    {
                        NumeroDoblez = 48;
                    }
                    else
                    {
                        NumeroDoblez = 16;
                    }
                }

                #endregion
                #region EntradasxFormato
                else
                {
                    produccion = db.Produccion.Where(x => x.Paginas == 16).FirstOrDefault();
                    produccion.Entradas64 = 0;
                    produccion.Entradas48 = 0;

                    if ((CantidadPaginasInt / 16) > 0)
                    {
                        detalle.EntradasPag16 = (CantidadPaginasInt / 16);
                        produccion.Impresion16 = detalle.EntradasPag16;
                        NumeroDoblez = 16;
                        switch (CantidadPaginasInt - (detalle.EntradasPag16 * 16))
                        {
                            case 12:
                                detalle.EntradasPag8 = 1;
                                produccion.Impresion08 = 1;
                                detalle.EntradasPag4 = 1;
                                produccion.Impresion04 = 1;
                                break;
                            case 8:
                                detalle.EntradasPag8 = 1;
                                produccion.Impresion08 = 1;
                                break;
                            case 4:
                                detalle.EntradasPag4 = 1;
                                produccion.Impresion04 = 1;
                                break;
                        }
                        produccion.Entradas16 = (detalle.EntradasPag16 + detalle.EntradasPag8 + detalle.EntradasPag4);
                    }
                    else if ((CantidadPaginasInt / 8) > 0)
                    {
                        detalle.EntradasPag8 = (CantidadPaginasInt / 8);
                        produccion.Impresion08 = detalle.EntradasPag8;
                        NumeroDoblez = 8;
                        switch (CantidadPaginasInt - (detalle.EntradasPag8 * 8))
                        {
                            case 4:
                                detalle.EntradasPag4 = 1;
                                produccion.Impresion04 = 1;
                                break;
                        }
                        produccion.Entradas16 = (detalle.EntradasPag8 + detalle.EntradasPag4);
                    }
                    produccion.Web88cms = Convert.ToDouble(CantidadPaginasInt) / Convert.ToDouble(NumeroDoblez);
                }
                #endregion
                #region Impresion Interior Nuevo


                string[] split = FormatoId.Split('x');
                double formatox = Convert.ToDouble(split[0]);
                double formatoy = Convert.ToDouble(split[1]);
                List<Formato> listaFormatos = db.Formato.Where(x => x.FormatoCerradoX == formatox && x.FormatoCerradoY == formatoy).ToList();
                //detalle.Formato = db.Formato.Where(x => x.FormatoCerradoX == formatox && x.FormatoCerradoY == formatoy).FirstOrDefault();
                if ((db.Papel.Where(x => x.IdPapel == (int)idPapelInterior).Select(x => x.NombrePapel).Contains("Bond")))
                {
                    listaFormatos = listaFormatos.Where(x => x.EntradasxFormatos != 48).ToList();
                    foreach (Formato f in listaFormatos.Where(x => x.EntradasxFormatos != 16).ToList())
                    {
                        f.EntradasxFormatos = 32;
                        f.Interior_Ancho = 88;
                    }
                }
                Papel papelInterior = db.Papel.Where(x => x.IdPapel == (int)idPapelInterior).FirstOrDefault();
                Papel papelTapa = ((CantidadPaginasTap > 0 && idPapelTap != null)) ? db.Papel.Where(x => x.IdPapel == (int)idPapelTap).FirstOrDefault() : null;
                detalle.MaquinaInterior = (papelInterior.Gramaje > 130) ? db.Maquina.Where(x => x.NombreMaquina == "Plana").FirstOrDefault() : db.Maquina.Where(x => x.NombreMaquina == "Rotativa").FirstOrDefault();
                if (papelTapa != null)
                {
                    detalle.MaquinaTapa = ((papelTapa.Gramaje > 130) ? db.Maquina.Where(x => x.NombreMaquina == "Plana").FirstOrDefault() : db.Maquina.Where(x => x.NombreMaquina == "Rotativa").FirstOrDefault());
                }
                List<Impresion> lista = db.Impresion.Include(x => x.TipoMoneda).Include(x => x.TipoMoneda.Monedas).Include(x => x.Maquina).Where(x => x.Maquina.NombreMaquina == detalle.MaquinaInterior.NombreMaquina).ToList();

                switch (NumeroDoblez)
                {
                    case 64:
                        detalle.CostoFijoPag64 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "64").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag48 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "48").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag32 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "32").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag24 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag16 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag12 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag8 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag4 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoVariablePag64 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "64").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag48 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "48").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag32 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "32").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag24 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag16 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag12 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag8 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag4 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);

                        break;
                    case 48:
                        detalle.CostoFijoPag48 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "48").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag32 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "32").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag24 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag16 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag12 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag8 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag4 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoVariablePag48 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "48").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag32 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "32").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag24 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag16 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag12 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag8 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag4 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);

                        break;
                    case 32:
                        detalle.CostoFijoPag32 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "32").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag24 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag16 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag12 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag8 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag4 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoVariablePag32 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "32").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag24 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag16 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag12 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag8 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag4 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);

                        break;
                    case 24:
                        detalle.CostoFijoPag24 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag16 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag12 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag8 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag4 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoVariablePag24 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag16 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag12 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag8 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag4 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);

                        break;
                    case 16:
                        detalle.CostoFijoPag16 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag12 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag8 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag4 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoVariablePag16 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag12 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag8 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag4 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);

                        break;
                    case 8:
                        detalle.CostoFijoPag8 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoFijoPag4 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                        detalle.CostoVariablePag8 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                        detalle.CostoVariablePag4 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);

                        break;
                    default: break;
                }

                #endregion
                #region Tapas Nuevo
                List<Impresion> ListImpTapa = db.Impresion.Include(x => x.Maquina).Include(x => x.TipoMoneda).Include(x => x.TipoMoneda.Monedas).Where(x => x.Maquina.NombreMaquina == MaquinaTap && x.NombreImpresion == "16").ToList();
                detalle.CostoFijoTapa = (CantidadPaginasTap > 0) ? Math.Ceiling(ListImpTapa.Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion * CantidadModelos).FirstOrDefault()) : 0;
                detalle.CostoVariableTapa = (CantidadPaginasTap > 0) ? (Math.Ceiling((((ListImpTapa.Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault()) / 1000.0) / CantidadPaginasTap) * 100.0) / 100.0) : 0;

                detalle.CantidadModelos = CantidadModelos;

                Formato TapaFormato = listaFormatos.Where(x => x.EntradasxFormatos == listaFormatos.Max(y => y.EntradasxFormatos)).FirstOrDefault();
                List<SubProceso> ListTerm = db.SubProceso.Include(x => x.TipoMoneda).Include(x => x.TipoMoneda.Monedas).ToList();
                detalle.CostoFijoQuintoColor = (IDQuintoColor != null) ? Math.Ceiling(ListTerm.Where(x => x.IdSubProceso == IDQuintoColor).Select(x => (x.CostoFijoSubProceso * CantidadPasadaQuintoColor * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault())).FirstOrDefault()) : 0;
                detalle.CostoVariableQuintoColor = (IDQuintoColor != null) ? (Math.Ceiling((((ListTerm.Where(x => x.IdSubProceso == IDQuintoColor).Select(x => x.CostoVariableSubProceso * CantidadPasadaQuintoColor * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) / CantidadPaginasTap) * 100.0) / 100.0) : 0;
                detalle.NombreQuintoColor = (IDQuintoColor != null) ? ListTerm.Where(x => x.IdSubProceso == IDQuintoColor).Select(x => x.NombreSubProceso).FirstOrDefault() : "";
                detalle.CostoFijoPlizado = (db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => x.Gramaje).FirstOrDefault() >= 170) ? ListTerm.Where(x => x.NombreSubProceso == "Plizado").Select(x => x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault() : 0;
                detalle.CostoVariablePlizado = (db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => x.Gramaje).FirstOrDefault() >= 170) ? (ListTerm.Where(x => x.NombreSubProceso == "Plizado").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault() / Convert.ToDouble(CantidadPaginasTap)) : 0;

                #endregion
                #region Encuadernacion Nuevo
                detalle.CostoFijoEncuadernacion = (EncuadernacionId != null) ? Math.Ceiling(db.Encuadernacion.Where(x => x.IdEncuadernacion == EncuadernacionId).Select(x => x.ValorFijo * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(y => y.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
                detalle.CostoVariableEncuadernacion = (EncuadernacionId != null) ? (Math.Ceiling(((db.Encuadernacion.Where(x => x.IdEncuadernacion == EncuadernacionId).Select(x => x.ValorVariable * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(y => y.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) * 100.0) / 100.0) : 0;
                detalle.NombreEncuadernacion = (EncuadernacionId != null) ? db.Encuadernacion.Where(x => x.IdEncuadernacion == EncuadernacionId).Select(x => x.NombreEncuadernacion).FirstOrDefault() : "";
                #endregion
                #region Terminaciones Nuevas
                double dobleentrada = (BarnizAcuoso == 4) && (CantidadPaginasTap > 0) ? 2 : 1;
                detalle.CostoFijoBarnizAcuosoTapa = ((CantidadPaginasTap > 0) && (BarnizAcuoso >= 2)) ? Math.Ceiling(ListTerm.Where(x => x.NombreSubProceso == "Barniz Acuoso (solo tiro)").Select(x => x.CostoFijoSubProceso * dobleentrada * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
                detalle.CostoVariableBarnizAcuosoTapa = ((CantidadPaginasTap > 0) && (BarnizAcuoso >= 2)) ? Math.Ceiling((((ListTerm.Where(x => x.NombreSubProceso == "Barniz Acuoso (solo tiro)").Select(x => x.CostoVariableSubProceso * dobleentrada * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) / CantidadPaginasTap) * 100.0) / 100.0 : 0;
                detalle.CostoVariableEmbolsado = (Embolsado != null) ? (ListTerm.Where(x => x.IdSubProceso == Embolsado).Select(x => ((x.CostoVariableSubProceso * CantidadTerminacionEmbolsado) / Convert.ToDouble(detalle.Tiraje))).FirstOrDefault()) : 0;
                detalle.NombreEmbolsado = (Embolsado != null) ? ListTerm.Where(x => x.IdSubProceso == Embolsado).Select(x => x.NombreSubProceso).FirstOrDefault() : "";
                detalle.CantidadTerminacionEmbolsado = (Embolsado != null) ? CantidadTerminacionEmbolsado : 0;
                detalle.CostoVariableLaminado = ((Laminado != null) && (CantidadPaginasTap > 0)) ? (Math.Ceiling(((ListTerm.Where(x => x.IdSubProceso == Laminado).Select(x => (x.CostoVariableSubProceso * (TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0)).FirstOrDefault()) / Convert.ToDouble(CantidadPaginasTap)) * 100.0) / 100.0) : 0;
                detalle.NombreLaminado = ((Laminado != null) && (CantidadPaginasTap > 0)) ? ListTerm.Where(x => x.IdSubProceso == Laminado).Select(x => x.NombreSubProceso).FirstOrDefault() : "";
                detalle.CostoFijoBarnizUV = (UV != null) ? Math.Ceiling(ListTerm.Where(x => x.IdSubProceso == UV).Select(x => x.CostoFijoSubProceso).FirstOrDefault()) : 0;
                if (UV != null)
                {
                    SubProceso subpro = db.SubProceso.Where(x => x.IdSubProceso == UV).FirstOrDefault();
                    if (subpro.NombreSubProceso == "Barniz UV 100% en el tiro")
                    {
                        detalle.CostoVariableBarnizUV = (Math.Ceiling((ListTerm.Where(x => x.IdSubProceso == UV).Select(x => ((x.CostoVariableSubProceso * ((TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0))) / Convert.ToDouble(CantidadPaginasTap)).FirstOrDefault()) * 100.0) / 100.0);
                    }
                    else if (subpro.NombreSubProceso == "Barniz UV con Reserva")
                    {
                        detalle.CostoVariableBarnizUV = (Math.Ceiling((ListTerm.Where(x => x.IdSubProceso == UV).Select(x => (x.CostoVariableSubProceso * (Tiraje * Convert.ToDouble(1.0 / CantidadPaginasTap)) / Tiraje)).FirstOrDefault()) * 100.0) / 100.0);
                    }
                    else if (subpro.NombreSubProceso == "Barniz UV con Glitter")
                    {
                        detalle.CostoVariableBarnizUV = (Math.Ceiling((ListTerm.Where(x => x.IdSubProceso == UV).Select(x => (x.CostoVariableSubProceso) / CantidadPaginasTap).FirstOrDefault()) * 100.0) / 100.0);
                    }
                    detalle.NombreBarnizUV = ListTerm.Where(x => x.IdSubProceso == UV).Select(x => x.NombreSubProceso).FirstOrDefault();
                }
                detalle.CostoFijoTroquel = (ddlTroquel != "No" && ddlTroquel != "" && ddlTroquel != null) ? (ListTerm.Where(x => x.NombreSubProceso == "Troquel").Select(x => (x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault())).FirstOrDefault()) : 0;
                detalle.CostoVariableTroquel = (ddlTroquel != "No" && ddlTroquel != "" && ddlTroquel != null) ? ((ListTerm.Where(x => x.NombreSubProceso == "Troquel").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / Convert.ToDouble(CantidadPaginasTap)) : 0;
                detalle.CostoFijoCorteFrontal = (CantidadPaginasTap > 0) ? (ListTerm.Where(x => x.NombreSubProceso == "Corte Frontal").Select(x => x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
                detalle.CostoVariableCorteFrontal = (CantidadPaginasTap > 0) ? (ListTerm.Where(x => x.NombreSubProceso == "Corte Frontal").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;

                #endregion
                #region Manualidades Pendiente
                detalle.CostoVariableAlzadoPlano = (AlzadoPlano != null && AlzadoPlano == 2) ? (ListTerm.Where(x => x.NombreSubProceso == "Alzado Plano").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
                detalle.CostoVariableEmbolsadoManual = (EmbolsadoManual != null && EmbolsadoManual == 2) ? (ListTerm.Where(x => x.NombreSubProceso == "Embolsado manual").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;

                detalle.CostoVariableDesembolsado = (Desembolsado != null && Desembolsado == 2) ? (ListTerm.Where(x => x.NombreSubProceso == "Desembolsado simple").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
                detalle.CostoVariableAlzado = (Alzado != null) ? (ListTerm.Where(x => x.IdSubProceso == Alzado).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
                detalle.CostoVariableInsercion = (Insercion != null) ? (ListTerm.Where(x => x.IdSubProceso == Insercion).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
                detalle.CostoVariablePegado = (Pegado != null) ? (ListTerm.Where(x => x.IdSubProceso == Pegado).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
                detalle.CostoVariableFajado = (Fajado != null) ? (ListTerm.Where(x => x.IdSubProceso == Fajado).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
                detalle.CostoVariableAdhesivo = (Pegado != null) ? (ListTerm.Where(x => x.NombreSubProceso == "Stopgard (9 mm.)"//Adhesivo
                                                                                                            ).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
                detalle.AdhesivoCms = Convert.ToDouble(cmsAdhesivo);
                detalle.CostoVariableAdhesivoTotal = Math.Ceiling((detalle.AdhesivoCms * detalle.CostoVariableAdhesivo) * 10.0) / 10.0;
                #region Embalaje
                Embalaje emb = db.Embalaje.Where(x => x.Estado == true).FirstOrDefault();
                Papel p = db.Papel.Where(x => x.IdPapel == idPapelInterior).FirstOrDefault();
                double LomoInterior = Math.Ceiling(((((p.Micron * Convert.ToDouble(CantidadPaginasInt)) / 2.0) / 1000.0) + p.Adhesivo) * 10.0) / 10.0;
                double LomoTapa = db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => ((x.Micron * 2.0) / 1000.0)).FirstOrDefault();
                double LomocTapa = (Math.Ceiling((LomoInterior + (LomoTapa)) * 10)) / 10;

                if (CatalogoId != null)
                {
                    try
                    {//Productos ingresados con formato de cajas
                        string[] dimension = db.Catalogo.Where(x => x.IdTipoCatalogo == CatalogoId).Select(x => x.DimensionesCajasStandar).FirstOrDefault().Split('x');
                        detalle.LibrosxCajas = Math.Floor(Convert.ToInt32(dimension[2]) / LomocTapa) * emb.Base;
                    }
                    catch (Exception ex)
                    {
                        //Todos los productos nuevos con formato standard - cjerias
                        string[] dimension = ("320x220x283").Split('x');
                        //string formmmat = db.Catalogo.Where(x => x.IdTipoCatalogo == CatalogoId).Select(x => x.FormatoSeleccionado).SingleOrDefault();
                        //string[] dimension = db.Catalogo.Where(x => x.FormatoSeleccionado == formmmat).Select(x => x.DimensionesCajasStandar).SingleOrDefault().Split('x');
                        detalle.LibrosxCajas = Math.Floor(Convert.ToInt32(dimension[2]) / LomocTapa) * emb.Base;
                    }
                }
                else
                {
                    detalle.LibrosxCajas = Math.Floor(emb.AltoCaja / LomocTapa) * emb.Base;
                }
                detalle.CantidadCajas = (int)Math.Ceiling(CantidadEnCajas / detalle.LibrosxCajas);
                detalle.CostoVariableSuministroCaja = (Math.Ceiling((
                    ((emb.CajaEstandar * detalle.CantidadCajas) / Convert.ToDouble(Tiraje)) * 100)) / 100);
                detalle.CostoVariableInsercionCajaySellado = (Math.Ceiling((
                    ((emb.EncajadoxCaja * detalle.CantidadCajas) / Convert.ToDouble(Tiraje)) * 100)) / 100);
                detalle.Enzunchadoxpqte = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(CantidadEnZuncho) / detalle.LibrosxCajas));
                detalle.CostoVariableEnzunchado = (Math.Ceiling(((emb.Enzunchado * detalle.Enzunchadoxpqte))) / Convert.ToDouble(Tiraje) * 100) / 100;
                detalle.CantidadenBolsa = CantidadEnBolsa;


                #endregion
                detalle.CostoVariablePegadoSticker = (Sticker != null && Sticker == 2) ? (ListTerm.Where(x => x.NombreSubProceso == "Pegado de Sticker").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
                #endregion
                #region Papel
                if ((CantidadPaginasInt > 0) && (idPapelInterior != null))
                {
                    detalle.Interior = new Interior();
                    detalle.Interior.CantidadPaginas = CantidadPaginasInt;
                    detalle.Interior.PapelId = (int)idPapelInterior;

                    Formato Web88 = listaFormatos.Where(x => x.EntradasxFormatos == 16).FirstOrDefault();
                    Formato Litho132 = listaFormatos.Where(x => x.EntradasxFormatos == 48).FirstOrDefault();
                    Formato Litho174 = listaFormatos.Where(x => x.EntradasxFormatos == 64).FirstOrDefault();

                    if (Litho174 != null)
                    {
                        detalle.FormatoId = listaFormatos.Where(x => x.EntradasxFormatos == 64).Select(x => x.IdFormato).FirstOrDefault();
                    }
                    else if (Litho132 != null)
                    {
                        detalle.FormatoId = listaFormatos.Where(x => x.EntradasxFormatos == 48).Select(x => x.IdFormato).FirstOrDefault();
                    }
                    else
                    {
                        detalle.FormatoId = listaFormatos.Where(x => x.EntradasxFormatos == 16).Select(x => x.IdFormato).FirstOrDefault();
                    }

                    detalle.Interior.Entradas = (float)(((Web88 != null) ? (((produccion.Entradas16 * detalle.MaquinaInterior.MermaFija) / 1000.0) * ((papelInterior.Gramaje * Web88.Interior_Alto * Web88.Interior_Ancho) / 10000.0)) : 0) +
                                                       ((Litho132 != null) ? (((produccion.Entradas48 * detalle.MaquinaInterior.MermaFija) / 1000.0) * ((papelInterior.Gramaje * Litho132.Interior_Alto * Litho132.Interior_Ancho) / 10000.0)) : 0) +
                                                       ((Litho174 != null) ? (((produccion.Entradas64 * detalle.MaquinaInterior.MermaFija) / 1000.0) * ((papelInterior.Gramaje * Litho174.Interior_Alto * Litho174.Interior_Ancho) / 10000.0)) : 0));

                    //detalle.Interior.Entradas = (float)((((detalle.EntradasPag64 + detalle.EntradasPag48 + detalle.EntradasPag32 + detalle.EntradasPag24 + detalle.EntradasPag16 + detalle.EntradasPag12 + detalle.EntradasPag8 + detalle.EntradasPag4) * detalle.MaquinaInterior.MermaFija)/1000.0) 
                    //                                        * (((papelInterior.Gramaje * detalle.Formato.Interior_Alto * detalle.Formato.Interior_Ancho) / 10000.0)));

                    detalle.Interior.CostoPapelinteriorFijo = Math.Ceiling(detalle.Interior.Entradas * papelInterior.PrecioKilos);

                    detalle.Interior.Tiradas = (float)(((Web88 != null) ? (((Tiraje * produccion.Web88cms * detalle.MaquinaInterior.MermaVariable) / 1000.0) * ((papelInterior.Gramaje * Web88.Interior_Alto * Web88.Interior_Ancho) / 10000.0)) : 0) +
                                               ((Litho132 != null) ? (((Tiraje * produccion.Litho132cms * detalle.MaquinaInterior.MermaVariable) / 1000.0) * ((papelInterior.Gramaje * Litho132.Interior_Alto * Litho132.Interior_Ancho) / 10000.0)) : 0) +
                                               ((Litho174 != null) ? (((Tiraje * produccion.Litho174cms * detalle.MaquinaInterior.MermaVariable) / 1000.0) * ((papelInterior.Gramaje * Litho174.Interior_Alto * Litho174.Interior_Ancho) / 10000.0)) : 0));
                    //detalle.Interior.Tiradas = (float)(((Tiraje * (Convert.ToDouble(CantidadPaginasInt) / Convert.ToDouble(NumeroDoblez)) * detalle.MaquinaInterior.MermaVariable) / 1000.0) * ((papelInterior.Gramaje * detalle.Formato.Interior_Alto * detalle.Formato.Interior_Ancho) / 10000.0));

                    detalle.Interior.CostoPapelInteriorVari = (Math.Ceiling(((papelInterior.PrecioKilos * detalle.Interior.Tiradas) / Convert.ToDouble(Tiraje)) * 100.0) / 100.0);

                    float a = (float)(((Web88 != null) ? (((Tiraje * produccion.Web88cms * detalle.MaquinaInterior.MermaVariable) + (produccion.Entradas16 * detalle.MaquinaInterior.MermaFija)) * ((papelInterior.Gramaje * Web88.Interior_Alto * Web88.Interior_Ancho) / 10000000.0)) : 0));
                    float b = (float)((Litho132 != null) ? (((Tiraje * produccion.Litho132cms * detalle.MaquinaInterior.MermaVariable) + (produccion.Entradas48 * detalle.MaquinaInterior.MermaFija)) * ((papelInterior.Gramaje * Litho132.Interior_Alto * Litho132.Interior_Ancho) / 10000000.0)) : 0);
                    float c = (float)((Litho174 != null) ? (((Tiraje * produccion.Litho174cms * detalle.MaquinaInterior.MermaVariable) + (produccion.Entradas64 * detalle.MaquinaInterior.MermaFija)) * ((papelInterior.Gramaje * Litho174.Interior_Alto * Litho174.Interior_Ancho) / 10000000.0)) : 0);


                    detalle.Interior.KilosPapel = (float)(((Web88 != null) ? (((Tiraje * produccion.Web88cms * detalle.MaquinaInterior.MermaVariable) + (produccion.Entradas16 * detalle.MaquinaInterior.MermaFija)) * ((papelInterior.Gramaje * Web88.Interior_Alto * Web88.Interior_Ancho) / 10000000.0)) : 0) +
                                                         ((Litho132 != null) ? (((Tiraje * produccion.Litho132cms * detalle.MaquinaInterior.MermaVariable) + (produccion.Entradas48 * detalle.MaquinaInterior.MermaFija)) * ((papelInterior.Gramaje * Litho132.Interior_Alto * Litho132.Interior_Ancho) / 10000000.0)) : 0) +
                                                         ((Litho174 != null) ? (((Tiraje * produccion.Litho174cms * detalle.MaquinaInterior.MermaVariable) + (produccion.Entradas64 * detalle.MaquinaInterior.MermaFija)) * ((papelInterior.Gramaje * ((Litho174 != null) ? Litho174.Interior_Alto : 0)) * ((Litho174 != null) ? Litho174.Interior_Ancho : 0) / 10000000.0)) : 0));


                    //detalle.Interior.KilosPapel = (float)(((Tiraje * (Convert.ToDouble(CantidadPaginasInt) / Convert.ToDouble(NumeroDoblez)) * detalle.MaquinaInterior.MermaVariable) 
                    //    + ((detalle.EntradasPag64 + detalle.EntradasPag48 + detalle.EntradasPag32 + detalle.EntradasPag24 + detalle.EntradasPag16 + detalle.EntradasPag12 + detalle.EntradasPag8 + detalle.EntradasPag4) * detalle.MaquinaInterior.MermaFija))
                    //                            * ((papelInterior.Gramaje * detalle.Formato.Interior_Alto * detalle.Formato.Interior_Ancho) / 10000000.0));


                    if ((CantidadPaginasTap > 0 && idPapelTap != 0 && idPapelTap != null))
                    {
                        detalle.Tapa = new Tapa();
                        detalle.Tapa.CantidadPaginas = (CantidadPaginasTap > 0) ? Convert.ToInt32(CantidadPaginasTap) : 0;
                        detalle.Tapa.PapelId = (int)idPapelTap;

                        detalle.Tapa.Entradas = (float)(((1 * detalle.MaquinaTapa.MermaFija * CantidadModelos) / 1000.0) * ((papelTapa.Gramaje * TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0));

                        detalle.Tapa.CostoPapelTapaFijo = Math.Ceiling(detalle.Tapa.Entradas * papelTapa.PrecioKilos);

                        detalle.Tapa.Tiradas = (float)(((Tiraje * CantidadPaginasTap * detalle.MaquinaTapa.MermaVariable) / 1000.0) * ((papelTapa.Gramaje * TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0));
                        double wnoo = ((Tiraje * CantidadPaginasTap * detalle.MaquinaTapa.MermaVariable) / 1000.0);
                        double wno2 = ((papelTapa.Gramaje * TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0);
                        double wan2 = wnoo * wno2;


                        double pesokkk = papelTapa.PrecioKilos;
                        detalle.Tapa.CostoPapelTapaVari = (Math.Ceiling(((papelTapa.PrecioKilos * detalle.Tapa.Tiradas) / Convert.ToDouble(Tiraje)) * 100.0) / 100.0);

                        detalle.Tapa.KilosPapel = (float)((((Tiraje * (1.0 / CantidadPaginasTap) * detalle.MaquinaTapa.MermaVariable) + (1 * detalle.MaquinaTapa.MermaFija * CantidadModelos)) / 1000.0) * ((papelTapa.Gramaje * TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0));
                    };
                    float PesoInterior = (float)((((TapaFormato.FormatoCerradoX / 10.0) * (TapaFormato.FormatoCerradoY / 10.0) * papelInterior.Gramaje) / 10000000.0) * (CantidadPaginasInt / 2));
                    float Pesotapa = (float)((((TapaFormato.FormatoCerradoX / 10.0) * (TapaFormato.FormatoCerradoY / 10.0) * ((papelTapa != null) ? papelTapa.Gramaje : 0)) / 10000000.0) * (4 / 2));
                    float Enc = 0.002f;

                    detalle.CostoVariablePallet = Math.Ceiling(((emb.PalletEstandar * Math.Ceiling(((PesoInterior + Pesotapa + Enc) * Convert.ToDouble(detalle.Tiraje)) / 700)) / Convert.ToDouble(Tiraje)) * 100) / 100;//CantidadPallet);
                }
                #endregion
                #region Totales
                detalle.TarifaFijaImpresion = Math.Ceiling((detalle.CostoFijoPag64 * detalle.EntradasPag64) + (detalle.CostoFijoPag48 * detalle.EntradasPag48) + (detalle.CostoFijoPag32 * detalle.EntradasPag32) + (detalle.CostoFijoPag24 * detalle.EntradasPag24) +
                                                (detalle.CostoFijoPag16 * detalle.EntradasPag16) + (detalle.CostoFijoPag12 * detalle.EntradasPag12) + (detalle.CostoFijoPag8 * detalle.EntradasPag8) + (detalle.CostoFijoPag4 * detalle.EntradasPag4)
                                                + detalle.CostoFijoTapa);
                detalle.TarifaVariableImpresion = (Math.Ceiling(((detalle.CostoVariablePag64 * detalle.EntradasPag64) + (detalle.CostoVariablePag48 * detalle.EntradasPag48) + (detalle.CostoVariablePag32 * detalle.EntradasPag32) + (detalle.CostoVariablePag24 * detalle.EntradasPag24) +
                                                    (detalle.CostoVariablePag16 * detalle.EntradasPag16) + (detalle.CostoVariablePag12 * detalle.EntradasPag12) + (detalle.CostoVariablePag8 * detalle.EntradasPag8) +
                                                    (detalle.CostoVariablePag4 * detalle.EntradasPag4) + detalle.CostoVariableTapa) * 100.0)) / 100.0;

                detalle.TarifaFijaEncuadernacion = Math.Ceiling(detalle.CostoFijoEncuadernacion + detalle.CostoFijoPlizado + detalle.CostoFijoTroquel + detalle.CostoFijoCorteFrontal);
                detalle.TarifaVariableEncuadernacion = (Math.Ceiling((detalle.CostoVariableEncuadernacion + detalle.CostoVariablePlizado + detalle.CostoVariableTroquel + detalle.CostoVariableCorteFrontal) * 100.0)) / 100.0;

                detalle.TarifaFijaDespacho = (detalle.CostoVariableSuministroCaja + detalle.CostoVariableInsercionCajaySellado + detalle.CostoVariableEnzunchado + detalle.CostoVariablePallet);

                detalle.TarifaFijaTerminacion = Math.Ceiling(detalle.CostoFijoQuintoColor + detalle.CostoFijoBarnizUV + detalle.CostoFijoBarnizAcuosoTapa);

                detalle.TarifaFijaPapel = Math.Ceiling(detalle.Interior.CostoPapelinteriorFijo + ((papelTapa != null) ? detalle.Tapa.CostoPapelTapaFijo : 0));
                detalle.TarifaVariablePapel = (Math.Ceiling((detalle.Interior.CostoPapelInteriorVari + ((papelTapa != null) ? detalle.Tapa.CostoPapelTapaVari : 0)) * 100.0)) / 100.0;
                detalle.TarifaVariableTerminacion = (Math.Ceiling((detalle.CostoVariableQuintoColor +
                                 detalle.CostoVariableBarnizUV + detalle.CostoVariableEmbolsado + detalle.CostoVariableLaminado + detalle.CostoVariableBarnizAcuosoTapa +
                                 detalle.CostoVariableAlzadoPlano + detalle.CostoVariableEmbolsadoManual + detalle.CostoVariableDesembolsado + detalle.CostoVariableAlzado +
                                 detalle.CostoVariableInsercion + detalle.CostoVariablePegado + detalle.CostoVariableFajado + detalle.CostoVariableAdhesivoTotal + detalle.CostoVariablePegadoSticker) * 100.0)) / 100.0;
                double TarifaVariableMecanica = (Math.Ceiling((detalle.CostoVariableQuintoColor +
                                 detalle.CostoVariableBarnizUV + detalle.CostoVariableEmbolsado + detalle.CostoVariableLaminado + detalle.CostoVariableBarnizAcuosoTapa + detalle.CostoVariableAdhesivoTotal) * 100.0)) / 100.0;
                detalle.ManufacturaFijo = Math.Ceiling(detalle.TarifaFijaImpresion + detalle.TarifaFijaEncuadernacion + detalle.TarifaFijaTerminacion);
                detalle.ManufacturaVari = (Math.Ceiling((detalle.TarifaVariableImpresion + detalle.TarifaVariableEncuadernacion + detalle.TarifaVariableTerminacion + detalle.TarifaFijaDespacho) * 100.0)) / 100.0;
                detalle.TotalNetoFijo = detalle.ManufacturaFijo + detalle.TarifaFijaPapel;
                detalle.TotalNetoVari = detalle.ManufacturaVari + detalle.TarifaVariablePapel;

                detalle.TotalNetoTotal = Math.Ceiling(detalle.TotalNetoFijo + ((detalle.TotalNetoVari - (detalle.TarifaVariableTerminacion - TarifaVariableMecanica)) * Convert.ToDouble(Tiraje)) +
                                         (detalle.CostoVariableAlzadoPlano * CantidadAlzadoPlano) + (detalle.CostoVariableDesembolsado * CantidadDesembolsado) + (detalle.CostoVariableAlzado * CantidadAlzado) +
                                         (detalle.CostoVariableInsercion * CantidadInsercion) + (detalle.CostoVariablePegado * CantidadPegado) + (detalle.CostoVariableFajado * CantidadFajado) +
                                         (detalle.CostoVariablePegadoSticker * CantidadPegadoSticker));
                detalle.PrecioUnitario = (Math.Ceiling((detalle.TotalNetoTotal / Convert.ToDouble(Tiraje)) * 100) / 100);

                #endregion
            } else
            {
                #region calcularsolo tapa
                string[] split = FormatoId.Split('x');
                double formatox = Convert.ToDouble(split[0]);
                double formatoy = Convert.ToDouble(split[1]);
                List<Formato> listaFormatos = db.Formato.Where(x => x.FormatoCerradoX == formatox && x.FormatoCerradoY == formatoy).ToList();
                List<Impresion> ListImpTapa = db.Impresion.Include(x => x.Maquina).Include(x => x.TipoMoneda).Include(x => x.TipoMoneda.Monedas).Where(x => x.Maquina.NombreMaquina == MaquinaTap && x.NombreImpresion == "16").ToList();
                detalle.CostoFijoTapa = (CantidadPaginasTap > 0) ? Math.Ceiling(ListImpTapa.Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion * CantidadModelos).FirstOrDefault()) : 0;
                detalle.CostoVariableTapa = (CantidadPaginasTap > 0) ? (Math.Ceiling((((ListImpTapa.Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault()) / 1000.0) / CantidadPaginasTap) * 100.0) / 100.0) : 0;

                detalle.CantidadModelos = CantidadModelos;

                Formato TapaFormato = listaFormatos.Where(x => x.EntradasxFormatos == listaFormatos.Max(y => y.EntradasxFormatos)).FirstOrDefault();
                List<SubProceso> ListTerm = db.SubProceso.Include(x => x.TipoMoneda).Include(x => x.TipoMoneda.Monedas).ToList();
                detalle.CostoFijoQuintoColor = (IDQuintoColor != null) ? Math.Ceiling(ListTerm.Where(x => x.IdSubProceso == IDQuintoColor).Select(x => (x.CostoFijoSubProceso * CantidadPasadaQuintoColor * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault())).FirstOrDefault()) : 0;
                detalle.CostoVariableQuintoColor = (IDQuintoColor != null) ? (Math.Ceiling((((ListTerm.Where(x => x.IdSubProceso == IDQuintoColor).Select(x => x.CostoVariableSubProceso * CantidadPasadaQuintoColor * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) / CantidadPaginasTap) * 100.0) / 100.0) : 0;
                detalle.NombreQuintoColor = (IDQuintoColor != null) ? ListTerm.Where(x => x.IdSubProceso == IDQuintoColor).Select(x => x.NombreSubProceso).FirstOrDefault() : "";
                detalle.CostoFijoPlizado = (db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => x.Gramaje).FirstOrDefault() >= 170) ? ListTerm.Where(x => x.NombreSubProceso == "Plizado").Select(x => x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault() : 0;
                detalle.CostoVariablePlizado = (db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => x.Gramaje).FirstOrDefault() >= 170) ? (ListTerm.Where(x => x.NombreSubProceso == "Plizado").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault() / Convert.ToDouble(CantidadPaginasTap)) : 0;
                #endregion
                #region Embolsado_ENC
                detalle.CostoFijoEncuadernacion = (EncuadernacionId != null) ? Math.Ceiling(db.Encuadernacion.Where(x => x.IdEncuadernacion == EncuadernacionId).Select(x => x.ValorFijo * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(y => y.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
                detalle.CostoVariableEmbolsado = (Embolsado != null) ? (ListTerm.Where(x => x.IdSubProceso == Embolsado).Select(x => ((x.CostoVariableSubProceso * CantidadTerminacionEmbolsado) / Convert.ToDouble(detalle.Tiraje))).FirstOrDefault()) : 0;
                detalle.NombreEmbolsado = (Embolsado != null) ? ListTerm.Where(x => x.IdSubProceso == Embolsado).Select(x => x.NombreSubProceso).FirstOrDefault() : "";
                #endregion
                #region ValorTerminaciones
                double dobleentrada = (BarnizAcuoso == 4) && (CantidadPaginasTap > 0) ? 2 : 1;
                detalle.CostoFijoBarnizAcuosoTapa = ((CantidadPaginasTap > 0) && (BarnizAcuoso >= 2)) ? Math.Ceiling(ListTerm.Where(x => x.NombreSubProceso == "Barniz Acuoso (solo tiro)").Select(x => x.CostoFijoSubProceso * dobleentrada * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
                detalle.CostoVariableBarnizAcuosoTapa = ((CantidadPaginasTap > 0) && (BarnizAcuoso >= 2)) ? Math.Ceiling((((ListTerm.Where(x => x.NombreSubProceso == "Barniz Acuoso (solo tiro)").Select(x => x.CostoVariableSubProceso * dobleentrada * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) / CantidadPaginasTap) * 100.0) / 100.0 : 0;
                detalle.CostoVariableEmbolsado = (Embolsado != null) ? (ListTerm.Where(x => x.IdSubProceso == Embolsado).Select(x => ((x.CostoVariableSubProceso * CantidadTerminacionEmbolsado) / Convert.ToDouble(detalle.Tiraje))).FirstOrDefault()) : 0;
                detalle.NombreEmbolsado = (Embolsado != null) ? ListTerm.Where(x => x.IdSubProceso == Embolsado).Select(x => x.NombreSubProceso).FirstOrDefault() : "";
                detalle.CantidadTerminacionEmbolsado = (Embolsado != null) ? CantidadTerminacionEmbolsado : 0;
                detalle.CostoVariableLaminado = ((Laminado != null) && (CantidadPaginasTap > 0)) ? (Math.Ceiling(((ListTerm.Where(x => x.IdSubProceso == Laminado).Select(x => (x.CostoVariableSubProceso * (TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0)).FirstOrDefault()) / Convert.ToDouble(CantidadPaginasTap)) * 100.0) / 100.0) : 0;
                detalle.NombreLaminado = ((Laminado != null) && (CantidadPaginasTap > 0)) ? ListTerm.Where(x => x.IdSubProceso == Laminado).Select(x => x.NombreSubProceso).FirstOrDefault() : "";
                detalle.CostoFijoBarnizUV = (UV != null) ? Math.Ceiling(ListTerm.Where(x => x.IdSubProceso == UV).Select(x => x.CostoFijoSubProceso).FirstOrDefault()) : 0;
                if (UV != null)
                {
                    SubProceso subpro = db.SubProceso.Where(x => x.IdSubProceso == UV).FirstOrDefault();
                    if (subpro.NombreSubProceso == "Barniz UV 100% en el tiro")
                    {
                        detalle.CostoVariableBarnizUV = (Math.Ceiling((ListTerm.Where(x => x.IdSubProceso == UV).Select(x => ((x.CostoVariableSubProceso * ((TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0))) / Convert.ToDouble(CantidadPaginasTap)).FirstOrDefault()) * 100.0) / 100.0);
                    }
                    else if (subpro.NombreSubProceso == "Barniz UV con Reserva")
                    {
                        detalle.CostoVariableBarnizUV = (Math.Ceiling((ListTerm.Where(x => x.IdSubProceso == UV).Select(x => (x.CostoVariableSubProceso * (Tiraje * Convert.ToDouble(1.0 / CantidadPaginasTap)) / Tiraje)).FirstOrDefault()) * 100.0) / 100.0);
                    }
                    else if (subpro.NombreSubProceso == "Barniz UV con Glitter")
                    {
                        detalle.CostoVariableBarnizUV = (Math.Ceiling((ListTerm.Where(x => x.IdSubProceso == UV).Select(x => (x.CostoVariableSubProceso) / CantidadPaginasTap).FirstOrDefault()) * 100.0) / 100.0);
                    }
                    detalle.NombreBarnizUV = ListTerm.Where(x => x.IdSubProceso == UV).Select(x => x.NombreSubProceso).FirstOrDefault();
                }
                detalle.CostoFijoTroquel = (ddlTroquel != "No" && ddlTroquel != "" && ddlTroquel != null) ? (ListTerm.Where(x => x.NombreSubProceso == "Troquel").Select(x => (x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault())).FirstOrDefault()) : 0;
                detalle.CostoVariableTroquel = (ddlTroquel != "No" && ddlTroquel != "" && ddlTroquel != null) ? ((ListTerm.Where(x => x.NombreSubProceso == "Troquel").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / Convert.ToDouble(CantidadPaginasTap)) : 0;
                detalle.CostoFijoCorteFrontal = (CantidadPaginasTap > 0) ? (ListTerm.Where(x => x.NombreSubProceso == "Corte Frontal").Select(x => x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
                detalle.CostoVariableCorteFrontal = (CantidadPaginasTap > 0) ? (ListTerm.Where(x => x.NombreSubProceso == "Corte Frontal").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
                #endregion
                #region Cajas y Sellado
                Embalaje emb = db.Embalaje.Where(x => x.Estado == true).FirstOrDefault();
                //Papel p = db.Papel.Where(x => x.IdPapel == idPapelInterior).FirstOrDefault();
                //double LomoInterior = Math.Ceiling(((((p.Micron * Convert.ToDouble(CantidadPaginasInt)) / 2.0) / 1000.0) + p.Adhesivo) * 10.0) / 10.0;
                double LomoTapa = db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => ((x.Micron * 2.0) / 1000.0)).FirstOrDefault();
                double LomocTapa = (Math.Ceiling((0 + (LomoTapa)) * 10)) / 10;// 0= lomoInterior

                if (CatalogoId != null)
                {
                    try
                    {//Productos ingresados con formato de cajas
                        string[] dimension = db.Catalogo.Where(x => x.IdTipoCatalogo == CatalogoId).Select(x => x.DimensionesCajasStandar).FirstOrDefault().Split('x');
                        detalle.LibrosxCajas = Math.Floor(Convert.ToInt32(dimension[2]) / LomocTapa) * emb.Base;
                    }
                    catch (Exception ex)
                    {
                        //Todos los productos nuevos con formato standard - cjerias
                        string[] dimension = ("320x220x283").Split('x');
                        //string formmmat = db.Catalogo.Where(x => x.IdTipoCatalogo == CatalogoId).Select(x => x.FormatoSeleccionado).SingleOrDefault();
                        //string[] dimension = db.Catalogo.Where(x => x.FormatoSeleccionado == formmmat).Select(x => x.DimensionesCajasStandar).SingleOrDefault().Split('x');
                        detalle.LibrosxCajas = Math.Floor(Convert.ToInt32(dimension[2]) / LomocTapa) * emb.Base;
                    }
                }
                else
                {
                    detalle.LibrosxCajas = Math.Floor(emb.AltoCaja / LomocTapa) * emb.Base;
                }

                detalle.CantidadCajas = (int)Math.Ceiling(CantidadEnCajas / detalle.LibrosxCajas);
                detalle.CostoVariableSuministroCaja = (Math.Ceiling((
                    ((emb.CajaEstandar * detalle.CantidadCajas) / Convert.ToDouble(Tiraje)) * 100)) / 100);
                detalle.CostoVariableInsercionCajaySellado = (Math.Ceiling((
                    ((emb.EncajadoxCaja * detalle.CantidadCajas) / Convert.ToDouble(Tiraje)) * 100)) / 100);
                // detalle.Enzunchadoxpqte = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(CantidadEnZuncho) / detalle.LibrosxCajas));
                // detalle.CostoVariableEnzunchado = (Math.Ceiling(((emb.Enzunchado * detalle.Enzunchadoxpqte))) / Convert.ToDouble(Tiraje) * 100) / 100;
                #endregion
                #region CostoTapas
                Papel papelTapa = ((CantidadPaginasTap > 0 && idPapelTap != null)) ? db.Papel.Where(x => x.IdPapel == (int)idPapelTap).FirstOrDefault() : null;
                detalle.MaquinaTapa = ((papelTapa.Gramaje > 130) ? db.Maquina.Where(x => x.NombreMaquina == "Plana").FirstOrDefault() : db.Maquina.Where(x => x.NombreMaquina == "Rotativa").FirstOrDefault());
                detalle.Tapa = new Tapa();
                detalle.Tapa.CantidadPaginas = (CantidadPaginasTap > 0) ? Convert.ToInt32(CantidadPaginasTap) : 0;
                detalle.Tapa.PapelId = (int)idPapelTap;
                detalle.Tapa.Entradas = (float)(((1 * detalle.MaquinaTapa.MermaFija * CantidadModelos) / 1000.0) * ((papelTapa.Gramaje * TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0));
                detalle.Tapa.CostoPapelTapaFijo = Math.Ceiling(detalle.Tapa.Entradas * papelTapa.PrecioKilos);
                detalle.Tapa.Tiradas = (float)(((Tiraje * CantidadPaginasTap * detalle.MaquinaTapa.MermaVariable) / 1000.0) * ((papelTapa.Gramaje * TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0));
                detalle.Tapa.CostoPapelTapaVari = (Math.Ceiling(((papelTapa.PrecioKilos * detalle.Tapa.Tiradas) / Convert.ToDouble(Tiraje)) * 100.0) / 100.0);
                detalle.Tapa.KilosPapel = (float)((((Tiraje * (1.0 / CantidadPaginasTap) * detalle.MaquinaTapa.MermaVariable) + (1 * detalle.MaquinaTapa.MermaFija * CantidadModelos)) / 1000.0) * ((papelTapa.Gramaje * TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0));

                detalle.Interior = new Interior();
                detalle.Interior = null;




                //float PesoInterior = (float)((((TapaFormato.FormatoCerradoX / 10.0) * (TapaFormato.FormatoCerradoY / 10.0) * papelInterior.Gramaje) / 10000000.0) * (CantidadPaginasInt / 2));
                float Pesotapa = (float)((((TapaFormato.FormatoCerradoX / 10.0) * (TapaFormato.FormatoCerradoY / 10.0) * ((papelTapa != null) ? papelTapa.Gramaje : 0)) / 10000000.0) * (4 / 2));
                float Enc = 0.002f;

                detalle.CostoVariablePallet = Math.Ceiling(((emb.PalletEstandar * Math.Ceiling(((0 + Pesotapa + Enc) * Convert.ToDouble(detalle.Tiraje)) / 700)) / Convert.ToDouble(Tiraje)) * 100) / 100;//CantidadPallet);
                #endregion

                #region Totales
                //230 x 300
                detalle.FormatoId = listaFormatos.Where(x => (x.FormatoCerradoX + " x " + x.FormatoCerradoY) == FormatoId).Select(x => x.IdFormato).FirstOrDefault();
                detalle.TarifaFijaImpresion = Math.Ceiling((detalle.CostoFijoPag64 * detalle.EntradasPag64) + (detalle.CostoFijoPag48 * detalle.EntradasPag48) + (detalle.CostoFijoPag32 * detalle.EntradasPag32) + (detalle.CostoFijoPag24 * detalle.EntradasPag24) +
                                                (detalle.CostoFijoPag16 * detalle.EntradasPag16) + (detalle.CostoFijoPag12 * detalle.EntradasPag12) + (detalle.CostoFijoPag8 * detalle.EntradasPag8) + (detalle.CostoFijoPag4 * detalle.EntradasPag4)
                                                + detalle.CostoFijoTapa);
                detalle.TarifaVariableImpresion = (Math.Ceiling(((detalle.CostoVariablePag64 * detalle.EntradasPag64) + (detalle.CostoVariablePag48 * detalle.EntradasPag48) + (detalle.CostoVariablePag32 * detalle.EntradasPag32) + (detalle.CostoVariablePag24 * detalle.EntradasPag24) +
                                                    (detalle.CostoVariablePag16 * detalle.EntradasPag16) + (detalle.CostoVariablePag12 * detalle.EntradasPag12) + (detalle.CostoVariablePag8 * detalle.EntradasPag8) +
                                                    (detalle.CostoVariablePag4 * detalle.EntradasPag4) + detalle.CostoVariableTapa) * 100.0)) / 100.0;

                detalle.TarifaFijaEncuadernacion = Math.Ceiling(detalle.CostoFijoEncuadernacion + detalle.CostoFijoPlizado + detalle.CostoFijoTroquel + detalle.CostoFijoCorteFrontal);
                detalle.TarifaVariableEncuadernacion = (Math.Ceiling((detalle.CostoVariableEncuadernacion + detalle.CostoVariablePlizado + detalle.CostoVariableTroquel + detalle.CostoVariableCorteFrontal) * 100.0)) / 100.0;

                detalle.TarifaFijaDespacho = (detalle.CostoVariableSuministroCaja + detalle.CostoVariableInsercionCajaySellado + detalle.CostoVariableEnzunchado + detalle.CostoVariablePallet);

                detalle.TarifaFijaTerminacion = Math.Ceiling(detalle.CostoFijoQuintoColor + detalle.CostoFijoBarnizUV + detalle.CostoFijoBarnizAcuosoTapa);

                detalle.TarifaFijaPapel = Math.Ceiling(0 + ((papelTapa != null) ? detalle.Tapa.CostoPapelTapaFijo : 0));
                detalle.TarifaVariablePapel = (Math.Ceiling((0 + ((papelTapa != null) ? detalle.Tapa.CostoPapelTapaVari : 0)) * 100.0)) / 100.0;
                detalle.TarifaVariableTerminacion = (Math.Ceiling((detalle.CostoVariableQuintoColor +
                                 detalle.CostoVariableBarnizUV + detalle.CostoVariableEmbolsado + detalle.CostoVariableLaminado + detalle.CostoVariableBarnizAcuosoTapa +
                                 detalle.CostoVariableAlzadoPlano + detalle.CostoVariableEmbolsadoManual + detalle.CostoVariableDesembolsado + detalle.CostoVariableAlzado +
                                 detalle.CostoVariableInsercion + detalle.CostoVariablePegado + detalle.CostoVariableFajado + detalle.CostoVariableAdhesivoTotal + detalle.CostoVariablePegadoSticker) * 100.0)) / 100.0;
                double TarifaVariableMecanica = (Math.Ceiling((detalle.CostoVariableQuintoColor +
                                 detalle.CostoVariableBarnizUV + detalle.CostoVariableEmbolsado + detalle.CostoVariableLaminado + detalle.CostoVariableBarnizAcuosoTapa + detalle.CostoVariableAdhesivoTotal) * 100.0)) / 100.0;
                detalle.ManufacturaFijo = Math.Ceiling(0 + detalle.TarifaFijaEncuadernacion + detalle.TarifaFijaTerminacion);
                detalle.ManufacturaVari = (Math.Ceiling((0 + detalle.TarifaVariableEncuadernacion + detalle.TarifaVariableTerminacion + detalle.TarifaFijaDespacho) * 100.0)) / 100.0;
                detalle.TotalNetoFijo = detalle.ManufacturaFijo + detalle.TarifaFijaPapel;
                detalle.TotalNetoVari = detalle.ManufacturaVari + detalle.TarifaVariablePapel;

                double Total_Impresion = detalle.CostoFijoTapa + (detalle.CostoVariableTapa * Tiraje);
                double Total_Papel = detalle.TarifaFijaPapel + (detalle.TarifaVariablePapel * Tiraje);
                double Total_Encuad = detalle.TarifaFijaEncuadernacion + (detalle.TarifaVariableEncuadernacion * Tiraje);
                double Total_Terminacion = detalle.TarifaFijaTerminacion + (detalle.TarifaVariableTerminacion * Tiraje);
                double Total_Despacho = detalle.TarifaFijaDespacho * Tiraje;

                //detalle.TotalNetoTotal = Math.Ceiling(detalle.TotalNetoFijo + ((detalle.TotalNetoVari - (detalle.TarifaVariableTerminacion - TarifaVariableMecanica)) * Convert.ToDouble(Tiraje)) +
                //                         (detalle.CostoVariableAlzadoPlano * CantidadAlzadoPlano) + (detalle.CostoVariableDesembolsado * CantidadDesembolsado) + (detalle.CostoVariableAlzado * CantidadAlzado) +
                //                         (detalle.CostoVariableInsercion * CantidadInsercion) + (detalle.CostoVariablePegado * CantidadPegado) + (detalle.CostoVariableFajado * CantidadFajado) +
                //                         (detalle.CostoVariablePegadoSticker * CantidadPegadoSticker));

                detalle.TotalNetoTotal = Total_Impresion + Total_Papel + Total_Encuad + Total_Terminacion + Total_Despacho;
                detalle.PrecioUnitario = (Math.Ceiling((detalle.TotalNetoTotal / Convert.ToDouble(Tiraje)) * 100) / 100);

                #endregion

                #region valores null
                detalle.EntradasPag64 = 0;
                detalle.EntradasPag48 = 0;
                detalle.EntradasPag32 = 0;
                detalle.EntradasPag16 = 0;
                detalle.EntradasPag12 = 0;
                detalle.EntradasPag8 = 0;
                detalle.EntradasPag4 = 0;

                detalle.CostoFijoPag64 = 0;
                detalle.CostoFijoPag48 = 0;
                detalle.CostoFijoPag32 = 0;
                detalle.CostoFijoPag16 = 0;
                detalle.CostoFijoPag12 = 0;
                detalle.CostoFijoPag8 = 0;
                detalle.CostoFijoPag4 = 0;

                detalle.CostoVariablePag64 = 0;
                detalle.CostoVariablePag48 = 0;
                detalle.CostoVariablePag32 = 0;
                detalle.CostoVariablePag16 = 0;
                detalle.CostoVariablePag12 = 0;
                detalle.CostoVariablePag8 = 0;
                detalle.CostoVariablePag4 = 0;
                 
               
                #endregion
            }
            return detalle;
        }

        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult SeleccionarPapel(int EmpresaID)
        {
            List<Papel> lista = db.Papel.Where(x => x.EmpresaId == EmpresaID).OrderBy(x => x.NombrePapel).ThenBy(x => x.Gramaje).ToList();
            return Json(new SelectList(lista, "IdPapel", "NombreCompletoPapel"),
                JsonRequestBehavior.AllowGet
            );
        }
        //version original
        //[Authorize(Roles = "Administrador,SuperUser,User")]
        //public string SeleccionarCatalogo(int IDCatalogo)
        //{
        //    string FormatoSeleccionado = db.Catalogo.Where(x => x.IdTipoCatalogo == IDCatalogo).Select(x => x.FormatoSeleccionado +","+  x.PapelInterior +"," + x.PapelTapa).FirstOrDefault();
        //    return FormatoSeleccionado;
        //}
        [WebMethod]
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult SeleccionarCatalogo(int IDCatalogo)
        {
            TipoCatalogo tipoc = db.Catalogo.Where(x => x.IdTipoCatalogo == IDCatalogo).FirstOrDefault();
            string EmpresaPapelInt = db.Papel.Where(x => x.IdPapel == tipoc.PapelInteriorId).Select(x => x.Empresa.NombreEmpresa).SingleOrDefault();
            string EmpresaPapelTapa = db.Papel.Where(x => x.IdPapel == tipoc.PapelTapaId).Select(x => x.Empresa.NombreEmpresa).SingleOrDefault();
            int? EmpresaPapelIntId = db.Papel.Where(x => x.IdPapel == tipoc.PapelInteriorId).Select(x => x.Empresa.IdEmpresa).SingleOrDefault();
            int? EmpresaPapelTapaId = db.Papel.Where(x => x.IdPapel == tipoc.PapelTapaId).Select(x => x.Empresa.IdEmpresa).SingleOrDefault();
            var resultado = new { FormatoSeleccionado = tipoc.FormatoSeleccionado, PapelInterior = tipoc.PapelInterior, PapelTapa = tipoc.PapelTapa, EmpresaPapelInt = EmpresaPapelInt, EmpresaPapelTapa = EmpresaPapelTapa, EmpresaIntID = EmpresaPapelIntId, EmpresaTapaId = EmpresaPapelTapaId };
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }




        // GET: Presupuesto/Create
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Create()
        {
            PresupuestoForm pres = new PresupuestoForm();
            pres.Encuadernaciones = db.Encuadernacion.ToList();
            pres.Formatos = db.Formato.ToList();
            pres.Papeles = db.Papel.Where(x => x.EmpresaId == 2).OrderBy(x => x.NombrePapel).ThenBy(x => x.Gramaje).ToList();
            pres.SubProcesos = db.SubProceso.Include("Proceso").ToList();
            pres.Catalogo = db.Catalogo.ToList();
            pres.Empresa = db.Empresa.ToList();
            List<SelectListItem> s = new List<SelectListItem>();
            s.Add(new SelectListItem() { Text = "Seleccione Cantidad", Value = "0" });
            for (int i = 4; i <= 200; i = i + 4)//antes 400
            {
                s.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.CantidadInt = s;
            ViewBag.ValorUF = string.Format("{0:#,0.00}", db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).Select(x => x.Valor).FirstOrDefault());


            //ViewBag.SubProceso = db.SubProceso.Include("Proceso").ToList();
            return View(pres);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public JsonResult Guardar(string NombrePresupuesto, int Tiraje, string SelectFormato, int? SelectEnc, int CantidadInt, int? SelectPapelIntId, int? CantidadTapa, int? TapaPapel,
                                    int? ddlQuintoColor, int? ddlBarnizAcuoso, int? ddlEmbolsado, int? ddlLaminado, int? ddlBarnizUV, int? ddlAlzadoPlano, int? ddlEmbolsadoManual, int? ddlPegadoSticker, int? ddlFajado, int? ddlPegado,
                                    int? ddlInsercion, int? ddlAlzado, int? ddlDesembolsado, int? ddlAdhesivo//, int? ddlAdhesivoCms, int CantidadCajas
                                    , int CantidadModelos, int ddlQuintoColorPasadas, int? CatalogoId, string NombreCatalogo, int CantidadAlzadoPlano, int CantidadDesembolsado, int CantidadAlzado, int CantidadInsercion,
                                        int CantidadPegado, int CantidadFajado, int CantidadPegadoSticker, int CantidadEnCajas, int CantidadEnZuncho, int CantidadEnBolsa, string ddlTroquel, int CantidadTerminacionEmbolsado)
        {

            Presupuesto p = ProcesarCalculo(SelectFormato, SelectEnc, CantidadInt, (CantidadTapa != null) ? Convert.ToInt32(CantidadTapa) : 0, "Plana", Tiraje, ddlQuintoColor, TapaPapel, SelectPapelIntId, ddlBarnizAcuoso, ddlEmbolsado, ddlLaminado, ddlBarnizUV, ddlAlzadoPlano,
                                            ddlEmbolsadoManual, ddlPegadoSticker, ddlFajado, ddlPegado, ddlInsercion, ddlAlzado, ddlDesembolsado, ddlAdhesivo, 4//, CantidadCajas
                                            , CantidadModelos, ddlQuintoColorPasadas, CantidadAlzadoPlano, CantidadDesembolsado, CantidadAlzado, CantidadInsercion, CantidadPegado, CantidadFajado, CantidadPegadoSticker, CatalogoId
                                            , CantidadEnCajas, CantidadEnZuncho, CantidadEnBolsa, ddlTroquel, CantidadTerminacionEmbolsado);
            TipoCatalogo tc;
            if (CatalogoId == null)
            {
                tc = new TipoCatalogo();
                tc.NombreTipoCatalogo = NombreCatalogo;
                tc.FormatoSeleccionado = SelectFormato;
                tc.PapelInterior = db.Papel.Where(x => x.IdPapel == (int)SelectPapelIntId).Select(x => x.NombrePapel + " " + x.Gramaje).SingleOrDefault();
                tc.PapelInteriorId = db.Papel.Where(x => x.IdPapel == (int)SelectPapelIntId).Select(x => x.IdPapel).SingleOrDefault();
                try
                {
                    tc.PapelTapaId = db.Papel.Where(x => x.IdPapel == (int)TapaPapel).Select(x => x.IdPapel).SingleOrDefault();
                    tc.PapelTapa = db.Papel.Where(x => x.IdPapel == (int)TapaPapel).Select(x => x.NombrePapel + " " + x.Gramaje).SingleOrDefault();
                }
                catch
                {
                    tc.PapelTapaId = null;
                    tc.PapelTapa = null;
                }

            }
            else
            {
                tc = db.Catalogo.Where(x => x.IdTipoCatalogo == (int)CatalogoId).FirstOrDefault();
            }
            Presupuesto pres2 = new Presupuesto();
            p.NombrePresupuesto = NombrePresupuesto;
            p.TipoCatalogoId = tc.IdTipoCatalogo;
            p.FechaCreacion = DateTime.Now;
            p.Usuarioid = User.Identity.GetUserId();
            p.BarnizAcuoso = (ddlBarnizAcuoso != null ? (int)ddlBarnizAcuoso : 0);
            p.CantEnCajas = CantidadEnCajas;
            p.CantEnZuncho = CantidadEnZuncho;

            p.EncuadernacionId = SelectEnc;

            List<Presupuesto_SubProceso> listaSubProceso = new List<Presupuesto_SubProceso>();
            if (ddlQuintoColor != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlQuintoColor, ValorFijoSubProceso = p.CostoFijoQuintoColor, ValorVariableSubProceso = p.CostoVariableQuintoColor, CantidadEjemplaresProceso = p.Tiraje }); };
            if ((ddlBarnizAcuoso != null) && (ddlBarnizAcuoso == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = db.SubProceso.Where(x => x.NombreSubProceso == "Barniz Acuoso (solo tiro)").Select(x => x.IdSubProceso).FirstOrDefault(), ValorFijoSubProceso = p.CostoFijoBarnizAcuosoTapa, ValorVariableSubProceso = p.CostoVariableBarnizAcuosoTapa, CantidadEjemplaresProceso = p.Tiraje }); };
            if (ddlEmbolsado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlEmbolsado, ValorVariableSubProceso = p.CostoVariableEmbolsado, CantidadEjemplaresProceso = p.CantidadTerminacionEmbolsado }); };
            if (ddlLaminado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlLaminado, ValorVariableSubProceso = p.CostoVariableLaminado, CantidadEjemplaresProceso = p.Tiraje }); };
            if (ddlBarnizUV != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlBarnizUV, ValorFijoSubProceso = p.CostoFijoBarnizUV, ValorVariableSubProceso = p.CostoVariableLaminado, CantidadEjemplaresProceso = p.Tiraje }); };
            if ((ddlAlzadoPlano != null) && (ddlAlzadoPlano == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = db.SubProceso.Where(x => x.NombreSubProceso == "Alzado Plano").Select(x => x.IdSubProceso).FirstOrDefault(), ValorVariableSubProceso = p.CostoVariableAlzadoPlano, CantidadEjemplaresProceso = CantidadAlzadoPlano }); };
            if ((ddlEmbolsadoManual != null) && (ddlEmbolsadoManual == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = db.SubProceso.Where(x => x.NombreSubProceso == "Embolsado manual").Select(x => x.IdSubProceso).FirstOrDefault(), ValorVariableSubProceso = p.CostoVariableEmbolsadoManual, CantidadEjemplaresProceso = Tiraje }); };
            if ((ddlDesembolsado != null) && (ddlDesembolsado == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = db.SubProceso.Where(x => x.NombreSubProceso == "Desembolsado simple").Select(x => x.IdSubProceso).FirstOrDefault(), ValorVariableSubProceso = p.CostoVariableDesembolsado, CantidadEjemplaresProceso = CantidadDesembolsado }); };
            if (ddlAlzado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlAlzado, ValorVariableSubProceso = p.CostoVariableAlzado, CantidadEjemplaresProceso = CantidadAlzado }); };
            if (ddlInsercion != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlInsercion, ValorVariableSubProceso = p.CostoVariableInsercion, CantidadEjemplaresProceso = CantidadInsercion }); };
            if (ddlPegado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlPegado, ValorVariableSubProceso = p.CostoVariablePegado, CantidadEjemplaresProceso = CantidadPegado }); };
            if (ddlFajado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlFajado, ValorVariableSubProceso = p.CostoVariableFajado, CantidadEjemplaresProceso = CantidadFajado }); };
            if (ddlPegado != null)
            {
                listaSubProceso.Add(new Presupuesto_SubProceso()
                {
                    PresupuestoId = p.IdPresupuesto,
                    SubProcesoId = 35//(int)ddlAdhesivo
                                                                                               ,
                    ValorVariableSubProceso = p.CostoVariableAdhesivoTotal,
                    CantidadEjemplaresProceso = CantidadPegado
                });
            };
            if ((ddlPegadoSticker != null) && (ddlPegadoSticker == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = 38, ValorVariableSubProceso = p.CostoVariablePegadoSticker, CantidadEjemplaresProceso = CantidadPegadoSticker }); };

            if (ModelState.IsValid)
            {
                if (CatalogoId == null)
                {

                    db.Catalogo.Add(tc);
                }
                try
                {
                    db.Presupuesto.Add(p);
                    db.Presupuesto_SubProceso.AddRange(listaSubProceso);
                    db.SaveChanges();
                    pres2.IdPresupuesto = db.Presupuesto.Max(item => item.IdPresupuesto);
                }
                catch(Exception exx)
                {

                }
            }
            else
            {
                pres2 = p;
            }
            TempData["Alerta"] = alertas.Resultado_Action("ok", "Ingresado", "");
            return Json(pres2, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Pdf(int id)
        {
            //var presupuesto = db.Presupuesto.Include(p => p.Formato).Include(p => p.Interior).Include(p => p.Tapa).Include(p => p.Interior.Papel)
            //                                        .Include(p => p.Tapa.Papel).Include(p => p.Encuadernacion).Include(p => p.Moneda).Include(p => p.Usuario).Where(x => x.IdPresupuesto == id).FirstOrDefault();

            //return View("OfertaComercial", presupuesto);
            return new ActionAsPdf("OfertaComercial", new { id = id })
            {
                PageOrientation = Rotativa.Options.Orientation.Portrait,
                PageSize = Rotativa.Options.Size.Letter,
                PageMargins = new Rotativa.Options.Margins(10, 10, 15, 10)
            };
            //return new PdfActionResult//RazorPDF.PdfResult
            //    ("OfertaComercial",presupuesto);
        }

        // POST: Presupuesto/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Create([Bind(Include = "IdPresupuesto,NombrePresupuesto,Tiraje,FormatoId,EncuadernacionId,InteriorId,TapaId,Presupuesto_ProcesoId,TotalNetoFijo,TotalNetoVari,TotalNetoTotal,PrecioUnitario")] Presupuesto presupuesto)
        {
            if (ModelState.IsValid)
            {
                db.Presupuesto.Add(presupuesto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EncuadernacionId = new SelectList(db.Encuadernacion, "IdEncuadernacion", "NombreEncuadernacion", presupuesto.EncuadernacionId);
            ViewBag.FormatoId = new SelectList(db.Formato, "IdFormato", "IdFormato", presupuesto.FormatoId);
            ViewBag.InteriorId = new SelectList(db.Interior, "IdInterior", "IdInterior", presupuesto.InteriorId);
            ViewBag.TapaId = new SelectList(db.Tapa, "IdTapa", "IdTapa", presupuesto.TapaId);
            return View(presupuesto);
        }

        // GET: Presupuesto/Edit/5
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Presupuesto presupuesto = db.Presupuesto.Find(id);
            if (presupuesto == null)
            {
                return HttpNotFound();
            }
            ViewBag.EncuadernacionId = new SelectList(db.Encuadernacion, "IdEncuadernacion", "NombreEncuadernacion", presupuesto.EncuadernacionId);
            ViewBag.FormatoId = new SelectList(db.Formato, "IdFormato", "IdFormato", presupuesto.FormatoId);
            ViewBag.InteriorId = new SelectList(db.Interior, "IdInterior", "IdInterior", presupuesto.InteriorId);
            ViewBag.TapaId = new SelectList(db.Tapa, "IdTapa", "IdTapa", presupuesto.TapaId);
            return View(presupuesto);
        }

        // POST: Presupuesto/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Edit([Bind(Include = "IdPresupuesto,NombrePresupuesto,Tiraje,FormatoId,EncuadernacionId,InteriorId,TapaId,Presupuesto_ProcesoId,TotalNetoFijo,TotalNetoVari,TotalNetoTotal,PrecioUnitario")] Presupuesto presupuesto)
        {
            if (ModelState.IsValid)
            {
                db.Entry(presupuesto).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EncuadernacionId = new SelectList(db.Encuadernacion, "IdEncuadernacion", "NombreEncuadernacion", presupuesto.EncuadernacionId);
            ViewBag.FormatoId = new SelectList(db.Formato, "IdFormato", "IdFormato", presupuesto.FormatoId);
            ViewBag.InteriorId = new SelectList(db.Interior, "IdInterior", "IdInterior", presupuesto.InteriorId);
            ViewBag.TapaId = new SelectList(db.Tapa, "IdTapa", "IdTapa", presupuesto.TapaId);
            return View(presupuesto);
        }

        // GET: Presupuesto/Delete/5
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Presupuesto presupuesto = db.Presupuesto.Where(x => x.IdPresupuesto == id).Include(p => p.Encuadernacion).Include(p => p.Formato).Include(p => p.Interior).Include(p => p.Tapa).Include(p => p.Interior.Papel).Include(p => p.Tapa.Papel).FirstOrDefault();
            if (presupuesto == null)
            {
                return HttpNotFound();
            }
            return View(presupuesto);
        }

        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult VistaEliminaPresupuesto(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Presupuesto presupuesto = db.Presupuesto.Where(x => x.IdPresupuesto == id).Include(p => p.Encuadernacion).Include(p => p.Formato).Include(p => p.Interior).Include(p => p.Tapa).Include(p => p.Interior.Papel).Include(p => p.Tapa.Papel).Include(p => p.Usuario).Include(p => p.TipoCatalogo).FirstOrDefault();
                if (presupuesto == null)
                {
                    return HttpNotFound();
                }
                //return View(presupuesto);
                ViewBag.IDPresu = id;
                return PartialView("_Eliminar", presupuesto);
            }
            catch
            {
                TempData["Alerta"] = alertas.Resultado_Action("Error", "", "Vuelva a intentarlo.");
                return View("Index");
            }
            // return View(Request.IsAjaxRequest() ? "View":"Edit", presupuesto);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Eliminar(int id)
        {
            try
            {
                var idSubPro = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == id);
                if (idSubPro != null)
                {
                    foreach (var item in idSubPro)
                    {
                        db.Presupuesto_SubProceso.Remove(item);
                    }
                }
                Presupuesto presupuesto = db.Presupuesto.Find(id);
                db.Presupuesto.Remove(presupuesto);
                db.SaveChanges();
                //db.Presupuesto.Remove(presupuesto);
                //db.SaveChanges();
                TempData["Alerta"] = alertas.Resultado_Action("ok", "Eliminado", " Presupuesto Nro " + id + " eliminado.");
                return RedirectToAction("Index");
            }
            catch(Exception ee)
            {
                TempData["Alerta"] = alertas.Resultado_Action("Error", "", "Vuelva a intentarlo.");
                return RedirectToAction("Index");
            }
        }
        // POST: Presupuesto/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult DeleteConfirmed(int id)
        {
            Presupuesto presupuesto = db.Presupuesto.Find(id);
            db.Presupuesto.Remove(presupuesto);
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

        public ActionResult ReajustePPTO(int id)
        {
            var presupuesto = db.Presupuesto.Include(p => p.Formato).Include(p => p.Interior).Include(p => p.Tapa).Include(p => p.Interior.Papel).Include(p => p.Interior.Maquina)
                                                    .Include(p => p.Tapa.Papel).Include(p => p.Tapa.Maquina).Include(p => p.Encuadernacion).Include(p => p.Moneda).Include(p => p.Usuario).Where(x => x.IdPresupuesto == id).FirstOrDefault();
            var pres = new PresupuestoForm() { NombrePresupuesto = presupuesto.NombrePresupuesto, Tiraje = presupuesto.Tiraje };
            return View(presupuesto);
        }



        public ActionResult _TablaDetallePPTO(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Presupuesto presupuesto = db.Presupuesto.Include(p => p.Formato).Include(p => p.Interior).Include(p => p.Tapa).Include(p => p.Interior.Papel).Include(p => p.Interior.Papel.Empresa)
                                                    .Include(p => p.Tapa.Papel).Include(p => p.Tapa.Papel.Empresa).Include(p => p.Encuadernacion).Include(p => p.Moneda).Include(p => p.Usuario)
                                                    .Include(p => p.TipoCatalogo).Where(x => x.IdPresupuesto == id).FirstOrDefault();
            if (presupuesto == null)
            {
                return HttpNotFound();
            }
            return View(presupuesto);
        }

        [HttpPost]
        public ActionResult AprobacionPPTO(Presupuesto pres)
        {
            try
            {
                MailMessage correo = new MailMessage();
                correo.From = new MailAddress("sistema.intranet@aimpresores.cl");
                correo.To.Add("juan.venegas@aimpresores.cl");
                correo.Subject = "Presupuesto aprobado N° " + pres.IdPresupuesto.ToString();
                correo.Body = "Estimado, <br /> se adjunta la oferta comercial y la tabla detalle del PPTO";
                correo.IsBodyHtml = true;
                correo.Priority = MailPriority.Normal;

                //Se almacena los archivos adjuntos en una carpeta, creada en el proyecto con anterioridad Temporal
                //String ruta = Server.MapPath("../Temporal");


                var pdfname = pres.IdPresupuesto + ".pdf";
                var path = Path.Combine(Server.MapPath("../Temporal"), pdfname);

                ActionAsPdf pdf = new ActionAsPdf("OfertaComercial", new { id = pres.IdPresupuesto })
                {
                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    PageSize = Rotativa.Options.Size.Letter,
                    PageMargins = new Rotativa.Options.Margins(10, 10, 15, 10),

                };
                //pdf.ExecuteResult(this.ControllerContext);
                byte[] bytes = pdf.BuildPdf(this.ControllerContext);

                Attachment adjunto = new Attachment(new MemoryStream(bytes), pdfname);
                correo.Attachments.Add(adjunto);

                var pdfname2 = pres.IdPresupuesto + "Detalle.pdf";
                var path2 = Path.Combine(Server.MapPath("../Temporal"), pdfname);

                ActionAsPdf pdf2 = new ActionAsPdf("_TablaDetallePPTO", new { id = pres.IdPresupuesto })
                {
                    PageOrientation = Rotativa.Options.Orientation.Portrait,
                    PageSize = Rotativa.Options.Size.Letter,
                    PageMargins = new Rotativa.Options.Margins(10, 10, 15, 10),

                };
                //pdf.ExecuteResult(this.ControllerContext);
                byte[] bytes2 = pdf2.BuildPdf(this.ControllerContext);

                Attachment adjunto2 = new Attachment(new MemoryStream(bytes2), pdfname2);
                correo.Attachments.Add(adjunto2);


                //Configuracion del servidor smtp
                SmtpClient smtp = new SmtpClient();
                smtp.Host = "mail.aimpresores.cl";
                //smtp.EnableSsl = true;
                //smtp.UseDefaultCredentials = true;
                smtp.Credentials = new System.Net.NetworkCredential("sistema.intranet@aimpresores.cl", "SI2013.");

                smtp.Send(correo);
                ViewBag.Mensaje = "Correo enviado correctamente";

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                TempData["Alerta"] = alertas.Resultado_Action("Error", "", "Vuelva a intentarlo.");
            }
            return RedirectToAction("Index");
        }

        public ActionResult CargaPresupuesto(int id, int Tipo)
        {
            Presupuesto presupuesto = db.Presupuesto.Include(p => p.Formato).Include(p => p.Interior).Include(p => p.Tapa).Include(p => p.Interior.Papel).Include(p => p.Interior.Papel.Empresa)
                                                    .Include(p => p.Tapa.Papel).Include(p => p.Tapa.Papel.Empresa).Include(p => p.Encuadernacion).Include(p => p.Moneda).Include(p => p.Usuario)
                                                    .Include(p => p.TipoCatalogo).Where(x => x.IdPresupuesto == id).FirstOrDefault();
            if (presupuesto == null)
            {
                TempData["Alerta"] = alertas.Resultado_Action("Error", "", "Vuelva a intentarlo.");
                return View("Index", "Presupuesto");
            }
            else
            {
                if (Tipo == 0)
                {
                    return PartialView("_AprobacionPPTO", presupuesto);
                }
                else
                    return PartialView("_Eliminar", presupuesto);
            }
        }



        //************** CAMBIOS PRESUPUESTADOR EVENTOS
        //cambios cjr
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Nuevo2()
        {
            PresupuestoForm pres = new PresupuestoForm();
            pres.Encuadernaciones = db.Encuadernacion.ToList();
            pres.Formatos = db.Formato.ToList();
            pres.Papeles = db.Papel.Where(x => x.EmpresaId == 2).OrderBy(x => x.NombrePapel).ThenBy(x => x.Gramaje).ToList();
            pres.SubProcesos = db.SubProceso.Include("Proceso").ToList();
            pres.Catalogo = db.Catalogo.ToList();
            pres.Empresa = db.Empresa.ToList();
            List<SelectListItem> s = new List<SelectListItem>();
            for (int i = 4; i <= 400; i = i + 4)
            {
                s.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.CantidadInt = s;
            ViewBag.ValorUF = string.Format("{0:#,0.00}", db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).Select(x => x.Valor).FirstOrDefault());
            return View(pres);
        }


        //[HttpPost]
        //[ValidateAntiForgeryToken] 
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public JsonResult CalcularCJR(int IDCatalogo, string NombreCatalogo, string Formato, int Tiraje, int CantPaginasInterior, string PapelInterior)
        {
            Presupuesto pres = new Presupuesto();

            pres.TipoCatalogoId = IDCatalogo;
            pres.NombreBarnizUV = "nombrefake";
            pres.Tiraje = Tiraje;
            pres.Interior.CantidadPaginas = CantPaginasInterior;
            //ProcesarCalculo(SelectFormato, SelectEnc, CantidadInt, (CantidadTapa != null) ? Convert.ToInt32(CantidadTapa) : 0, "Plana", Tiraje, ddlQuintoColor, TapaPapel, SelectPapelIntId, ddlBarnizAcuoso, ddlEmbolsado, ddlLaminado, ddlBarnizUV
            //                                 , ddlAlzadoPlano, ddlEmbolsadoManual, ddlPegadoSticker, ddlFajado, ddlPegado, ddlInsercion, ddlAlzado, ddlDesembolsado, ddlAdhesivo, 4//, CantidadCajas
            //                                 , CantidadModelos, ddlQuintoColorPasadas, CantidadAlzadoPlano, CantidadDesembolsado, CantidadAlzado, CantidadInsercion, CantidadPegado, CantidadFajado, CantidadPegadoSticker, CatalogoId
            //                                 , CantidadEnCajas, CantidadEnZuncho, CantidadEnBolsa, ddlTroquel, CantidadTerminacionEmbolsado);
            // if (CatalogoId != null)
            // {
            //     string TipoCatalogo = db.Catalogo.Where(x => x.IdTipoCatalogo == (int)CatalogoId).Select(x => x.NombreTipoCatalogo).FirstOrDefault();
            //     pres.NombrePresupuesto = TipoCatalogo + " " + NombrePresupuesto;
            // }
            // else
            // {
            //     pres.NombrePresupuesto = NombreCatalogo + " " + NombrePresupuesto;
            // }

            return Json(pres, JsonRequestBehavior.AllowGet);
        }



        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Modificar(int? id)
        {
            if (id != null && id > 0)
            {
                Presupuesto presupuesto = db.Presupuesto.Find(id);





                if (presupuesto == null)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    PresupuestoForm pres = new PresupuestoForm();
                    pres.Encuadernaciones = db.Encuadernacion.ToList();
                    pres.Formatos = db.Formato.ToList();
                    pres.Papeles = db.Papel.Where(x => x.EmpresaId == 2).OrderBy(x => x.NombrePapel).ThenBy(x => x.Gramaje).ToList();
                    pres.SubProcesos = db.SubProceso.Include("Proceso").ToList();
                    pres.Catalogo = db.Catalogo.ToList();
                    pres.Empresa = db.Empresa.ToList();
                    List<SelectListItem> s = new List<SelectListItem>();
                    for (int i = 4; i <= 200; i = i + 4)//antes 400
                    {
                        s.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
                    }
                    ViewBag.CantidadInt = s;
                    ViewBag.ValorUF = string.Format("{0:#,0.00}", db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).Select(x => x.Valor).FirstOrDefault());

                    //carga variables a modificar
                    ViewBag.Formato = db.Formato.Where(x => x.IdFormato == presupuesto.FormatoId).Select(x => x.FormatoCerradoX + " x " + x.FormatoCerradoY).First();
                    ViewBag.NombrePresu = presupuesto.NombrePresupuesto;
                    ViewBag.Tiraje = presupuesto.Tiraje;
                    ViewBag.PagsInterior = db.Interior.Where(x => x.IdInterior == presupuesto.InteriorId).Select(x => x.CantidadPaginas).First();
                    //var idInt= db.Interior.Where(x => x.IdInterior == presupuesto.InteriorId).Select(x => x.PapelId).First();
                    //db.Papel.Where(x => x.IdPapel == 1011).Select(x => x.NombrePapel + " " + x.Gramaje).First();
                    ViewBag.PapelInt = db.Interior.Where(x => x.IdInterior == presupuesto.InteriorId).Select(x => x.PapelId).First();



                    //ViewBag.PagsTap= db.Tapa.Where(x => x.IdTapa == presupuesto.InteriorId).Select(x => x.CantidadPaginas).First();
                    //var idTap = db.Tapa.Where(x => x.IdTapa == presupuesto.TapaId).Select(x => x.PapelId).First();
                    //db.Papel.Where(x => x.IdPapel == 1014).Select(x => x.NombrePapel + " " + x.Gramaje.ToString()).First();
                    ViewBag.PapelTap = db.Tapa.Where(x => x.IdTapa == presupuesto.TapaId).Select(x => x.PapelId).First();

                    return View(pres);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }
        }



        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Editar()
        {
            PresupuestoForm pres = new PresupuestoForm();
            pres.Encuadernaciones = db.Encuadernacion.ToList();
            pres.Formatos = db.Formato.ToList();
            //pres.Papeles = db.Papel.Where(x => x.EmpresaId == 2).OrderBy(x => x.NombrePapel).ThenBy(x => x.Gramaje).ToList();
            pres.SubProcesos = db.SubProceso.Include("Proceso").ToList();
            pres.Catalogo = db.Catalogo.ToList();
            pres.Empresa = db.Empresa.ToList();
            List<SelectListItem> s = new List<SelectListItem>();
            for (int i = 4; i <= 200; i = i + 4)//antes 400
            {
                s.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.CantidadInt = s;
            ViewBag.ValorUF = string.Format("{0:#,0.00}", db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).Select(x => x.Valor).FirstOrDefault());

            ViewBag.IDPresu = Request.QueryString["IdP"];
            int idP = Convert.ToInt32(Request.QueryString["IdP"]);
            Presupuesto presupuesto = db.Presupuesto.Find(idP);
            ViewBag.IdPresup = idP;
            ViewBag.Catalogo = presupuesto.TipoCatalogoId.ToString();
            ViewBag.NombrePresu = presupuesto.NombrePresupuesto;
            ViewBag.TirajePresu = presupuesto.Tiraje;
            ViewBag.Formato = db.Formato.Where(x => x.IdFormato == presupuesto.FormatoId).Select(x => x.FormatoCerradoX + " x " + x.FormatoCerradoY).First();
            //Interior
            ViewBag.PagsInterior = db.Interior.Where(x => x.IdInterior == presupuesto.InteriorId).Select(x => x.CantidadPaginas).First();
            int IdPapInt = db.Interior.Where(x => x.IdInterior == presupuesto.InteriorId).Select(x => x.PapelId).First();
            ViewBag.PapelInt = db.Interior.Where(x => x.IdInterior == presupuesto.InteriorId).Select(x => x.PapelId).First();
            ViewBag.TipoEnc = presupuesto.EncuadernacionId;

            int idEmpInt = db.Papel.Where(x => x.IdPapel == IdPapInt).Select(x => x.EmpresaId).First();
            ViewBag.PapelesInterior = db.Papel.Where(x => x.EmpresaId == idEmpInt).OrderBy(x => x.NombrePapel).ThenBy(x => x.Gramaje).ToList();
            ViewBag.ProveedorInterior = idEmpInt;
            //Validacion productos sin tapa
            if (presupuesto.TapaId != null)
            {
                ViewBag.Texto = "con tapa";
                ViewBag.PagsTapa = "4";
                int idTap = db.Tapa.Where(x => x.IdTapa == presupuesto.TapaId).Select(x => x.PapelId).First();
                int idEmpTap = db.Papel.Where(x => x.IdPapel == idTap).Select(x => x.EmpresaId).First();
                ViewBag.PapelTap = idTap;
                ViewBag.ProveedorTapa = idEmpTap;
                ViewBag.PapelesTapa = new SelectList((db.Papel.Where(x => x.EmpresaId == idEmpTap).OrderBy(x => x.NombrePapel).ThenBy(x => x.Gramaje).ToList()), "IdPapel", "NombreCompletoPapel");
            }
            else
            {
                ViewBag.Texto = "Sin tapa";
                ViewBag.PapelesTapa = new SelectList((db.Papel.Where(x => x.EmpresaId == 2).OrderBy(x => x.NombrePapel).ThenBy(x => x.Gramaje).ToList()), "IdPapel", "NombreCompletoPapel");
            }
            //Terminacion adicional tapa
            ViewBag.QuintoColor = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 39 || x.SubProcesoId == 40 || x.SubProcesoId == 41).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.Laminado = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 12 || x.SubProcesoId == 13 || x.SubProcesoId == 14).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.BarnizUV = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 15 || x.SubProcesoId == 16 || x.SubProcesoId == 17).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.EmbolsadoENC = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 7 || x.SubProcesoId == 9 || x.SubProcesoId == 17).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.EmbolsadoENCcant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 7 || x.SubProcesoId == 9 || x.SubProcesoId == 17).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.AlzadoPlano = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto && x.SubProcesoId == 21).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.AlzadoPlanoCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto && x.SubProcesoId == 21).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.DesembolsadoSimple = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto && x.SubProcesoId == 22).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.DesembolsadoSimpleCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto && x.SubProcesoId == 22).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.Alzado = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 23 || x.SubProcesoId == 24).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.AlzadoCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 23 || x.SubProcesoId == 24).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.InsercionManual = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 25 || x.SubProcesoId == 26 || x.SubProcesoId == 27).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.InsercionManualCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 25 || x.SubProcesoId == 26 || x.SubProcesoId == 27).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.PegadoSachet = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 28 || x.SubProcesoId == 29 || x.SubProcesoId == 31).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.PegadoSachetCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 28 || x.SubProcesoId == 29 || x.SubProcesoId == 31).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.Fajado = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 32 || x.SubProcesoId == 33).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.FajadoCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 32 || x.SubProcesoId == 33).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.PegadoSticker = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto && x.SubProcesoId == 38).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.PegadoStickerCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto && x.SubProcesoId == 38).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.BarnizAcuoso = presupuesto.BarnizAcuoso;
            ViewBag.CantidadEnCajas = presupuesto.CantEnCajas;
            ViewBag.CantidadEnZuncho = presupuesto.CantEnZuncho;

            return View(pres);
        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrador,SuperUser,User")]
        //public ActionResult Editar([Bind(Include = "IdPresupuesto,NombrePresupuesto,Tiraje,FormatoId,EncuadernacionId,InteriorId,TapaId,Presupuesto_ProcesoId,TotalNetoFijo,TotalNetoVari,TotalNetoTotal,PrecioUnitario")] Presupuesto presupuesto)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Presupuesto.Add(presupuesto);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.EncuadernacionId = new SelectList(db.Encuadernacion, "IdEncuadernacion", "NombreEncuadernacion", presupuesto.EncuadernacionId);
        //    ViewBag.FormatoId = new SelectList(db.Formato, "IdFormato", "IdFormato", presupuesto.FormatoId);
        //    ViewBag.InteriorId = new SelectList(db.Interior, "IdInterior", "IdInterior", presupuesto.InteriorId);
        //    ViewBag.TapaId = new SelectList(db.Tapa, "IdTapa", "IdTapa", presupuesto.TapaId);
        //    return View(presupuesto);
        //}

        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Duplicar()
        {
            PresupuestoForm pres = new PresupuestoForm();
            pres.Encuadernaciones = db.Encuadernacion.ToList();
            pres.Formatos = db.Formato.ToList();
            //pres.Papeles = db.Papel.Where(x => x.EmpresaId == 2).OrderBy(x => x.NombrePapel).ThenBy(x => x.Gramaje).ToList();
            pres.SubProcesos = db.SubProceso.Include("Proceso").ToList();
            pres.Catalogo = db.Catalogo.ToList();
            pres.Empresa = db.Empresa.ToList();
            List<SelectListItem> s = new List<SelectListItem>();
            for (int i = 4; i <= 200; i = i + 4)//antes 400
            {
                s.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.CantidadInt = s;
            ViewBag.ValorUF = string.Format("{0:#,0.00}", db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).Select(x => x.Valor).FirstOrDefault());

            ViewBag.IDPresu = Request.QueryString["IdP"];
            int idP = Convert.ToInt32(Request.QueryString["IdP"]);
            Presupuesto presupuesto = db.Presupuesto.Find(idP);
            ViewBag.Catalogo = presupuesto.TipoCatalogoId.ToString();
            ViewBag.NombrePresu = presupuesto.NombrePresupuesto;
            ViewBag.TirajePresu = presupuesto.Tiraje;
            ViewBag.Formato = db.Formato.Where(x => x.IdFormato == presupuesto.FormatoId).Select(x => x.FormatoCerradoX + " x " + x.FormatoCerradoY).First();
            //Interior
            ViewBag.PagsInterior = db.Interior.Where(x => x.IdInterior == presupuesto.InteriorId).Select(x => x.CantidadPaginas).First();
            int IdPapInt = db.Interior.Where(x => x.IdInterior == presupuesto.InteriorId).Select(x => x.PapelId).First();
            ViewBag.PapelInt = db.Interior.Where(x => x.IdInterior == presupuesto.InteriorId).Select(x => x.PapelId).First();
            ViewBag.TipoEnc = presupuesto.EncuadernacionId;

            int idEmpInt = db.Papel.Where(x => x.IdPapel == IdPapInt).Select(x => x.EmpresaId).First();
            ViewBag.PapelesInterior = db.Papel.Where(x => x.EmpresaId == idEmpInt).OrderBy(x => x.NombrePapel).ThenBy(x => x.Gramaje).ToList();
            ViewBag.ProveedorInterior = idEmpInt;
            //Validacion productos sin tapa
            if (presupuesto.TapaId != null)
            {
                ViewBag.Texto = "con tapa";
                ViewBag.PagsTapa = "4";
                int idTap = db.Tapa.Where(x => x.IdTapa == presupuesto.TapaId).Select(x => x.PapelId).First();
                int idEmpTap = db.Papel.Where(x => x.IdPapel == idTap).Select(x => x.EmpresaId).First();
                ViewBag.PapelTap = idTap;
                ViewBag.ProveedorTapa = idEmpTap;
                ViewBag.PapelesTapa = new SelectList((db.Papel.Where(x => x.EmpresaId == idEmpTap).OrderBy(x => x.NombrePapel).ThenBy(x => x.Gramaje).ToList()), "IdPapel", "NombreCompletoPapel");
            }
            else
            {
                ViewBag.Texto = "Sin tapa";
                ViewBag.PapelesTapa = new SelectList((db.Papel.Where(x => x.EmpresaId == 2).OrderBy(x => x.NombrePapel).ThenBy(x => x.Gramaje).ToList()), "IdPapel", "NombreCompletoPapel");
            }
            //Terminacion adicional tapa
            ViewBag.QuintoColor = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 39 || x.SubProcesoId == 40 || x.SubProcesoId == 41).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.Laminado = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 12 || x.SubProcesoId == 13 || x.SubProcesoId == 14).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.BarnizUV = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 15 || x.SubProcesoId == 16 || x.SubProcesoId == 17).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.EmbolsadoENC = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 7 || x.SubProcesoId == 9 || x.SubProcesoId == 17).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.EmbolsadoENCcant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 7 || x.SubProcesoId == 9 || x.SubProcesoId == 17).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.AlzadoPlano = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto && x.SubProcesoId == 21).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.AlzadoPlanoCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto && x.SubProcesoId == 21).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.DesembolsadoSimple = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto && x.SubProcesoId == 22).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.DesembolsadoSimpleCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto && x.SubProcesoId == 22).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.Alzado = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 23 || x.SubProcesoId == 24).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.AlzadoCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 23 || x.SubProcesoId == 24).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.InsercionManual = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 25 || x.SubProcesoId == 26 || x.SubProcesoId == 27).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.InsercionManualCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 25 || x.SubProcesoId == 26 || x.SubProcesoId == 27).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.PegadoSachet = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 28 || x.SubProcesoId == 29 || x.SubProcesoId == 31).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.PegadoSachetCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 28 || x.SubProcesoId == 29 || x.SubProcesoId == 31).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.Fajado = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 32 || x.SubProcesoId == 33).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.FajadoCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto).Where(x => x.SubProcesoId == 32 || x.SubProcesoId == 33).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.PegadoSticker = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto && x.SubProcesoId == 38).Select(x => x.SubProcesoId).FirstOrDefault();
            ViewBag.PegadoStickerCant = db.Presupuesto_SubProceso.Where(x => x.PresupuestoId == presupuesto.IdPresupuesto && x.SubProcesoId == 38).Select(x => x.CantidadEjemplaresProceso).FirstOrDefault();

            ViewBag.BarnizAcuoso = presupuesto.BarnizAcuoso;
            ViewBag.CantidadEnCajas = presupuesto.CantEnCajas;
            ViewBag.CantidadEnZuncho = presupuesto.CantEnZuncho;

            return View(pres);
        }






        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public JsonResult CalcularMod(string NombrePresupuesto, int Tiraje, string SelectFormato, int? SelectEnc, int CantidadInt, int? SelectPapelIntId, int? CantidadTapa,
                                int? TapaPapel, int? ddlQuintoColor, int? ddlBarnizAcuoso, int? ddlEmbolsado, int? ddlLaminado, int? ddlBarnizUV, int? ddlAlzadoPlano, int? ddlEmbolsadoManual,
                                int? ddlPegadoSticker, int? ddlFajado, int? ddlPegado, int? ddlInsercion, int? ddlAlzado, int? ddlDesembolsado, int? ddlAdhesivo, //int? ddlAdhesivoCms,int CantidadCajas,
                                int CantidadModelos, int ddlQuintoColorPasadas, int? CatalogoId, string NombreCatalogo, int CantidadAlzadoPlano, int CantidadDesembolsado, int CantidadAlzado, int CantidadInsercion,
                                int CantidadPegado, int CantidadFajado, int CantidadPegadoSticker, int CantidadEnCajas, int CantidadEnZuncho, int CantidadEnBolsa, string ddlTroquel, int CantidadTerminacionEmbolsado)
        {
            Presupuesto pres = ProcesarCalculoMod(SelectFormato, SelectEnc, CantidadInt, (CantidadTapa != null) ? Convert.ToInt32(CantidadTapa) : 0, "Plana", Tiraje, ddlQuintoColor, TapaPapel, SelectPapelIntId, ddlBarnizAcuoso, ddlEmbolsado, ddlLaminado, ddlBarnizUV
                                            , ddlAlzadoPlano, ddlEmbolsadoManual, ddlPegadoSticker, ddlFajado, ddlPegado, ddlInsercion, ddlAlzado, ddlDesembolsado, ddlAdhesivo, 4//, CantidadCajas
                                            , CantidadModelos, ddlQuintoColorPasadas, CantidadAlzadoPlano, CantidadDesembolsado, CantidadAlzado, CantidadInsercion, CantidadPegado, CantidadFajado, CantidadPegadoSticker, CatalogoId
                                            , CantidadEnCajas, CantidadEnZuncho, CantidadEnBolsa, ddlTroquel, CantidadTerminacionEmbolsado);
            if (CatalogoId != null)
            {
                string TipoCatalogo = db.Catalogo.Where(x => x.IdTipoCatalogo == (int)CatalogoId).Select(x => x.NombreTipoCatalogo).FirstOrDefault();
                pres.NombrePresupuesto = TipoCatalogo + " " + NombrePresupuesto;
            }
            else
            {
                pres.NombrePresupuesto = NombreCatalogo + " " + NombrePresupuesto;
            }

            return Json(pres, JsonRequestBehavior.AllowGet);
        }

        public Presupuesto ProcesarCalculoMod(string FormatoId, int? EncuadernacionId, int CantidadPaginasInt, int CantidadPaginasTap, string MaquinaTap, int Tiraje, int? IDQuintoColor
            , int? idPapelTap, int? idPapelInterior, int? BarnizAcuoso, int? Embolsado, int? Laminado, int? UV, int? AlzadoPlano, int? EmbolsadoManual, int? Sticker, int? Fajado, int? Pegado, int? Insercion, int? Alzado, int? Desembolsado,
            int? Adhesivo, int? cmsAdhesivo, int CantidadModelos, int CantidadPasadaQuintoColor, int CantidadAlzadoPlano, int CantidadDesembolsado, int CantidadAlzado, int CantidadInsercion,
                                        int CantidadPegado, int CantidadFajado, int CantidadPegadoSticker, int? CatalogoId, int CantidadEnCajas, int CantidadEnZuncho, int CantidadEnBolsa, string ddlTroquel, int CantidadTerminacionEmbolsado)
        {

            Presupuesto detalle = new Presupuesto();
            detalle.MonedaId = db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).Select(x => x.IdMoneda).FirstOrDefault();
            detalle.Tiraje = Tiraje;
            #region nreEntradasxFormato
            //la produccion es la tabla de pliegos totales impreso por tipo doblez.
            Produccion produccion = db.Produccion.Where(x => x.Paginas == CantidadPaginasInt).FirstOrDefault();

            int NumeroDoblez = 4;
            if (db.Papel.Where(x => x.IdPapel == (int)idPapelInterior).Select(x => x.NombrePapel).Contains("Bond"))
            {
                if ((CantidadPaginasInt / 32) > 0)
                {
                    produccion.Entradas16 = 0;
                    produccion.Entradas48 = 0;
                    produccion.Entradas64 = 0;
                    detalle.EntradasPag32 = (CantidadPaginasInt / 32);
                    produccion.Impresion32 = detalle.EntradasPag32;
                    NumeroDoblez = 32;
                    switch (CantidadPaginasInt - (detalle.EntradasPag32 * 32))
                    {
                        case 28:
                            detalle.EntradasPag16 = 1; produccion.Entradas16 = 1;
                            produccion.Impresion08 = 1; detalle.EntradasPag8 = 1;
                            detalle.EntradasPag4 = 1; produccion.Impresion04 = 1;
                            break;
                        case 24:
                            detalle.EntradasPag16 = 1; produccion.Entradas16 = 1;
                            detalle.EntradasPag8 = 1; produccion.Impresion08 = 1;
                            break;
                        case 20:
                            detalle.EntradasPag16 = 1; produccion.Entradas16 = 1;
                            detalle.EntradasPag4 = 1; produccion.Impresion04 = 1;
                            break;
                        case 16:
                            detalle.EntradasPag16 = 1; produccion.Entradas16 = 1;
                            break;
                        case 12:
                            detalle.EntradasPag8 = 1; produccion.Impresion08 = 1;
                            detalle.EntradasPag4 = 1; produccion.Impresion04 = 1;
                            break;
                        case 8:
                            detalle.EntradasPag8 = 1; produccion.Impresion08 = 1;
                            break;
                        case 4:
                            detalle.EntradasPag4 = 1; produccion.Impresion04 = 1;
                            break;
                    }
                    produccion.Entradas64 = (detalle.EntradasPag32);
                    produccion.Entradas16 = (detalle.EntradasPag16 + detalle.EntradasPag8 + detalle.EntradasPag4);
                }
                else if ((CantidadPaginasInt / 16) > 0)
                {
                    detalle.EntradasPag16 = (CantidadPaginasInt / 16);
                    produccion.Impresion16 = detalle.EntradasPag16;
                    NumeroDoblez = 16;
                    switch (CantidadPaginasInt - (detalle.EntradasPag16 * 16))
                    {
                        case 12:
                            detalle.EntradasPag8 = 1;
                            produccion.Impresion08 = 1;
                            detalle.EntradasPag4 = 1;
                            produccion.Impresion04 = 1;
                            break;
                        case 8:
                            detalle.EntradasPag8 = 1;
                            produccion.Impresion08 = 1;
                            break;
                        case 4:
                            detalle.EntradasPag4 = 1;
                            produccion.Impresion04 = 1;
                            break;
                    }
                    produccion.Entradas16 = (detalle.EntradasPag16 + detalle.EntradasPag8 + detalle.EntradasPag4);
                }
                else if ((CantidadPaginasInt / 8) > 0)
                {
                    detalle.EntradasPag8 = (CantidadPaginasInt / 8);
                    produccion.Impresion08 = detalle.EntradasPag8;
                    NumeroDoblez = 8;
                    switch (CantidadPaginasInt - (detalle.EntradasPag8 * 8))
                    {
                        case 4:
                            detalle.EntradasPag4 = 1;
                            produccion.Impresion04 = 1;
                            break;
                    }
                    produccion.Entradas16 = (detalle.EntradasPag8 + detalle.EntradasPag4);
                }
                produccion.Web88cms = Convert.ToDouble(CantidadPaginasInt) / Convert.ToDouble(NumeroDoblez);
            }
            else if (FormatoId != "230 x 300")
            {
                detalle.EntradasPag64 = produccion.Impresion64;
                detalle.EntradasPag48 = produccion.Impresion48;
                detalle.EntradasPag32 = produccion.Impresion32;
                detalle.EntradasPag24 = produccion.Impresion24;
                detalle.EntradasPag16 = produccion.Impresion16;
                detalle.EntradasPag8 = produccion.Impresion08;
                detalle.EntradasPag4 = produccion.Impresion04;
                if (produccion.Entradas64 > 0)
                {
                    NumeroDoblez = 64;
                }
                else if (produccion.Entradas48 > 0)
                {
                    NumeroDoblez = 48;
                }
                else
                {
                    NumeroDoblez = 16;
                }
            }

            #endregion
            #region EntradasxFormato
            else
            {
                produccion = db.Produccion.Where(x => x.Paginas == 16).FirstOrDefault();
                produccion.Entradas64 = 0;
                produccion.Entradas48 = 0;

                if ((CantidadPaginasInt / 16) > 0)
                {
                    detalle.EntradasPag16 = (CantidadPaginasInt / 16);
                    produccion.Impresion16 = detalle.EntradasPag16;
                    NumeroDoblez = 16;
                    switch (CantidadPaginasInt - (detalle.EntradasPag16 * 16))
                    {
                        case 12:
                            detalle.EntradasPag8 = 1;
                            produccion.Impresion08 = 1;
                            detalle.EntradasPag4 = 1;
                            produccion.Impresion04 = 1;
                            break;
                        case 8:
                            detalle.EntradasPag8 = 1;
                            produccion.Impresion08 = 1;
                            break;
                        case 4:
                            detalle.EntradasPag4 = 1;
                            produccion.Impresion04 = 1;
                            break;
                    }
                    produccion.Entradas16 = (detalle.EntradasPag16 + detalle.EntradasPag8 + detalle.EntradasPag4);
                }
                else if ((CantidadPaginasInt / 8) > 0)
                {
                    detalle.EntradasPag8 = (CantidadPaginasInt / 8);
                    produccion.Impresion08 = detalle.EntradasPag8;
                    NumeroDoblez = 8;
                    switch (CantidadPaginasInt - (detalle.EntradasPag8 * 8))
                    {
                        case 4:
                            detalle.EntradasPag4 = 1;
                            produccion.Impresion04 = 1;
                            break;
                    }
                    produccion.Entradas16 = (detalle.EntradasPag8 + detalle.EntradasPag4);
                }
                produccion.Web88cms = Convert.ToDouble(CantidadPaginasInt) / Convert.ToDouble(NumeroDoblez);
            }
            #endregion
            #region Impresion Interior Nuevo


            string[] split = FormatoId.Split('x');
            double formatox = Convert.ToDouble(split[0]);
            double formatoy = Convert.ToDouble(split[1]);
            List<Formato> listaFormatos = db.Formato.Where(x => x.FormatoCerradoX == formatox && x.FormatoCerradoY == formatoy).ToList();
            //detalle.Formato = db.Formato.Where(x => x.FormatoCerradoX == formatox && x.FormatoCerradoY == formatoy).FirstOrDefault();
            if ((db.Papel.Where(x => x.IdPapel == (int)idPapelInterior).Select(x => x.NombrePapel).Contains("Bond")))
            {
                listaFormatos = listaFormatos.Where(x => x.EntradasxFormatos != 48).ToList();
                foreach (Formato f in listaFormatos.Where(x => x.EntradasxFormatos != 16).ToList())
                {
                    f.EntradasxFormatos = 32;
                    f.Interior_Ancho = 88;
                }
            }
            Papel papelInterior = db.Papel.Where(x => x.IdPapel == (int)idPapelInterior).FirstOrDefault();
            Papel papelTapa = ((CantidadPaginasTap > 0 && idPapelTap != null)) ? db.Papel.Where(x => x.IdPapel == (int)idPapelTap).FirstOrDefault() : null;
            detalle.MaquinaInterior = (papelInterior.Gramaje > 130) ? db.Maquina.Where(x => x.NombreMaquina == "Plana").FirstOrDefault() : db.Maquina.Where(x => x.NombreMaquina == "Rotativa").FirstOrDefault();
            if (papelTapa != null)
            {
                detalle.MaquinaTapa = ((papelTapa.Gramaje > 130) ? db.Maquina.Where(x => x.NombreMaquina == "Plana").FirstOrDefault() : db.Maquina.Where(x => x.NombreMaquina == "Rotativa").FirstOrDefault());
            }
            List<Impresion> lista = db.Impresion.Include(x => x.TipoMoneda).Include(x => x.TipoMoneda.Monedas).Include(x => x.Maquina).Where(x => x.Maquina.NombreMaquina == detalle.MaquinaInterior.NombreMaquina).ToList();

            switch (NumeroDoblez)
            {
                case 64:
                    detalle.CostoFijoPag64 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "64").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag48 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "48").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag32 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "32").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag24 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag16 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag12 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag8 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag4 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoVariablePag64 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "64").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag48 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "48").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag32 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "32").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag24 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag16 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag12 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag8 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag4 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);

                    break;
                case 48:
                    detalle.CostoFijoPag48 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "48").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag32 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "32").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag24 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag16 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag12 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag8 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag4 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoVariablePag48 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "48").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag32 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "32").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag24 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag16 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag12 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag8 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag4 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);

                    break;
                case 32:
                    detalle.CostoFijoPag32 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "32").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag24 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag16 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag12 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag8 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag4 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoVariablePag32 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "32").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag24 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag16 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag12 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag8 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag4 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);

                    break;
                case 24:
                    detalle.CostoFijoPag24 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag16 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag12 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag8 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag4 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoVariablePag24 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "24").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag16 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag12 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag8 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag4 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);

                    break;
                case 16:
                    detalle.CostoFijoPag16 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag12 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag8 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag4 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoVariablePag16 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "16").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag12 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "12").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag8 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag4 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);

                    break;
                case 8:
                    detalle.CostoFijoPag8 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoFijoPag4 = Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion).FirstOrDefault());
                    detalle.CostoVariablePag8 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "8").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);
                    detalle.CostoVariablePag4 = (Math.Ceiling(((Math.Ceiling(lista.Where(x => x.NombreImpresion == "4").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault())) / 1000.0) * 100.0) / 100.0);

                    break;
                default: break;
            }

            #endregion
            #region Tapas Nuevo
            List<Impresion> ListImpTapa = db.Impresion.Include(x => x.Maquina).Include(x => x.TipoMoneda).Include(x => x.TipoMoneda.Monedas).Where(x => x.Maquina.NombreMaquina == MaquinaTap && x.NombreImpresion == "16").ToList();
            detalle.CostoFijoTapa = (CantidadPaginasTap > 0) ? Math.Ceiling(ListImpTapa.Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion * CantidadModelos).FirstOrDefault()) : 0;
            detalle.CostoVariableTapa = (CantidadPaginasTap > 0) ? (Math.Ceiling((((ListImpTapa.Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault()) / 1000.0) / CantidadPaginasTap) * 100.0) / 100.0) : 0;

            detalle.CantidadModelos = CantidadModelos;

            Formato TapaFormato = listaFormatos.Where(x => x.EntradasxFormatos == listaFormatos.Max(y => y.EntradasxFormatos)).FirstOrDefault();
            List<SubProceso> ListTerm = db.SubProceso.Include(x => x.TipoMoneda).Include(x => x.TipoMoneda.Monedas).ToList();
            detalle.CostoFijoQuintoColor = (IDQuintoColor != null) ? Math.Ceiling(ListTerm.Where(x => x.IdSubProceso == IDQuintoColor).Select(x => (x.CostoFijoSubProceso * CantidadPasadaQuintoColor * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault())).FirstOrDefault()) : 0;
            detalle.CostoVariableQuintoColor = (IDQuintoColor != null) ? (Math.Ceiling((((ListTerm.Where(x => x.IdSubProceso == IDQuintoColor).Select(x => x.CostoVariableSubProceso * CantidadPasadaQuintoColor * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) / CantidadPaginasTap) * 100.0) / 100.0) : 0;
            detalle.NombreQuintoColor = (IDQuintoColor != null) ? ListTerm.Where(x => x.IdSubProceso == IDQuintoColor).Select(x => x.NombreSubProceso).FirstOrDefault() : "";
            detalle.CostoFijoPlizado = (db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => x.Gramaje).FirstOrDefault() >= 170) ? ListTerm.Where(x => x.NombreSubProceso == "Plizado").Select(x => x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault() : 0;
            detalle.CostoVariablePlizado = (db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => x.Gramaje).FirstOrDefault() >= 170) ? (ListTerm.Where(x => x.NombreSubProceso == "Plizado").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault() / Convert.ToDouble(CantidadPaginasTap)) : 0;

            #endregion
            #region Encuadernacion Nuevo
            detalle.CostoFijoEncuadernacion = (EncuadernacionId != null) ? Math.Ceiling(db.Encuadernacion.Where(x => x.IdEncuadernacion == EncuadernacionId).Select(x => x.ValorFijo * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(y => y.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
            detalle.CostoVariableEncuadernacion = (EncuadernacionId != null) ? (Math.Ceiling(((db.Encuadernacion.Where(x => x.IdEncuadernacion == EncuadernacionId).Select(x => x.ValorVariable * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(y => y.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) * 100.0) / 100.0) : 0;
            detalle.NombreEncuadernacion = (EncuadernacionId != null) ? db.Encuadernacion.Where(x => x.IdEncuadernacion == EncuadernacionId).Select(x => x.NombreEncuadernacion).FirstOrDefault() : "";
            #endregion
            #region Terminaciones Nuevas
            double dobleentrada = (BarnizAcuoso == 4) && (CantidadPaginasTap > 0) ? 2 : 1;
            detalle.CostoFijoBarnizAcuosoTapa = ((CantidadPaginasTap > 0) && (BarnizAcuoso >= 2)) ? Math.Ceiling(ListTerm.Where(x => x.NombreSubProceso == "Barniz Acuoso (solo tiro)").Select(x => x.CostoFijoSubProceso * dobleentrada * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
            detalle.CostoVariableBarnizAcuosoTapa = ((CantidadPaginasTap > 0) && (BarnizAcuoso >= 2)) ? Math.Ceiling((((ListTerm.Where(x => x.NombreSubProceso == "Barniz Acuoso (solo tiro)").Select(x => x.CostoVariableSubProceso * dobleentrada * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) / CantidadPaginasTap) * 100.0) / 100.0 : 0;
            detalle.CostoVariableEmbolsado = (Embolsado != null) ? (ListTerm.Where(x => x.IdSubProceso == Embolsado).Select(x => ((x.CostoVariableSubProceso * CantidadTerminacionEmbolsado) / Convert.ToDouble(detalle.Tiraje))).FirstOrDefault()) : 0;
            detalle.NombreEmbolsado = (Embolsado != null) ? ListTerm.Where(x => x.IdSubProceso == Embolsado).Select(x => x.NombreSubProceso).FirstOrDefault() : "";
            detalle.CantidadTerminacionEmbolsado = (Embolsado != null) ? CantidadTerminacionEmbolsado : 0;
            detalle.CostoVariableLaminado = ((Laminado != null) && (CantidadPaginasTap > 0)) ? (Math.Ceiling(((ListTerm.Where(x => x.IdSubProceso == Laminado).Select(x => (x.CostoVariableSubProceso * (TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0)).FirstOrDefault()) / Convert.ToDouble(CantidadPaginasTap)) * 100.0) / 100.0) : 0;
            detalle.NombreLaminado = ((Laminado != null) && (CantidadPaginasTap > 0)) ? ListTerm.Where(x => x.IdSubProceso == Laminado).Select(x => x.NombreSubProceso).FirstOrDefault() : "";
            detalle.CostoFijoBarnizUV = (UV != null) ? Math.Ceiling(ListTerm.Where(x => x.IdSubProceso == UV).Select(x => x.CostoFijoSubProceso).FirstOrDefault()) : 0;
            if (UV != null)
            {
                SubProceso subpro = db.SubProceso.Where(x => x.IdSubProceso == UV).FirstOrDefault();
                if (subpro.NombreSubProceso == "Barniz UV 100% en el tiro")
                {
                    detalle.CostoVariableBarnizUV = (Math.Ceiling((ListTerm.Where(x => x.IdSubProceso == UV).Select(x => ((x.CostoVariableSubProceso * ((TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0))) / Convert.ToDouble(CantidadPaginasTap)).FirstOrDefault()) * 100.0) / 100.0);
                }
                else if (subpro.NombreSubProceso == "Barniz UV con Reserva")
                {
                    detalle.CostoVariableBarnizUV = (Math.Ceiling((ListTerm.Where(x => x.IdSubProceso == UV).Select(x => (x.CostoVariableSubProceso * (Tiraje * Convert.ToDouble(1.0 / CantidadPaginasTap)) / Tiraje)).FirstOrDefault()) * 100.0) / 100.0);
                }
                else if (subpro.NombreSubProceso == "Barniz UV con Glitter")
                {
                    detalle.CostoVariableBarnizUV = (Math.Ceiling((ListTerm.Where(x => x.IdSubProceso == UV).Select(x => (x.CostoVariableSubProceso) / CantidadPaginasTap).FirstOrDefault()) * 100.0) / 100.0);
                }
                detalle.NombreBarnizUV = ListTerm.Where(x => x.IdSubProceso == UV).Select(x => x.NombreSubProceso).FirstOrDefault();
            }
            detalle.CostoFijoTroquel = (ddlTroquel != "No" && ddlTroquel != "" && ddlTroquel != null) ? (ListTerm.Where(x => x.NombreSubProceso == "Troquel").Select(x => (x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault())).FirstOrDefault()) : 0;
            detalle.CostoVariableTroquel = (ddlTroquel != "No" && ddlTroquel != "" && ddlTroquel != null) ? ((ListTerm.Where(x => x.NombreSubProceso == "Troquel").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / Convert.ToDouble(CantidadPaginasTap)) : 0;
            detalle.CostoFijoCorteFrontal = (CantidadPaginasTap > 0) ? (ListTerm.Where(x => x.NombreSubProceso == "Corte Frontal").Select(x => x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
            detalle.CostoVariableCorteFrontal = (CantidadPaginasTap > 0) ? (ListTerm.Where(x => x.NombreSubProceso == "Corte Frontal").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;

            #endregion
            #region Manualidades Pendiente
            detalle.CostoVariableAlzadoPlano = (AlzadoPlano != null && AlzadoPlano == 2) ? (ListTerm.Where(x => x.NombreSubProceso == "Alzado Plano").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariableEmbolsadoManual = (EmbolsadoManual != null && EmbolsadoManual == 2) ? (ListTerm.Where(x => x.NombreSubProceso == "Embolsado manual").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;

            detalle.CostoVariableDesembolsado = (Desembolsado != null && Desembolsado == 2) ? (ListTerm.Where(x => x.NombreSubProceso == "Desembolsado simple").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariableAlzado = (Alzado != null) ? (ListTerm.Where(x => x.IdSubProceso == Alzado).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariableInsercion = (Insercion != null) ? (ListTerm.Where(x => x.IdSubProceso == Insercion).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariablePegado = (Pegado != null) ? (ListTerm.Where(x => x.IdSubProceso == Pegado).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariableFajado = (Fajado != null) ? (ListTerm.Where(x => x.IdSubProceso == Fajado).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariableAdhesivo = (Pegado != null) ? (ListTerm.Where(x => x.NombreSubProceso == "Stopgard (9 mm.)"//Adhesivo
                                                                                                        ).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.AdhesivoCms = Convert.ToDouble(cmsAdhesivo);
            detalle.CostoVariableAdhesivoTotal = Math.Ceiling((detalle.AdhesivoCms * detalle.CostoVariableAdhesivo) * 10.0) / 10.0;
            #region Embalaje
            Embalaje emb = db.Embalaje.Where(x => x.Estado == true).FirstOrDefault();
            Papel p = db.Papel.Where(x => x.IdPapel == idPapelInterior).FirstOrDefault();
            double LomoInterior = Math.Ceiling(((((p.Micron * Convert.ToDouble(CantidadPaginasInt)) / 2.0) / 1000.0) + p.Adhesivo) * 10.0) / 10.0;
            double LomoTapa = db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => ((x.Micron * 2.0) / 1000.0)).FirstOrDefault();
            double LomocTapa = (Math.Ceiling((LomoInterior + (LomoTapa)) * 10)) / 10;

            if (CatalogoId != null)
            {
                try
                {//Productos ingresados con formato de cajas
                    string[] dimension = db.Catalogo.Where(x => x.IdTipoCatalogo == CatalogoId).Select(x => x.DimensionesCajasStandar).FirstOrDefault().Split('x');
                    detalle.LibrosxCajas = Math.Floor(Convert.ToInt32(dimension[2]) / LomocTapa) * emb.Base;
                }
                catch (Exception ex)
                {
                    //Todos los productos nuevos con formato standard - cjerias
                    string[] dimension = ("320x220x283").Split('x');
                    //string formmmat = db.Catalogo.Where(x => x.IdTipoCatalogo == CatalogoId).Select(x => x.FormatoSeleccionado).SingleOrDefault();
                    //string[] dimension = db.Catalogo.Where(x => x.FormatoSeleccionado == formmmat).Select(x => x.DimensionesCajasStandar).SingleOrDefault().Split('x');
                    detalle.LibrosxCajas = Math.Floor(Convert.ToInt32(dimension[2]) / LomocTapa) * emb.Base;
                }
            }
            else
            {
                detalle.LibrosxCajas = Math.Floor(emb.AltoCaja / LomocTapa) * emb.Base;
            }
            detalle.CantidadCajas = (int)Math.Ceiling(CantidadEnCajas / detalle.LibrosxCajas);
            detalle.CostoVariableSuministroCaja = (Math.Ceiling((
                ((emb.CajaEstandar * detalle.CantidadCajas) / Convert.ToDouble(Tiraje)) * 100)) / 100);
            detalle.CostoVariableInsercionCajaySellado = (Math.Ceiling((
                ((emb.EncajadoxCaja * detalle.CantidadCajas) / Convert.ToDouble(Tiraje)) * 100)) / 100);
            detalle.Enzunchadoxpqte = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(CantidadEnZuncho) / detalle.LibrosxCajas));
            detalle.CostoVariableEnzunchado = (Math.Ceiling(((emb.Enzunchado * detalle.Enzunchadoxpqte))) / Convert.ToDouble(Tiraje) * 100) / 100;
            detalle.CantidadenBolsa = CantidadEnBolsa;


            #endregion
            detalle.CostoVariablePegadoSticker = (Sticker != null && Sticker == 2) ? (ListTerm.Where(x => x.NombreSubProceso == "Pegado de Sticker").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            #endregion
            #region Papel
            if ((CantidadPaginasInt > 0) && (idPapelInterior != null))
            {
                detalle.Interior = new Interior();
                detalle.Interior.CantidadPaginas = CantidadPaginasInt;
                detalle.Interior.PapelId = (int)idPapelInterior;

                Formato Web88 = listaFormatos.Where(x => x.EntradasxFormatos == 16).FirstOrDefault();
                Formato Litho132 = listaFormatos.Where(x => x.EntradasxFormatos == 48).FirstOrDefault();
                Formato Litho174 = listaFormatos.Where(x => x.EntradasxFormatos == 64).FirstOrDefault();

                if (Litho174 != null)
                {
                    detalle.FormatoId = listaFormatos.Where(x => x.EntradasxFormatos == 64).Select(x => x.IdFormato).FirstOrDefault();
                }
                else if (Litho132 != null)
                {
                    detalle.FormatoId = listaFormatos.Where(x => x.EntradasxFormatos == 48).Select(x => x.IdFormato).FirstOrDefault();
                }
                else
                {
                    detalle.FormatoId = listaFormatos.Where(x => x.EntradasxFormatos == 16).Select(x => x.IdFormato).FirstOrDefault();
                }

                detalle.Interior.Entradas = (float)(((Web88 != null) ? (((produccion.Entradas16 * detalle.MaquinaInterior.MermaFija) / 1000.0) * ((papelInterior.Gramaje * Web88.Interior_Alto * Web88.Interior_Ancho) / 10000.0)) : 0) +
                                                   ((Litho132 != null) ? (((produccion.Entradas48 * detalle.MaquinaInterior.MermaFija) / 1000.0) * ((papelInterior.Gramaje * Litho132.Interior_Alto * Litho132.Interior_Ancho) / 10000.0)) : 0) +
                                                   ((Litho174 != null) ? (((produccion.Entradas64 * detalle.MaquinaInterior.MermaFija) / 1000.0) * ((papelInterior.Gramaje * Litho174.Interior_Alto * Litho174.Interior_Ancho) / 10000.0)) : 0));

                //detalle.Interior.Entradas = (float)((((detalle.EntradasPag64 + detalle.EntradasPag48 + detalle.EntradasPag32 + detalle.EntradasPag24 + detalle.EntradasPag16 + detalle.EntradasPag12 + detalle.EntradasPag8 + detalle.EntradasPag4) * detalle.MaquinaInterior.MermaFija)/1000.0) 
                //                                        * (((papelInterior.Gramaje * detalle.Formato.Interior_Alto * detalle.Formato.Interior_Ancho) / 10000.0)));

                detalle.Interior.CostoPapelinteriorFijo = Math.Ceiling(detalle.Interior.Entradas * papelInterior.PrecioKilos);

                detalle.Interior.Tiradas = (float)(((Web88 != null) ? (((Tiraje * produccion.Web88cms * detalle.MaquinaInterior.MermaVariable) / 1000.0) * ((papelInterior.Gramaje * Web88.Interior_Alto * Web88.Interior_Ancho) / 10000.0)) : 0) +
                                           ((Litho132 != null) ? (((Tiraje * produccion.Litho132cms * detalle.MaquinaInterior.MermaVariable) / 1000.0) * ((papelInterior.Gramaje * Litho132.Interior_Alto * Litho132.Interior_Ancho) / 10000.0)) : 0) +
                                           ((Litho174 != null) ? (((Tiraje * produccion.Litho174cms * detalle.MaquinaInterior.MermaVariable) / 1000.0) * ((papelInterior.Gramaje * Litho174.Interior_Alto * Litho174.Interior_Ancho) / 10000.0)) : 0));
                //detalle.Interior.Tiradas = (float)(((Tiraje * (Convert.ToDouble(CantidadPaginasInt) / Convert.ToDouble(NumeroDoblez)) * detalle.MaquinaInterior.MermaVariable) / 1000.0) * ((papelInterior.Gramaje * detalle.Formato.Interior_Alto * detalle.Formato.Interior_Ancho) / 10000.0));

                detalle.Interior.CostoPapelInteriorVari = (Math.Ceiling(((papelInterior.PrecioKilos * detalle.Interior.Tiradas) / Convert.ToDouble(Tiraje)) * 100.0) / 100.0);

                float a = (float)(((Web88 != null) ? (((Tiraje * produccion.Web88cms * detalle.MaquinaInterior.MermaVariable) + (produccion.Entradas16 * detalle.MaquinaInterior.MermaFija)) * ((papelInterior.Gramaje * Web88.Interior_Alto * Web88.Interior_Ancho) / 10000000.0)) : 0));
                float b = (float)((Litho132 != null) ? (((Tiraje * produccion.Litho132cms * detalle.MaquinaInterior.MermaVariable) + (produccion.Entradas48 * detalle.MaquinaInterior.MermaFija)) * ((papelInterior.Gramaje * Litho132.Interior_Alto * Litho132.Interior_Ancho) / 10000000.0)) : 0);
                float c = (float)((Litho174 != null) ? (((Tiraje * produccion.Litho174cms * detalle.MaquinaInterior.MermaVariable) + (produccion.Entradas64 * detalle.MaquinaInterior.MermaFija)) * ((papelInterior.Gramaje * Litho174.Interior_Alto * Litho174.Interior_Ancho) / 10000000.0)) : 0);


                detalle.Interior.KilosPapel = (float)(((Web88 != null) ? (((Tiraje * produccion.Web88cms * detalle.MaquinaInterior.MermaVariable) + (produccion.Entradas16 * detalle.MaquinaInterior.MermaFija)) * ((papelInterior.Gramaje * Web88.Interior_Alto * Web88.Interior_Ancho) / 10000000.0)) : 0) +
                                                     ((Litho132 != null) ? (((Tiraje * produccion.Litho132cms * detalle.MaquinaInterior.MermaVariable) + (produccion.Entradas48 * detalle.MaquinaInterior.MermaFija)) * ((papelInterior.Gramaje * Litho132.Interior_Alto * Litho132.Interior_Ancho) / 10000000.0)) : 0) +
                                                     ((Litho174 != null) ? (((Tiraje * produccion.Litho174cms * detalle.MaquinaInterior.MermaVariable) + (produccion.Entradas64 * detalle.MaquinaInterior.MermaFija)) * ((papelInterior.Gramaje * ((Litho174 != null) ? Litho174.Interior_Alto : 0)) * ((Litho174 != null) ? Litho174.Interior_Ancho : 0) / 10000000.0)) : 0));


                //detalle.Interior.KilosPapel = (float)(((Tiraje * (Convert.ToDouble(CantidadPaginasInt) / Convert.ToDouble(NumeroDoblez)) * detalle.MaquinaInterior.MermaVariable) 
                //    + ((detalle.EntradasPag64 + detalle.EntradasPag48 + detalle.EntradasPag32 + detalle.EntradasPag24 + detalle.EntradasPag16 + detalle.EntradasPag12 + detalle.EntradasPag8 + detalle.EntradasPag4) * detalle.MaquinaInterior.MermaFija))
                //                            * ((papelInterior.Gramaje * detalle.Formato.Interior_Alto * detalle.Formato.Interior_Ancho) / 10000000.0));


                if ((CantidadPaginasTap > 0 && idPapelTap != 0 && idPapelTap != null))
                {
                    detalle.Tapa = new Tapa();
                    detalle.Tapa.CantidadPaginas = (CantidadPaginasTap > 0) ? Convert.ToInt32(CantidadPaginasTap) : 0;
                    detalle.Tapa.PapelId = (int)idPapelTap;

                    detalle.Tapa.Entradas = (float)(((1 * detalle.MaquinaTapa.MermaFija * CantidadModelos) / 1000.0) * ((papelTapa.Gramaje * TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0));

                    detalle.Tapa.CostoPapelTapaFijo = Math.Ceiling(detalle.Tapa.Entradas * papelTapa.PrecioKilos);

                    detalle.Tapa.Tiradas = (float)(((Tiraje * CantidadPaginasTap * detalle.MaquinaTapa.MermaVariable) / 1000.0) * ((papelTapa.Gramaje * TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0));

                    detalle.Tapa.CostoPapelTapaVari = (Math.Ceiling(((papelTapa.PrecioKilos * detalle.Tapa.Tiradas) / Convert.ToDouble(Tiraje)) * 100.0) / 100.0);

                    detalle.Tapa.KilosPapel = (float)((((Tiraje * (1.0 / CantidadPaginasTap) * detalle.MaquinaTapa.MermaVariable) + (1 * detalle.MaquinaTapa.MermaFija * CantidadModelos)) / 1000.0) * ((papelTapa.Gramaje * TapaFormato.TapaDiptica_Alto * TapaFormato.TapaDiptica_Ancho) / 10000.0));
                };
                float PesoInterior = (float)((((TapaFormato.FormatoCerradoX / 10.0) * (TapaFormato.FormatoCerradoY / 10.0) * papelInterior.Gramaje) / 10000000.0) * (CantidadPaginasInt / 2));
                float Pesotapa = (float)((((TapaFormato.FormatoCerradoX / 10.0) * (TapaFormato.FormatoCerradoY / 10.0) * ((papelTapa != null) ? papelTapa.Gramaje : 0)) / 10000000.0) * (4 / 2));
                float Enc = 0.002f;

                detalle.CostoVariablePallet = Math.Ceiling(((emb.PalletEstandar * Math.Ceiling(((PesoInterior + Pesotapa + Enc) * Convert.ToDouble(detalle.Tiraje)) / 700)) / Convert.ToDouble(Tiraje)) * 100) / 100;//CantidadPallet);
            }
            #endregion
            #region Totales
            detalle.TarifaFijaImpresion = Math.Ceiling((detalle.CostoFijoPag64 * detalle.EntradasPag64) + (detalle.CostoFijoPag48 * detalle.EntradasPag48) + (detalle.CostoFijoPag32 * detalle.EntradasPag32) + (detalle.CostoFijoPag24 * detalle.EntradasPag24) +
                                            (detalle.CostoFijoPag16 * detalle.EntradasPag16) + (detalle.CostoFijoPag12 * detalle.EntradasPag12) + (detalle.CostoFijoPag8 * detalle.EntradasPag8) + (detalle.CostoFijoPag4 * detalle.EntradasPag4)
                                            + detalle.CostoFijoTapa);
            detalle.TarifaVariableImpresion = (Math.Ceiling(((detalle.CostoVariablePag64 * detalle.EntradasPag64) + (detalle.CostoVariablePag48 * detalle.EntradasPag48) + (detalle.CostoVariablePag32 * detalle.EntradasPag32) + (detalle.CostoVariablePag24 * detalle.EntradasPag24) +
                                                (detalle.CostoVariablePag16 * detalle.EntradasPag16) + (detalle.CostoVariablePag12 * detalle.EntradasPag12) + (detalle.CostoVariablePag8 * detalle.EntradasPag8) +
                                                (detalle.CostoVariablePag4 * detalle.EntradasPag4) + detalle.CostoVariableTapa) * 100.0)) / 100.0;

            detalle.TarifaFijaEncuadernacion = Math.Ceiling(detalle.CostoFijoEncuadernacion + detalle.CostoFijoPlizado + detalle.CostoFijoTroquel + detalle.CostoFijoCorteFrontal);
            detalle.TarifaVariableEncuadernacion = (Math.Ceiling((detalle.CostoVariableEncuadernacion + detalle.CostoVariablePlizado + detalle.CostoVariableTroquel + detalle.CostoVariableCorteFrontal) * 100.0)) / 100.0;

            detalle.TarifaFijaDespacho = (detalle.CostoVariableSuministroCaja + detalle.CostoVariableInsercionCajaySellado + detalle.CostoVariableEnzunchado + detalle.CostoVariablePallet);

            detalle.TarifaFijaTerminacion = Math.Ceiling(detalle.CostoFijoQuintoColor + detalle.CostoFijoBarnizUV + detalle.CostoFijoBarnizAcuosoTapa);

            detalle.TarifaFijaPapel = Math.Ceiling(detalle.Interior.CostoPapelinteriorFijo + ((papelTapa != null) ? detalle.Tapa.CostoPapelTapaFijo : 0));
            detalle.TarifaVariablePapel = (Math.Ceiling((detalle.Interior.CostoPapelInteriorVari + ((papelTapa != null) ? detalle.Tapa.CostoPapelTapaVari : 0)) * 100.0)) / 100.0;
            detalle.TarifaVariableTerminacion = (Math.Ceiling((detalle.CostoVariableQuintoColor +
                             detalle.CostoVariableBarnizUV + detalle.CostoVariableEmbolsado + detalle.CostoVariableLaminado + detalle.CostoVariableBarnizAcuosoTapa +
                             detalle.CostoVariableAlzadoPlano + detalle.CostoVariableEmbolsadoManual + detalle.CostoVariableDesembolsado + detalle.CostoVariableAlzado +
                             detalle.CostoVariableInsercion + detalle.CostoVariablePegado + detalle.CostoVariableFajado + detalle.CostoVariableAdhesivoTotal + detalle.CostoVariablePegadoSticker) * 100.0)) / 100.0;
            double TarifaVariableMecanica = (Math.Ceiling((detalle.CostoVariableQuintoColor +
                             detalle.CostoVariableBarnizUV + detalle.CostoVariableEmbolsado + detalle.CostoVariableLaminado + detalle.CostoVariableBarnizAcuosoTapa + detalle.CostoVariableAdhesivoTotal) * 100.0)) / 100.0;
            detalle.ManufacturaFijo = Math.Ceiling(detalle.TarifaFijaImpresion + detalle.TarifaFijaEncuadernacion + detalle.TarifaFijaTerminacion);
            detalle.ManufacturaVari = (Math.Ceiling((detalle.TarifaVariableImpresion + detalle.TarifaVariableEncuadernacion + detalle.TarifaVariableTerminacion + detalle.TarifaFijaDespacho) * 100.0)) / 100.0;
            detalle.TotalNetoFijo = detalle.ManufacturaFijo + detalle.TarifaFijaPapel;
            detalle.TotalNetoVari = detalle.ManufacturaVari + detalle.TarifaVariablePapel;

            detalle.TotalNetoTotal = Math.Ceiling(detalle.TotalNetoFijo + ((detalle.TotalNetoVari - (detalle.TarifaVariableTerminacion - TarifaVariableMecanica)) * Convert.ToDouble(Tiraje)) +
                                     (detalle.CostoVariableAlzadoPlano * CantidadAlzadoPlano) + (detalle.CostoVariableDesembolsado * CantidadDesembolsado) + (detalle.CostoVariableAlzado * CantidadAlzado) +
                                     (detalle.CostoVariableInsercion * CantidadInsercion) + (detalle.CostoVariablePegado * CantidadPegado) + (detalle.CostoVariableFajado * CantidadFajado) +
                                     (detalle.CostoVariablePegadoSticker * CantidadPegadoSticker));
            detalle.PrecioUnitario = (Math.Ceiling((detalle.TotalNetoTotal / Convert.ToDouble(Tiraje)) * 100) / 100);

            #endregion
            return detalle;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult GuardarMod(string NombrePresupuesto, int Tiraje, string SelectFormato, int? SelectEnc, int CantidadInt, int? SelectPapelIntId, int? CantidadTapa, int? TapaPapel,
                            int? ddlQuintoColor, int? ddlBarnizAcuoso, int? ddlEmbolsado, int? ddlLaminado, int? ddlBarnizUV, int? ddlAlzadoPlano, int? ddlEmbolsadoManual, int? ddlPegadoSticker, int? ddlFajado, int? ddlPegado,
                            int? ddlInsercion, int? ddlAlzado, int? ddlDesembolsado, int? ddlAdhesivo//, int? ddlAdhesivoCms, int CantidadCajas
                            , int CantidadModelos, int ddlQuintoColorPasadas, int? CatalogoId, string NombreCatalogo, int CantidadAlzadoPlano, int CantidadDesembolsado, int CantidadAlzado, int CantidadInsercion,
                                int CantidadPegado, int CantidadFajado, int CantidadPegadoSticker, int CantidadEnCajas, int CantidadEnZuncho, int CantidadEnBolsa, string ddlTroquel, int CantidadTerminacionEmbolsado, int IdPresupuestoFormo)
        {

            Presupuesto p = ProcesarCalculoMod(SelectFormato, SelectEnc, CantidadInt, (CantidadTapa != null) ? Convert.ToInt32(CantidadTapa) : 0, "Plana", Tiraje, ddlQuintoColor, TapaPapel, SelectPapelIntId, ddlBarnizAcuoso, ddlEmbolsado, ddlLaminado, ddlBarnizUV, ddlAlzadoPlano,
                                            ddlEmbolsadoManual, ddlPegadoSticker, ddlFajado, ddlPegado, ddlInsercion, ddlAlzado, ddlDesembolsado, ddlAdhesivo, 4//, CantidadCajas
                                            , CantidadModelos, ddlQuintoColorPasadas, CantidadAlzadoPlano, CantidadDesembolsado, CantidadAlzado, CantidadInsercion, CantidadPegado, CantidadFajado, CantidadPegadoSticker, CatalogoId
                                            , CantidadEnCajas, CantidadEnZuncho, CantidadEnBolsa, ddlTroquel, CantidadTerminacionEmbolsado);
            TipoCatalogo tc;
            if (CatalogoId == null)
            {
                tc = new TipoCatalogo();
                tc.NombreTipoCatalogo = NombreCatalogo;
                tc.FormatoSeleccionado = SelectFormato;
                tc.PapelInterior = db.Papel.Where(x => x.IdPapel == (int)SelectPapelIntId).Select(x => x.NombrePapel + " " + x.Gramaje).SingleOrDefault();
                tc.PapelInteriorId = db.Papel.Where(x => x.IdPapel == (int)SelectPapelIntId).Select(x => x.IdPapel).SingleOrDefault();
                try
                {
                    tc.PapelTapaId = db.Papel.Where(x => x.IdPapel == (int)TapaPapel).Select(x => x.IdPapel).SingleOrDefault();
                    tc.PapelTapa = db.Papel.Where(x => x.IdPapel == (int)TapaPapel).Select(x => x.NombrePapel + " " + x.Gramaje).SingleOrDefault();
                }
                catch
                {
                    tc.PapelTapaId = null;
                    tc.PapelTapa = null;
                }

            }
            else
            {
                tc = db.Catalogo.Where(x => x.IdTipoCatalogo == (int)CatalogoId).FirstOrDefault();
            }
            Presupuesto pres2 = new Presupuesto();
            p.IdPresupuesto = IdPresupuestoFormo;
         //   p.Interior.IdInterior = db.Presupuesto.Where(x => x.IdPresupuesto == p.IdPresupuesto).Select(x => x.InteriorId).FirstOrDefault();
            p.TapaId= db.Presupuesto.Where(x => x.IdPresupuesto == p.IdPresupuesto).Select(x => x.TapaId).FirstOrDefault();
            p.NombrePresupuesto = NombrePresupuesto;
            p.TipoCatalogoId = tc.IdTipoCatalogo;
            p.FechaCreacion = DateTime.Now;
            p.Usuarioid = User.Identity.GetUserId();
            p.BarnizAcuoso = (ddlBarnizAcuoso != null ? (int)ddlBarnizAcuoso : 0);
            p.CantEnCajas = CantidadEnCajas;
            p.CantEnZuncho = CantidadEnZuncho;
            p.Estado = 1;
            p.EncuadernacionId = SelectEnc;

            List<Presupuesto_SubProceso> listaSubProceso = new List<Presupuesto_SubProceso>();
            if (ddlQuintoColor != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlQuintoColor, ValorFijoSubProceso = p.CostoFijoQuintoColor, ValorVariableSubProceso = p.CostoVariableQuintoColor, CantidadEjemplaresProceso = p.Tiraje }); };
            if ((ddlBarnizAcuoso != null) && (ddlBarnizAcuoso == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = db.SubProceso.Where(x => x.NombreSubProceso == "Barniz Acuoso (solo tiro)").Select(x => x.IdSubProceso).FirstOrDefault(), ValorFijoSubProceso = p.CostoFijoBarnizAcuosoTapa, ValorVariableSubProceso = p.CostoVariableBarnizAcuosoTapa, CantidadEjemplaresProceso = p.Tiraje }); };
            if (ddlEmbolsado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlEmbolsado, ValorVariableSubProceso = p.CostoVariableEmbolsado, CantidadEjemplaresProceso = p.CantidadTerminacionEmbolsado }); };
            if (ddlLaminado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlLaminado, ValorVariableSubProceso = p.CostoVariableLaminado, CantidadEjemplaresProceso = p.Tiraje }); };
            if (ddlBarnizUV != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlBarnizUV, ValorFijoSubProceso = p.CostoFijoBarnizUV, ValorVariableSubProceso = p.CostoVariableLaminado, CantidadEjemplaresProceso = p.Tiraje }); };
            if ((ddlAlzadoPlano != null) && (ddlAlzadoPlano == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = db.SubProceso.Where(x => x.NombreSubProceso == "Alzado Plano").Select(x => x.IdSubProceso).FirstOrDefault(), ValorVariableSubProceso = p.CostoVariableAlzadoPlano, CantidadEjemplaresProceso = CantidadAlzadoPlano }); };
            if ((ddlEmbolsadoManual != null) && (ddlEmbolsadoManual == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = db.SubProceso.Where(x => x.NombreSubProceso == "Embolsado manual").Select(x => x.IdSubProceso).FirstOrDefault(), ValorVariableSubProceso = p.CostoVariableEmbolsadoManual, CantidadEjemplaresProceso = Tiraje }); };
            if ((ddlDesembolsado != null) && (ddlDesembolsado == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = db.SubProceso.Where(x => x.NombreSubProceso == "Desembolsado simple").Select(x => x.IdSubProceso).FirstOrDefault(), ValorVariableSubProceso = p.CostoVariableDesembolsado, CantidadEjemplaresProceso = CantidadDesembolsado }); };
            if (ddlAlzado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlAlzado, ValorVariableSubProceso = p.CostoVariableAlzado, CantidadEjemplaresProceso = CantidadAlzado }); };
            if (ddlInsercion != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlInsercion, ValorVariableSubProceso = p.CostoVariableInsercion, CantidadEjemplaresProceso = CantidadInsercion }); };
            if (ddlPegado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlPegado, ValorVariableSubProceso = p.CostoVariablePegado, CantidadEjemplaresProceso = CantidadPegado }); };
            if (ddlFajado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlFajado, ValorVariableSubProceso = p.CostoVariableFajado, CantidadEjemplaresProceso = CantidadFajado }); };
            if (ddlPegado != null)
            {
                listaSubProceso.Add(new Presupuesto_SubProceso()
                {
                    PresupuestoId = p.IdPresupuesto,
                    SubProcesoId = 35//(int)ddlAdhesivo
                                                                                               ,
                    ValorVariableSubProceso = p.CostoVariableAdhesivoTotal,
                    CantidadEjemplaresProceso = CantidadPegado
                });
            };
            if ((ddlPegadoSticker != null) && (ddlPegadoSticker == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = 38, ValorVariableSubProceso = p.CostoVariablePegadoSticker, CantidadEjemplaresProceso = CantidadPegadoSticker }); };

            if (ModelState.IsValid)
            {

                if (ModInfoPresu(p) == true)
                {
                    if (ModPapelInterior(p) == true)
                    {
                        if (ModPapelTapa(p) == true)
                        {
                            if (ModSubProcesos(p.IdPresupuesto) == true)
                            {
                                db.Presupuesto_SubProceso.AddRange(listaSubProceso);
                                db.SaveChanges();
                            }
                        }
                    }
                }
            }

            pres2 = p;
            
            TempData["Alerta"] = alertas.Resultado_Action("ok", "Modificado", "");
            return Json(pres2, JsonRequestBehavior.AllowGet);
           
        }


        public bool ModInfoPresu(Presupuesto presu)
        {
            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["IntranetConnectionString"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "ModInfoPresu";
            //add any parameters the stored procedure might require
            cmd.Parameters.Add(new SqlParameter("@IDPresu", presu.IdPresupuesto));
            cmd.Parameters.Add(new SqlParameter("@Catalogo", presu.TipoCatalogoId));
            cmd.Parameters.Add(new SqlParameter("@NombrePresupuesto", presu.NombrePresupuesto));
            cmd.Parameters.Add(new SqlParameter("@Formato", presu.FormatoId));
            cmd.Parameters.Add(new SqlParameter("@Tiraje", presu.Tiraje));
            cmd.Parameters.Add(new SqlParameter("@Encuadernacion", presu.EncuadernacionId));

            cmd.Parameters.Add(new SqlParameter("@TotalNetoFijo", presu.TotalNetoFijo));
            cmd.Parameters.Add(new SqlParameter("@TotalNetoVari", presu.TotalNetoVari));
            cmd.Parameters.Add(new SqlParameter("@TotalNetoTotal", presu.TotalNetoTotal));

            cmd.Parameters.Add(new SqlParameter("@PrecioUnitario", presu.PrecioUnitario));
            cmd.Parameters.Add(new SqlParameter("@Moneda", presu.MonedaId));

            cmd.Parameters.Add(new SqlParameter("@EntradasPag16", presu.EntradasPag16));
            cmd.Parameters.Add(new SqlParameter("@EntradasPag12", presu.EntradasPag12));
            cmd.Parameters.Add(new SqlParameter("@EntradasPag8", presu.EntradasPag8));
            cmd.Parameters.Add(new SqlParameter("@EntradasPag4", presu.EntradasPag4));

            cmd.Parameters.Add(new SqlParameter("@CostoFijoPag16", presu.CostoFijoPag16));
            cmd.Parameters.Add(new SqlParameter("@CostoFijoPag12", presu.CostoFijoPag12));
            cmd.Parameters.Add(new SqlParameter("@CostoFijoPag8", presu.CostoFijoPag8));
            cmd.Parameters.Add(new SqlParameter("@CostoFijoPag4", presu.CostoFijoPag4));

            cmd.Parameters.Add(new SqlParameter("@CostoVariablePag16", presu.CostoVariablePag16));
            cmd.Parameters.Add(new SqlParameter("@CostoVariablePag12", presu.CostoVariablePag12));
            cmd.Parameters.Add(new SqlParameter("@CostoVariablePag8", presu.CostoVariablePag8));
            cmd.Parameters.Add(new SqlParameter("@CostoVariablePag4", presu.CostoVariablePag4));

            cmd.Parameters.Add(new SqlParameter("@CostoFijoEncuadernacion", presu.CostoFijoEncuadernacion));
            cmd.Parameters.Add(new SqlParameter("@CostoVariableEncuadernacion", presu.CostoVariableEncuadernacion));

            cmd.Parameters.Add(new SqlParameter("@CostoFijoTapa", presu.CostoFijoTapa));
            cmd.Parameters.Add(new SqlParameter("@CostoVariableTapa", presu.CostoVariableTapa));

            cmd.Parameters.Add(new SqlParameter("@TarifaFijaImpresion", presu.TarifaFijaImpresion));
            cmd.Parameters.Add(new SqlParameter("@TarifaVariableImpresion", presu.TarifaVariableImpresion));

            cmd.Parameters.Add(new SqlParameter("@TarifaFijaPapel", presu.TarifaFijaPapel));
            cmd.Parameters.Add(new SqlParameter("@TarifaVariablePapel", presu.TarifaVariablePapel));

            cmd.Parameters.Add(new SqlParameter("@TarifaFijaEncuadernacion", presu.TarifaFijaEncuadernacion));
            cmd.Parameters.Add(new SqlParameter("@TarifaVariableEncuadernacion", presu.TarifaVariableEncuadernacion));

            cmd.Parameters.Add(new SqlParameter("@TarifaFijaTerminacion", presu.TarifaFijaTerminacion));
            cmd.Parameters.Add(new SqlParameter("@TarifaVariableTerminacion", presu.TarifaVariableTerminacion));

            cmd.Parameters.Add(new SqlParameter("@CantidadCajas", presu.CantidadCajas));
            cmd.Parameters.Add(new SqlParameter("@Enzunchadoxpqte", presu.Enzunchadoxpqte));
            cmd.Parameters.Add(new SqlParameter("@CantidadPallet", presu.CantidadPallet));
            cmd.Parameters.Add(new SqlParameter("@CostoVariablePallet", presu.CostoVariablePallet));

            cmd.Parameters.Add(new SqlParameter("@EntradasPag64", presu.EntradasPag64));
            cmd.Parameters.Add(new SqlParameter("@EntradasPag48", presu.EntradasPag48));
            cmd.Parameters.Add(new SqlParameter("@EntradasPag32", presu.EntradasPag32));
            cmd.Parameters.Add(new SqlParameter("@EntradasPag24", presu.EntradasPag24));

            cmd.Parameters.Add(new SqlParameter("@CostoFijoPag64", presu.CostoFijoPag64));
            cmd.Parameters.Add(new SqlParameter("@CostoFijoPag48", presu.CostoFijoPag48));
            cmd.Parameters.Add(new SqlParameter("@CostoFijoPag32", presu.CostoFijoPag32));
            cmd.Parameters.Add(new SqlParameter("@CostoFijoPag24", presu.CostoFijoPag24));

            cmd.Parameters.Add(new SqlParameter("@CostoVariablePag64", presu.CostoVariablePag64));
            cmd.Parameters.Add(new SqlParameter("@CostoVariablePag48", presu.CostoVariablePag48));
            cmd.Parameters.Add(new SqlParameter("@CostoVariablePag32", presu.CostoVariablePag32));
            cmd.Parameters.Add(new SqlParameter("@CostoVariablePag24", presu.CostoVariablePag24));

            cmd.Parameters.Add(new SqlParameter("@TarifaFijaDespacho", presu.TarifaFijaDespacho));
            cmd.Parameters.Add(new SqlParameter("@ManufacturaFijo", presu.ManufacturaFijo));
            cmd.Parameters.Add(new SqlParameter("@ManufacturaVari", presu.ManufacturaVari));

            cmd.Parameters.Add(new SqlParameter("@CantidadEnBolsa", presu.CantidadenBolsa));
            cmd.Parameters.Add(new SqlParameter("@LibrosxCajas", presu.LibrosxCajas));
            cmd.Parameters.Add(new SqlParameter("@CantidadTerminacionEmbolsado", presu.CantidadTerminacionEmbolsado));
            cmd.Parameters.Add(new SqlParameter("@CantidadModelos", presu.CantidadModelos));

            cmd.Parameters.Add(new SqlParameter("@BarnizAcuoso", presu.BarnizAcuoso));
            cmd.Parameters.Add(new SqlParameter("@CantEnCajas", presu.CantEnCajas));
            cmd.Parameters.Add(new SqlParameter("@CantEnZuncho", presu.CantEnZuncho));

            cnn.Open();
            object o = cmd.ExecuteScalar();
            cnn.Close();
            return true;
        }

        public bool ModPapelInterior(Presupuesto presu)
        {
            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["IntranetConnectionString"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "ModPapelInterior";
            //add any parameters the stored procedure might require
            cmd.Parameters.Add(new SqlParameter("@IDInterior", presu.Interior.IdInterior));
            cmd.Parameters.Add(new SqlParameter("@CantidadPaginas", presu.Interior.CantidadPaginas));
            cmd.Parameters.Add(new SqlParameter("@PapelID", presu.Interior.PapelId));
            cmd.Parameters.Add(new SqlParameter("@KilosPapel", presu.Interior.KilosPapel));

            cmd.Parameters.Add(new SqlParameter("@Entradas", presu.Interior.Entradas));
            cmd.Parameters.Add(new SqlParameter("@Tiradas", presu.Interior.Tiradas));

            cmd.Parameters.Add(new SqlParameter("@CostoPapelInteriorFijo", presu.Interior.CostoPapelinteriorFijo));
            cmd.Parameters.Add(new SqlParameter("@CostoPapelInteriorVari", presu.Interior.CostoPapelInteriorVari));

            cnn.Open();
            object o = cmd.ExecuteScalar();
            cnn.Close();
            return true;
        }

        public bool ModPapelTapa(Presupuesto presu)
        {
            int DesdeBDD = ((presu.TapaId == null) ? 0 : (int)presu.TapaId);
            int DesdeActual = ((presu.Tapa == null) ? 0 : 1);

            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["IntranetConnectionString"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "ModPapelTapa";
            //add any parameters the stored procedure might require
            cmd.Parameters.Add(new SqlParameter("@IDPresu", presu.IdPresupuesto));
            cmd.Parameters.Add(new SqlParameter("@CantidadPaginas", (presu.Tapa != null ? presu.Tapa.CantidadPaginas : 0)));
            cmd.Parameters.Add(new SqlParameter("@PapelID", (presu.Tapa != null ? presu.Tapa.PapelId : 0)));
            cmd.Parameters.Add(new SqlParameter("@KilosPapel", (presu.Tapa != null ? presu.Tapa.KilosPapel : 0)));

            cmd.Parameters.Add(new SqlParameter("@Entradas", (presu.Tapa != null ? presu.Tapa.Entradas : 0)));
            cmd.Parameters.Add(new SqlParameter("@Tiradas", (presu.Tapa != null ? presu.Tapa.Tiradas : 0)));

            cmd.Parameters.Add(new SqlParameter("@CostoPapelTapaFijo", (presu.Tapa != null ? presu.Tapa.CostoPapelTapaFijo : 0)));
            cmd.Parameters.Add(new SqlParameter("@CostoPapelTapaVari", (presu.Tapa != null ? presu.Tapa.CostoPapelTapaVari : 0)));
            cmd.Parameters.Add(new SqlParameter("@DesdeBDD", DesdeBDD));
            cmd.Parameters.Add(new SqlParameter("@DesdeActual", DesdeActual));


            cnn.Open();
            try
            {
                object o = cmd.ExecuteScalar();
            }
            catch (Exception ee)
            {
                var algo = ee.ToString();
            }
            cnn.Close();
            return true;
        }

        public bool ModSubProcesos(int IDPresu)
        { 
            string cnnString = System.Configuration.ConfigurationManager.ConnectionStrings["IntranetConnectionString"].ConnectionString;
            SqlConnection cnn = new SqlConnection(cnnString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = cnn;
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.CommandText = "ModSubProcesos";
            //add any parameters the stored procedure might require
            cmd.Parameters.Add(new SqlParameter("@IDPresu", IDPresu));
            cnn.Open();
            try
            {
                object o = cmd.ExecuteScalar();
            }
            catch (Exception ee)
            {
                var algo = ee.ToString();
            }
            cnn.Close();
            return true;
        }
    }
}
