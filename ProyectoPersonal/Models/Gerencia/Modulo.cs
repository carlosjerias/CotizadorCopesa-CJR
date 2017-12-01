using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Gerencia
{
    public class Modulo
    {
        [Key]
        public int IdModulo { get; set; }
        public int IdSeccion { get; set; }
        [StringLength(150)]
        public string NombreModulo { get; set; }
        public string ControladorModulo { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
    }
}