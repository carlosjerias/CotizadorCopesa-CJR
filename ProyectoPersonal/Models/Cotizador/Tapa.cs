using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Tapa
    {
        [Key]
        public int IdTapa { get; set; }
        public int CantidadPaginas { get; set; }
        public int PapelId { get; set; }
        public Papel Papel { get; set; }
        public List<Presupuesto> Presupuestos { get; set; }
        public float KilosPapel { get; set; }
        public float Entradas { get; set; }
        public float Tiradas { get; set; }
        public double CostoPapelTapaFijo { get; set; }
        public double CostoPapelTapaVari { get; set; }
        public int? MaquinaId { get; set; }
        public Maquina Maquina { get; set; }
    }
}