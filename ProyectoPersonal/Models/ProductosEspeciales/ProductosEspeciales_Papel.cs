using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.ProductosEspeciales
{
    public class ProductosEspeciales_Papel
    {
        [Key]
        public int IdPapel { get; set; }
        public string Nombre { get; set; }
        public int Gramaje { get; set; }
        public int Ancho { get; set; }
        public int Largo { get; set; }
        public string Descripcion { get; set; }
        public double PrecioKilos { get; set; }
    }
}