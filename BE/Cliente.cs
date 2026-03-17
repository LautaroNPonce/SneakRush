using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public string Correo { get; set; }
        public string Clave { get; set; }
        public bool Reestablecer { get; set; }
        public string ConfirmarContraseña { get; set; }

        // Esto lo uso para que tenga permisos
        public IList<PermisoComponente> Permisos { get; set; } = new List<PermisoComponente>();
    }
}
