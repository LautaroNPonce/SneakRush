using BE;
using BLL;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SneakRush.Controllers
{
    /*[Authorize]*/ // Con esto nadie que no este registrado va a poder acceder a ningun de estos forms si es que no se encuentra autorizado
    public class MantenimientoController : Controller, BLL_ObserverIdioma
    {
        public MantenimientoController()
        {
            //  Lo utilizo para suscribir el controlador al sistema de idiomas
            BLL_IdiomaSubject.AgregarObservador(this);
        }

        // Este método lo uso para que se ejecutará automáticamente
        // cuando cambio el idioma desde el ComboBox (JS → BLL_IdiomaSubject)
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

        public ActionResult Categoria()
        {
            if (!EsAdministrador())
            {
                ViewBag.Mensaje = "No tenés permiso para acceder a esta sección.";
                return View("~/Views/Shared/NoAutorizado.cshtml");
            }
            return View();
        }

        public ActionResult Marca()
        {
            if (!EsAdministrador())
            {
                ViewBag.Mensaje = "No tenés permiso para acceder a esta sección.";
                return View("~/Views/Shared/NoAutorizado.cshtml");
            }
            return View();
        }

        public ActionResult Producto()
        {
            if (!EsAdministrador())
            {
                ViewBag.Mensaje = "No tenés permiso para acceder a esta sección.";
                return View("~/Views/Shared/NoAutorizado.cshtml");
            }
            return View();
        }

        // ----  Categoria ---- //

        #region Categoria

        [HttpGet]
        public JsonResult ListarCategoria()
        {
            List<Categoria> olista = new List<Categoria>();
            olista = new BLL_Categoria().Listar();
            return Json(new { data = olista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarCategoria(Categoria obj)
        {
            if (!EsAdministrador())
            {
                return Json(new { resultado = false, mensaje = "No tenés permiso para realizar esta acción." }, JsonRequestBehavior.AllowGet);
            }

            object resultado; // Para alamacenar cualquier tipo de valor
            string mensaje = string.Empty;
            bool esAlta = obj.IdCategoria == 0;

            if (esAlta)
            {
                resultado = new BLL_Categoria().Registrar(obj, out mensaje);
            }
            else
            {
                resultado = new BLL_Categoria().Modificar(obj, out mensaje);
            }

            // Lo use para que si solo fuera exitosa, registramos en bitácora
            if ((esAlta && (int)resultado > 0) || (!esAlta && (bool)resultado))
            {
                string accion = esAlta ? "Alta de categoría" : "Modificación de categoría";
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    TipoUsuario = "Administrador",
                    Accion = accion,
                    Detalle = $"El administrador {User.Identity.Name} realizó una {accion.ToLower()} con nombre '{obj.Descripcion}'.",
                    Usuario = User.Identity.Name
                });
            }

            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarCategoria(int id)
        {
            if (!EsAdministrador())
            {
                return Json(new { resultado = false, mensaje = "No tenés permiso para realizar esta acción." }, JsonRequestBehavior.AllowGet);
            }

            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new BLL_Categoria().Eliminar(id, out mensaje);

            if (respuesta)
            {
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    TipoUsuario = "Administrador",
                    Accion = "Eliminación de categoría",
                    Detalle = $"El administrador {User.Identity.Name} eliminó la categoría con ID {id}.",
                    Usuario = User.Identity.Name
                });
            }

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet); // devuelve la logica obtenida
        }

        #endregion

        // ---- Marca ----- //

        #region Marca

        [HttpGet]
        public JsonResult ListarMarca()
        {
            List<Marca> olista = new List<Marca>();
            olista = new BLL_Marca().Listar();
            return Json(new { data = olista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarMarca(Marca obj)
        {
            if (!EsAdministrador())
            {
                return Json(new { resultado = false, mensaje = "No tenés permiso para realizar esta acción." }, JsonRequestBehavior.AllowGet);
            }

            object resultado;
            string mensaje = string.Empty;
            bool esAlta = obj.IdMarca == 0;

            if (esAlta)
                resultado = new BLL_Marca().Registrar(obj, out mensaje);
            else
                resultado = new BLL_Marca().Modificar(obj, out mensaje);

            if ((esAlta && (int)resultado > 0) || (!esAlta && (bool)resultado))
            {
                string accion = esAlta ? "Alta de marca" : "Modificación de marca";
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    TipoUsuario = "Administrador",
                    Accion = accion,
                    Detalle = $"El administrador {User.Identity.Name} realizó una {accion.ToLower()} con nombre '{obj.Descripcion}'.",
                    Usuario = User.Identity.Name
                });
            }

            return Json(new { resultado = resultado, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult EliminarMarca(int id)
        {
            if (!EsAdministrador())
            {
                return Json(new { resultado = false, mensaje = "No tenés permiso para realizar esta acción." }, JsonRequestBehavior.AllowGet);
            }

            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new BLL_Marca().Eliminar(id, out mensaje);

            if (respuesta)
            {
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    TipoUsuario = "Administrador",
                    Accion = "Eliminación de marca",
                    Detalle = $"El administrador {User.Identity.Name} eliminó la marca con ID {id}.",
                    Usuario = User.Identity.Name
                });
            }

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        // ---- Producto ----- //

        #region Producto

        [HttpGet]
        public JsonResult ListarProducto()
        {
            List<Producto> olista = new List<Producto>();
            olista = new BLL_Producto().Listar();
            return Json(new { data = olista }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GuardarProducto(string objeto, HttpPostedFileBase archivoImagen)
        {
            if (!EsAdministrador())
            {
                return Json(new { resultado = false, mensaje = "No tenés permiso para realizar esta acción." }, JsonRequestBehavior.AllowGet);
            }

            string mensaje = string.Empty;
            bool Operacio_Exitosa = true;
            bool GuardarImagenExitosa = true;

            // Convierto el producto en texto  lo convierto a un obejo producto
            Producto oProducto = JsonConvert.DeserializeObject<Producto>(objeto);
            bool esAlta = oProducto.IdProducto == 0; 

            decimal precio;
            if (decimal.TryParse(oProducto.PrecioSerie, NumberStyles.AllowDecimalPoint, new CultureInfo("en-US"), out precio))
            {
                oProducto.Precio = precio;
            }
            else
            {
                return Json(new { OperacioExitosa = false, mensaje = "El formato del precio debe ser ##:##" }, JsonRequestBehavior.AllowGet);
            }

            if (esAlta)
            {
                int IdProductoGenerado = new BLL_Producto().Registrar(oProducto, out mensaje);

                if (IdProductoGenerado != 0)
                {
                    oProducto.IdProducto = IdProductoGenerado;
                }
                else
                {
                    Operacio_Exitosa = false;
                }
            }
            else
            {
                Operacio_Exitosa = new BLL_Producto().Modificar(oProducto, out mensaje);
            }

            if (Operacio_Exitosa)
            {
                // estas es la bitacora 
                string accion = esAlta ? "Alta de producto" : "Modificación de producto";

                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    TipoUsuario = "Administrador",
                    Accion = accion,
                    Detalle = $"El administrador {User.Identity.Name} realizó una {accion.ToLower()} sobre el producto '{oProducto.Nombre}' (ID: {oProducto.IdProducto})",
                    Usuario = User.Identity.Name
                });


                if (archivoImagen != null)
                {
                    string RutaGuardar = ConfigurationManager.AppSettings["ServidorFotos"];
                    string Extension = Path.GetExtension(archivoImagen.FileName);
                    string NombreImagen = string.Concat(oProducto.IdProducto.ToString(), Extension);

                    try
                    {
                        archivoImagen.SaveAs(Path.Combine(RutaGuardar, NombreImagen));
                    }
                    catch (Exception ex)
                    {
                        string msg = ex.Message;
                        GuardarImagenExitosa = false;
                    }

                    if (GuardarImagenExitosa)
                    {
                        oProducto.RutaImagen = RutaGuardar;
                        oProducto.NombreImagen = NombreImagen;
                        bool respuesta = new BLL_Producto().GuardarDatosImagen(oProducto, out mensaje);
                    }
                    else
                    {
                        mensaje = "El producto se guardó con éxito pero hubo un problema con la imagen";
                    }
                }
            }

            return Json(new { OperacioExitosa = Operacio_Exitosa, IdGenerado = oProducto.IdProducto, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult ImagenProducto(int id) 
        {
            bool conversion;
            Producto oProducto = new BLL_Producto().Listar().Where(p => p.IdProducto == id).FirstOrDefault();
            string textoBase64 = BLL_Encriptacion.ConvertirBase64(Path.Combine(oProducto.RutaImagen, oProducto.NombreImagen), out conversion);

            return Json(new
            {
                conversion = conversion,
                textoBase64 = textoBase64,
                extension = Path.GetExtension(oProducto.NombreImagen)
            },
              JsonRequestBehavior.AllowGet
            );
        }

        public JsonResult EliminarProducto(int id)
        {
            if (!EsAdministrador())
            {
                return Json(new { resultado = false, mensaje = "No tenés permiso para realizar esta acción." }, JsonRequestBehavior.AllowGet);
            }

            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new BLL_Producto().Eliminar(id, out mensaje);

            if (respuesta)
            {
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    TipoUsuario = "Administrador",
                    Accion = "Eliminación de producto",
                    Detalle = $"El administrador {User.Identity.Name} eliminó el producto con ID {id}.",
                    Usuario = User.Identity.Name
                });
            }

            return Json(new { resultado = respuesta, mensaje = mensaje }, JsonRequestBehavior.AllowGet);
        }

        #endregion

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