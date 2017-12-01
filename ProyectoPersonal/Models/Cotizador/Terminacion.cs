using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Terminacion
    {
        [Key]
        public int IdTerminacion { get; set; }
        public string Nombreterminacion { get; set; }
        public int CostoFijoTerminacion { get; set; }
        public int CostoVariableTerminacion { get; set; }
        public int? InteriorId { get; set; }
        public Interior Interior { get; set; }
        public int? TapaId { get; set; }
        public Tapa Tapa { get; set; }
    }
}