using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Maquina
    {
        [Key]
        public int IdMaquina { get; set; }
        public string NombreMaquina { get; set; }
        public string TipoMaquina { get; set; }
        public int MermaFija { get; set; }
        public double MermaVariable { get; set; }
        public double Desarrollo { get; set; }
        public List<Interior> Interiores { get; set; }
        public List<Tapa> Tapas { get; set; }
        public List<Impresion> Impresiones { get; set; }
    }
}