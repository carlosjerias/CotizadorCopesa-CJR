using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Presupuesto_SubProceso
    {
        [Key]
        public int IdPresupuestoSubProceso { get; set; }
        
        public int PresupuestoId { get; set; }
        [ForeignKey("PresupuestoId")]
        public virtual Presupuesto Presupuesto { get; set; }
        public int SubProcesoId { get; set; }
        [ForeignKey("SubProcesoId")]
        public virtual SubProceso SubProceso { get; set; }

        public double ValorFijoSubProceso { get; set; }
        public double ValorVariableSubProceso { get; set; }
        public double ValorTotalSubProceso { get; set; }
    }
}