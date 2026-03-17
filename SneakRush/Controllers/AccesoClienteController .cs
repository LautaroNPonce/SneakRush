using BE;
using BLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SneakRush.Controllers
{
    public class AccesoClienteController : Controller, BLL_ObserverIdioma
    {
        public AccesoClienteController()
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
        public ActionResult Registrar()
        {
            return View();
        }
        public ActionResult Reestablecer()
        {
            return View();
        }
        public ActionResult CambiarContraseña()
        {
            return View();
        }

        public ActionResult SistemaNoDisponible()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Registrar(Cliente objeto)
        {
            int resultado;
            string mensaje = string.Empty;

            // Esto se usa para pasar datos desde un controlador a una vista
            ViewData["Nombres"] = string.IsNullOrEmpty(objeto.Nombres) ? "" : objeto.Nombres;
            ViewData["Apellidos"] = string.IsNullOrEmpty(objeto.Apellidos) ? "" : objeto.Apellidos;
            ViewData["Correo"] = string.IsNullOrEmpty(objeto.Correo) ? "" : objeto.Correo;

            // Esto confirma si las claves que el usuario esta ingresando son correctos
            if (objeto.Clave != objeto.ConfirmarContraseña) 
            {
                ViewBag.Error = "Las contraseñas no coinciden entre si. ¡¡¡Intentalo Nuevamente!!!";
                return View();
            }

            // Esto es la Validacion de Politica Seguridad
            else if (!BLL_Encriptacion.ValidarPoliticaSeguridad(objeto.Clave, out string politicaMensaje))
            {
                ViewBag.Error = politicaMensaje;
                return View();
            }


            resultado = new BLL_Cliente().Registrar(objeto, out mensaje);

            if (resultado > 0)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index", "AccesoCliente");
            }
            else 
            {
                ViewBag.Error = mensaje;
                return View();
            }
        }

        [HttpPost]
        public ActionResult Index(string correo, string contraseña)
        {
            Cliente oCliente = null; 
            oCliente = new BLL_Cliente().Listar().Where(item => item.Correo == correo && item.Clave == BLL_Encriptacion.ConvertirClave(contraseña)).FirstOrDefault();

            if (oCliente == null)
            {
                ViewBag.Error = "El correo o la contraseña no son correctos. ¡¡¡Intentalo Nuevamente!!!";
                return View();
            }
            else
            {
                if (oCliente.Reestablecer)
                {
                    TempData["IdCliente"] = oCliente.IdCliente;
                    return RedirectToAction("CambiarContraseña", "AccesoCliente");
                }
                else
                {
                    // Esto lo uso para verificar la integridad y no este habilitada la pagina web
                    var errores = new BLL_DigitoVerificador().VerificarIntegridad();
                    if (errores.Any())
                    {
                        new BLL_Bitacora().Registrar(new Bitacora
                        {
                            Fecha = DateTime.Now,
                            TipoUsuario = "Usuario",
                            Accion = "Acceso bloqueado",
                            Detalle = $"El cliente {oCliente.Nombres} {oCliente.Apellidos} intentó ingresar pero el sistema no estaba disponible.",
                            Usuario = oCliente.Correo
                        });

                        return RedirectToAction("SistemaNoDisponible", "AccesoCliente");
                    }
                    
                    FormsAuthentication.SetAuthCookie(oCliente.Correo, false);

                    // Permite guardar la info del cliente y puedo obtener la info a traves de todo el proyecto
                    Session["Cliente"] = oCliente;
                    
                    //Agregado: Mantener consistencia con Administrador/ Webmaster
                    Session["IdUsuario"] = oCliente.IdCliente;
                    Session["Usuario"] = oCliente.Correo;
                    Session["Rol"] = "Cliente";
                    Session["NombreCompleto"] = $"{oCliente.Nombres} {oCliente.Apellidos}";
                    Session["Permisos"] = null;

                    var bitacora = new Bitacora
                    {
                        Fecha = DateTime.Now,
                        TipoUsuario = "Usuario",
                        Accion = "Inicio de sesión",
                        Detalle = $"El cliente {oCliente.Nombres} {oCliente.Apellidos} inició sesión.",
                        Usuario = oCliente.Correo
                    };
                    new BLL_Bitacora().Registrar(bitacora);

                    ViewBag.Error = null;
                    return RedirectToAction("Index", "TiendaOnline");

                }
            }
        }

        [HttpPost]
        public ActionResult Reestablecer(string correo)
        {
            Cliente oCliente = new Cliente();
            oCliente = new BLL_Cliente().Listar().Where(item => item.Correo == correo).FirstOrDefault();

            if (oCliente == null)
            {
                ViewBag.Error = "No se encontro el Cliente vinculado a ese correo. ¡¡¡Intentalo Nuevamente!!!";
                return View();
            }

            string mensaje = string.Empty;
            bool respuesta = new BLL_Cliente().ReestablecerContraseña(oCliente.IdCliente, correo, out mensaje);

            if (respuesta)
            {
                ViewBag.Error = null;
                return RedirectToAction("Index", "AccesoCliente");
            }
            else
            {
                ViewBag.Error = mensaje;
                return View();
            }
        }

        public ActionResult CerrarSesion()
        {
            // Sirve para recuperar los datos del cliente para la bitácora
            Cliente cliente = Session["Cliente"] as Cliente;
            string nombre = cliente != null ? $"{cliente.Nombres} {cliente.Apellidos}" : "Desconocido";
            string correo = cliente?.Correo ?? "desconocido@cliente.com";

            var bitacora = new Bitacora
            {
                Fecha = DateTime.Now,
                TipoUsuario = "Usuario",
                Accion = "Cierre de sesión",
                Detalle = $"El cliente {nombre} cerró sesión.",
                Usuario = correo
            };
            new BLL_Bitacora().Registrar(bitacora);

            // Cerrar sesión
            Session["Cliente"] = null;
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "TiendaOnline");
        }

        [HttpPost]
        public ActionResult CambiarContraseña(string idcliente, string contraseñaactual, string nuevacontraseña, string confirmarcontraseña)
        {

            // Validar que el idcliente sea un número válido
            if (!int.TryParse(idcliente, out int idClienteInt))
            {
                ViewBag.Error = "El ID del cliente no es válido.";
                return View();
            }

            // Obtener el cliente por ID
            Cliente oCliente = new BLL_Cliente().Listar().FirstOrDefault(u => u.IdCliente == idClienteInt);

            if (oCliente == null)
            {
                ViewBag.Error = "No se encontró el cliente.";
                return View();
            }

            // Verificar que la contraseña actual sea correcta
            if (oCliente.Clave != BLL_Encriptacion.ConvertirClave(contraseñaactual))
            {
                TempData["IdCliente"] = idClienteInt;
                ViewData["vContraseña"] = ""; // Limpiar campo
                ViewBag.Error = "La contraseña actual no es correcta. ¡Inténtalo nuevamente!";
                return View();
            }

            // Verificar que las contraseñas nuevas coincidan
            if (nuevacontraseña != confirmarcontraseña)
            {
                TempData["IdCliente"] = idClienteInt;
                ViewData["vContraseña"] = contraseñaactual;
                ViewBag.Error = "Las contraseñas no coinciden. ¡Inténtalo nuevamente!";
                return View();
            }

            // Esto es la Validacion de Politica Seguridad
            else if (!BLL_Encriptacion.ValidarPoliticaSeguridad(nuevacontraseña, out string politicaMensaje))
            {
                TempData["IdCliente"] = idClienteInt;
                ViewData["vContraseña"] = contraseñaactual;
                ViewBag.Error = politicaMensaje;
                return View();
            }

            // Encriptar la nueva contraseña
            string contraseñaEncriptada = BLL_Encriptacion.ConvertirClave(nuevacontraseña);

            // Intentar actualizar la contraseña
            string mensaje = string.Empty;
            bool resultado = new BLL_Cliente().CambiarContraseña(idClienteInt, contraseñaEncriptada, out mensaje);

            if (resultado)
            {
                return RedirectToAction("Index");
            }
            else
            {
                TempData["IdCliente"] = idClienteInt;
                ViewBag.Error = mensaje;
                return View();
            }
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
