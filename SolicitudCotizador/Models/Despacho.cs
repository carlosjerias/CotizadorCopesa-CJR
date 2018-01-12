using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SolicitudCotizador.Models
{
    public class Despacho
    {
        [Key]
        public int DespachoId { get; set; }
        public string NombreDespacho { get; set; }
        public List<Solicitud> Solicitudes { get; set; }
    }
}