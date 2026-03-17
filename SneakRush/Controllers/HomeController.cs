using BE;
using BLL;
using ClosedXML.Excel;
using Presentacion_Administrador.Controllers;
using Presentacion_Administrador.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Xml.Serialization;
using static BE.Venta;
using static BLL.BLL_Idioma;


namespace SneakRush.Controllers
{
    /*[Authorize] */// Con esto nadie que no este registrado va a poder acceder a ningun de estos forms si es que no se encuentra autorizado
    public class HomeController : Presentacion_Administrador.Controllers.BaseController, BLL_ObserverIdioma
    {
        public HomeController() 
        {
            //  Lo utilizo para suscribir el controlador al sistema de idiomas
            BLL_IdiomaSubject.AgregarObservador(this);
        }

        // Este método lo uso para que se ejecutará automáticamente
        public void ActualizarIdioma(string codigoIdioma)
        {
            // Verifico que haya contexto HTTP antes de acceder a Session
            if (System.Web.HttpContext.Current != null && System.Web.HttpContext.Current.Session != null)
            {
                System.Web.HttpContext.Current.Session["IdiomaActual"] = codigoIdioma;
            }

            // Con esto manejo la depuracion
            System.Diagnostics.Debug.WriteLine($"Idioma actualizado en HomeController → {codigoIdioma}");
        }

        private bool EsAdministrador()
        {
            return Session["Rol"] != null && Session["Rol"].ToString() == "Administrador";
        }

        public ActionResult Index()
        {
            if (!EsAdministrador())
            {
                TempData["Mensaje"] = "No tenés permiso para acceder a esta sección.";
                return RedirectToAction("NoAutorizado", "Home");

            }

            // Esto lo uso para mostrar el idioma actual en la vista
            ViewBag.Idioma = Session["IdiomaActual"] ?? "es";

            return View();
        }

        public ActionResult Usuarios()
        {
            if (Session["Rol"] == null || Session["Rol"].ToString() != "Administrador")
            {
                TempData["Mensaje"] = "No tenés permiso para acceder a esta sección.";
                return RedirectToAction("NoAutorizado", "Home");
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult NoAutorizado()
        {
            ViewBag.Mensaje = TempData["Mensaje"] ?? "No tenés permiso para acceder a esta sección.";
            return View("NoAutorizado");
        }

        [HttpGet] 
        public JsonResult ListarUsuarios() 
        {
            if (!EsAdministrador())
                return Json(new { error = "Acceso no autorizado" }, JsonRequestBehavior.AllowGet);

            var lista = new BLL_Usuarios()
                    .Listar()
                    .Where(u => u.Rol == "Administrador")
                    .ToList();

            return Json(new { data = lista }, JsonRequestBehavior.AllowGet);

            //var lista = new BLL_Usuarios()
            //        .Listar()
            //        .Where(u => u.Rol == "Administrador")
            //        .ToList();

            //return Json(new { data = lista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarUsuarios(Usuario obj)
        {
            if (!EsAdministrador())
            {
                return Json(new { error = "Acceso no autorizado" }, JsonRequestBehavior.AllowGet);
            }

            object resultado; // para alamacenar cualquier tipo de valor
            string mensaje = string.Empty;
            obj.Rol = "Administrador";

            if (obj.IdUsuario == 0)
            {
                resultado = new BLL_Usuarios().Registrar(obj, out mensaje);
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Accion = "Agregar usuario",
                    Detalle = $"El administrador agregó al usuario {obj.Nombres} {obj.Apellidos}.",
                    Usuario = Session["Usuario"].ToString(),
                    TipoUsuario = "Administrador"
                });
            }
            else
            {
                resultado = new BLL_Usuarios().Modificar(obj, out mensaje);
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Accion = "Modificar usuario",
                    Detalle = $"El administrador modificó al usuario {obj.Nombres} {obj.Apellidos}.",
                    Usuario = Session["Usuario"].ToString(),
                    TipoUsuario = "Administrador"
                });
            }
            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet); // Lo usamos para devolver la logica obtenida
        }

        public JsonResult EliminarUsuarios(int id)
        {
            if (!EsAdministrador())
            {
                return Json(new { error = "Acceso no autorizado" }, JsonRequestBehavior.AllowGet);
            }

            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new BLL_Usuarios().Eliminar(id, out mensaje);

            if (respuesta)
            {
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Accion = "Eliminar usuario",
                    Detalle = $"El administrador eliminó al usuario con ID {id}.",
                    Usuario = Session["Usuario"].ToString(),
                    TipoUsuario = "Administrador"
                });
            }

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult ListaReporte(string fechainicio, string fechafin, string idtrasferencia)
        {
            if (!EsAdministrador())
            {
                return Json(new { error = "Acceso no autorizado" }, JsonRequestBehavior.AllowGet);
            }

            List<Reporte> olista = new List<Reporte>();
            olista = new BLL_Reporte().Ventas(fechainicio, fechafin, idtrasferencia);

            return Json(new { data = olista }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult VistaDashBoards()
        {
            if (!EsAdministrador())
            {
                return Json(new { error = "Acceso no autorizado" }, JsonRequestBehavior.AllowGet);
            }

            DashBoard objeto = new BLL_Reporte().VerDashBoards();
            return Json(new { resultado = objeto }, JsonRequestBehavior.AllowGet);
        }

        [PermisoRequerido("PC_VER_BITACORA")]
        [HttpGet]
        public ActionResult Bitacora()
        {
            //if (Session["Usuario"] == null || (Session["Rol"]?.ToString() != "Administrador" && Session["Rol"]?.ToString() != "Webmaster"))
            //{
            //    TempData["Mensaje"] = "No tenés permiso para acceder a esta sección.";
            //    return RedirectToAction("NoAutorizado", "Home");
            //}

            // Lo uso para obtener la fecha actual 
            string fechaHoy = DateTime.Now.ToString("yyyy-MM-dd");
            var lista = new BLL_Bitacora().Listar();

            ViewBag.TipoUsuario = "";
            ViewBag.FechaInicio = fechaHoy;
            ViewBag.FechaFin = fechaHoy;

            return View(lista);
        }

        [HttpPost]
        public ActionResult Bitacora(string tipoUsuario, string fechaInicio, string fechaFin)
        {
            // Esto lo uso caundo el usuario selecciona "Todos", mandamos null al SP
            string tipoFinal = string.IsNullOrEmpty(tipoUsuario) || tipoUsuario == "Todos" ? null : tipoUsuario;

            var lista = new BLL_Bitacora().Filtrar(tipoFinal, fechaInicio, fechaFin);

            ViewBag.TipoUsuario = tipoUsuario;
            ViewBag.FechaInicio = fechaInicio;
            ViewBag.FechaFin = fechaFin;

            return View(lista);
        }

        // Para Exportar el XML a mi Compu
        public ActionResult ExportarVentasXml()
        {
            try
            {
                // Lo que hago aca es obtener ventas con detalles
                List<VentaExportXML> lista = new BLL_Venta().ListarVentasConDetalles();

                // Implemento una  Serializacion a XML
                XmlSerializer serializer = new XmlSerializer(typeof(List<VentaExportXML>));
                StringWriter writer = new StringWriter();
                serializer.Serialize(writer, lista);
                byte[] bytes = Encoding.UTF8.GetBytes(writer.ToString());

                // Registro todo en la bitacora
                string usuario = Session["Usuario"]?.ToString() ?? "Sin sesión";
                string tipoUsuario = Session["Rol"]?.ToString() ?? "Desconocido";

                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = usuario,
                    TipoUsuario = tipoUsuario,
                    Accion = "Descarga XML",
                    Detalle = $"Se exportaron {lista.Count} ventas en XML desde el sistema."
                });

                // Me devuelve el archivo
                return File(bytes, "application/xml", "ventas.xml");
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, "Error: " + ex.Message);
            }
        }


        [HttpPost]
        public ActionResult ImportarXML(HttpPostedFileBase archivoXml)
        {
            if (archivoXml == null || archivoXml.ContentLength == 0)
            {
                TempData["Error"] = "No se recibió ningún archivo o está vacío.";
                return RedirectToAction("Index");
            }

            try
            {

                using (StreamReader reader = new StreamReader(archivoXml.InputStream, Encoding.UTF8, detectEncodingFromByteOrderMarks: true))
                {
                    string xmlContent = reader.ReadToEnd();

                    // Log para verificar contenido
                    System.Diagnostics.Debug.WriteLine(" XML leído como string:");
                    System.Diagnostics.Debug.WriteLine(xmlContent);

                    // Volvemos a deserializar desde texto
                    XmlSerializer serializer = new XmlSerializer(typeof(List<VentaExportXML>));
                    using (StringReader stringReader = new StringReader(xmlContent))
                    {
                        var listaVentas = (List<VentaExportXML>)serializer.Deserialize(stringReader);

                        foreach (var ventaXml in listaVentas)
                        {
                            new BLL_Venta().ImportarDesdeXML(ventaXml);
                        }

                        // Registro todo en la bitacora
                        string usuario = Session["Usuario"]?.ToString() ?? "Sin sesión";
                        string tipoUsuario = Session["Rol"]?.ToString() ?? "Desconocido";

                        new BLL_Bitacora().Registrar(new Bitacora
                        {
                            Fecha = DateTime.Now,
                            Usuario = usuario,
                            TipoUsuario = tipoUsuario,
                            Accion = "Importación XML",
                            Detalle = $"Se importaron {listaVentas.Count} ventas desde un archivo XML al sistema."
                        });

                    }
                }

                TempData["Mensaje"] = "Importación exitosa.";
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ERROR COMPLETO:");
                System.Diagnostics.Debug.WriteLine(ex.ToString()); // Esto lo utilice para que me muestre si es que hubiera el error completo
                TempData["Error"] = "Error al importar: " + ex.Message;
            }

            return RedirectToAction("Index");
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
}

// Prueba si estoy haciendo bien lo de idiomas
//public ActionResult TestIdioma()
//{
//    try
//    {
//        GestorIdiomas.CargarIdioma("en");
//        string texto = GestorIdiomas.Traducir("dashboard_title");
//        return Content($"Traducción encontrada: {texto}");
//    }
//    catch (Exception ex)
//    {
//        return Content($"Error: {ex.Message}");
//    }
//}