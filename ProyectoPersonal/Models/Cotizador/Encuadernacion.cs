using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Encuadernacion
    {
        [Key]
        public int IdEncuadernacion { get; set; }
        public string NombreEncuadernacion { get; set; }
        public double ValorFijo { get; set; }
        public double ValorVariable { get; set; }
        public List<Presupuesto> Presupuestos { get; set; }
        public int? TipoMonedaId { get; set; }
        public TipoMoneda TipoMoneda { get; set; }
        public string DescripcionEnc { get; set; }
    }
}