using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class SubProceso
    {
        [Key]
        public int IdSubProceso { get; set; }
        [Required(ErrorMessage ="El campo {0} es obligatorio")]
        [Display(Name ="Nombre")]
        public string NombreSubProceso { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Costo Fijo")]
        public double CostoFijoSubProceso { get; set; }
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Costo Variable")]
        public double CostoVariableSubProceso { get; set; }
        [Display(Name = "Obs")]
        public string Observacion { get; set; }
        [Display(Name = "Proceso")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int ProcesoId { get; set; }
        public Proceso Proceso { get; set; }
        [Display(Name = "Doblez")]
        public int? DoblezId { get; set; }
        
        public Doblez Doblez { get; set; }
        [Display(Name = "Medida")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int? UnidadMedidaId { get; set; }
        
        public UnidadMedida UnidadMedida { get; set; }
        [Display(Name = "Tipo Moneda")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int? TipoMonedaId { get; set; }
        
        public TipoMoneda TipoMoneda { get; set; }

        //public int? Presupuesto_SubProcesoId { get; set; }
        //public Presupuesto_SubProceso Presupuesto_SubProceso { get; set; }
    }
}