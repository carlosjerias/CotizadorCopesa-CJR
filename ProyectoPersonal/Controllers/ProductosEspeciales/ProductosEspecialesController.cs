﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ProyectoPersonal.Models;
using ProyectoPersonal.Models.ProductosEspeciales;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace ProyectoPersonal.Controllers.ProductosEspeciales
{
    public class ProductosEspecialesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();



        public ActionResult Create()
        {
            ProductosEspeciales_PresupuestoForm p = new ProductosEspeciales_PresupuestoForm();
            p.Formatos = db.ProductosEspeciales_Formato.ToList();
            ViewBag.Formato = new SelectList(db.ProductosEspeciales_Formato, "IdFormato", "NombreFormato");
            ViewBag.ValorUF = string.Format("{0:#,0.00}", db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).Select(x => x.Valor).FirstOrDefault());
            ViewBag.Papeles = new SelectList(db.ProductosEspeciales_Papel, "IdPapel", "Descripcion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,SuperUser,User")]
        public JsonResult Calcular(string Maquina,string NombrePresupuesto,int Formato, int Cantidad,int ElementosEnTamaño,int Papel,string QuintoColorPMS,string QuintoColorFluor,
            string QuintoColorMetalico,string BarnizAcuoso,string BarnizUVBrillante,string BarnizUVBrillanteSelectivo,string BarnizGlitterSelectivo,string PolitermolaminadoBrillante,
            string PolitermolaminadoOpaco,string Plisado,string Troquel,string Corte,string DoblezDiptico,string Mecanizado,string Despacho, string ButtonType)
        {
            ProductosEspeciales_Presupuesto pres = new ProductosEspeciales_Presupuesto();
            double Entradas = 1.0;
            double gramaje = db.ProductosEspeciales_Papel.Where(x => x.IdPapel == Papel).Select(x => x.Gramaje).FirstOrDefault();
            double Ancho = db.ProductosEspeciales_Papel.Where(x => x.IdPapel == Papel).Select(x => x.Ancho).FirstOrDefault();
            double Largo = db.ProductosEspeciales_Papel.Where(x => x.IdPapel == Papel).Select(x => x.Largo).FirstOrDefault();
            double CostoPapel= db.ProductosEspeciales_Papel.Where(x => x.IdPapel == Papel).Select(x => x.PrecioKilos).FirstOrDefault();
            double ValorUF = db.Moneda.Where(x => x.Estado == true && x.TipoMonedaId == 2).Select(x => x.Valor).FirstOrDefault();

            double MermaVariable = db.ProductosEspeciales_Impresion.Where(x => x.PaginasPorEntrada == 16 && x.Maquina.IdMaquina == 2).Select(x => x.MermaVariable).FirstOrDefault();
            double MermaFija = db.ProductosEspeciales_Impresion.Where(x => x.PaginasPorEntrada == 16 && x.Maquina.IdMaquina == 2).Select(x => x.MermaFija).FirstOrDefault();
            double ImpresionValorFijo= db.ProductosEspeciales_Impresion.Where(x => x.PaginasPorEntrada == 16 && x.Maquina.IdMaquina == 2).Select(x => x.ValorFijo).FirstOrDefault();
            double ImpresionValorVariable = db.ProductosEspeciales_Impresion.Where(x => x.PaginasPorEntrada == 16 && x.Maquina.IdMaquina == 2).Select(x => x.ValorVariable).FirstOrDefault();

            double KilosPapel = (((((double)Cantidad / (double)ElementosEnTamaño) * MermaVariable) + MermaFija) * ((gramaje * Ancho * Largo) / 10000000.0));

            double CostoFijoImpresion = ((ImpresionValorFijo * ValorUF));
            double CostoVariableImpresion = (((ImpresionValorVariable * ValorUF) / 1000) / (double)ElementosEnTamaño);

            double CostoFijoPapel = ((((gramaje * Ancho * Largo) / 10000.0) * (MermaFija / 1000.0)) * CostoPapel);
            double CostoVariablePapel = (((((gramaje * Ancho * Largo) / 10000.0) * ((((double)Cantidad / (double)ElementosEnTamaño) * MermaVariable) / 1000.0)) * CostoPapel) / (double)Cantidad);

            double CostoFijo5PMS = 0; double CostoVariable5PMS = 0;
            if (QuintoColorPMS != "")
            {
                double Fijo5PMS= db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 2).Select(x => x.CostoFijo).FirstOrDefault();
                double vari5PMS = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 2).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijo5PMS = (Fijo5PMS * ValorUF);
                CostoVariable5PMS = (((vari5PMS * ValorUF) / 1000) / (double)ElementosEnTamaño);
            }

            double CostoFijoFluor = 0; double CostoVariableFluor = 0;
            if (QuintoColorFluor != "")
            {
                double Fijo5Fluor = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 3).Select(x => x.CostoFijo).FirstOrDefault();
                double vari5Fluor = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 3).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoFluor = (Fijo5Fluor * ValorUF);
                CostoVariableFluor = (((vari5Fluor * ValorUF) / 1000) / (double)ElementosEnTamaño);
            }

            double CostoFijoMetal = 0; double CostoVariableMetal = 0;
            if (QuintoColorMetalico != "")
            {
                double Fijo5Metal = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 4).Select(x => x.CostoFijo).FirstOrDefault();
                double vari5Metal = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 4).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoMetal = (Fijo5Metal * ValorUF);
                CostoVariableMetal = (((vari5Metal * ValorUF) / 1000) / (double)ElementosEnTamaño);
            }
            
             double CostoFijoBarnizAcuoso = 0; double CostoVariableBarnizAcuoso = 0;
            if (BarnizAcuoso != "")
            {
                double FijoBarnizAcuoso = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 5).Select(x => x.CostoFijo).FirstOrDefault();
                double variBarnizAcuoso = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 5).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoBarnizAcuoso = (FijoBarnizAcuoso * ValorUF);
                CostoVariableBarnizAcuoso = (((variBarnizAcuoso * ValorUF) / 1000) / (double)ElementosEnTamaño);
            }

            //INICIO PROCESOS DE TERMINACION
            double CostoFijoBarnizUVBrillante = 0;double CostoVariableBarnizUVBrillante = 0;
            if (BarnizUVBrillante != "")
            {
                double FijoBarnizUVBrillante = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 6).Select(x => x.CostoFijo).FirstOrDefault();
                double variBarnizUVBrillante = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 6).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoBarnizUVBrillante = FijoBarnizUVBrillante;
                CostoVariableBarnizUVBrillante = (variBarnizUVBrillante / (double)ElementosEnTamaño);
            }
            double CostoFijoBarnizUVBrillanteSelectivo = 0; double CostoVariableBarnizUVBrillanteSelectivo = 0;
            if (BarnizUVBrillanteSelectivo != "")
            {
                double FijoBarnizUVBrillanteSelectivo = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 7).Select(x => x.CostoFijo).FirstOrDefault();
                double variBarnizUVBrillanteSelectivo = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 7).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoBarnizUVBrillanteSelectivo = FijoBarnizUVBrillanteSelectivo;
                CostoVariableBarnizUVBrillanteSelectivo = (variBarnizUVBrillanteSelectivo / (double)ElementosEnTamaño);
            }
            double CostoFijoBarnizUVGlitter = 0; double CostoVariableBarnizUVGlitter = 0;
            if (BarnizGlitterSelectivo != "")
            {
                double FijoBarnizUVGlitter = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 8).Select(x => x.CostoFijo).FirstOrDefault();
                double variBarnizUVGlitter = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 8).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoBarnizUVGlitter = FijoBarnizUVGlitter;
                CostoVariableBarnizUVGlitter = (variBarnizUVGlitter / (double)ElementosEnTamaño);
            }
            double CostoFijoPoliBrillante = 0; double CostoVariablePoliBrillante = 0;
            if (PolitermolaminadoBrillante != "")
            {
                double FijoPoliBrillante = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 9).Select(x => x.CostoFijo).FirstOrDefault();
                double variPoliBrillante = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 9).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoPoliBrillante = FijoPoliBrillante;
                CostoVariablePoliBrillante = (variPoliBrillante * ((Ancho * Largo) / 10000.0) / (double)ElementosEnTamaño);
            }
            double CostoFijoPoliOpaco = 0; double CostoVariablePoliOpaco = 0;
            if (PolitermolaminadoOpaco != "")
            {
                double FijoPoliOpaco = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 10).Select(x => x.CostoFijo).FirstOrDefault();
                double variPoliOpaco = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 10).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoPoliOpaco = FijoPoliOpaco;
                CostoVariablePoliOpaco = (variPoliOpaco * ((Ancho * Largo) / 10000.0) / (double)ElementosEnTamaño);
            }

            //INICIO PROCESOS ENCUADERNACION
            double CostoFijoPlisado = 0; double CostoVariablePlisado = 0;
            if (Plisado != "")
            {
                double FijoPlisado = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 11).Select(x => x.CostoFijo).FirstOrDefault();
                double variPlisado = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 11).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoPlisado = FijoPlisado;
                CostoVariablePlisado = (variPlisado / (double)ElementosEnTamaño);
            }
            double CostoFijoTroquel = 0; double CostoVariableTroquel = 0;
            if (Troquel != "")
            {
                double FijoTroquel = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 12).Select(x => x.CostoFijo).FirstOrDefault();
                double variTroquel = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 12).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoTroquel = FijoTroquel;
                CostoVariableTroquel = (variTroquel / (double)ElementosEnTamaño);
            }
            double CostoFijoCorte = 0; double CostoVariableCorte = 0;
            if (Corte != "")
            {
                double FijoCorte = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 13).Select(x => x.CostoFijo).FirstOrDefault();
                double variCorte = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 13).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoCorte = FijoCorte;
                CostoVariableCorte = variCorte;
            }
            double CostoFijoDoblezDiptico = 0; double CostoVariableDoblezDiptico = 0;
            if (DoblezDiptico != "")
            {
                double FijoDoblezDiptico = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 14).Select(x => x.CostoFijo).FirstOrDefault();
                double variDoblezDiptico = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 14).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoDoblezDiptico = FijoDoblezDiptico;
                CostoVariableDoblezDiptico = variDoblezDiptico;
            }
            double CostoFijoMecanizado = 0; double CostoVariableMecanizado = 0;
            if (Mecanizado != "")
            {
                double FijoMecanizado = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 15).Select(x => x.CostoFijo).FirstOrDefault();
                double variMecanizado = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 15).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoMecanizado = FijoMecanizado;
                CostoVariableMecanizado = variMecanizado;
            }
            double CostoFijoDespacho = 0; double CostoVariableDespacho = 0;
            if (Despacho != "")
            {
                double FijoDespacho = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 16).Select(x => x.CostoFijo).FirstOrDefault();
                double variDespacho = db.ProductosEspeciales_SubProceso.Where(x => x.IdSubproceso == 16).Select(x => x.CostoVariable).FirstOrDefault();
                CostoFijoDespacho = FijoDespacho;
                CostoVariableDespacho = variDespacho;
            }



            if (ButtonType == "Next")
            {
                // Do Next Here
            }
            else if (ButtonType == "Save")
            {
                //Do save here
            }
            else
            {
            }

            pres.Cantidad = Cantidad;
            pres.Entradas = (int)Entradas;
            pres.ImpresionCostoFijo = CostoFijoImpresion;
            pres.ImpresionCostoVariable = CostoVariableImpresion;

            pres.PapelCostoFijo = CostoFijoPapel;
            pres.PapelCostoVariable = CostoVariablePapel;

            pres.QuintoColorPMSCostoFijo = CostoFijo5PMS;
            pres.QuintoColorPMSCostoVariable = CostoVariable5PMS;

            pres.QuintoColorFluorCostoFijo = CostoFijoFluor;
            pres.QuintoColorFluorCostoVariable = CostoVariableFluor;

            pres.QuintoColorMetalCostoFijo = CostoFijoMetal;
            pres.QuintoColorMetalCostoVariable = CostoVariableMetal;

            pres.BarnizAcuosoCostoFijo = CostoFijoBarnizAcuoso;
            pres.BarnizAcuosoCostoVariable = CostoVariableBarnizAcuoso;

            pres.ImpresionCostoFijoTotal = (CostoFijoImpresion + CostoFijo5PMS + CostoFijoFluor + CostoFijoMetal + CostoFijoBarnizAcuoso);
            pres.ImpresionCostoVariableTotal = (CostoVariableImpresion + CostoVariable5PMS + CostoVariableFluor + CostoVariableMetal + CostoVariableBarnizAcuoso);

            //PROCESOS TERMINACION
            pres.BarnizUVBrillanteCostoFijo = CostoFijoBarnizUVBrillante;
            pres.BarnizUVBrillanteCostoVariable = CostoVariableBarnizUVBrillante;

            pres.BarnizUVBrillanteSelectivoCostoFijo = CostoFijoBarnizUVBrillanteSelectivo;
            pres.BarnizUVBrillanteSelectivoCostoVariable = CostoVariableBarnizUVBrillanteSelectivo;

            pres.BarnizGlitterCostoFijo = CostoFijoBarnizUVGlitter;
            pres.BarnizGlitterCostoVariable = CostoVariableBarnizUVGlitter;

            pres.PoliBrillanteCostoFijo = CostoFijoPoliBrillante;
            pres.PoliBrillanteCostoVariable = CostoVariablePoliBrillante;

            pres.PoliOpacoCostoFijo = CostoFijoPoliOpaco;
            pres.PoliOpacoCostoVariable = CostoVariablePoliOpaco;

            pres.TerminacionCostoFijoTotal = (CostoFijoBarnizUVBrillante + CostoFijoBarnizUVBrillanteSelectivo + CostoFijoBarnizUVGlitter + CostoFijoPoliBrillante + CostoFijoPoliOpaco);
            pres.TerminacionCostoVariableTotal = (CostoVariableBarnizUVBrillante + CostoVariableBarnizUVBrillanteSelectivo + CostoVariableBarnizUVGlitter + CostoVariablePoliBrillante + CostoVariablePoliOpaco);

            //PROCESOS ENCUADERNACION
            pres.PlisadoCostoFijo = CostoFijoPlisado;
            pres.PlisadoCostoVariable = CostoVariablePlisado;

            pres.TroquelCostoFijo = CostoFijoTroquel;
            pres.TroquelCostoVariable = CostoVariableTroquel;

            pres.CorteCostoFijo = CostoFijoCorte;
            pres.CorteCostoVariable = CostoVariableCorte;

            pres.DoblezDipticoCostoFijo = CostoFijoDoblezDiptico;
            pres.DoblezDipticoCostoVariable = CostoVariableDoblezDiptico;

            pres.MecanizadoCostoFijo = CostoFijoMecanizado;
            pres.MecanizadoCostoVariable = CostoVariableMecanizado;

            pres.EncuadernacionCostoFijoTotal = (CostoFijoPlisado + CostoFijoTroquel + CostoFijoCorte + CostoFijoDoblezDiptico + CostoFijoMecanizado);
            pres.EncuadernacionCostoVariableTotal = (CostoVariablePlisado + CostoVariableTroquel + CostoVariableCorte + CostoVariableDoblezDiptico + CostoVariableMecanizado);

            pres.DespachoCostoFijo = CostoFijoDespacho;
            pres.DespachoCostoVariable = CostoVariableDespacho;

            pres.NetoCostoFijoTotal = (pres.ImpresionCostoFijoTotal + pres.TerminacionCostoFijoTotal + pres.EncuadernacionCostoFijoTotal + pres.DespachoCostoFijo + pres.PapelCostoFijo);
            pres.NetoCostoVariableTotal = (pres.PapelCostoVariable + pres.ImpresionCostoVariableTotal + pres.TerminacionCostoVariableTotal + pres.EncuadernacionCostoVariableTotal + pres.DespachoCostoVariable);

            double totalll = (pres.NetoCostoFijoTotal + (pres.NetoCostoVariableTotal * (double)Cantidad));
            double mitad = totalll / (double)Cantidad;
            pres.PrecioUnitario = mitad;



            string jsonText = @"[{'id': '5241585099662481339'," +
    "'displayName': 'Music'," +
    "'name': 'music'," +
    "'slug': 'music'," +
    "'imageUrl': 'http://kcdn3.klout.com/static/images/music-1333561300502.png'" +
  "},  { 'id': '6953585193220490118','displayName': 'Celebrities','name': 'celebrities','slug': 'celebrities'," +
    "'imageUrl': 'http://kcdn3.klout.com/static/images/topics/celebrities_b32741b6703151cc7bd85fba24c44c52.png'" +
  "}]";

            dynamic dynJson = JsonConvert.DeserializeObject(jsonText);
            foreach (var item in dynJson)
            {
                var algo = item.id;
            }


            return Json(pres, JsonRequestBehavior.AllowGet);

 
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "IdPresupuesto,NombrePresupuesto,Formato,Cantidad,Maquina,ElementosEnTamaño,Entradas,Gramaje,Ancho,Largo,MonedaId,Usuarioid")] ProductosEspeciales_Presupuesto productosEspeciales_Presupuesto)
        {
            if (ModelState.IsValid)
            {
                db.ProductosEspeciales_Presupuesto.Add(productosEspeciales_Presupuesto);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Usuarioid = new SelectList(db.ApplicationUsers, "Id", "NombrePrimer", productosEspeciales_Presupuesto.Usuarioid);
            return View(productosEspeciales_Presupuesto);
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