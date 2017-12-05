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

namespace ProyectoPersonal.Controllers.Cotizador
{
    public class PresupuestoController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Presupuesto
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Index()
        {
            var presupuesto = db.Presupuesto.Include(p => p.Formato).Include(p => p.Interior).Include(p => p.Tapa).Include(p => p.Interior.Papel).Include(p => p.Tapa.Papel);
            return View(presupuesto.ToList());

        }

        public ActionResult OfertaComercial(int id)
        {
            var presupuesto = db.Presupuesto.Include(p => p.Formato).Include(p => p.Interior).Include(p => p.Tapa).Include(p => p.Interior.Papel)
                                                    .Include(p => p.Tapa.Papel).Include(p => p.Encuadernacion).Include(p => p.Moneda).Include(p => p.Usuario).Where(x => x.IdPresupuesto == id).FirstOrDefault();

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
        public JsonResult Calcular(string NombrePresupuesto, int Tiraje, string SelectFormato, int? SelectEnc, int CantidadInt, int? SelectPapelIntId, int? CantidadTapa,
                                        int? TapaPapel, int? ddlQuintoColor, int? ddlBarnizAcuoso, int? ddlEmbolsado, int? ddlLaminado, int? ddlBarnizUV, int? ddlAlzadoPlano, int? ddlEmbolsadoManual,
                                        int? ddlPegadoSticker, int? ddlFajado, int? ddlPegado, int? ddlInsercion, int? ddlAlzado, int? ddlDesembolsado, int? ddlAdhesivo, //int? ddlAdhesivoCms,int CantidadCajas,
                                        int CantidadModelos, int ddlQuintoColorPasadas, int? CatalogoId, string NombreCatalogo, int CantidadAlzadoPlano, int CantidadDesembolsado, int CantidadAlzado, int CantidadInsercion,
                                        int CantidadPegado, int CantidadFajado, int CantidadPegadoSticker)
        {
            Presupuesto pres = ProcesarCalculo(SelectFormato, SelectEnc, CantidadInt, (CantidadTapa != null) ? Convert.ToInt32(CantidadTapa) : 0, "Plana", Tiraje, ddlQuintoColor, TapaPapel, SelectPapelIntId, ddlBarnizAcuoso, ddlEmbolsado, ddlLaminado, ddlBarnizUV
                                            , ddlAlzadoPlano, ddlEmbolsadoManual, ddlPegadoSticker, ddlFajado, ddlPegado, ddlInsercion, ddlAlzado, ddlDesembolsado, ddlAdhesivo, 4//, CantidadCajas
                                            , CantidadModelos, ddlQuintoColorPasadas, CantidadAlzadoPlano, CantidadDesembolsado, CantidadAlzado, CantidadInsercion, CantidadPegado, CantidadFajado, CantidadPegadoSticker);
            pres.NombrePresupuesto = NombrePresupuesto;

            return Json(pres, JsonRequestBehavior.AllowGet);
        }

        public Presupuesto ProcesarCalculo(string FormatoId, int? EncuadernacionId, int CantidadPaginasInt, int CantidadPaginasTap, string MaquinaTap, int Tiraje, int? IDQuintoColor
            , int? idPapelTap, int? idPapelInterior, int? BarnizAcuoso, int? Embolsado, int? Laminado, int? UV, int? AlzadoPlano, int? EmbolsadoManual, int? Sticker, int? Fajado, int? Pegado, int? Insercion, int? Alzado, int? Desembolsado,
            int? Adhesivo, int? cmsAdhesivo, int CantidadModelos, int CantidadPasadaQuintoColor, int CantidadAlzadoPlano, int CantidadDesembolsado, int CantidadAlzado, int CantidadInsercion,
                                        int CantidadPegado, int CantidadFajado, int CantidadPegadoSticker)
        {

            Presupuesto detalle = new Presupuesto();
            detalle.MonedaId = db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).Select(x => x.IdMoneda).FirstOrDefault();
            detalle.Tiraje = Tiraje;
            #region EntradasxFormato

            int NumeroDoblez = 0;
            if ((CantidadPaginasInt / 64) > 0)
            {
                detalle.EntradasPag64 = (CantidadPaginasInt / 64);
                NumeroDoblez = 64;
                switch (CantidadPaginasInt - (detalle.EntradasPag64 * 64))
                {
                    case 60:
                        detalle.EntradasPag48 = 1;
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 56:
                        detalle.EntradasPag48 = 1;
                        detalle.EntradasPag8 = 1;
                        break;
                    case 52:
                        detalle.EntradasPag48 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 48:
                        detalle.EntradasPag48 = 1;
                        break;
                    case 44:
                        detalle.EntradasPag32 = 1;
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 40:
                        detalle.EntradasPag32 = 1;
                        detalle.EntradasPag8 = 1;
                        break;
                    case 36:
                        detalle.EntradasPag32 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 32:
                        detalle.EntradasPag32 = 1;
                        break;
                    case 28:
                        detalle.EntradasPag24 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 24:
                        detalle.EntradasPag24 = 1;
                        break;
                    case 20:
                        detalle.EntradasPag16 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 16:
                        detalle.EntradasPag16 = 1;
                        break;
                    case 12:
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 8:
                        detalle.EntradasPag8 = 1;
                        break;
                    case 4:
                        detalle.EntradasPag4 = 1;
                        break;
                }
            }
            else if ((CantidadPaginasInt / 48) > 0)
            {
                detalle.EntradasPag48 = (CantidadPaginasInt / 48);
                NumeroDoblez = 48;
                switch (CantidadPaginasInt - (detalle.EntradasPag48 * 48))
                {
                    case 44:
                        detalle.EntradasPag32 = 1;
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 40:
                        detalle.EntradasPag32 = 1;
                        detalle.EntradasPag8 = 1;
                        break;
                    case 36:
                        detalle.EntradasPag32 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 32:
                        detalle.EntradasPag32 = 1;
                        break;
                    case 28:
                        detalle.EntradasPag24 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 24:
                        detalle.EntradasPag24 = 1;
                        break;
                    case 20:
                        detalle.EntradasPag16 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 16:
                        detalle.EntradasPag16 = 1;
                        break;
                    case 12:
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 8:
                        detalle.EntradasPag8 = 1;
                        break;
                    case 4:
                        detalle.EntradasPag4 = 1;
                        break;
                }
            }
            else if ((CantidadPaginasInt / 32) > 0)
            {
                detalle.EntradasPag32 = (CantidadPaginasInt / 32);
                NumeroDoblez = 32;
                switch (CantidadPaginasInt - (detalle.EntradasPag32 * 32))
                {
                    case 28:
                        detalle.EntradasPag24 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 24:
                        detalle.EntradasPag24 = 1;
                        break;
                    case 20:
                        detalle.EntradasPag16 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 16:
                        detalle.EntradasPag16 = 1;
                        break;
                    case 12:
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 8:
                        detalle.EntradasPag8 = 1;
                        break;
                    case 4:
                        detalle.EntradasPag4 = 1;
                        break;
                }
            }
            else if ((CantidadPaginasInt / 24) > 0)
            {
                detalle.EntradasPag24 = (CantidadPaginasInt / 24);
                NumeroDoblez = 24;
                switch (CantidadPaginasInt - (detalle.EntradasPag24 * 24))
                {
                    case 20:
                        detalle.EntradasPag16 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 16:
                        detalle.EntradasPag16 = 1;
                        break;
                    case 12:
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 8:
                        detalle.EntradasPag8 = 1;
                        break;
                    case 4:
                        detalle.EntradasPag4 = 1;
                        break;
                }
            }
            else if ((CantidadPaginasInt / 16) > 0)
            {
                detalle.EntradasPag16 = (CantidadPaginasInt / 16);
                NumeroDoblez = 16;
                switch (CantidadPaginasInt - (detalle.EntradasPag16 * 16))
                {
                    case 12:
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 8:
                        detalle.EntradasPag8 = 1;
                        break;
                    case 4:
                        detalle.EntradasPag4 = 1;
                        break;
                }
            }
            else if ((CantidadPaginasInt / 8) > 0)
            {
                detalle.EntradasPag8 = (CantidadPaginasInt / 8);
                NumeroDoblez = 8;
                switch (CantidadPaginasInt - (detalle.EntradasPag8 * 8))
                {
                    case 4:
                        detalle.EntradasPag4 = 1;
                        break;
                }
            }

            //switch (SelectFormato.EntradasxFormatos)
            //{
            //    case 64:
            //        break;
            //    case 48:
            //        break;
            //    case 32:
            //        break;
            //    case 24:
            //        break;
            //    case 16:
            //        detalle.EntradasPag16 = (CantidadPaginasInt / 16);
            //        cantidadfaltante = CantidadPaginasInt - (detalle.EntradasPag16 * 16);
            //        if (cantidadfaltante >= 8)
            //        {
            //            detalle.EntradasPag8 = 1;
            //            cantidadfaltante -= 8;
            //            if (cantidadfaltante == 4)
            //            {
            //                detalle.EntradasPag4 = 1;
            //                cantidadfaltante = 0;
            //            }
            //        }
            //        else if (cantidadfaltante != 0)
            //        {
            //            detalle.EntradasPag4 = 1;
            //            cantidadfaltante = 0;
            //        }
            //        break;
            //    case 12:
            //        detalle.EntradasPag12 = (CantidadPaginasInt / 12);
            //        cantidadfaltante = CantidadPaginasInt - (detalle.EntradasPag12 * 12);
            //        if (cantidadfaltante >= 8)
            //        {
            //            detalle.EntradasPag4 = 2;
            //            cantidadfaltante -= 8;
            //            if (cantidadfaltante == 4)
            //            {
            //                detalle.EntradasPag4 = 1;
            //                cantidadfaltante = 0;
            //            }
            //        }
            //        else if (cantidadfaltante != 0)
            //        {
            //            detalle.EntradasPag4 = 1;
            //            cantidadfaltante = 0;
            //        }
            //        break;
            //    case 8:
            //        detalle.EntradasPag8 = (CantidadPaginasInt / 8);
            //        cantidadfaltante = CantidadPaginasInt - (detalle.EntradasPag8 * 8);
            //        if (cantidadfaltante == 4)
            //        {
            //            detalle.EntradasPag4 = 1;
            //            cantidadfaltante = 0;
            //        }
            //        break;
            //    default:
            //        break;

            //}
            #endregion
            #region Impresion Interior Nuevo

            int doblezdelFormato = 0;
            if(NumeroDoblez<=47)
            {
                doblezdelFormato = 16;
            }
            else if (NumeroDoblez <= 48)
            {
                doblezdelFormato = 48;
            }
            else
            {
                doblezdelFormato = 64;
            }
            string[] split = FormatoId.Split('x');
            double formatox = Convert.ToDouble(split[0]);
            double formatoy = Convert.ToDouble(split[1]);
            detalle.Formato = db.Formato.Where(x => x.FormatoCerradoX == formatox && x.FormatoCerradoY == formatoy && x.EntradasxFormatos == doblezdelFormato).FirstOrDefault();
            if (detalle.Formato == null)
            {
                detalle.Formato = db.Formato.Where(x => x.FormatoCerradoX == formatox && x.FormatoCerradoY == formatoy && x.EntradasxFormatos == 16).FirstOrDefault();
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
            detalle.CostoVariableTapa = (CantidadPaginasTap > 0) ? (Math.Ceiling(((ListImpTapa.Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault()) / 1000.0) * 100.0) / 100.0) / CantidadPaginasTap : 0;

            List<SubProceso> ListTerm = db.SubProceso.Include(x => x.TipoMoneda).Include(x => x.TipoMoneda.Monedas).ToList();
            detalle.CostoFijoQuintoColor = (IDQuintoColor != null) ? Math.Ceiling(ListTerm.Where(x => x.IdSubProceso == IDQuintoColor).Select(x => x.CostoFijoSubProceso * CantidadPasadaQuintoColor * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
            detalle.CostoVariableQuintoColor = (IDQuintoColor != null) ? (Math.Ceiling(((ListTerm.Where(x => x.IdSubProceso == IDQuintoColor).Select(x => x.CostoVariableSubProceso * CantidadPasadaQuintoColor * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) * 100.0) / 100.0) : 0;

            detalle.CostoFijoPlizado = (db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => x.Gramaje).FirstOrDefault() >= 170) ? ListTerm.Where(x => x.NombreSubProceso == "Plizado").Select(x => x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault() : 0;
            detalle.CostoVariablePlizado = (db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => x.Gramaje).FirstOrDefault() >= 170) ? (ListTerm.Where(x => x.NombreSubProceso == "Plizado").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault() / Convert.ToDouble(CantidadPaginasTap)) : 0;

            #endregion
            #region Encuadernacion Nuevo
            detalle.CostoFijoEncuadernacion = (EncuadernacionId != null) ? Math.Ceiling(db.Encuadernacion.Where(x => x.IdEncuadernacion == EncuadernacionId).Select(x => x.ValorFijo * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(y => y.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
            detalle.CostoVariableEncuadernacion = (EncuadernacionId != null) ? (Math.Ceiling(((db.Encuadernacion.Where(x => x.IdEncuadernacion == EncuadernacionId).Select(x => x.ValorVariable * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(y => y.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) * 100.0) / 100.0) : 0;

            #endregion
            #region Terminaciones Nuevas
            double dobleentrada = (BarnizAcuoso == 4)&&(CantidadPaginasTap>0) ? 2 : 1;
            detalle.CostoFijoBarnizAcuosoTapa = ((CantidadPaginasTap > 0) && (BarnizAcuoso >= 2)) ? Math.Ceiling(ListTerm.Where(x => x.NombreSubProceso == "Barniz Acuoso (solo tiro)").Select(x => x.CostoFijoSubProceso * dobleentrada  * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
            detalle.CostoVariableBarnizAcuosoTapa = ((CantidadPaginasTap > 0) && (BarnizAcuoso >= 2)) ? Math.Ceiling(((ListTerm.Where(x => x.NombreSubProceso == "Barniz Acuoso (solo tiro)").Select(x => x.CostoVariableSubProceso * dobleentrada * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) * 100.0) / 100.0 : 0;
            detalle.CostoVariableEmbolsado = (Embolsado != null) ? (ListTerm.Where(x => x.IdSubProceso == Embolsado).Select(x => x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariableLaminado = ((Laminado != null)&& (CantidadPaginasTap > 0)) ? (ListTerm.Where(x => x.IdSubProceso == Laminado).Select(x => (x.CostoVariableSubProceso * (detalle.Formato.TapaDiptica_Alto * detalle.Formato.TapaDiptica_Ancho) / 10000.0)).FirstOrDefault())/Convert.ToDouble(CantidadPaginasTap) : 0;
            detalle.CostoFijoBarnizUV = (UV != null) ? Math.Ceiling(ListTerm.Where(x => x.IdSubProceso == UV).Select(x => x.CostoFijoSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariableBarnizUV = (UV != null) ? Math.Ceiling(ListTerm.Where(x => x.IdSubProceso == UV).Select(x => x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoFijoTroquel = (db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => x.Gramaje).FirstOrDefault() >= 300) ? (ListTerm.Where(x => x.NombreSubProceso == "Troquel").Select(x => x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
            detalle.CostoVariableTroquel = (db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => x.Gramaje).FirstOrDefault() >= 300) ? ((ListTerm.Where(x => x.NombreSubProceso == "Troquel").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / Convert.ToDouble(CantidadPaginasTap)) : 0;
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
            detalle.CostoVariableAdhesivo = (Adhesivo != null) ? (ListTerm.Where(x => x.IdSubProceso == Adhesivo).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.AdhesivoCms = Convert.ToDouble(cmsAdhesivo);
            detalle.CostoVariableAdhesivoTotal = Math.Ceiling((detalle.AdhesivoCms * detalle.CostoVariableAdhesivo) * 10.0) / 10.0;
            #region Embalaje
            Embalaje emb = db.Embalaje.Where(x => x.Estado == true).FirstOrDefault();
            Papel p = db.Papel.Where(x => x.IdPapel == idPapelInterior).FirstOrDefault();
            double LomoInterior = Math.Ceiling((((p.Micron * Convert.ToDouble(CantidadPaginasInt))/10000.0) + p.Adhesivo)*10.0)/10.0;
            double LomoTapa = db.Papel.Where(x => x.IdPapel == idPapelTap).Select(x => (x.Micron/10000.0)).FirstOrDefault();
            double LomocTapa = (Math.Ceiling((LomoInterior +(2*LomoTapa))*10))/10;
            double librosxCajas = Math.Floor(emb.AltoCaja / LomocTapa)* emb.Base;
            double CantidadDeCajas = Math.Ceiling(Convert.ToDouble(Tiraje) / librosxCajas);
            detalle.CostoVariableSuministroCaja = //(Math.Ceiling((
                (emb.CajaEstandar * CantidadDeCajas);// / Convert.ToDouble(Tiraje)) * 100)) / 100;
            detalle.CostoVariableInsercionCajaySellado = //(Math.Ceiling((
                (emb.EncajadoxCaja * CantidadDeCajas);// / Convert.ToDouble(Tiraje))*100))/100;

            detalle.CostoVariableEnzunchado = 0;//(Math.Ceiling((
                                                //(emb.Enzunchado * (Tiraje - CantidadCajas)); /// Convert.ToDouble(Enzunchadoxpqte)) * 100)) / 100;
            

            #endregion
            detalle.CostoVariablePegadoSticker = (Sticker != null && Sticker == 2) ? (ListTerm.Where(x => x.NombreSubProceso == "Pegado de Sticker").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            #endregion
            #region Papel
            if ((CantidadPaginasInt > 0) && (idPapelInterior != null))
            {
                detalle.Interior = new Interior();
                detalle.Interior.CantidadPaginas = CantidadPaginasInt;
                detalle.Interior.PapelId = (int)idPapelInterior;

                detalle.Interior.Entradas = (float)((((detalle.EntradasPag64 + detalle.EntradasPag48 + detalle.EntradasPag32 + detalle.EntradasPag24 + detalle.EntradasPag16 + detalle.EntradasPag12 + detalle.EntradasPag8 + detalle.EntradasPag4) * detalle.MaquinaInterior.MermaFija) / 1000.0)
                                            * ((papelInterior.Gramaje * detalle.Formato.Interior_Alto * detalle.Formato.Interior_Ancho) / 10000.0));

                detalle.Interior.CostoPapelinteriorFijo = Math.Ceiling(detalle.Interior.Entradas * papelInterior.PrecioKilos);

                detalle.Interior.Tiradas = (float)(((Tiraje * (Convert.ToDouble(CantidadPaginasInt) / Convert.ToDouble(NumeroDoblez)) * detalle.MaquinaInterior.MermaVariable) / 1000.0) * ((papelInterior.Gramaje * detalle.Formato.Interior_Alto * detalle.Formato.Interior_Ancho) / 10000.0));

                detalle.Interior.CostoPapelInteriorVari = Math.Ceiling(papelInterior.PrecioKilos * detalle.Interior.Tiradas);

                detalle.Interior.KilosPapel = (float)(((Tiraje * (Convert.ToDouble(CantidadPaginasInt) / Convert.ToDouble(NumeroDoblez)) * detalle.MaquinaInterior.MermaVariable) + ((detalle.EntradasPag64 + detalle.EntradasPag48 + detalle.EntradasPag32 + detalle.EntradasPag24 + detalle.EntradasPag16 + detalle.EntradasPag12 + detalle.EntradasPag8 + detalle.EntradasPag4) * detalle.MaquinaInterior.MermaFija))
                                            * ((papelInterior.Gramaje * detalle.Formato.Interior_Alto * detalle.Formato.Interior_Ancho) / 10000.0));
                if ((CantidadPaginasTap > 0 && idPapelTap != 0))
                {
                    detalle.Tapa = new Tapa();
                    detalle.Tapa.CantidadPaginas = (CantidadPaginasTap > 0) ? Convert.ToInt32(CantidadPaginasTap) : 0;
                    detalle.Tapa.PapelId = (int)idPapelTap;

                    detalle.Tapa.Entradas = (float)(((1 * detalle.MaquinaTapa.MermaFija) / 1000.0) * ((papelTapa.Gramaje * detalle.Formato.TapaDiptica_Alto * detalle.Formato.TapaDiptica_Ancho) / 10000.0));

                    detalle.Tapa.CostoPapelTapaFijo = Math.Ceiling(detalle.Tapa.Entradas * papelTapa.PrecioKilos);
                                        
                    detalle.Tapa.Tiradas = (float)(((Tiraje * CantidadPaginasTap * detalle.MaquinaTapa.MermaVariable) / 1000.0) * ((papelTapa.Gramaje * detalle.Formato.TapaDiptica_Alto * detalle.Formato.TapaDiptica_Ancho) / 10000.0));

                    detalle.Tapa.CostoPapelTapaVari = Math.Ceiling(papelTapa.PrecioKilos * detalle.Tapa.Tiradas);

                    detalle.Tapa.KilosPapel = (float)((((Tiraje * (1.0 / CantidadPaginasTap) * detalle.MaquinaTapa.MermaVariable) + (1 * detalle.MaquinaTapa.MermaFija)) / 1000.0) * ((papelTapa.Gramaje * detalle.Formato.TapaDiptica_Alto * detalle.Formato.TapaDiptica_Ancho) / 10000.0));
                };
                float PesoInterior = (float)((((detalle.Formato.FormatoCerradoX/10.0) * (detalle.Formato.FormatoCerradoY / 10.0) * papelInterior.Gramaje) / 10000000.0) * (CantidadPaginasInt / 2));
                float Pesotapa = (float)((((detalle.Formato.FormatoCerradoX / 10.0) * (detalle.Formato.FormatoCerradoY / 10.0) * ((papelTapa != null) ?papelTapa.Gramaje: 0)) / 10000000.0) * (4 / 2));
                float Enc = 0.002f;
                
                detalle.CostoVariablePallet = (emb.PalletEstandar * (int)Math.Ceiling(((PesoInterior + Pesotapa + Enc) * Tiraje) / 700));//CantidadPallet);
            }
            #endregion
            #region Totales
            detalle.TarifaFijaImpresion = Math.Ceiling((detalle.CostoFijoPag64 * detalle.EntradasPag64) + (detalle.CostoFijoPag48 * detalle.EntradasPag48) + (detalle.CostoFijoPag32 * detalle.EntradasPag32) + (detalle.CostoFijoPag24 * detalle.EntradasPag24) +
                                            (detalle.CostoFijoPag16 * detalle.EntradasPag16) + (detalle.CostoFijoPag12 * detalle.EntradasPag12) + (detalle.CostoFijoPag8 * detalle.EntradasPag8) + (detalle.CostoFijoPag4 * detalle.EntradasPag4)
                                            + detalle.CostoFijoTapa);
            detalle.TarifaVariableImpresion = (Math.Ceiling(((detalle.CostoVariablePag64 * detalle.EntradasPag64) + (detalle.CostoVariablePag48 * detalle.EntradasPag48) + (detalle.CostoVariablePag32 * detalle.EntradasPag32) + (detalle.CostoVariablePag24 * detalle.EntradasPag24) +
                                                (detalle.CostoVariablePag16 * detalle.EntradasPag16) + (detalle.CostoVariablePag12 * detalle.EntradasPag12) + (detalle.CostoVariablePag8 * detalle.EntradasPag8) +
                                                (detalle.CostoVariablePag4 * detalle.EntradasPag4) + detalle.CostoVariableTapa)*100.0))/100.0;

            detalle.TarifaFijaEncuadernacion = Math.Ceiling(detalle.CostoFijoEncuadernacion + detalle.CostoFijoPlizado + detalle.CostoFijoTroquel + detalle.CostoFijoCorteFrontal);
            detalle.TarifaVariableEncuadernacion = (Math.Ceiling((detalle.CostoVariableEncuadernacion + detalle.CostoVariablePlizado + detalle.CostoVariableTroquel + detalle.CostoVariableCorteFrontal)*100.0))/100.0;

            detalle.TarifaFijaDespacho = (detalle.CostoVariableSuministroCaja + detalle.CostoVariableInsercionCajaySellado + detalle.CostoVariableEnzunchado + detalle.CostoVariablePallet);

            detalle.TarifaFijaTerminacion = Math.Ceiling(detalle.CostoFijoQuintoColor + detalle.CostoFijoBarnizUV + detalle.CostoFijoBarnizAcuosoTapa);

            detalle.TarifaFijaPapel = Math.Ceiling(detalle.Interior.CostoPapelinteriorFijo + ((papelTapa != null) ? detalle.Tapa.CostoPapelTapaFijo:0));
            detalle.TarifaVariablePapel = (Math.Ceiling((detalle.Interior.CostoPapelInteriorVari + ((papelTapa != null) ? detalle.Tapa.CostoPapelTapaVari:0))*100.0))/100.0;
            detalle.TarifaVariableTerminacion = (Math.Ceiling((detalle.CostoVariableQuintoColor +
                             detalle.CostoVariableBarnizUV + detalle.CostoVariableEmbolsado + detalle.CostoVariableLaminado + detalle.CostoVariableBarnizAcuosoTapa +
                             detalle.CostoVariableAlzadoPlano + detalle.CostoVariableEmbolsadoManual + detalle.CostoVariableDesembolsado + detalle.CostoVariableAlzado +
                             detalle.CostoVariableInsercion + detalle.CostoVariablePegado + detalle.CostoVariableFajado + detalle.CostoVariableAdhesivoTotal + detalle.CostoVariablePegadoSticker)*100.0))/100.0;
            double TarifaVariableMecanica = (Math.Ceiling((detalle.CostoVariableQuintoColor +
                             detalle.CostoVariableBarnizUV + detalle.CostoVariableEmbolsado + detalle.CostoVariableLaminado + detalle.CostoVariableBarnizAcuosoTapa + detalle.CostoVariableAdhesivoTotal) * 100.0)) / 100.0;
            detalle.TotalNetoFijo = Math.Ceiling(detalle.TarifaFijaImpresion + detalle.TarifaFijaEncuadernacion + detalle.TarifaFijaTerminacion + detalle.TarifaFijaPapel + detalle.TarifaFijaDespacho);
            detalle.TotalNetoVari = (Math.Ceiling((detalle.TarifaVariableImpresion + detalle.TarifaVariableEncuadernacion + detalle.TarifaVariableTerminacion + detalle.TarifaVariablePapel)*100.0))/100.0;

            detalle.TotalNetoTotal = Math.Ceiling(detalle.TotalNetoFijo + detalle.TarifaFijaDespacho + ((detalle.TotalNetoVari - (detalle.TarifaVariableTerminacion - TarifaVariableMecanica)) * Convert.ToDouble(Tiraje)) + 
                                     (detalle.CostoVariableAlzadoPlano * CantidadAlzadoPlano) + (detalle.CostoVariableDesembolsado * CantidadDesembolsado) + (detalle.CostoVariableAlzado * CantidadAlzado) + 
                                     (detalle.CostoVariableInsercion * CantidadInsercion) + (detalle.CostoVariablePegado * CantidadPegado) + (detalle.CostoVariableFajado * CantidadFajado) + 
                                     (detalle.CostoVariablePegadoSticker * CantidadPegadoSticker));
            detalle.PrecioUnitario = (Math.Ceiling((detalle.TotalNetoTotal / Convert.ToDouble(Tiraje)) * 100) / 100);

            #endregion
            return detalle;
        }

        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult SeleccionarPapel(int EmpresaID)
        {
            List<Papel> lista = db.Papel.Where(x => x.EmpresaId == EmpresaID).ToList();
            return Json(new SelectList(lista, "IdPapel", "NombreCompletoPapel"),
                JsonRequestBehavior.AllowGet
            );
        }

        // GET: Presupuesto/Create
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public ActionResult Create()
        {
            PresupuestoForm pres = new PresupuestoForm();
            pres.Encuadernaciones = db.Encuadernacion.ToList();
            pres.Formatos = db.Formato.ToList();
            pres.Papeles = db.Papel.Where(x=> x.EmpresaId==2).ToList();
            pres.SubProcesos = db.SubProceso.Include("Proceso").ToList();
            pres.Catalogo = db.Catalogo.ToList();
            pres.Empresa = db.Empresa.ToList();
            List<SelectListItem> s = new List<SelectListItem>();
            for (int i = 4; i <= 400; i = i + 4)
            {
                s.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.CantidadInt = s;
            ViewBag.ValorUF = db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).Select(x => x.Valor).FirstOrDefault();
            //ViewBag.SubProceso = db.SubProceso.Include("Proceso").ToList();
            return View(pres);
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Administrador,SuperUser,User")]
        //public JsonResult Guardar(string NombrePresupuesto, int Tiraje, string SelectFormato, int? SelectEnc, int CantidadInt, int? SelectPapelIntId, int? CantidadTapa, int? TapaPapel,
        //                            int? ddlQuintoColor, int? ddlBarnizAcuoso, int? ddlEmbolsado, int? ddlLaminado, int? ddlBarnizUV, int? ddlAlzadoPlano, int? ddlEmbolsadoManual, int? ddlPegadoSticker, int? ddlFajado, int? ddlPegado,
        //                            int? ddlInsercion, int? ddlAlzado, int? ddlDesembolsado, int? ddlAdhesivo//, int? ddlAdhesivoCms, int CantidadCajas
        //                            , int CantidadModelos, int ddlQuintoColorPasadas, int? CatalogoId, string NombreCatalogo)
        //{
        //    TipoCatalogo tc;
        //    if (CatalogoId == null)
        //    {
        //        tc = new TipoCatalogo();
        //        tc.NombreTipoCatalogo = NombreCatalogo;
        //    }
        //    else
        //    {
        //        tc = db.Catalogo.Where(x => x.IdTipoCatalogo == (int)CatalogoId).FirstOrDefault();
        //    }
        //    Presupuesto p = ProcesarCalculo(SelectFormato, SelectEnc, CantidadInt, (CantidadTapa != null) ? Convert.ToInt32(CantidadTapa) : 0, "Plana", Tiraje, ddlQuintoColor, SelectPapelIntId, TapaPapel, ddlBarnizAcuoso, ddlEmbolsado, ddlLaminado, ddlBarnizUV, ddlAlzadoPlano,
        //                                    ddlEmbolsadoManual, ddlPegadoSticker, ddlFajado, ddlPegado, ddlInsercion, ddlAlzado, ddlDesembolsado, ddlAdhesivo, 4//, CantidadCajas
        //                                    , CantidadModelos, ddlQuintoColorPasadas);
        //    Presupuesto pres2 = new Presupuesto();
        //    p.NombrePresupuesto = NombrePresupuesto;
        //    p.TipoCatalogoId = tc.IdTipoCatalogo;
        //    p.FechaCreacion = DateTime.Now;
        //    p.Usuarioid = User.Identity.GetUserId();

        //    p.EncuadernacionId = SelectEnc;

        //    List<Presupuesto_SubProceso> listaSubProceso = new List<Presupuesto_SubProceso>();
        //    if (ddlQuintoColor != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlQuintoColor, ValorFijoSubProceso = p.CostoFijoQuintoColor, ValorVariableSubProceso = p.CostoVariableQuintoColor }); };
        //    if ((ddlBarnizAcuoso != null) && (ddlBarnizAcuoso == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = db.SubProceso.Where(x => x.NombreSubProceso == "Barniz Acuoso (solo tiro)").Select(x => x.IdSubProceso).FirstOrDefault(), ValorFijoSubProceso = p.CostoFijoBarnizAcuosoTapa, ValorVariableSubProceso = p.CostoVariableBarnizAcuosoTapa }); };
        //    if (ddlEmbolsado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlEmbolsado, ValorVariableSubProceso = p.CostoVariableEmbolsado }); };
        //    if (ddlLaminado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlLaminado, ValorVariableSubProceso = p.CostoVariableLaminado }); };
        //    if (ddlBarnizUV != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlBarnizUV, ValorFijoSubProceso = p.CostoFijoBarnizUV, ValorVariableSubProceso = p.CostoVariableLaminado }); };
        //    if ((ddlAlzadoPlano != null) && (ddlAlzadoPlano == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = db.SubProceso.Where(x => x.NombreSubProceso == "Alzado Plano").Select(x => x.IdSubProceso).FirstOrDefault(), ValorVariableSubProceso = p.CostoVariableAlzadoPlano }); };
        //    if ((ddlEmbolsadoManual != null) && (ddlEmbolsadoManual == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = db.SubProceso.Where(x => x.NombreSubProceso == "Embolsado manual").Select(x => x.IdSubProceso).FirstOrDefault(), ValorVariableSubProceso = p.CostoVariableEmbolsadoManual }); };
        //    if ((ddlDesembolsado != null) && (ddlDesembolsado == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = db.SubProceso.Where(x => x.NombreSubProceso == "Desembolsado simple").Select(x => x.IdSubProceso).FirstOrDefault(), ValorVariableSubProceso = p.CostoVariableDesembolsado }); };
        //    if (ddlAlzado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlAlzado, ValorVariableSubProceso = p.CostoVariableAlzado }); };
        //    if (ddlInsercion != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlInsercion, ValorVariableSubProceso = p.CostoVariableInsercion }); };
        //    if (ddlPegado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlPegado, ValorVariableSubProceso = p.CostoVariablePegado }); };
        //    if (ddlFajado != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlFajado, ValorVariableSubProceso = p.CostoVariableFajado }); };
        //    if (ddlAdhesivo != null) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = (int)ddlAdhesivo, ValorVariableSubProceso = p.CostoVariableAdhesivoTotal }); };
        //    if ((ddlPegadoSticker != null) && (ddlPegadoSticker == 2)) { listaSubProceso.Add(new Presupuesto_SubProceso() { PresupuestoId = p.IdPresupuesto, SubProcesoId = db.SubProceso.Where(x => x.NombreSubProceso == "Pegado de Sticker").Select(x => x.IdSubProceso).FirstOrDefault(), ValorVariableSubProceso = p.CostoVariablePegadoSticker }); };

        //    if (ModelState.IsValid)
        //    {
        //        db.Catalogo.Add(tc);
        //        db.Presupuesto.Add(p);
        //        db.Presupuesto_SubProceso.AddRange(listaSubProceso);
        //        db.SaveChanges();
        //        pres2.IdPresupuesto = db.Presupuesto.Max(item => item.IdPresupuesto);
        //    }
        //    else
        //    {
        //        pres2 = p;
        //    }

        //    return Json(pres2, JsonRequestBehavior.AllowGet);
        //}

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
            var presupuesto = db.Presupuesto.Include(p => p.Formato).Include(p => p.Interior).Include(p => p.Tapa).Include(p => p.Interior.Papel).Include(p=>p.Interior.Maquina)
                                                    .Include(p => p.Tapa.Papel).Include(p => p.Tapa.Maquina).Include(p => p.Encuadernacion).Include(p => p.Moneda).Include(p => p.Usuario).Where(x => x.IdPresupuesto == id).FirstOrDefault();
            var pres = new PresupuestoForm() { NombrePresupuesto = presupuesto.NombrePresupuesto, Tiraje = presupuesto.Tiraje};
            return View(presupuesto);
        }
        [HttpGet]
        public ActionResult View2()
        {
            ViewBag.TiposCatalogos = db.Catalogo;
            ViewBag.Formatos = db.Formato.Select(x => new { NombreFormato = x.FormatoCerradoX + " x " + x.FormatoCerradoY }).Distinct().ToList();//, "IdFormato", "NombreFormato");
            ViewBag.Encuadernaciones = db.Encuadernacion;//, "IdEncuadernacion", "NombreEncuadernacion");
            ViewBag.Papeles = db.Papel.ToList();
            List<SelectListItem> s = new List<SelectListItem>();
            for (int i = 4; i <= 400; i = i + 4)
            {
                s.Add(new SelectListItem() { Text = i.ToString(), Value = i.ToString() });
            }
            ViewBag.CantidadInt = s;
            ViewBag.SubProcesoQuintoColorId = db.SubProceso.ToList();
            ViewBag.SubProcesoAcuosoId = db.SubProceso.ToList();
            ViewBag.SubProcesoLaminadoId = db.SubProceso.ToList();
            ViewBag.SubProcesoBarnizUVId = db.SubProceso.ToList();
            ViewBag.SubProcesoEmbolsadoId = db.SubProceso.ToList();
            ViewBag.SubProcesoAlzadoPlanoId = db.SubProceso.ToList();
            ViewBag.SubProcesoDesembolsadoId = db.SubProceso.ToList();
            ViewBag.SubProcesoAlzadoId = db.SubProceso.ToList();
            ViewBag.SubProcesoInsercionId = db.SubProceso.ToList();
            ViewBag.SubProcesoSachetId = db.SubProceso.ToList();
            ViewBag.SubProcesoFajadoId = db.SubProceso.ToList();
            ViewBag.SubProcesoAdhesivoId = db.SubProceso.ToList();
            ViewBag.SubProcesoStickerId = db.SubProceso.ToList();
            ViewBag.EmpresaPapelInteriorId = db.Empresa.ToList();
            ViewBag.EmpresaPapelTapaId = db.Empresa.ToList();
            return View();
        }
        [HttpPost]
        public Tuple<string, double, double> NewCalcular(PresupuestoView presView)
        {
            ViewBag.TiposCatalogos = db.Catalogo;
            ViewBag.Formatos = db.Formato;//, "IdFormato", "NombreFormato");
            ViewBag.Encuadernaciones = db.Encuadernacion;//, "IdEncuadernacion", "NombreEncuadernacion");
            return new Tuple<string, double, double>("",1,1);
        }

        [HttpPost]
        public Presupuesto View2(PresupuestoView pres)
        {
            Presupuesto detalle = new Presupuesto();
            detalle.MonedaId = db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).Select(x => x.IdMoneda).FirstOrDefault();
            detalle.Tiraje = pres.Presupuesto.Tiraje;
            #region EntradasxFormato

            int NumeroDoblez = 0;
            int CantidadPaginasInt = pres.Interior.CantidadPaginas;
            if ((CantidadPaginasInt / 64) > 0)
            {
                detalle.EntradasPag64 = (CantidadPaginasInt / 64);
                NumeroDoblez = 64;
                switch (CantidadPaginasInt - (detalle.EntradasPag64 * 64))
                {
                    case 60:
                        detalle.EntradasPag48 = 1;
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 56:
                        detalle.EntradasPag48 = 1;
                        detalle.EntradasPag8 = 1;
                        break;
                    case 52:
                        detalle.EntradasPag48 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 48:
                        detalle.EntradasPag48 = 1;
                        break;
                    case 44:
                        detalle.EntradasPag32 = 1;
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 40:
                        detalle.EntradasPag32 = 1;
                        detalle.EntradasPag8 = 1;
                        break;
                    case 36:
                        detalle.EntradasPag32 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 32:
                        detalle.EntradasPag32 = 1;
                        break;
                    case 28:
                        detalle.EntradasPag24 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 24:
                        detalle.EntradasPag24 = 1;
                        break;
                    case 20:
                        detalle.EntradasPag16 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 16:
                        detalle.EntradasPag16 = 1;
                        break;
                    case 12:
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 8:
                        detalle.EntradasPag8 = 1;
                        break;
                    case 4:
                        detalle.EntradasPag4 = 1;
                        break;
                }
            }
            else if ((CantidadPaginasInt / 48) > 0)
            {
                detalle.EntradasPag48 = (CantidadPaginasInt / 48);
                NumeroDoblez = 48;
                switch (CantidadPaginasInt - (detalle.EntradasPag48 * 48))
                {
                    case 44:
                        detalle.EntradasPag32 = 1;
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 40:
                        detalle.EntradasPag32 = 1;
                        detalle.EntradasPag8 = 1;
                        break;
                    case 36:
                        detalle.EntradasPag32 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 32:
                        detalle.EntradasPag32 = 1;
                        break;
                    case 28:
                        detalle.EntradasPag24 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 24:
                        detalle.EntradasPag24 = 1;
                        break;
                    case 20:
                        detalle.EntradasPag16 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 16:
                        detalle.EntradasPag16 = 1;
                        break;
                    case 12:
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 8:
                        detalle.EntradasPag8 = 1;
                        break;
                    case 4:
                        detalle.EntradasPag4 = 1;
                        break;
                }
            }
            else if ((CantidadPaginasInt / 32) > 0)
            {
                detalle.EntradasPag32 = (CantidadPaginasInt / 32);
                NumeroDoblez = 32;
                switch (CantidadPaginasInt - (detalle.EntradasPag32 * 32))
                {
                    case 28:
                        detalle.EntradasPag24 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 24:
                        detalle.EntradasPag24 = 1;
                        break;
                    case 20:
                        detalle.EntradasPag16 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 16:
                        detalle.EntradasPag16 = 1;
                        break;
                    case 12:
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 8:
                        detalle.EntradasPag8 = 1;
                        break;
                    case 4:
                        detalle.EntradasPag4 = 1;
                        break;
                }
            }
            else if ((CantidadPaginasInt / 24) > 0)
            {
                detalle.EntradasPag24 = (CantidadPaginasInt / 24);
                NumeroDoblez = 24;
                switch (CantidadPaginasInt - (detalle.EntradasPag24 * 24))
                {
                    case 20:
                        detalle.EntradasPag16 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 16:
                        detalle.EntradasPag16 = 1;
                        break;
                    case 12:
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 8:
                        detalle.EntradasPag8 = 1;
                        break;
                    case 4:
                        detalle.EntradasPag4 = 1;
                        break;
                }
            }
            else if ((CantidadPaginasInt / 16) > 0)
            {
                detalle.EntradasPag16 = (CantidadPaginasInt / 16);
                NumeroDoblez = 16;
                switch (CantidadPaginasInt - (detalle.EntradasPag16 * 16))
                {
                    case 12:
                        detalle.EntradasPag8 = 1;
                        detalle.EntradasPag4 = 1;
                        break;
                    case 8:
                        detalle.EntradasPag8 = 1;
                        break;
                    case 4:
                        detalle.EntradasPag4 = 1;
                        break;
                }
            }
            else if ((CantidadPaginasInt / 8) > 0)
            {
                detalle.EntradasPag8 = (CantidadPaginasInt / 8);
                NumeroDoblez = 8;
                switch (CantidadPaginasInt - (detalle.EntradasPag8 * 8))
                {
                    case 4:
                        detalle.EntradasPag4 = 1;
                        break;
                }
            }
            
            #endregion
            #region Impresion Interior Nuevo

            int doblezdelFormato = 0;
            if (NumeroDoblez <= 47)
            {
                doblezdelFormato = 16;
            }
            else if (NumeroDoblez <= 48)
            {
                doblezdelFormato = 48;
            }
            else
            {
                doblezdelFormato = 64;
            }
            string[] split = pres.FormatoId.Split('x');
            double formatox = Convert.ToDouble(split[0]);
            double formatoy = Convert.ToDouble(split[1]);
            detalle.Formato = db.Formato.Where(x => x.FormatoCerradoX == formatox && x.FormatoCerradoY == formatoy && x.EntradasxFormatos == doblezdelFormato).FirstOrDefault();
            if (detalle.Formato == null)
            {
                detalle.Formato = db.Formato.Where(x => x.FormatoCerradoX == formatox && x.FormatoCerradoY == formatoy && x.EntradasxFormatos == 16).FirstOrDefault();
            }
            Papel papelInterior = db.Papel.Where(x => x.IdPapel == pres.PapelInteriorId).FirstOrDefault();
            Papel papelTapa = ((pres.Tapas.CantidadPaginas > 0 && pres.PapelTapaId != 0)) ? db.Papel.Where(x => x.IdPapel == pres.PapelTapaId).FirstOrDefault() : null;
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











            List<Impresion> ListImpTapa = db.Impresion.Include(x => x.Maquina).Include(x => x.TipoMoneda).Include(x => x.TipoMoneda.Monedas).Where(x => x.Maquina.NombreMaquina == "Plana"// MaquinaTap 
            
            
            
            
            && x.NombreImpresion == "16").ToList();





































            detalle.CostoFijoTapa = (pres.Tapas.CantidadPaginas > 0) ? Math.Ceiling(ListImpTapa.Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.ValorFijoImpresion * pres.CantidadModelosTapas).FirstOrDefault()) : 0;
            detalle.CostoVariableTapa = (pres.Tapas.CantidadPaginas > 0) ? (Math.Ceiling(((ListImpTapa.Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.valorvariableImpresion).FirstOrDefault()) / 1000.0) * 100.0) / 100.0) / pres.Tapas.CantidadPaginas : 0;

            List<SubProceso> ListTerm = db.SubProceso.Include(x => x.TipoMoneda).Include(x => x.TipoMoneda.Monedas).ToList();
            detalle.CostoFijoQuintoColor = (pres.SubProcesoQuintoColorId != 0) ? Math.Ceiling(ListTerm.Where(x => x.IdSubProceso == pres.SubProcesoQuintoColorId).Select(x => x.CostoFijoSubProceso * pres.PasadasQuintoColor * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
            detalle.CostoVariableQuintoColor = (pres.SubProcesoQuintoColorId != 0) ? (Math.Ceiling(((ListTerm.Where(x => x.IdSubProceso == pres.SubProcesoQuintoColorId).Select(x => x.CostoVariableSubProceso * pres.PasadasQuintoColor * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) * 100.0) / 100.0) : 0;

            detalle.CostoFijoPlizado = (db.Papel.Where(x => x.IdPapel == pres.PapelTapaId).Select(x => x.Gramaje).FirstOrDefault() >= 170) ? ListTerm.Where(x => x.NombreSubProceso == "Plizado").Select(x => x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault() : 0;
            detalle.CostoVariablePlizado = (db.Papel.Where(x => x.IdPapel == pres.PapelTapaId).Select(x => x.Gramaje).FirstOrDefault() >= 170) ? (ListTerm.Where(x => x.NombreSubProceso == "Plizado").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault() / Convert.ToDouble(pres.Tapas.CantidadPaginas)) : 0;

            #endregion
            #region Encuadernacion Nuevo
            detalle.CostoFijoEncuadernacion = (pres.EncuadernacionId != 0) ? Math.Ceiling(db.Encuadernacion.Where(x => x.IdEncuadernacion == pres.EncuadernacionId).Select(x => x.ValorFijo * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(y => y.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
            detalle.CostoVariableEncuadernacion = (pres.EncuadernacionId != 0) ? (Math.Ceiling(((db.Encuadernacion.Where(x => x.IdEncuadernacion == pres.EncuadernacionId).Select(x => x.ValorVariable * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(y => y.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) * 100.0) / 100.0) : 0;

            #endregion
            #region Terminaciones Nuevas
            double dobleentrada = (pres.SubProcesoAcuosoId == 4) && (pres.Tapas.CantidadPaginas > 0) ? 2 : 1;
            detalle.CostoFijoBarnizAcuosoTapa = ((pres.Tapas.CantidadPaginas > 0) && (pres.SubProcesoAcuosoId >= 2)) ? Math.Ceiling(ListTerm.Where(x => x.NombreSubProceso == "Barniz Acuoso (solo tiro)").Select(x => x.CostoFijoSubProceso * dobleentrada * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
            detalle.CostoVariableBarnizAcuosoTapa = ((pres.Tapas.CantidadPaginas > 0) && (pres.SubProcesoAcuosoId >= 2)) ? Math.Ceiling(((ListTerm.Where(x => x.NombreSubProceso == "Barniz Acuoso (solo tiro)").Select(x => x.CostoVariableSubProceso * dobleentrada * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / 1000.0) * 100.0) / 100.0 : 0;
            detalle.CostoVariableEmbolsado = (pres.SubProcesoEmbolsadoId != 0) ? (ListTerm.Where(x => x.IdSubProceso == pres.SubProcesoEmbolsadoId).Select(x => x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariableLaminado = ((pres.SubProcesoLaminadoId != 0) && (pres.Tapas.CantidadPaginas > 0)) ? (ListTerm.Where(x => x.IdSubProceso == pres.SubProcesoLaminadoId).Select(x => (x.CostoVariableSubProceso * (detalle.Formato.TapaDiptica_Alto * detalle.Formato.TapaDiptica_Ancho) / 10000.0)).FirstOrDefault()) / Convert.ToDouble(pres.Tapas.CantidadPaginas) : 0;
            detalle.CostoFijoBarnizUV = (pres.SubProcesoBarnizUVId != 0) ? Math.Ceiling(ListTerm.Where(x => x.IdSubProceso == pres.SubProcesoBarnizUVId).Select(x => x.CostoFijoSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariableBarnizUV = (pres.SubProcesoBarnizUVId != 0) ? Math.Ceiling(ListTerm.Where(x => x.IdSubProceso == pres.SubProcesoBarnizUVId).Select(x => x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoFijoTroquel = (db.Papel.Where(x => x.IdPapel == pres.PapelTapaId).Select(x => x.Gramaje).FirstOrDefault() >= 300) ? (ListTerm.Where(x => x.NombreSubProceso == "Troquel").Select(x => x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
            detalle.CostoVariableTroquel = (db.Papel.Where(x => x.IdPapel == pres.PapelTapaId).Select(x => x.Gramaje).FirstOrDefault() >= 300) ? ((ListTerm.Where(x => x.NombreSubProceso == "Troquel").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) / Convert.ToDouble(pres.Tapas.CantidadPaginas)) : 0;
            detalle.CostoFijoCorteFrontal = (pres.Tapas.CantidadPaginas > 0) ? (ListTerm.Where(x => x.NombreSubProceso == "Corte Frontal").Select(x => x.CostoFijoSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;
            detalle.CostoVariableCorteFrontal = (pres.Tapas.CantidadPaginas > 0) ? (ListTerm.Where(x => x.NombreSubProceso == "Corte Frontal").Select(x => x.CostoVariableSubProceso * x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault()).FirstOrDefault()) : 0;

            #endregion
            #region Manualidades Pendiente
            detalle.CostoVariableAlzadoPlano = (pres.SubProcesoAlzadoPlanoId  == 2) ? (ListTerm.Where(x => x.NombreSubProceso == "Alzado Plano").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            //detalle.CostoVariableEmbolsadoManual = (EmbolsadoManual != null && EmbolsadoManual == 2) ? (ListTerm.Where(x => x.NombreSubProceso == "Embolsado manual").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;

            detalle.CostoVariableDesembolsado = (pres.SubProcesoDesembolsadoId  == 2) ? (ListTerm.Where(x => x.NombreSubProceso == "Desembolsado simple").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariableAlzado = (pres.SubProcesoAlzadoId != 0) ? (ListTerm.Where(x => x.IdSubProceso == pres.SubProcesoAlzadoId).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariableInsercion = (pres.SubProcesoInsercionId != 0) ? (ListTerm.Where(x => x.IdSubProceso == pres.SubProcesoInsercionId).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariablePegado = (pres.SubProcesoSachetId != 0) ? (ListTerm.Where(x => x.IdSubProceso == pres.SubProcesoSachetId).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariableFajado = (pres.SubProcesoFajadoId != 0) ? (ListTerm.Where(x => x.IdSubProceso == pres.SubProcesoFajadoId).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.CostoVariableAdhesivo = (pres.SubProcesoAdhesivoId != 0) ? (ListTerm.Where(x => x.IdSubProceso == pres.SubProcesoAdhesivoId).Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            detalle.AdhesivoCms = Convert.ToDouble(4);
            detalle.CostoVariableAdhesivoTotal = Math.Ceiling((detalle.AdhesivoCms * detalle.CostoVariableAdhesivo) * 10.0) / 10.0;
            #region Embalaje
            Embalaje emb = db.Embalaje.Where(x => x.Estado == true).FirstOrDefault();
            Papel p = db.Papel.Where(x => x.IdPapel == pres.PapelInteriorId).FirstOrDefault();
            double LomoInterior = Math.Ceiling((((p.Micron * Convert.ToDouble(CantidadPaginasInt)) / 10000.0) + p.Adhesivo) * 10.0) / 10.0;
            double LomoTapa = db.Papel.Where(x => x.IdPapel == pres.PapelTapaId).Select(x => (x.Micron / 10000.0)).FirstOrDefault();
            double LomocTapa = (Math.Ceiling((LomoInterior + (2 * LomoTapa)) * 10)) / 10;
            double librosxCajas = Math.Floor(emb.AltoCaja / LomocTapa) * emb.Base;
            double CantidadDeCajas = Math.Ceiling(Convert.ToDouble(detalle.Tiraje) / librosxCajas);
            detalle.CostoVariableSuministroCaja = //(Math.Ceiling((
                (emb.CajaEstandar * CantidadDeCajas);// / Convert.ToDouble(Tiraje)) * 100)) / 100;
            detalle.CostoVariableInsercionCajaySellado = //(Math.Ceiling((
                (emb.EncajadoxCaja * CantidadDeCajas);// / Convert.ToDouble(Tiraje))*100))/100;

            detalle.CostoVariableEnzunchado = 0;//(Math.Ceiling((
                                                //(emb.Enzunchado * (Tiraje - CantidadCajas)); /// Convert.ToDouble(Enzunchadoxpqte)) * 100)) / 100;


            #endregion
            detalle.CostoVariablePegadoSticker = (pres.SubProcesoStickerId == 2) ? (ListTerm.Where(x => x.NombreSubProceso == "Pegado de Sticker").Select(x => x.TipoMoneda.Monedas.Where(i => i.Estado == true).Select(i => i.Valor).FirstOrDefault() * x.CostoVariableSubProceso).FirstOrDefault()) : 0;
            #endregion
            #region Papel
            if ((CantidadPaginasInt > 0) && (pres.PapelInteriorId != 0))
            {
                detalle.Interior = new Interior();
                detalle.Interior.CantidadPaginas = CantidadPaginasInt;
                detalle.Interior.PapelId = pres.PapelInteriorId;

                detalle.Interior.Entradas = (float)((((detalle.EntradasPag64 + detalle.EntradasPag48 + detalle.EntradasPag32 + detalle.EntradasPag24 + detalle.EntradasPag16 + detalle.EntradasPag12 + detalle.EntradasPag8 + detalle.EntradasPag4) * detalle.MaquinaInterior.MermaFija) / 1000.0)
                                            * ((papelInterior.Gramaje * detalle.Formato.Interior_Alto * detalle.Formato.Interior_Ancho) / 10000.0));

                double CostoPapelinteriorFijo = Math.Ceiling(detalle.Interior.Entradas * papelInterior.PrecioKilos);

                detalle.Interior.Tiradas = (float)(((detalle.Tiraje * (Convert.ToDouble(CantidadPaginasInt) / Convert.ToDouble(NumeroDoblez)) * detalle.MaquinaInterior.MermaVariable) / 1000.0) * ((papelInterior.Gramaje * detalle.Formato.Interior_Alto * detalle.Formato.Interior_Ancho) / 10000.0));

                double CostoPapelInteriorVari = Math.Ceiling(papelInterior.PrecioKilos * detalle.Interior.Tiradas);

                detalle.Interior.KilosPapel = (float)(((detalle.Tiraje * (Convert.ToDouble(CantidadPaginasInt) / Convert.ToDouble(NumeroDoblez)) * detalle.MaquinaInterior.MermaVariable) + ((detalle.EntradasPag64 + detalle.EntradasPag48 + detalle.EntradasPag32 + detalle.EntradasPag24 + detalle.EntradasPag16 + detalle.EntradasPag12 + detalle.EntradasPag8 + detalle.EntradasPag4) * detalle.MaquinaInterior.MermaFija))
                                            * ((papelInterior.Gramaje * detalle.Formato.Interior_Alto * detalle.Formato.Interior_Ancho) / 10000.0));
                if ((pres.Tapas.CantidadPaginas > 0 && pres.PapelTapaId != 0))
                {
                    detalle.Tapa = new Tapa();
                    detalle.Tapa.CantidadPaginas = (pres.Tapas.CantidadPaginas > 0) ? Convert.ToInt32(pres.Tapas.CantidadPaginas) : 0;
                    detalle.Tapa.PapelId = pres.PapelTapaId;

                    detalle.Tapa.Entradas = (float)(((1 * detalle.MaquinaTapa.MermaFija) / 1000.0) * ((papelTapa.Gramaje * detalle.Formato.TapaDiptica_Alto * detalle.Formato.TapaDiptica_Ancho) / 10000.0));

                    double CostoPapelTapaFijo = Math.Ceiling(detalle.Tapa.Entradas * papelTapa.PrecioKilos);

                    detalle.Tapa.Tiradas = (float)(((detalle.Tiraje * pres.Tapas.CantidadPaginas * detalle.MaquinaTapa.MermaVariable) / 1000.0) * ((papelTapa.Gramaje * detalle.Formato.TapaDiptica_Alto * detalle.Formato.TapaDiptica_Ancho) / 10000.0));

                    double CostoPapelTapaVari = Math.Ceiling(papelTapa.PrecioKilos * detalle.Tapa.Tiradas);

                    detalle.Tapa.KilosPapel = (float)((((detalle.Tiraje * (1.0 / pres.Tapas.CantidadPaginas) * detalle.MaquinaTapa.MermaVariable) + (1 * detalle.MaquinaTapa.MermaFija)) / 1000.0) * ((papelTapa.Gramaje * detalle.Formato.TapaDiptica_Alto * detalle.Formato.TapaDiptica_Ancho) / 10000.0));
                };
                float PesoInterior = (float)((((detalle.Formato.FormatoCerradoX / 10.0) * (detalle.Formato.FormatoCerradoY / 10.0) * papelInterior.Gramaje) / 10000000.0) * (CantidadPaginasInt / 2));
                float Pesotapa = (float)((((detalle.Formato.FormatoCerradoX / 10.0) * (detalle.Formato.FormatoCerradoY / 10.0) * ((papelTapa != null) ? papelTapa.Gramaje : 0)) / 10000000.0) * (4 / 2));
                float Enc = 0.002f;

                detalle.CostoVariablePallet = (emb.PalletEstandar * (int)Math.Ceiling(((PesoInterior + Pesotapa + Enc) * detalle.Tiraje) / 700));//CantidadPallet);
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
            detalle.TarifaVariableEncuadernacion = (Math.Ceiling((detalle.CostoVariableEncuadernacion + detalle.CostoVariablePlizado + detalle.CostoVariableTroquel + detalle.CostoVariableCorteFrontal +
                            detalle.CostoVariableSuministroCaja + detalle.CostoVariableInsercionCajaySellado + detalle.CostoVariableEnzunchado + detalle.CostoVariablePallet) * 100.0)) / 100.0;

            detalle.TarifaFijaTerminacion = Math.Ceiling(detalle.CostoFijoQuintoColor + detalle.CostoFijoBarnizUV + detalle.CostoFijoBarnizAcuosoTapa);

            detalle.TarifaVariableTerminacion = (Math.Ceiling((detalle.CostoVariableQuintoColor +
                             detalle.CostoVariableBarnizUV + detalle.CostoVariableEmbolsado + detalle.CostoVariableLaminado + detalle.CostoVariableBarnizAcuosoTapa +
                             detalle.CostoVariableAlzadoPlano + detalle.CostoVariableEmbolsadoManual + detalle.CostoVariableDesembolsado + detalle.CostoVariableAlzado +
                             detalle.CostoVariableInsercion + detalle.CostoVariablePegado + detalle.CostoVariableFajado + detalle.CostoVariableAdhesivoTotal + detalle.CostoVariablePegadoSticker) * 100.0)) / 100.0;

            detalle.TotalNetoFijo = Math.Ceiling(detalle.TarifaFijaImpresion + detalle.TarifaFijaEncuadernacion + detalle.TarifaFijaTerminacion + detalle.TarifaFijaPapel);
            detalle.TotalNetoVari = (Math.Ceiling((detalle.TarifaVariableImpresion + detalle.TarifaVariableEncuadernacion + detalle.TarifaVariableTerminacion + detalle.TarifaVariablePapel) * 100.0)) / 100.0;

            detalle.TotalNetoTotal = Math.Ceiling(detalle.TotalNetoFijo + ((detalle.TotalNetoVari - (detalle.CostoVariableSuministroCaja + detalle.CostoVariableInsercionCajaySellado + detalle.CostoVariableEnzunchado + detalle.CostoVariablePallet))
                                        * Convert.ToDouble(pres.Presupuesto.Tiraje)) + (detalle.CostoVariableSuministroCaja + detalle.CostoVariableInsercionCajaySellado + detalle.CostoVariableEnzunchado + detalle.CostoVariablePallet));
            detalle.PrecioUnitario = (Math.Ceiling((detalle.TotalNetoTotal / Convert.ToDouble(pres.Presupuesto.Tiraje)) * 100) / 100);

            #endregion
            return detalle;
        }
    }
}
