using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SolicitudCotizador.Models
{
    public class Tiraje
    {
        [Key]
        public int IdTiraje { get; set; }
        public int Cantidad { get; set; }
        public int SolicitudId { get; set; }
        public Solicitud Solicitud { get; set; }
    }
}