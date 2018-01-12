using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SolicitudCotizador.Models
{
    public class Solicitud
    {
        [Key]
        public int IdSolicitud { get; set; }

        [Display(Name = "Nombre")]
        public string NombreProducto { get; set; }
        public int FormatoId { get; set; }
        public Formato Formato { get; set; }
        public int EncuadernacionId { get; set; }
        public Encuadernacion Encuadernacion { get; set; }
        public List<Tiraje> Tirajes { get; set; }
        public DateTime FechaProduccion { get; set; }
        public string Embalaje { get; set; }
        public int DespachoId { get; set; }
        public Despacho Despacho { get; set; }
        public int CantidadPaginasTotales { get; set; }
        public string ColoresTotales { get; set; }
        
        public List<Proceso> Procesos { get; set; }
        [NotMapped]
        public int Tiraje1 { get; set; }
        [NotMapped]
        public int Tiraje2 { get; set; }
        [NotMapped]
        public int Tiraje3 { get; set; }
        [NotMapped]
        public string FormatoCerradoX { get; set; }
        [NotMapped]
        public string FormatoCerradoY { get; set; }
        [NotMapped]
        public string FormatoAbiertoX { get; set; }
        [NotMapped]
        public string FormatoAbiertoY { get; set; }
        [NotMapped]
        [Display(Name = "N° Paginas Interior")]
        public int CantidadInterior { get; set; }
        [NotMapped]
        public string Papel { get; set; }
        [NotMapped]
        public string Gramaje { get; set; }
        [NotMapped]
        public List<Colores> Colores { get; set; }
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        [NotMapped]
        [Display(Name ="Nombre Cliente")]
        public string NombreCliente { get; set; }
        [NotMapped]
        [Display(Name ="Rut")]
        public int RutCliente { get; set; }
        [NotMapped]
        public int RutDigitoVerificador { get; set; }
        [NotMapped]
        public string Contacto { get; set; }
        [NotMapped]
        public string Correo { get; set; }
        [NotMapped]
        public string Telefono { get; set; }
        [NotMapped]
        [Display(Name = "Fecha Producción")]
        public string FechaProduccionSemana { get; set; }
    }
}