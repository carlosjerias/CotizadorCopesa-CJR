using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Cotizador
{
    public class Embalaje
    {
        [Key]
        public int idEmbalaje { get; set; }
        public int Base { get; set; }
        public int AltoCaja { get; set; }
        public int CajaEstandar { get; set; }
        public int PalletEstandar { get; set; }
        public double EncajadoxCaja { get; set; }
        public double Enzunchado { get; set; }
        public bool Estado { get; set; }
    }

}