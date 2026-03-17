using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Presentacion_Administrador.Controllers
{
    public class AccesoWebmasterController : BaseController, BLL_ObserverIdioma
    {
        public AccesoWebmasterController()
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
            System.Diagnostics.Debug.WriteLine($"Idioma actualizado en AccesoClienteController → {codigoIdioma}");
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CambiarContraseña()
        {
            return View();
        }

        public ActionResult Reestablecer()
        {
            return View();
        }

        public ActionResult AlertaIntegridad()
        {
            var errores = new BLL_DigitoVerificador().VerificarIntegridad();
            return View(errores);
        }

        [HttpPost]
        public ActionResult Index(string correo, string contraseña)
        {
            Usuario oUsuario = new BLL_Usuarios().Listar()
                .FirstOrDefault(u => u.Correo == correo && u.Clave == BLL_Encriptacion.ConvertirClave(contraseña));

            if (oUsuario == null || oUsuario.Rol != "Webmaster")
            {
                // ViewBag = Sirve para guardar info pero que se va a compartir con la misma vista la cual estamos utilizando
                ViewBag.Error = "No tiene permisos para ingresar como webmaster.";
                return View();
            }

            if (oUsuario.Reestablecer)
            {
                // TempData = Sirve para guardar info y compartirlo atraves de multiples vistas que estan dentro de un mismo controlador
                TempData["IdUsuario"] = oUsuario.IdUsuario;
                return RedirectToAction("CambiarContraseña");
            }

            //Es una autenticacion del usuario por su correo 
            FormsAuthentication.SetAuthCookie(oUsuario.Correo, false);

            //Session["Rol"] = oUsuario.Rol;
            //Session["NombreCompleto"] = $"{oUsuario.Nombres} {oUsuario.Apellidos}";
            //Session["WebmasterCorreo"] = oUsuario.Correo;

            Session["IdUsuario"] = oUsuario.IdUsuario;
            Session["Usuario"] = oUsuario.Correo;// Igual que admin/cliente
            Session["Rol"] = oUsuario.Rol;// Webmaster
            Session["NombreCompleto"] = $"{oUsuario.Nombres} {oUsuario.Apellidos}";
            Session["Permisos"] = null;// Forzar recarga en BaseController
            Session["WebmasterCorreo"] = oUsuario.Correo;

            new BLL_Bitacora().Registrar(new Bitacora
            {
                Fecha = DateTime.Now,
                TipoUsuario = "Webmaster",
                Accion = "Inicio de sesión",
                Detalle = $"El webmaster {oUsuario.Nombres} {oUsuario.Apellidos} inició sesión.",
                Usuario = oUsuario.Correo
            });

            // Esto lo que hace es la verificación de integridad
            var errores = new BLL_DigitoVerificador().VerificarIntegridad();
            if (errores.Any())
            {
                TempData["ErroresIntegridad"] = errores;
                return RedirectToAction("AlertaIntegridad");
            }

            return RedirectToAction("Index", "HomeWebmaster");

        }


        [HttpPost]
        public ActionResult CambiarContraseña(string idusuario, string contraseñaactual, string nuevacontraseña, string confirmarcontraseña)
        {
            Usuario oUsuario = new Usuario();
            oUsuario = new BLL_Usuarios().Listar().Where(u => u.IdUsuario == int.Parse(idusuario)).FirstOrDefault();

            if (oUsuario.Clave != BLL_Encriptacion.ConvertirClave(contraseñaactual))
            {
                TempData["IdUsuario"] = idusuario;
                ViewData["vContraseña"] = "";
                ViewBag.Error = "La contraseña actual no es correcta. ¡¡¡Intentalo Nuevamente!!!";
                return View();
            }
            else if (nuevacontraseña != confirmarcontraseña)
            {
                TempData["IdUsuario"] = idusuario;
                ViewData["vContraseña"] = contraseñaactual;
                ViewBag.Error = "Las contraseñas no son correctas. ¡¡¡Intentalo Nuevamente!!!";
                return View();
            }

            // Esto es la Validacion de Politica Seguridad
            else if (!BLL_Encriptacion.ValidarPoliticaSeguridad(nuevacontraseña, out string politicaMensaje))
            {
                TempData["IdUsuario"] = idusuario;
                ViewData["vContraseña"] = contraseñaactual;
                ViewBag.Error = politicaMensaje;
                return View();
            }

            ViewData["vContraseña"] = "";
            nuevacontraseña = BLL_Encriptacion.ConvertirClave(nuevacontraseña);
            string mensaje = string.Empty;
            bool respuesta = new BLL_Usuarios().CambiarContraseña(int.Parse(idusuario), nuevacontraseña, out mensaje);

            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["IdUsuario"] = idusuario;
                ViewBag.Error = mensaje;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Reestablecer(string correo)
        {
            Usuario oUsuario = new Usuario();
            oUsuario = new BLL_Usuarios().Listar().Where(item => item.Correo == correo).FirstOrDefault();

            if (oUsuario == null)
            {
                ViewBag.Error = "No se encontro el usuario vinculado a ese correo. ¡¡¡Intentalo Nuevamente!!!";
                return View();
            }

            string mensaje = string.Empty;
            bool respuesta = new BLL_Usuarios().ReestablecerContraseña(oUsuario.IdUsuario, correo, out mensaje);

            if (respuesta)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index", "AccesoWebmaster");
            }
            else
            {
                ViewBag.Error = mensaje;
                return View();
            }
        }

        public ActionResult CerrarSesion()
        {
            // Buscar al usuario para saber su nombre
            string correo = User.Identity.Name;
            string tipoUsuario = Session["Rol"]?.ToString() ?? "Usuario";
            string nombre = Session["NombreCompleto"]?.ToString() ?? correo;

            var bitacora = new Bitacora
            {
                Fecha = DateTime.Now,
                TipoUsuario = tipoUsuario,
                Accion = "Cierre de sesión",
                Detalle = $"El usuario {nombre} cerró sesión.",
                Usuario = correo
            };
            new BLL_Bitacora().Registrar(bitacora);

            // Esto lo utilizo para limpiar la sesión y autentificar
            Session["Usuario"] = null;
            Session["Rol"] = null;
            Session["NombreCompleto"] = null;
            FormsAuthentication.SignOut();

            // Esto funcion cumple de volver directamente a la tienda online
            return RedirectToAction("Index", "TiendaOnline");

        }

        [HttpPost]
        public ActionResult RecalcularDigitos()
        {
            var bll = new BLL_DigitoVerificador();
            bll.RecalcularTodo();

            new BLL_Bitacora().Registrar(new Bitacora
            {
                Accion = "Recalcular DV",
                Detalle = "El webmaster recalculó los dígitos verificadores horizontales y verticales.",
                Usuario = Session["WebmasterCorreo"].ToString(),
                TipoUsuario = "Webmaster"
            });

            TempData["Mensaje"] = "Dígitos recalculados correctamente.";
            return RedirectToAction("AlertaIntegridad");
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