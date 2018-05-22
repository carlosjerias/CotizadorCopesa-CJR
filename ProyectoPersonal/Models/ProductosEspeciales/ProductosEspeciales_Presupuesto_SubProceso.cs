using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.ProductosEspeciales
{
    public class ProductosEspeciales_Presupuesto_SubProceso
    {
        [Key]
        public int IdProductoEspecial_SubProceso { get; set; }

        public int PresupuestoId { get; set; }
        [ForeignKey("PresupuestoId")]
        public virtual ProductosEspeciales_Presupuesto Presupuesto { get; set; }

        public int SubProcesoId { get; set; }
        [ForeignKey("SubProcesoId")]
        public virtual ProductosEspeciales_SubProceso SubProceso { get; set; }

        public double ValorFijo { get; set; }
        public double ValorVariable { get; set; }
        public int CantidadEjemplares { get; set; }


    }
}