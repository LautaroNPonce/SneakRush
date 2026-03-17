using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BLL
{
    public class BLL_Permiso
    {
        private DAL_Permiso dal = new DAL_Permiso();

        // Traigo todos los permisos (familias y patentes) que tiene un usuario
        public List<PermisoComponente> ObtenerPermisosPorUsuario(int idUsuario)
        {
            // Esto ya llama a tu DAL y arma la lista de permisos asignados
            var lista = dal.ListarPermisosUsuario(idUsuario);
            return lista;
        }

        public bool UsuarioTienePermiso(int idUsuario, string codigoPermiso)
        {
            var permisos = ObtenerPermisosPorUsuario(idUsuario);
            return permisos.Any(p => p.Codigo == codigoPermiso);
        }

        public List<Familia> ListarFamiliasConHijos()
        {
            var permisos = dal.Listar();                 
            var familiasBase = permisos.OfType<Familia>().ToList(); 

            List<Familia> resultado = new List<Familia>();

            foreach (var f in familiasBase)
            {
                
                Familia fam = new Familia
                {
                    Id = f.Id,
                    Nombre = f.Nombre,
                    Codigo = f.Codigo
                };

                CargarHijos(fam);

                resultado.Add(fam);
            }

            return resultado;
        }

        private void CargarHijos(Familia familia)
        {
            var hijos = dal.ListarHijos(familia.Id);

            foreach (var hijo in hijos)
            {
                PermisoComponente copia;

                if (hijo is Familia famHijo)
                {
          
                    copia = new Familia
                    {
                        Id = famHijo.Id,
                        Nombre = famHijo.Nombre,
                        Codigo = famHijo.Codigo
                    };

                   
                    CargarHijos((Familia)copia);
                }
                else
                {
                    
                    copia = new Patente
                    {
                        Id = hijo.Id,
                        Nombre = hijo.Nombre,
                        Codigo = hijo.Codigo
                    };
                }

                familia.AgregarHijo(copia);
            }
        }

        public List<Familia> ObtenerFamiliasPermitidasPorRol(string rol)
        {
            // Obtengo lista de códigos segun rol
            List<string> codigosRol;

            switch (rol)
            {
                case "Administrador":
                    codigosRol = CodigosAdmin;
                    break;

                case "Webmaster":
                    codigosRol = CodigosWebmaster;
                    break;

                case "Cliente":
                    codigosRol = CodigosCliente;
                    break;

                default:
                    return new List<Familia>();
            }

            // Traigo todas las familias con sus hijos
            var familias = ListarFamiliasConHijos();

            List<Familia> resultado = new List<Familia>();

            foreach (var f in familias)
            {
                Familia copia = new Familia
                {
                    Id = f.Id,
                    Nombre = f.Nombre,
                    Codigo = f.Codigo
                };

                // Filtro las patentes segun rol
                foreach (var hijo in f.ObtenerHijos())
                {
                    if (codigosRol.Contains(hijo.Codigo))
                        copia.AgregarHijo(hijo);
                }
                resultado.Add(copia);
            }

            return resultado;
        }


        public List<PermisoComponente> Listar()
        {
            return dal.Listar();
        }

        public List<PermisoComponente> ListarPermisosUsuario(int idUsuario)
        {
            return dal.ListarPermisosUsuario(idUsuario);
        }

        public List<PermisoComponente> ListarPermisosCliente(int idCliente)
        {
            return dal.ListarPermisosCliente(idCliente);
        }

        public bool AsignarPermisoAUsuario(int idUsuario, int idPermiso)
        {
            return dal.AsignarPermisoAUsuario(idUsuario, idPermiso);
        }

        public bool EliminarPermisosUsuario(int idUsuario)
        {
            return dal.EliminarPermisosUsuario(idUsuario);
        }

        public bool AsignarPermisoACliente(int idCliente, int idPermiso)
        {
            return dal.AsignarPermisoACliente(idCliente, idPermiso);
        }

        public bool EliminarPermisosCliente(int idCliente)
        {
            return dal.EliminarPermisosCliente(idCliente);
        }

        public void GuardarPermisosUsuario(int idUsuario, int[] permisos)
        {
            EliminarPermisosUsuario(idUsuario);

            foreach (var p in permisos)
                AsignarPermisoAUsuario(idUsuario, p);
        }

        public void GuardarPermisosCliente(int idCliente, int[] permisos)
        {
            EliminarPermisosCliente(idCliente);

            foreach (var p in permisos)
                AsignarPermisoACliente(idCliente, p);
        }

        public bool UsuarioTienePermiso(Usuario usuario, string codigo)
        {
            if (usuario?.Permisos == null)
                return false;

            foreach (var permiso in usuario.Permisos)
            {
                if (TienePermisoRecursivo(permiso, codigo))
                    return true;
            }

            return false;
        }


        private bool TienePermisoRecursivo(PermisoComponente componente, string codigo)
        {
            if (componente.Codigo == codigo)
                return true;

            foreach (var hijo in componente.ObtenerHijos())
            {
                if (TienePermisoRecursivo(hijo, codigo))
                    return true;
            }

            return false;
        }

        private readonly List<string> CodigosAdmin = new List<string>
        {
            "PC_VER_BITACORA","PC_EXPORTAR_XML","PC_IMPORTAR_XML",
            
            "USER_ADMIN_CREAR","USER_ADMIN_MODIFICAR","USER_ADMIN_ELIMINAR",
            
            "CAT_CREAR","CAT_MODIFICAR","CAT_ELIMINAR",
            
            "MARCA_CREAR","MARCA_MODIFICAR","MARCA_ELIMINAR",
            
            "PROD_CREAR","PROD_MODIFICAR","PROD_ELIMINAR",
            
            "PERMISOS_ASIGNAR","FAM_ADMIN"
        };

        private readonly List<string> CodigosWebmaster = new List<string>
        {
            "WM_RECALCULAR_DV","WM_BACKUP","WM_RESTORE",
            
            "WM_CREAR","WM_MODIFICAR","WM_ELIMINAR",
            
            "FAM_WEB"
        };

        private readonly List<string> CodigosCliente = new List<string>
        {
            "CLIENTE_VER_PRODUCTO","CLIENTE_AGREGAR_CARRITO","CLIENTE_COMPRAR","CLIENTE_VER_CARRITO","CLIENTE_VER_HISTORIAL",
            
            "FAM_CLIENT"
        };

        public void AsignarPermisosPorRol(string rol, int idUsuario)
        {
            List<string> codigos;

            switch (rol)
            {
                case "Administrador":
                    codigos = CodigosAdmin;
                    break;
                case "Webmaster":
                    codigos = CodigosWebmaster;
                    break;
                case "Cliente":
                    codigos = CodigosCliente;
                    break;
                default:
                    return;
            }

            var permisos = dal.Listar();

            foreach (var codigo in codigos)
            {
                var permiso = permisos.FirstOrDefault(p => p.Codigo == codigo);
                if (permiso != null)
                    dal.AsignarPermisoAUsuario(idUsuario, permiso.Id);
            }
        }

        public bool UsuarioTienePermisoRecursivo(PermisoComponente comp, string codigo)
        {
            if (comp.Codigo == codigo)
                return true;

            foreach (var hijo in comp.ObtenerHijos())
            {
                if (UsuarioTienePermisoRecursivo(hijo, codigo))
                    return true;
            }
            return false;
        }

    }
}