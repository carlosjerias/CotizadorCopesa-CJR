using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SolicitudCotizador.Models
{
    public class Cliente
    {
        [Key]
        public int IdCliente { get; set; }
        public string RutCliente { get; set; }
        public int CodCliente { get; set; }
        public string NombreCliente { get; set; }
        public string ContactoCliente { get; set; }
        public string EmailCliente { get; set; }
        public string Telefono { get; set; }
        public List<Solicitud> Solicitudes { get; set; }
    }
}