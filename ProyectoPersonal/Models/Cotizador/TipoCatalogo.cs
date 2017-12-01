using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class TipoCatalogo
    {
        [Key]
        public int IdTipoCatalogo { get; set; }
        public string NombreTipoCatalogo { get; set; }
        public List<Presupuesto> Presupuestos { get; set; }
    }
}