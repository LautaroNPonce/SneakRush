using BE;
using BLL;
using Presentacion_Administrador.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;


namespace Presentacion_Administrador.Controllers
{
    public class HomeWebmasterController : Controller, BLL_ObserverIdioma
    {
        public HomeWebmasterController()
        {
            //  Lo utilizo para suscribir el controlador al sistema de idiomas
            BLL_IdiomaSubject.AgregarObservador(this);
        }

        // Este metodo lo uso para que se ejecutara automáticamente
        // cuando cambio el idioma desde el ComboBox (JS → BLL_IdiomaSubject)
        public void ActualizarIdioma(string codigoIdioma)
        {
            // Verifico que haya contexto HTTP antes de acceder a Session
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
            {
                System.Web.HttpContext.Current.Session["IdiomaActual"] = codigoIdioma;
            }

            // Con esto manejo la depuracion
            System.Diagnostics.Debug.WriteLine($"Idioma actualizado en HomeWebmasterController → {codigoIdioma}");
        }

        private bool EsWebmaster()
        {
            return Session["Rol"] != null && Session["Rol"].ToString() == "Webmaster";
        }

        //[Authorize]
        public ActionResult Index()
        {
            //if (!EsWebmaster()) 
            //{
            //    return RedirectToAction("Index", "TiendaOnline");
            //}

            //return View();
            if (!EsWebmaster())
            {
                ViewBag.Mensaje = "No tenés permiso para acceder a esta sección.";
                return View("~/Views/Shared/NoAutorizado.cshtml");
            }

            // Esto lo uso para mostrar el idioma actual en la vista
            ViewBag.Idioma = Session["IdiomaActual"] ?? "es";

            return View();
        }

        //[Authorize]
        public ActionResult BackupRestore()
        {
            //if (!EsWebmaster())
            //{
            //    return RedirectToAction("Index", "TiendaOnline");
            //}

            //return View();
            if (!EsWebmaster())
            {
                ViewBag.Mensaje = "No tenés permiso para acceder a esta sección.";
                return View("~/Views/Shared/NoAutorizado.cshtml");
            }

            return View();
        }

        [HttpGet] // Obtener lista de Webmasters (usuarios con Rol == "Webmaster")
        public JsonResult ListarWebmasters()
        {
            if (!EsWebmaster())
                return Json(new { data = new List<Usuario>() }, JsonRequestBehavior.AllowGet);

            var lista = new BLL_Usuarios()
                        .Listar()
                        .Where(u => u.Rol == "Webmaster")
                        .ToList();

            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarWebmaster(Usuario obj)
        {
            if (!EsWebmaster())
            {
                return Json(new { resultado = false, mensaje = "No tenés permiso para realizar esta acción." }, JsonRequestBehavior.AllowGet);
            }

            obj.Rol = "Webmaster"; // Me aseguro que el rol fuera el Webmaster
            string mensaje = string.Empty;
            object resultado;

            if (obj.IdUsuario == 0)
            {
                resultado = new BLL_Usuarios().Registrar(obj, out mensaje);
            }
            else
            {
                resultado = new BLL_Usuarios().Modificar(obj, out mensaje);
            }

            new BLL_Bitacora().Registrar(new Bitacora
            {
                Accion = (obj.IdUsuario == 0) ? "Agregar webmaster" : "Modificar webmaster",
                Detalle = (obj.IdUsuario == 0)
                ? $"El webmaster agregó a {obj.Nombres} {obj.Apellidos}."
                : $"El webmaster modificó a {obj.Nombres} {obj.Apellidos}.",
                Usuario = Session["WebmasterCorreo"]?.ToString() ?? "Desconocido",
                TipoUsuario = "Webmaster"
            });


            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost] // Eliminar un Webmaster por ID
        public JsonResult EliminarWebmaster(int id)
        {
            if (!EsWebmaster())
            {
                return Json(new { resultado = false, mensaje = "No tenés permiso para realizar esta acción." }, JsonRequestBehavior.AllowGet);
            }

            string mensaje = string.Empty;
            bool respuesta = new BLL_Usuarios().Eliminar(id, out mensaje);

            new BLL_Bitacora().Registrar(new Bitacora
            {
                Accion = "Eliminar webmaster",
                Detalle = $"El webmaster eliminó al usuario con ID {id}.",
                Usuario = Session["WebmasterCorreo"]?.ToString() ?? "Desconocido",
                TipoUsuario = "Webmaster"
            });

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CambiarIdioma(string codigo)
        {
            try
            {
                // Se notifican a todos los observadores
                BLL_IdiomaSubject.CambiarIdioma(codigo);

                // Devuelvo un OK vacío (no hay vista asociada)
                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                // Si algo sale mal, registramos el error
                System.Diagnostics.Debug.WriteLine($"Error al cambiar idioma: {ex.Message}");
                return new HttpStatusCodeResult(500, "Error al cambiar idioma");
            }
        }
    }

    //public class HomeWebmasterController : BaseController, BLL_ObserverIdioma
    //{
    //    public HomeWebmasterController()
    //    {
    //        // Suscribir al sistema de idiomas
    //        BLL_IdiomaSubject.AgregarObservador(this);
    //    }

    //    public void ActualizarIdioma(string codigoIdioma)
    //    {
    //        if (System.Web.HttpContext.Current != null &&
    //            System.Web.HttpContext.Current.Session != null)
    //        {
    //            System.Web.HttpContext.Current.Session["IdiomaActual"] = codigoIdioma;
    //        }

    //        System.Diagnostics.Debug.WriteLine($"Idioma actualizado en HomeWebmasterController → {codigoIdioma}");
    //    }

    //    public ActionResult Index()
    //    {
    //        ViewBag.Idioma = Session["IdiomaActual"] ?? "es";
    //        return View();
    //    }

    //    
    //    [PermisoRequerido("WM_RESTORE")]
    //    public ActionResult BackupRestore()
    //    {
    //        return View();
    //    }

    //    /
    //    [HttpGet]
    //    [PermisoRequerido("WM_MODIFICAR")]
    //    public JsonResult ListarWebmasters()
    //    {
    //        var lista = new BLL_Usuarios()
    //                    .Listar()
    //                    .Where(u => u.Rol == "Webmaster")
    //                    .ToList();

    //        return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
    //    }

    //    
    //    [HttpPost]
    //    [PermisoRequerido("WM_MODIFICAR")]
    //    public JsonResult GuardarWebmaster(Usuario obj)
    //    {
    //        obj.Rol = "Webmaster";
    //        string mensaje = string.Empty;
    //        object resultado;

    //        if (obj.IdUsuario == 0)
    //            resultado = new BLL_Usuarios().Registrar(obj, out mensaje);
    //        else
    //            resultado = new BLL_Usuarios().Modificar(obj, out mensaje);

    //        new BLL_Bitacora().Registrar(new Bitacora
    //        {
    //            Accion = (obj.IdUsuario == 0) ? "Agregar webmaster" : "Modificar webmaster",
    //            Detalle = (obj.IdUsuario == 0)
    //            ? $"El webmaster agregó a {obj.Nombres} {obj.Apellidos}."
    //            : $"El webmaster modificó a {obj.Nombres} {obj.Apellidos}.",
    //            Usuario = Session["WebmasterCorreo"]?.ToString() ?? "Desconocido",
    //            TipoUsuario = "Webmaster"
    //        });

    //        return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
    //    }

    //    
    //    [HttpPost]
    //    [PermisoRequerido("WM_ELIMINAR")]
    //    public JsonResult EliminarWebmaster(int id)
    //    {
    //        string mensaje = string.Empty;
    //        bool respuesta = new BLL_Usuarios().Eliminar(id, out mensaje);

    //        new BLL_Bitacora().Registrar(new Bitacora
    //        {
    //            Accion = "Eliminar webmaster",
    //            Detalle = $"El webmaster eliminó al usuario con ID {id}.",
    //            Usuario = Session["WebmasterCorreo"]?.ToString() ?? "Desconocido",
    //            TipoUsuario = "Webmaster"
    //        });

    //        return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
    //    }
    //}

}