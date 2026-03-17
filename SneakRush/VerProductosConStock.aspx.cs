using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace Presentacion_Administrador
{
    public partial class VerProductosConStock : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                var lista = new BLL_Producto().ListarConStock();

                var xdoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", "yes"),
                    new XElement("Productos",
                        new XAttribute("fechaGeneracion", DateTime.Now.ToString("s")),
                        from p in lista
                        select new XElement("Producto",
                            new XElement("IdProducto", p.IdProducto),
                            new XElement("Nombre", p.Nombre),
                            new XElement("Marca", p.Marca),
                            new XElement("Categoria", p.Categoria),
                            new XElement("Precio", p.Precio),
                            new XElement("Stock", p.Stock)
                        )
                    )
                );

                // Si se pidió como descarga
                if (Request.QueryString["download"] == "1")
                {
                    Response.AddHeader("Content-Disposition", "attachment; filename=ProductosConStock.xml");

                    // Registro en bitácora
                    string usuario = (HttpContext.Current.Session["Usuario"] != null)
                        ? HttpContext.Current.Session["Usuario"].ToString()
                        : "Sin sesión";

                    string tipoUsuario = (HttpContext.Current.Session["Rol"] != null)
                        ? HttpContext.Current.Session["Rol"].ToString()
                        : "Desconocido";

                    new BLL_Bitacora().Registrar(new Bitacora
                    {
                        Fecha = DateTime.Now,
                        Usuario = usuario,
                        TipoUsuario = tipoUsuario,
                        Accion = "Descarga XML",
                        Detalle = "Se descargó el archivo ProductosConStock.xml con los productos disponibles en stock."
                    });
                }

                Response.Clear();
                Response.ContentType = "application/xml; charset=utf-8";
                Response.ContentEncoding = Encoding.UTF8;
                Response.Write(xdoc.ToString());
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Response.Clear();
                Response.ContentType = "application/xml; charset=utf-8";
                Response.Write($@"<?xml version=""1.0"" encoding=""utf-8""?><Error><Mensaje>No se pudo generar el XML.</Mensaje><Detalle>{HttpUtility.HtmlEncode(ex.Message)}</Detalle></Error>");
                Response.Flush();
                HttpContext.Current.ApplicationInstance.CompleteRequest();
            }
        }
    }
}


