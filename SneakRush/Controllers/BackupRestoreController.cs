using BE;
using BLL;
using Presentacion_Administrador.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentacion_Administrador.Controllers
{
    public class BackupRestoreController : BaseController
    {

        // Esto lo hice para que evitar que alguien entre manualmente escribiendo la URL y use funciones que no le corresponden.
        private bool UsuarioEsWebmaster()
        {
            return Session["Rol"] != null && Session["Rol"].ToString() == "Webmaster";
        }

        public ActionResult Index()
        {
            if (!UsuarioEsWebmaster())
            {
                ViewBag.Mensaje = "No tenés permiso para acceder a esta sección.";
                return View("~/Views/Shared/NoAutorizado.cshtml");
            }

            return View("BackupRestore");
        }


        [HttpPost]
        public ActionResult Backup()
        {
            if (!UsuarioEsWebmaster())
            {
                return Json(new { exito = false, mensaje = "No tenés permiso para realizar esta acción." });
            }

            try
            {
                string rutaCarpeta = @"C:\Backups\";

                if (!Directory.Exists(rutaCarpeta))
                {
                    Directory.CreateDirectory(rutaCarpeta);
                }

                string nombreArchivo = $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
                string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

                new BLL_BackupRestore().RealizarBackup(rutaCompleta);

                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Accion = "Backup",
                    Detalle = "El webmaster realizó un respaldo de la base de datos.",
                    Usuario = Session["WebmasterCorreo"].ToString(),
                    TipoUsuario = "Webmaster"
                });


                return Json(new { exito = true, mensaje = "Backup realizado correctamente." });
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = "Error al realizar el backup: " + ex.Message });
            }

        }

        [HttpPost]
        public ActionResult Restore(HttpPostedFileBase archivo)
        {
            if (!UsuarioEsWebmaster())
            {
                return Json(new { exito = false, mensaje = "No tenés permiso para realizar esta acción." });
            }

            try
            {
                if (archivo != null && archivo.ContentLength > 0)
                {
                    string rutaArchivo = Path.Combine(@"C:\Backups\", Path.GetFileName(archivo.FileName));
                    archivo.SaveAs(rutaArchivo);

                    new BLL_BackupRestore().RealizarRestore(rutaArchivo);

                    new BLL_Bitacora().Registrar(new Bitacora
                    {
                        Accion = "Restore",
                        Detalle = "El webmaster restauró la base de datos desde un archivo.",
                        Usuario = Session["WebmasterCorreo"].ToString(),
                        TipoUsuario = "Webmaster"
                    });


                    return Json(new { exito = true, mensaje = "Restauración realizada correctamente." });
                }
                else
                {
                    return Json(new { exito = false, mensaje = "Por favor seleccione un archivo .bak válido." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { exito = false, mensaje = "Error al restaurar la base de datos: " + ex.ToString() });
            }
        }
    }

    //public class BackupRestoreController : BaseController
    //{
    //    // 🟩 Vista principal del módulo Backup/Restore
    //    // Requiere permiso de acceso general al área del webmaster
    //    [PermisoRequerido("FAM_WEB")]
    //    public ActionResult Index()
    //    {
    //        return View("BackupRestore");
    //    }

    //    // 🟦 BACKUP DE BASE DE DATOS
    //    [HttpPost]
    //    [PermisoRequerido("WM_BACKUP")]
    //    public ActionResult Backup()
    //    {
    //        try
    //        {
    //            string rutaCarpeta = @"C:\Backups\";

    //            if (!Directory.Exists(rutaCarpeta))
    //            {
    //                Directory.CreateDirectory(rutaCarpeta);
    //            }

    //            string nombreArchivo = $"Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
    //            string rutaCompleta = Path.Combine(rutaCarpeta, nombreArchivo);

    //            new BLL_BackupRestore().RealizarBackup(rutaCompleta);

    //            new BLL_Bitacora().Registrar(new Bitacora
    //            {
    //                Accion = "Backup",
    //                Detalle = "El webmaster realizó un respaldo de la base de datos.",
    //                Usuario = Session["WebmasterCorreo"]?.ToString(),
    //                TipoUsuario = "Webmaster"
    //            });

    //            return Json(new { exito = true, mensaje = "Backup realizado correctamente." });
    //        }
    //        catch (Exception ex)
    //        {
    //            return Json(new { exito = false, mensaje = "Error al realizar el backup: " + ex.Message });
    //        }
    //    }


    //    // 🟥 RESTORE DE BASE DE DATOS
    //    [HttpPost]
    //    [PermisoRequerido("WM_RESTORE")]
    //    public ActionResult Restore(HttpPostedFileBase archivo)
    //    {
    //        try
    //        {
    //            if (archivo != null && archivo.ContentLength > 0)
    //            {
    //                string rutaArchivo = Path.Combine(@"C:\Backups\", Path.GetFileName(archivo.FileName));
    //                archivo.SaveAs(rutaArchivo);

    //                new BLL_BackupRestore().RealizarRestore(rutaArchivo);

    //                new BLL_Bitacora().Registrar(new Bitacora
    //                {
    //                    Accion = "Restore",
    //                    Detalle = "El webmaster restauró la base de datos desde un archivo.",
    //                    Usuario = Session["WebmasterCorreo"]?.ToString(),
    //                    TipoUsuario = "Webmaster"
    //                });

    //                return Json(new { exito = true, mensaje = "Restauración realizada correctamente." });
    //            }
    //            else
    //            {
    //                return Json(new { exito = false, mensaje = "Seleccione un archivo .bak válido." });
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            return Json(new { exito = false, mensaje = "Error al restaurar la base de datos: " + ex.ToString() });
    //        }
    //    }
    //}

}