using BE;
using BLL;
using DocumentFormat.OpenXml.EMMA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SneakRush.Models;

namespace Presentacion_Administrador.Controllers
{
    public class PermisosAdministradorController : Controller
    {
        private readonly BLL_Usuarios bllUsuarios = new BLL_Usuarios();
        private readonly BLL_Permiso bllPermiso = new BLL_Permiso();
        private readonly BLL_Cliente bllCliente = new BLL_Cliente();

        private bool EsAdministrador()
        {
            return Session["Rol"] != null && Session["Rol"].ToString() == "Administrador";
        }

        public ActionResult Index(string tipo = "", string rol = "")
        {
            if (!EsAdministrador())
                return RedirectToAction("NoAutorizado", "Home");

            var listaFinal = new List<VMEntidadPermisos>();

            var usuarios = bllUsuarios.Listar();
            foreach (var u in usuarios)
            {
                listaFinal.Add(new VMEntidadPermisos
                {
                    Id = u.IdUsuario,
                    Tipo = "Usuario",
                    Nombre = u.Nombres,
                    Apellido = u.Apellidos,
                    Correo = u.Correo,
                    Rol = u.Rol,
                    Permisos = bllPermiso.ListarPermisosUsuario(u.IdUsuario)
                });
            }

            var clientes = bllCliente.Listar();
            foreach (var c in clientes)
            {
                listaFinal.Add(new VMEntidadPermisos
                {
                    Id = c.IdCliente,
                    Tipo = "Cliente",
                    Nombre = c.Nombres,
                    Apellido = c.Apellidos,
                    Correo = c.Correo,
                    Rol = "Cliente",
                    Permisos = bllPermiso.ListarPermisosCliente(c.IdCliente)
                });
            }


            
            if (!string.IsNullOrEmpty(tipo))
                listaFinal = listaFinal.Where(x => x.Tipo == tipo).ToList();

            if (!string.IsNullOrEmpty(rol))
                listaFinal = listaFinal.Where(x => x.Rol == rol).ToList();

            ViewBag.TipoFiltro = tipo;
            ViewBag.RolFiltro = rol;

            return View(listaFinal);
        }


        [HttpGet]
        public ActionResult Editar(int id, string tipo)
        {
            if (!EsAdministrador())
                return RedirectToAction("NoAutorizado", "Home");

            VMEntidadPermisos vm = new VMEntidadPermisos();

            if (tipo == "Usuario")
            {
                var u = bllUsuarios.ObtenerPorId(id);
                if (u == null) return RedirectToAction("Index");

                vm.Id = u.IdUsuario;
                vm.Tipo = "Usuario";
                vm.Nombre = u.Nombres;
                vm.Apellido = u.Apellidos;
                vm.Correo = u.Correo;
                vm.Rol = u.Rol;
                vm.Permisos = bllPermiso.ListarPermisosUsuario(id);
            }
            else if (tipo == "Cliente")
            {
                var c = bllCliente.ObtenerPorId(id);
                if (c == null) return RedirectToAction("Index");

                vm.Id = c.IdCliente;
                vm.Tipo = "Cliente";
                vm.Nombre = c.Nombres;
                vm.Apellido = c.Apellidos;
                vm.Correo = c.Correo;
                vm.Rol = "Cliente";
                vm.Permisos = bllPermiso.ListarPermisosCliente(id);
            }

            // Cargar familias filtradas por rol
            var familiasFiltradas = bllPermiso.ObtenerFamiliasPermitidasPorRol(vm.Rol);

            //Evitar null
            if (familiasFiltradas == null)
                familiasFiltradas = new List<BE.Familia>();
            // Enviar a la vista
            ViewBag.Familias = familiasFiltradas;

            return View(vm);
        }



        [HttpPost]
        public ActionResult GuardarPermisos(int id, string tipo, List<int> PermisosSeleccionados)
        {
            if (!EsAdministrador())
                return RedirectToAction("NoAutorizado", "Home");

            try
            {
                // Buscar entidad segun su tipo
                bool esCliente = tipo == "Cliente";

                var entidad = esCliente
                    ? (object)bllCliente.ObtenerPorId(id)
                    : (object)bllUsuarios.ObtenerPorId(id);

                if (entidad == null)
                {
                    TempData["Error"] = "No se encontró la entidad seleccionada.";
                    return RedirectToAction("Index");
                }

                // Eliminar permisos actuales
                if (esCliente)
                    bllPermiso.EliminarPermisosCliente(id);
                else
                    bllPermiso.EliminarPermisosUsuario(id);

                // Asignar los nuevos (si hay)
                if (PermisosSeleccionados != null)
                {
                    foreach (var idPermiso in PermisosSeleccionados)
                    {
                        if (esCliente)
                            bllPermiso.AsignarPermisoACliente(id, idPermiso);
                        else
                            bllPermiso.AsignarPermisoAUsuario(id, idPermiso);
                    }
                }

                TempData["Mensaje"] = "Permisos actualizados correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al guardar permisos: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        private bool TienePermiso(string codigoPermiso)
        {
            var permisos = Session["Permisos"] as List<PermisoComponente>;
            if (permisos == null) return false;

            BLL_Permiso bll = new BLL_Permiso();

            foreach (var permiso in permisos)
            {
                if (bll.UsuarioTienePermisoRecursivo(permiso, codigoPermiso))
                    return true;
            }

            return false;
        }
    }
}