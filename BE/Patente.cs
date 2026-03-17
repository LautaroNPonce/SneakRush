using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // La clase no tiene hijos, solo representa un permiso concreto.
    public class Patente : PermisoComponente
    {
        public override void AgregarHijo(PermisoComponente componente)
        {
            // Una patente es una hoja: no puede tener hijos
            throw new NotImplementedException("Una patente no puede tener hijos");
        }

        public override void QuitarHijo(PermisoComponente componente)
        {
            throw new NotImplementedException("Una patente no puede tener hijos");
        }

        public override IList<PermisoComponente> ObtenerHijos()
        {
            // Devuelvo lista vacía para no romper recorridos
            return new List<PermisoComponente>();
        }
    }
}
