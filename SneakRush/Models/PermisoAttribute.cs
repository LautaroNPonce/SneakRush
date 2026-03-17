using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BLL;
using BE;

namespace Presentacion_Administrador.Models
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            System.Diagnostics.Debug.WriteLine("➡ BaseController ejecutado");

            if (Session["Permisos"] == null)
            {
                var bll = new BLL_Permiso();

                // 🔹 ADMIN / WEBMASTER (usuarios del sistema)
                if (Session["IdUsuario"] != null)
                {
                    int idUsuario = (int)Session["IdUsuario"];
                    var lista = bll.ListarPermisosUsuario(idUsuario);

                    System.Diagnostics.Debug.WriteLine("➡ Permisos USUARIO obtenidos: " + lista.Count);
                    Session["Permisos"] = lista;
                }
                // 🔹 CLIENTE (tienda online)
                else if (Session["IdCliente"] != null)
                {
                    int idCliente = (int)Session["IdCliente"];
                    var lista = bll.ListarPermisosCliente(idCliente);

                    System.Diagnostics.Debug.WriteLine("➡ Permisos CLIENTE obtenidos: " + lista.Count);
                    Session["Permisos"] = lista;
                }
            }
        }
    }
}