using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class ErrorVerificacion
    {
        public string Tabla { get; set; }
        public string TipoError { get; set; } // "Modificación", "Eliminación", "DVV"
        public int? IdRegistro { get; set; } // lo uso para los null  errores de DVV
        public string CamposModificados { get; set; }
    }
}
