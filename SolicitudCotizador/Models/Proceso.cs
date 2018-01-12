using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SolicitudCotizador.Models
{
    public class Proceso
    {
        [Key]
        public int IdProceso { get; set; }
        public string NombreProceso { get; set; }
        public int CantidadProceso { get; set; }
        public string Papel { get; set; }
        public int Gramaje { get; set; }
        
        public List<Colores> Coloress { get; set; }
        [DataType(DataType.MultilineText)]
        public string Observacion { get; set; }
        public int? SolicitudId { get; set; }
        public Solicitud Solicitud { get; set; }
    }
}