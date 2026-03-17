using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;

namespace BLL
{
    public class BLL_Idioma
    {
        public class GestorIdiomas
        {
            private static Dictionary<string, string> _textosActuales;

            /// Cargo los idiomas desde el archivo JSON (ej: "es", "en").
            public static void CargarIdioma(string codigoIdioma)
            {
                string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Idiomas", $"lang_{codigoIdioma}.json");


                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Archivo no encontrado: {path}");
                }

                string json = File.ReadAllText(path);
                _textosActuales = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                // Avisa si se cambio el idioma
                BLL_IdiomaSubject.CambiarIdioma(codigoIdioma);
            }

            /// Obtengo el texto traducido según el ID especificado.
            public static string Traducir(string idTexto)
            {
                if (_textosActuales != null && _textosActuales.ContainsKey(idTexto))
                    return _textosActuales[idTexto];
                return idTexto; // devuelve el ID si no encuentra traducción
            }

            /// Devuelve todos los textos cargados (útil para la vista inicial).
            public static Dictionary<string, string> ObtenerTextos()
            {
                return _textosActuales;
            }
        }
    }
}
