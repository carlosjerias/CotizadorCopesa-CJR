using ProyectoPersonal.Models.Cotizador;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.ProductosEspeciales
{
    public class ProductosEspeciales_SubProceso
    {
        [Key]
        public int IdSubproceso { get; set; }
        public string Nombre { get; set; }
        public double CostoFijo { get; set; }
        public double CostoVariable { get; set; }
        public string Observacion { get; set; }


        //public int? UnidadMedidaId { get; set; }
        //public UnidadMedida UnidadMedida { get; set; }

        //public int? TipoMonedaId { get; set; }
        //public TipoMoneda TipoMoneda { get; set; }

    }
}