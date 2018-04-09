using ProyectoPersonal.Models.Gerencia;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using System.Web.Script.Serialization;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Presupuesto
    {
        [Key]
        public int IdPresupuesto { get; set; }
        [Display(Name = "Nombre")]
        public string NombrePresupuesto { get; set; }

        public int TipoCatalogoId { get; set; }
        public TipoCatalogo TipoCatalogo { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0,0}")]
        public int Tiraje { get; set; }
        public int? FormatoId { get; set; }
        public Formato Formato { get; set; }

        public int? EncuadernacionId { get; set; }
        public Encuadernacion Encuadernacion { get; set; }

        public int InteriorId { get; set; }
        public Interior Interior { get; set; }

        public int? TapaId { get; set; }
        public Tapa Tapa { get; set; }
        
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0,0}")]
        public double TotalNetoFijo { get; set; }
        [DisplayFormat(DataFormatString = "{0:n}")]
        public double TotalNetoVari { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:0,0}")]
        public double TotalNetoTotal { get; set; }
        public double PrecioUnitario { get; set; }

        public int? MonedaId { get; set; }
        public Moneda Moneda { get; set; }
        [NotMapped]
        [ScaffoldColumn(false)]
        public List<Papel> ListaPapeles { get; set; }

        public int EntradasPag64 { get; set; }
        public int EntradasPag48 { get; set; }
        public int EntradasPag32 { get; set; }
        public int EntradasPag24 { get; set; }
        public int EntradasPag16 { get; set; }
        public int EntradasPag12 { get; set; }
        public int EntradasPag8 { get; set; }
        public int EntradasPag4 { get; set; }
        public double CostoFijoPag64 { get; set; }
        public double CostoFijoPag48 { get; set; }
        public double CostoFijoPag32 { get; set; }
        public double CostoFijoPag24 { get; set; }
        public double CostoFijoPag16 { get; set; }
        public double CostoFijoPag12 { get; set; }
        public double CostoFijoPag8 { get; set; }
        public double CostoFijoPag4 { get; set; }
        public double CostoVariablePag64 { get; set; }
        public double CostoVariablePag48 { get; set; }
        public double CostoVariablePag32 { get; set; }
        public double CostoVariablePag24 { get; set; }
        public double CostoVariablePag16 { get; set; }
        public double CostoVariablePag12 { get; set; }
        public double CostoVariablePag8 { get; set; }
        public double CostoVariablePag4 { get; set; }
        public double CostoFijoEncuadernacion { get; set; }
        public double CostoVariableEncuadernacion { get; set; }
        public double CostoFijoTapa { get; set; }
        public double CostoVariableTapa { get; set; }
        public int CantidadModelos { get; set; }
        [NotMapped]
        public int CostoVariableCajas { get; set; }

        public double CostoVariablePallet { get; set; }
        [NotMapped]
        [ScriptIgnore]
        public Maquina MaquinaInterior { get; set; }
        [NotMapped]
        [ScriptIgnore]
        public Maquina MaquinaTapa { get; set; }
        [NotMapped]
        public string NombreEncuadernacion { get; set; }
        [NotMapped]
        public string NombreQuintoColor { get; set; }
        [NotMapped]
        public string NombreEmbolsado { get; set; }
        [NotMapped]
        public string NombreLaminado { get; set; }
        [NotMapped]
        public string NombreBarnizUV { get; set; }
        [Display(Name = "Cantidad de Cajas")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int CantidadCajas { get; set; }
        [Display(Name = "Cantidad de Enzunchado x pqte")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public int Enzunchadoxpqte { get; set; }
        public int CantidadenBolsa { get; set; }
        public double LibrosxCajas { get; set; }
        public int CantidadTerminacionEmbolsado { get; set; }
        [Display(Name = "Cantidad de Pallet")]
        public int CantidadPallet { get; set; }
        [NotMapped]
        public double CostoFijoQuintoColor { get; set; }
        [NotMapped]
        public double CostoVariableQuintoColor { get; set; }
        [NotMapped]
        public double CostoFijoPlizado { get; set; }
        [NotMapped]
        public double CostoVariablePlizado { get; set; }
        [NotMapped]
        public double CostoFijoBarnizUV { get; set; }
        [NotMapped]
        public double CostoVariableBarnizUV { get; set; }
        [NotMapped]
        public double CostoVariableEmbolsado { get; set; }
        [NotMapped]
        public double CostoVariableLaminado { get; set; }
        [NotMapped]
        public double CostoFijoBarnizAcuosoTapa { get; set; }
        [NotMapped]
        public double CostoVariableBarnizAcuosoTapa { get; set; }
        [NotMapped]
        public double CostoFijoTroquel { get; set; }
        [NotMapped]
        public double CostoVariableTroquel { get; set; }
        [NotMapped]
        public double CostoFijoCorteFrontal { get; set; }
        [NotMapped]
        public double CostoVariableCorteFrontal { get; set; }
        [NotMapped]
        public double CostoVariableAlzadoPlano { get; set; }
        [NotMapped]
        public double CostoVariableEmbolsadoManual { get; set; }
        [NotMapped]
        public double CostoVariableDesembolsado { get; set; }
        [NotMapped]
        public double CostoVariableAlzado { get; set; }
        [NotMapped]
        public double CostoVariableInsercion { get; set; }
        [NotMapped]
        public double CostoVariablePegado { get; set; }
        [NotMapped]
        public double CostoVariableFajado { get; set; }
        [NotMapped]
        public double CostoVariableAdhesivo { get; set; }
        [NotMapped]
        public double CostoVariableAdhesivoTotal { get; set; }
        [NotMapped]
        public double AdhesivoCms { get; set; }
        [NotMapped]
        public double CostoVariableSuministroCaja { get; set; }
        [NotMapped]
        public double CostoVariableEnzunchado { get; set; }
        [NotMapped]
        public double CostoVariablePegadoSticker { get; set; }
        [NotMapped]
        public double CostoVariableInsercionCajaySellado { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TarifaFijaImpresion { get; set; }
        [DisplayFormat(DataFormatString = "{0:n}")]
        public double TarifaVariableImpresion { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TarifaFijaPapel { get; set; }
        [DisplayFormat(DataFormatString = "{0:n}")]
        public double TarifaVariablePapel { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TarifaFijaEncuadernacion { get; set; }
        [DisplayFormat(DataFormatString = "{0:n}")]
        public double TarifaVariableEncuadernacion { get; set; }
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public double TarifaFijaTerminacion { get; set; }
        //
        //[DisplayFormat(DataFormatString = "{0:#,0.00}")]
        [DisplayFormat(DataFormatString = "{0:n}")]
        public double TarifaFijaDespacho { get; set; }
        [DisplayFormat(DataFormatString = "{0:n}")]
        public double TarifaVariableTerminacion { get; set; }
        [DisplayFormat(DataFormatString = "{0:n}")]
        public double ManufacturaFijo { get; set; }
        [DisplayFormat(DataFormatString = "{0:n}")]
        public double ManufacturaVari { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime FechaCreacion { get; set; }
        public int Estado { get; set; }
        public int BarnizAcuoso { get; set; }
        public int CantEnCajas { get; set; }
        public int CantEnZuncho { get; set; }

        public string Usuarioid { get; set; }
        public ApplicationUser Usuario { get; set; }

    }
    [DataContract(IsReference = true)]
    public class PresupuestoForm
    {
        [Key]
        public int IdPresupuestoFormo { get; set; }
        [NotMapped]
        [Display(Name = "Nombre")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(250, MinimumLength = 2, ErrorMessage = "El campo {0} debe tener de longitud de {2} a {1}.")]
        public string NombrePresupuesto { get; set; }

        public List<TipoCatalogo> Catalogo { get; set; }
        [NotMapped]
        public int? CatalogoId { get; set; }
        [NotMapped]
        public string NombreCatalogo { get; set; }
        public List<Empresa> Empresa { get; set; }
        public int EmpresaPapelInterior { get; set; }
        public int EmpresaPapelTapa { get; set; }
        public int CantidadAlzadoPlano { get; set; }
        public int CantidadDesembolsado { get; set; }
        public int CantidadAlzado { get; set; }
        public int CantidadInsercion { get; set; }
        public int CantidadPegado { get; set; }
        public int CantidadFajado { get; set; }
        public int CantidadPegadoSticker { get; set; }

        [NotMapped]
        public List<Encuadernacion> Encuadernaciones { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Encuadernación")]
        public string SelectEnc { get; set; }
        [NotMapped]
        [Display(Name = "Tiraje")]
        [Range(10, 10000000, ErrorMessage = "El campo {0} debe ser mayor a {1} y menor a {2}.")]
        public int Tiraje { get; set; }
        [NotMapped]
        public List<Formato> Formatos { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Formato")]
        public string SelectFormato { get; set; }
        [NotMapped]
        public int CantidadTerminacionEmbolsado { get; set; }
        [NotMapped]
        public List<Papel> Papeles { get; set; }
        [NotMapped]
        [Display(Name = "Papel de Cliente")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string SelectPapelIntId { get; set; }
        [NotMapped]
        public string SelectPapelTapId { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Cantidad")]
        public int CantidadInt { get; set; }
        [NotMapped]
        [Display(Name = "Cantidad")]
        public int CantidadTap { get; set; }
        //[NotMapped]
        //public List<Papel> GramajesInt { get; set; }
        //[NotMapped]
        //[Required(ErrorMessage = "El campo {0} es obligatorio")]
        //[Range(1, 40000000000000000, ErrorMessage = "El campo {0} es obligatorio")]
        //[Display(Name = "Gramaje")]
        //public int SelectGramajeInt { get; set; }
        //[NotMapped]
        //public int SelectGramajeTap { get; set; }
        [NotMapped]
        public List<Proceso> Procesos { get; set; }
        [NotMapped]
        public List<SubProceso> SubProcesos { get; set; }
        [NotMapped]
        public int IdBarnizAcuoso { get; set; }
        [NotMapped]
        public int IdBarnizUV { get; set; }
        [NotMapped]
        public int IdLaminado { get; set; }
        [NotMapped]
        public int IdQuintoColor { get; set; }
        [NotMapped]
        public int IdTroquel { get; set; }
        [NotMapped]
        public int IdEmbolsado { get; set; }
        [NotMapped]
        public int IdAlzadoElementoPlano { get; set; }
        [NotMapped]
        public int IdEmbolsadoManual { get; set; }
        [NotMapped]
        public int IdDesembolsado { get; set; }
        [NotMapped]
        public int IdAlzado { get; set; }
        [NotMapped]
        public int IdInsercion { get; set; }
        [NotMapped]
        public int IdPegado { get; set; }
        [NotMapped]
        public int IdFajado { get; set; }
        [NotMapped]
        public int IdAdhesivo { get; set; }
        [NotMapped]
        public int IdSuministroCajas { get; set; }
        [NotMapped]
        public int IdCajaYSellados { get; set; }
        [NotMapped]
        public int IdEnzunchado { get; set; }
        [NotMapped]
        public int IdPegadoSticker { get; set; }
        [NotMapped]
        public string AdhesivoCms { get; set; }
        [Display(Name = "Cantidad en Cajas")]
        [NotMapped]
        public int CantidadCajas { get; set; }
        [NotMapped]
        public int CostoVariableCajas { get; set; }
        [Display(Name = "Cantidad de Enzunchado x pqte")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        [NotMapped]
        public int Enzunchadoxpqte { get; set; }
        [NotMapped]
        public int CostoVariableEnzunchadoxpqte { get; set; }
        [Display(Name = "Cantidad de Pallet")]
        [NotMapped]
        public int CantidadPallet { get; set; }
        [NotMapped]
        public double CostoVariablePallet { get; set; }
        [NotMapped]
        public int CantidadModelos { get; set; }
        public int CantidadEnCajas { get; set; }
        public int CantidadEnZuncho { get; set; }
        public int CantidadEnBolsa { get; set; }
    }

    public class PresupuestoView
    {
        public int TipoCatalogoId { get; set; }
        public List<TipoCatalogo> TiposCatalogos { get; set; }
        public Presupuesto Presupuesto { get; set; }
        public int EncuadernacionId { get; set; }
        public List<Encuadernacion> Encuadernaciones { get; set; }
        public string FormatoId { get; set; }
        public List<Formato> Formatos { get; set; }
        public int SubProcesoQuintoColorId { get; set; }
        public int PasadasQuintoColor { get; set; }
        public int SubProcesoAcuosoId { get; set; }
        public int SubProcesoLaminadoId { get; set; }
        public int SubProcesoBarnizUVId { get; set; }
        public int SubProcesoEmbolsadoId { get; set; }
        public int SubProcesoAlzadoPlanoId { get; set; }
        public int SubProcesoDesembolsadoId { get; set; }
        public int SubProcesoAlzadoId { get; set; }
        public int SubProcesoInsercionId { get; set; }
        public int SubProcesoSachetId { get; set; }
        public int SubProcesoFajadoId { get; set; }
        public int SubProcesoAdhesivoId { get; set; }
        public int SubProcesoStickerId { get; set; }
        public List<Presupuesto_SubProceso> SubProcesos { get; set; }
        public Interior Interior { get; set; }
        public Tapa Tapas { get; set; }
        public int CantidadModelosTapas { get; set; }
        public int PapelInteriorId { get; set; }
        public int PapelTapaId { get; set; }
        public List<Papel> Papeles { get; set; }
        public int EmpresaPapelInteriorId { get; set; }
        public int EmpresaPapelTapaId { get; set; }
        public List<Empresa> Empresas { get; set; }
    }
}
