using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Bitacora
    {
        public int IdBitacora { get; set; }
        public DateTime Fecha { get; set; }
        public string Accion { get; set; }
        public string Detalle { get; set; }
        public string Usuario { get; set; }
        public string TipoUsuario { get; set; }

    }
}
