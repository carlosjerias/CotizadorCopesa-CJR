using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SolicitudCotizador.Models
{
    public class Formato
    {
        [Key]
        public int IdFormato { get; set; }
        public double FormatoCerradoX { get; set; }
        public double FormatoCerradoY { get; set; }
        public double FormatoExtendidoX { get; set; }
        public double FormatoExtendidoY { get; set; }
        [NotMapped]
        [ScaffoldColumn(false)]
        public string NombreFormato { get { return FormatoCerradoX + " x " + FormatoCerradoY; } }
        [NotMapped]
        [ScaffoldColumn(false)]
        public string FormatoExtendido { get { return (Convert.ToInt32(FormatoCerradoX) * 2).ToString() + " x " + FormatoCerradoY; } }
        public List<Solicitud> Solicitudes { get; set; }
    }
}