using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Impresion
    {
        [Key]
        public int IdImpresion { get; set; }
        public string NombreImpresion { get; set; }
        public double ValorFijoImpresion { get; set; }
        public double valorvariableImpresion { get; set; }
        public int? MaquinaId { get; set; }
        public Maquina Maquina { get; set; }
        public int? TipoMonedaId { get; set; }
        public TipoMoneda TipoMoneda { get; set; }

    }
}