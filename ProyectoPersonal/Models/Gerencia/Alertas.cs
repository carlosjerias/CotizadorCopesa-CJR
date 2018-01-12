using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProyectoPersonal.Models.Gerencia
{
    public class Alertas
    {
        public object[] Resultado_Action(string Tipo, string Accion, string Mensaje)
        {
            if (Tipo.ToLower() == "ok")
            {

                return new object[] { "alert alert-success", "Se ha " + Accion + " correctamente! ", Mensaje };
            }
            else //if (Tipo.ToLower() == "error")
            {
                return new object[] { "alert alert-danger", "Ha ocurrido un error! ", Mensaje };
            }
        }
    }
}
