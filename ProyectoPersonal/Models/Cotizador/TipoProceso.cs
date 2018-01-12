using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class TipoProceso
    {
        [Key]
        public int IdTipoProceso { get; set; }
        public string NombreTipoProceso { get; set; }
        public List<Proceso> Procesos { get; set; }
    }
}