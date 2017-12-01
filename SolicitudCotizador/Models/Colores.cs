using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SolicitudCotizador.Models
{
    public class Colores
    {
        [Key]
        public int IdColores { get; set; }
        public int NumeroColor { get; set; }
        public int? ProcesoId { get; set; }
        public Proceso Proceso { get; set; }
    }
}