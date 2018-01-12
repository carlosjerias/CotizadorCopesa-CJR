
using ProyectoPersonal.Models.Cotizador;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Gerencia
{
    public class Empresa
    {
        [Key]
        public int IdEmpresa { get; set; }
        [Display(Name ="Nombre")]
        public string NombreEmpresa { get; set; }
        [Display(Name = "Rut")]
        public int RutEmpresa { get; set; }
        [Display(Name = "Digito")]
        public int DgtVerificadorEmpresa { get; set; }
        [Display(Name = "Estado")]
        public int EstadoEmpresa { get; set; }
        
        public virtual ICollection<ApplicationUser> Usuarios { get; set; }
        public List<Papel> Papeles { get; set; }
    }
}