using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SolicitudCotizador.Models
{
    public class Encuadernacion
    {
        [Key]
        public int IdEncuadernacion { get; set; }
        public string NombreEncuadernacion { get; set; }
        public List<Solicitud> Solicitudes { get; set; }
    }
}