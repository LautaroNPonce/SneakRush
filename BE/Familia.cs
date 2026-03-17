using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    // La clase familia sí puede contener hijos: otras familias o patentes.
    public class Familia : PermisoComponente
    {
        private readonly IList<PermisoComponente> _hijos = new List<PermisoComponente>();

        public override void AgregarHijo(PermisoComponente componente)
        {
            if (!_hijos.Contains(componente))
                _hijos.Add(componente);
        }

        public override void QuitarHijo(PermisoComponente componente)
        {
            if (_hijos.Contains(componente))
                _hijos.Remove(componente);
        }

        public override IList<PermisoComponente> ObtenerHijos()
        {
            return _hijos;
        }
    }
}
