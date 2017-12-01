using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Proceso
    {
        [Key]
        public int IdProceso { get; set; }
        public string NombreProceso { get; set; }
        public int? TipoProcesoId { get; set; }
        public TipoProceso TipoProceso { get; set; }
        public List<SubProceso> SubProcesos { get; set; }
    }
}