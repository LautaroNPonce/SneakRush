using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BE;
using System.Web.Security;

namespace SneakRush.Controllers
{
    public class AccesoController : Controller, BLL_ObserverIdioma
    {
        public AccesoController()
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

        [HttpGet]
        public ActionResult VerificarCorreo()
        {
            return View();
        }

        [HttpPost]
        public ActionResult VerificarCorreo(string correo)
        {
            var cliente = new BLL_Cliente().Listar().FirstOrDefault(c => c.Correo == correo);
            var usuario = new BLL_Usuarios().Listar().FirstOrDefault(u => u.Correo == correo);

            if (cliente != null)
            {
                return RedirectToAction("Index", "AccesoCliente");
            }
            else if (usuario != null)
            {
                if (usuario.Rol == "Administrador")
                {
                    return RedirectToAction("Index", "AccesoAdmin");
                }
                else if (usuario.Rol == "Webmaster")
                {
                    return RedirectToAction("Index", "AccesoWebmaster");
                }
            }
            return RedirectToAction("Registrar", "AccesoCliente");
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