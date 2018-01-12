using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Gerencia
{
    public class Departamento
    {
        [Key]
        public int IdDepartamento { get; set; }
        public string NombreDepartamento { get; set; }
        public virtual ICollection<ApplicationUser> Usuarios { get; set; }
    }
}