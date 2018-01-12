using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Moneda
    {
        [Key]
        public int IdMoneda { get; set; }
        [Required]
        public int? TipoMonedaId { get; set; }
        public TipoMoneda TipoMoneda { get; set; }
        [Required]
        [DisplayFormat(DataFormatString = "{0:#,0.00}")]
        public double Valor { get; set; }
        [Required]
        public DateTime FechaValor { get; set; }

        public List<Presupuesto> Presupuestos { get; set; }
        [Required]
        public bool Estado { get; set; }
    }
}