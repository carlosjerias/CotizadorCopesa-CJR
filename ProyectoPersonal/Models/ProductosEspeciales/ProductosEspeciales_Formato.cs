using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.ProductosEspeciales
{
    public class ProductosEspeciales_Formato
    {
        [Key]
        public int IdFormato { get; set; }
        public double FormatoCerradoX { get; set; }
        public double FormatoCerradoY { get; set; }
        public double FormatoExtendidoX { get; set; }
        public double FormatoExtendidoY { get; set; }
        public string NombreFormato { get; set; }

    }
}