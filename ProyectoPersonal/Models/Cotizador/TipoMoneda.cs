using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class TipoMoneda
    {
        [Key]
        public int IdTipoMoneda { get; set; }
        public string NombreTipoMoneda { get; set; }
        public string SiglasTipoMoneda { get; set; }
        public List<Moneda> Monedas { get; set; }
        public List<Encuadernacion> Encuadernaciones { get; set; }
        public List<Impresion> Impresiones { get; set; }
        public List<SubProceso> SubProcesos { get; set; }
    }
}