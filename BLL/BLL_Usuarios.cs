using BE;
using DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Usuarios
    {
        private DAL_Usuarios ObjetoDAL = new DAL_Usuarios();

        public List<Usuario> Listar()
        {
            try
            {
                //throw new Exception("Error de prueba: fallo simulado en BLL_Usuarios.Listar()");
                var lista = ObjetoDAL.Listar();

                // Descifro el correo para usarlo normal
                foreach (var u in lista)
                {
                    if (!string.IsNullOrEmpty(u.Correo))
                    {
                        try
                        {
                            if (BLL_Encriptacion.EsBase64(u.Correo))
                            {
                                u.Correo = BLL_Encriptacion.DesencriptarTexto(u.Correo);
                            }

                        }
                        catch (Exception ex)
                        {
                            // Registra en la bitacora advertencias por un cliente específico 
                            new BLL_Bitacora().Registrar(new Bitacora
                            {
                                Fecha = DateTime.Now,
                                Usuario = "Sistema",
                                TipoUsuario = "Advertencia",
                                Accion = "Error al desencriptar correo en BLL_Usuarios.Listar()",
                                Detalle = $"IdUsuario: {u.IdUsuario}, Correo original: {u.Correo}, Error: {ex.Message}"
                            });
                        }
                    }
                }

                return lista;
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Usuarios.Listar()",
                    Detalle = ex.Message
                });

                return new List<Usuario>();
            }
        }

        public int Registrar(Usuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(obj.Nombres))
                {
                    Mensaje = "El nombre del usuario está vacío. Completar obligatoriamente."; // validaciones
                }
                else if (string.IsNullOrEmpty(obj.Apellidos))
                {
                    Mensaje = "El apellido del usuario está vacío. Completar obligatoriamente.";
                }
                else if (string.IsNullOrEmpty(obj.Correo))
                {
                    Mensaje = "El correo del usuario está vacío. Completar obligatoriamente.";
                }

                if (!string.IsNullOrEmpty(Mensaje))
                {
                    return 0;
                }

                string clave = BLL_Encriptacion.GenerarContraseña(); // Esta clave se va a enviar por a los usuarios y con esto va a tener que acceder
                string asunto = "Creación de cuenta en la página SneakRush";
                string mensaje_correo = "<h3>Su cuenta en SneakRush fue creada exitosamente</h3><br/><p>Su contraseña para acceder es: !clave</p>";
                mensaje_correo = mensaje_correo.Replace("!clave", clave);


                if (!BLL_Encriptacion.EnviarCorreo(obj.Correo, asunto, mensaje_correo))
                {
                    Mensaje = "No se ha podido enviar el correo correctamente";
                    return 0;
                }

                // Cifro el correo antes de guardarlo en BD
                obj.Correo = BLL_Encriptacion.EncriptarTexto(obj.Correo);

                obj.Clave = BLL_Encriptacion.ConvertirClave(clave);

                if (string.IsNullOrEmpty(obj.Rol)) // Esto lo que hace si el rol esta vacion o null por defecto va como "Administrador" sino si el rol ya vino desde el "Webmaster" se mantiene.
                {
                    obj.Rol = "Administrador";
                }

                int idGenerado = ObjetoDAL.Registrar(obj, out Mensaje);

                if (idGenerado > 0)
                {
                    try
                    {
                        BLL_DigitoVerificador dv = new BLL_DigitoVerificador();
                        dv.RecalcularDVH("Usuario", "IdUsuario");
                        dv.RecalcularDVV("Usuario");
                    }
                    catch (Exception ex)
                    {
                        Mensaje += " | Error al recalcular los dígitos verificadores: " + ex.Message;

                        // Bitacora de error general
                        new BLL_Bitacora().Registrar(new Bitacora
                        {
                            Fecha = DateTime.Now,
                            Usuario = "Sistema",
                            TipoUsuario = "Error interno",
                            Accion = "Error en BLL_Usuarios.Registrar() - RecalcularDV",
                            Detalle = ex.Message
                        });

                    }
                }

                return idGenerado;
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Usuarios.Registrar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error inesperado al registrar el usuario.";
                return 0;
            }
        }


        public bool Modificar(Usuario obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try 
            {
                if (string.IsNullOrEmpty(obj.Nombres))
                {
                    Mensaje = "El nombre del usuario está vacío. Completar obligatoriamente.";
                }
                else if (string.IsNullOrEmpty(obj.Apellidos))
                {
                    Mensaje = "El nombre del usuario está vacío. Completar obligatoriamente.";
                }
                else if (string.IsNullOrEmpty(obj.Correo))
                {
                    Mensaje = "El correo del usuario está vacío. Completar obligatoriamente.";
                }
                if (!string.IsNullOrEmpty(Mensaje))
                {
                    return false;
                }

                // Cifro el correo antes de guardar cambios
                obj.Correo = BLL_Encriptacion.EncriptarTexto(obj.Correo);

                bool resultado = ObjetoDAL.Modificar(obj, out Mensaje);

                if (resultado)
                {
                    try
                    {
                        BLL_DigitoVerificador dv = new BLL_DigitoVerificador();
                        dv.RecalcularDVH("Usuario", "IdUsuario");
                        dv.RecalcularDVV("Usuario");
                    }
                    catch (Exception ex)
                    {
                        Mensaje += " | Error al recalcular los dígitos verificadores: " + ex.Message;
                        resultado = false;

                        // Bitacora de error general
                        new BLL_Bitacora().Registrar(new Bitacora
                        {
                            Fecha = DateTime.Now,
                            Usuario = "Sistema",
                            TipoUsuario = "Error interno",
                            Accion = "Error en BLL_Usuarios.Modificar() - RecalcularDV",
                            Detalle = ex.Message
                        });
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Usuarios.Modificar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error inesperado al modificar el usuario.";
                return false;
            }
        }

        public bool Eliminar(int id, out string Mensaje)
        {
            try
            {
                return ObjetoDAL.Eliminar(id, out Mensaje);
            }
            catch (Exception ex)
            {
                Mensaje = "Ocurrió un error al eliminar el usuario.";

                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Usuarios.Eliminar()",
                    Detalle = ex.Message
                });

                return false;
            }
        }

        public bool CambiarContraseña(int idusuario, string nuevacontraseña, out string Mensaje)
        {
            try
            {
                return ObjetoDAL.CambiarContraseña(idusuario, nuevacontraseña, out Mensaje);
            }
            catch (Exception ex)
            {
                Mensaje = "Ocurrió un error al cambiar la contraseña.";

                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Usuarios.CambiarContraseña()",
                    Detalle = ex.Message
                });

                return false;
            }
        }

        public bool ReestablecerContraseña(int idusuario, string correo, out string Mensaje)
        {
            Mensaje = string.Empty;

            try 
            {
                string nuevacontraseña = BLL_Encriptacion.GenerarContraseña();
                bool resultado = ObjetoDAL.ReestablecerContraseña(idusuario, BLL_Encriptacion.ConvertirClave(nuevacontraseña), out Mensaje);

                if (resultado)
                {
                    string asunto = "Su contraseña de la página SneakRush fue reestablecida";
                    string mensaje_correo = "<h3>Su cuenta en SneakRush fue reestablecida exitosamente</h3><br/><p>Su contraseña para acceder ahora es: !clave</p>";
                    mensaje_correo = mensaje_correo.Replace("!clave", nuevacontraseña);
                    bool respuesta = BLL_Encriptacion.EnviarCorreo(correo, asunto, mensaje_correo);

                    if (respuesta)
                    {
                        return true;
                    }
                    else
                    {
                        Mensaje = "No se ha podido enviar el correo correctamente";
                        return false;
                    }
                }
                else
                {
                    Mensaje = "No se ha podido reestablecer la contraseña correctamente";
                    return false;
                }
            }
            catch (Exception ex)
            {
                Mensaje = "Ocurrió un error al restablecer la contraseña.";

                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Usuarios.ReestablecerContraseña()",
                    Detalle = ex.Message
                });

                return false;
            }

        }
        public Usuario ObtenerPorId(int id)
        {
            return new DAL_Usuarios().Listar().FirstOrDefault(u => u.IdUsuario == id);
        }


        public void CargarPermisos(Usuario usuario)
        {
            if (usuario == null) return;

            var bllPermiso = new BLL_Permiso();

            // Cargar permisos directos desde la base
            usuario.Permisos = bllPermiso.ListarPermisosUsuario(usuario.IdUsuario);

            // Expandir familias (Composite) recursivamente
            List<PermisoComponente> permisosExpandidos = new List<PermisoComponente>();

            foreach (var permiso in usuario.Permisos)
            {
                ExpandirPermiso(permiso, permisosExpandidos);
            }

            usuario.Permisos = permisosExpandidos;
        }

        private void ExpandirPermiso(PermisoComponente componente, IList<PermisoComponente> lista)
        {
            lista.Add(componente);

            foreach (var hijo in componente.ObtenerHijos())
            {
                ExpandirPermiso(hijo, lista);
            }
        }
    }
}