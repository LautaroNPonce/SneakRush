using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BE;

namespace SneakRush.Models
{
    public class VMEntidadPermisos
    {
        public int Id { get; set; }
        public string Tipo { get; set; }   // "Usuario" o "Cliente"
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }    // Para usuarios: Admin/Webmaster. Para clientes: "Cliente"
        public IList<PermisoComponente> Permisos { get; set; } = new List<PermisoComponente>();
    }
}