using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Producto
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public Marca oMarca { get; set; } // Hacemos referencia a nuestra clase marca, oMarca es un alias de objeto 
        public Categoria oCategoria { get; set; }
        public decimal Precio { get; set; }
        public string PrecioSerie { get; set; }
        public int Stock { get; set; }
        public string RutaImagen { get; set; }
        public string NombreImagen { get; set; }
        public bool Activo { get; set; }

        // Con esto el fomato que vamos a estar mostrando las imagenes
        public string Base64 { get; set; }

         // Es una extension del tipo de imagen que tiene asiganada el producto (jpg o png)
         public string Extension { get; set; }
    }

    [Serializable]
    public class ProductoStockDTO
    {
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Marca { get; set; }
        public string Categoria { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
    }
}
