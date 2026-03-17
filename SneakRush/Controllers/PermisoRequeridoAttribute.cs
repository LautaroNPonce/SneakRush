using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Presentacion_Administrador.Controllers
{
    public class PermisoRequeridoAttribute :Attribute
    {
        public string CodigoPermiso { get; set; }

        public PermisoRequeridoAttribute(string codigo)
        {
            CodigoPermiso = codigo;
        }
    }
}