using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class UnidadMedida
    {
        [Key]
        public int IdUnidadMedida { get; set; }
        public string NombreUnidadMedida { get; set; }
        public string SiglaUnidad { get; set; }
        public List<SubProceso> SubProcesos { get; set; }
    }
}