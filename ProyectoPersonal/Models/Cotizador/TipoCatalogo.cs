using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class TipoCatalogo
    {
        [Key]
        public int IdTipoCatalogo { get; set; }
        public string NombreTipoCatalogo { get; set; }
        public List<Presupuesto> Presupuestos { get; set; }
        public string FormatoSeleccionado { get; set; }
        public string DimensionesCajasStandar { get; set; }
        public string DimensionesCajasSachet { get; set; }
        public string PapelInterior { get; set; }
        public string PapelTapa { get; set; }
    }
}