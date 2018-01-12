using ProyectoPersonal.Models.Gerencia;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Papel
    {
        [Key]
        public int IdPapel { get; set; }
        [Display(Name ="Papel")]
        public string NombrePapel { get; set; }
        public int Gramaje { get; set; }
        
        public List<Interior> Interiores { get; set; }
        public List<Tapa> Tapas { get; set; }
        public double Micron { get; set; }
        public double Adhesivo { get; set; }
        public double PrecioKilos { get; set; }
        [NotMapped]
        [ScaffoldColumn(false)]
        public int Paginas { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; }
        [NotMapped]
        public string NombreCompletoPapel { get { return NombrePapel + " " + Gramaje; } }
    }
}