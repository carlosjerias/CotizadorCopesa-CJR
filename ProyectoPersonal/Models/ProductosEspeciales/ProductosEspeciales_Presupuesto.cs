using ProyectoPersonal.Models.Cotizador;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.ProductosEspeciales
{
    public class ProductosEspeciales_Presupuesto
    {
        [Key]
        public int IdPresupuesto { get; set; }
        public string NombrePresupuesto { get; set; }

        public int Cantidad { get; set; }
        public string Maquina { get; set; }
        public int ElementosEnTamaño { get; set; }
        public int Entradas { get; set; }
        //agregar IDPapel
        public int Gramaje { get; set; }
        public int Ancho { get; set; }
        public int Largo { get; set; }
        public double CostoPapel { get; set; }
        public double MermaVariable { get; set; }
        public double MermaFija { get; set; }
        public double Kilos { get; set; }

        public double ImpresionCostoFijo { get; set; }
        public double ImpresionCostoVariable { get; set; }
        public double PapelCostoFijo { get; set; }
        public double PapelCostoVariable { get; set; }
        public double EncuadernacionCostoFijo { get; set; }
        public double EncuadernacionCostoVariable { get; set; }
        public double TerminacionCostoFijo { get; set; }
        public double TerminacionCostoVariable { get; set; }

        [NotMapped]
        public double QuintoColorPMSCostoFijo { get; set; }
        [NotMapped]
        public double QuintoColorPMSCostoVariable { get; set; }

        [NotMapped]
        public double QuintoColorFluorCostoFijo { get; set; }
        [NotMapped]
        public double QuintoColorFluorCostoVariable { get; set; }

        [NotMapped]
        public double QuintoColorMetalCostoFijo { get; set; }
        [NotMapped]
        public double QuintoColorMetalCostoVariable { get; set; }

        [NotMapped]
        public double BarnizAcuosoCostoFijo { get; set; }
        [NotMapped]
        public double BarnizAcuosoCostoVariable { get; set; }
        [NotMapped]
        public double ImpresionCostoFijoTotal { get; set; }
        [NotMapped]
        public double ImpresionCostoVariableTotal { get; set; }

        //PROCESOS TERMINACION
        [NotMapped]
        public double BarnizUVBrillanteCostoFijo { get; set; }
        [NotMapped]
        public double BarnizUVBrillanteCostoVariable { get; set; }
        [NotMapped]
        public double BarnizUVBrillanteSelectivoCostoFijo { get; set; }
        [NotMapped]
        public double BarnizUVBrillanteSelectivoCostoVariable { get; set; }
        [NotMapped]
        public double BarnizGlitterCostoFijo { get; set; }
        [NotMapped]
        public double BarnizGlitterCostoVariable { get; set; }
        [NotMapped]
        public double PoliBrillanteCostoFijo { get; set; }
        [NotMapped]
        public double PoliBrillanteCostoVariable { get; set; }
        [NotMapped]
        public double PoliOpacoCostoFijo { get; set; }
        [NotMapped]
        public double PoliOpacoCostoVariable { get; set; }

        //TERMINACIONES ENCUADERNACION
        [NotMapped]
        public double PlisadoCostoFijo { get; set; }
        [NotMapped]
        public double PlisadoCostoVariable { get; set; }
        [NotMapped]
        public double TroquelCostoFijo { get; set; }
        [NotMapped]
        public double TroquelCostoVariable { get; set; }
        [NotMapped]
        public double CorteCostoFijo { get; set; }
        [NotMapped]
        public double CorteCostoVariable { get; set; }
        [NotMapped]
        public double DoblezDipticoCostoFijo { get; set; }
        [NotMapped]
        public double DoblezDipticoCostoVariable { get; set; }
        [NotMapped]
        public double MecanizadoCostoFijo { get; set; }
        [NotMapped]
        public double MecanizadoCostoVariable { get; set; }

        [NotMapped]
        public double TerminacionCostoFijoTotal { get; set; }
        [NotMapped]
        public double TerminacionCostoVariableTotal { get; set; }

        [NotMapped]
        public double EncuadernacionCostoFijoTotal { get; set; }
        [NotMapped]
        public double EncuadernacionCostoVariableTotal { get; set; }

        [NotMapped]
        public double DespachoCostoFijo { get; set; }
        [NotMapped]
        public double DespachoCostoVariable { get; set; }

        [NotMapped]
        public double NetoCostoFijoTotal { get; set; }
        [NotMapped]
        public double NetoCostoVariableTotal { get; set; }
        [NotMapped]
        public double PrecioUnitario { get; set; }




        public int FormatoId { get; set; }
        public ProductosEspeciales_Formato Formato { get; set; }


        public int PapelId { get; set; }
        public ProductosEspeciales_Papel Papel { get; set; }


        public int? MonedaId { get; set; }
        public Moneda Moneda { get; set; }

        public string Usuarioid { get; set; }
        public ApplicationUser Usuario { get; set; }
    }

    public class ProductosEspeciales_PresupuestoForm
    {
        [Key]
        public int IdPresupuesto { get; set; }
        public string NombrePresupuesto { get; set; }
        public string Formato { get; set; }
        public int Papel { get; set; }
        public int Cantidad { get; set; }
        public string Maquina { get; set; }
        public int ElementosEnTamaño { get; set; }
   
        public int Entradas { get; set; }
        public int Gramaje { get; set; }
        public int Ancho { get; set; }
        public int Largo { get; set; }

        public int? Despacho { get; set; }
        public int? QuintoColorPMS { get; set; }
        public int? QuintoColorFluor { get; set; }
        public int? QuintoColorMetalico { get; set; }
        public int? BarnizAcuoso { get; set; }

        public int? BarnizUVBrillante { get; set; }
        public int? BarnizUVBrillanteSelectivo { get; set; }
        public int? BarnizGlitterSelectivo { get; set; }
        public int? PolitermolaminadoBrillante { get; set; }
        public int? PolitermolaminadoOpaco { get; set; }

        public int? Plisado { get; set; }
        public int? Troquel { get; set; }
        public int? Corte { get; set; }
        public int? DoblezDiptico { get; set; }
        public int? Adhesivo { get; set; }
        public int? EncEnPagDefinida { get; set; }
        public int? Mecanizado { get; set; }
        public int? Sobre { get; set; }



        public List<ProductosEspeciales_Formato> Formatos { get; set; }
        public List<ProductosEspeciales_Papel> Papeles { get; set; }

    }
}