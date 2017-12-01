using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Doblez
    {
        [Key]
        public int IdDoblez { get; set; }
        public string NombreDoblez { get; set; }
        public int NroDoblez { get; set; }
        public List<Formato> Formatos { get; set; }
        public List<SubProceso> SubProcesos { get; set; }
    }
}