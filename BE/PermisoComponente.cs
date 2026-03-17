using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // Para aclarar esta clase es el “Component” del patrón Composite.
    public abstract class PermisoComponente
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Codigo { get; set; }

        // Composite: operaciones comunes
        public abstract void AgregarHijo(PermisoComponente componente);
        public abstract void QuitarHijo(PermisoComponente componente);
        public abstract IList<PermisoComponente> ObtenerHijos();
    }
}
