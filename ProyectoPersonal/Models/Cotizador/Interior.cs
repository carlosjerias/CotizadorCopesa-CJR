using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Interior
    {
        [Key]
        public int IdInterior { get; set; }
        [Display(Name ="Cantidad Paginas")]
        public int CantidadPaginas { get; set; }
        public int PapelId { get; set; }
        public Papel Papel { get; set; }
        public List<Presupuesto> Presupuestos { get; set; }
        [DisplayFormat(DataFormatString = "{0:#,0.00}")]
        public float KilosPapel { get; set; }
        public float Entradas { get; set; }
        public float Tiradas { get; set; }
        public double CostoPapelinteriorFijo { get; set; }
        public double CostoPapelInteriorVari { get; set; }
        public int? MaquinaId { get; set; }
        public Maquina Maquina { get; set; }
    }
}