using ProyectoPersonal.Models.Cotizador;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.ProductosEspeciales
{
    public class ProductosEspeciales_Impresion
    {
        [Key]
        public int ImpresionId { get; set; }
        public int PaginasPorEntrada { get; set; }
        public double ValorFijo { get; set; }
        public double ValorVariable { get; set; }
        public double MermaFija { get; set; }
        public double MermaVariable { get; set; }
        public string Moneda { get; set; }
        public string Descripcion { get; set; }

        public Maquina Maquina { get; set; }
    }
}