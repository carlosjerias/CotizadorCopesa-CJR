using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Produccion
    {
        [Key]
        public int IdProduccion { get; set; }
        public int Paginas { get; set; }
        public double Litho132cms { get; set; }
        public double Litho174cms { get; set; }
        public double Web88cms { get; set; }
        public int Impresion64 { get; set; }
        public int Impresion48 { get; set; }
        public int Impresion32 { get; set; }
        public int Impresion24 { get; set; }
        public int Impresion16 { get; set; }
        public int Impresion08 { get; set; }
        public int Impresion04 { get; set; }
        public int Entradas64 { get; set; }
        public int Entradas48 { get; set; }
        public int Entradas16 { get; set; }

    }
}