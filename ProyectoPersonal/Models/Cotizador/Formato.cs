using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Formato
    {
        [Key]
        public int IdFormato { get; set; }
        public double FormatoCerradoX { get; set; }
        public double FormatoCerradoY { get; set; }
        public double FormatoExtendidoX { get; set; }
        public double FormatoExtendidoY { get; set; }
        public int EntradasxFormatos { get; set; }
        public double Interior_Ancho { get; set; }
        public double Interior_Alto { get; set; }
        public double TapaDiptica_Ancho { get; set; }
        public double TapaDiptica_Alto { get; set; }
        public double TapaTriptica_Ancho_ { get; set; }
        public double TapaTriptica_Alto { get; set; }
        [NotMapped]
        public string NombreFormato { get { return FormatoCerradoX + " x " + FormatoCerradoY; } }
        [NotMapped]
        [ScaffoldColumn(false)]
        public string FormatoExtendido { get { return (Convert.ToInt32(FormatoCerradoX)*2).ToString() + " x " + FormatoCerradoY; } }
        public int? DoblezId { get; set; }
        public Doblez Doblez { get; set; }
        public List<Presupuesto> Presupuestos { get; set; }
    }
}