using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BLL
{
    public class BLL_IdiomaSubject
    {
        private static List<BLL_ObserverIdioma> _observadores = new List<BLL_ObserverIdioma>();
        private static string _idiomaActual = "es";

        public static void AgregarObservador(BLL_ObserverIdioma observador)
        {
            if (!_observadores.Contains(observador))
                _observadores.Add(observador);
        }

        public static void QuitarObservador(BLL_ObserverIdioma observador)
        {
            if (_observadores.Contains(observador))
                _observadores.Remove(observador);
        }

        public static void CambiarIdioma(string codigoIdioma)
        {
            _idiomaActual = codigoIdioma;
            NotificarObservadores();
        }

        public static void NotificarObservadores()
        {
            foreach (var obs in _observadores)
                obs.ActualizarIdioma(_idiomaActual);
        }

        public static string ObtenerIdiomaActual()
        {
            return _idiomaActual;
        }
    }
}

