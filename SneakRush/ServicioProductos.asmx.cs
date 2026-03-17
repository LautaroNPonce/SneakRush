using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using BE;
using BLL;

namespace Presentacion_Administrador
{
    /// <summary>
    /// Descripción breve de ServicioProductos
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class ServicioProductos : System.Web.Services.WebService
    {
        [WebMethod(Description = "Devuelve los productos con stock > 0")]
        public List<ProductoStockDTO> ObtenerProductosConStock()
        {
            return new BLL_Producto().ListarConStock();
        }
    }
}
