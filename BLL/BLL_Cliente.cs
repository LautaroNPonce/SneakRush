using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Cliente
    {
        private DAL_Cliente ObjetoDAL = new DAL_Cliente();
        private DAL_Permiso dal = new DAL_Permiso();

        public int Registrar(Cliente obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try 
            {
                if (string.IsNullOrEmpty(obj.Nombres))
                {
                    Mensaje = "El nombre del cliente está vacío. Completar obligatoriamente.";
                }
                else if (string.IsNullOrEmpty(obj.Apellidos))
                {
                    Mensaje = "El apellido del cliente está vacío. Completar obligatoriamente.";
                }
                else if (string.IsNullOrEmpty(obj.Correo))
                {
                    Mensaje = "El correo del cliente está vacío. Completar obligatoriamente.";
                }

                if (!string.IsNullOrEmpty(Mensaje)) 
                {
                    return 0;
                }

                // Cifro el correo antes de guardar
                obj.Correo = BLL_Encriptacion.EncriptarTexto(obj.Correo);

                obj.Clave = BLL_Encriptacion.ConvertirClave(obj.Clave);

                int idGenerado = ObjetoDAL.Registrar(obj, out Mensaje);

                if (idGenerado > 0)
                {
                    try
                    {
                        BLL_DigitoVerificador dv = new BLL_DigitoVerificador();
                        dv.RecalcularDVH("Cliente", "IdCliente");
                        dv.RecalcularDVV("Cliente");
                    }
                    catch (Exception ex)
                    {
                        Mensaje += " | Error al recalcular DVH/DVV: " + ex.Message;
                    }
                }

                return idGenerado;
            }
            catch (Exception ex)
            {
                // Bitacora de errores internos
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Cliente.Registrar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error interno al registrar el cliente.";
                return 0;
            }

        }
        
        public List<Cliente> Listar()
        {
            var lista = new List<Cliente>();

            try
            {
                lista = ObjetoDAL.Listar();

                // Descifro el correo para usarlo normal
                foreach (var c in lista)
                {
                    if (!string.IsNullOrEmpty(c.Correo))
                    {
                        try
                        {
                            if (BLL_Encriptacion.EsBase64(c.Correo))
                            {
                                c.Correo = BLL_Encriptacion.DesencriptarTexto(c.Correo);
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
                                Accion = "Error al desencriptar correo en BLL_Cliente.Listar()",
                                Detalle = $"IdCliente: {c.IdCliente}, Correo: {c.Correo}, Error: {ex.Message}"
                            });
                        }
                    }
                }

                return lista;
            }
            catch (Exception ex)
            {
                // Bitacora de errores internos
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Cliente.Listar()",
                    Detalle = ex.Message
                });

                return new List<Cliente>();
            }
        }

        public bool CambiarContraseña(int idCliente, string nuevacontraseña, out string Mensaje)
        {
            Mensaje = string.Empty;
            try
            {
                return ObjetoDAL.CambiarContraseña(idCliente, nuevacontraseña, out Mensaje);
            }
            catch (Exception ex)
            {
                // Bitacora de errores internos
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Cliente.CambiarContraseña()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error interno al cambiar la contraseña.";
                return false;
            }
        }

        public bool ReestablecerContraseña(int idCliente, string correo, out string Mensaje)
        {
            Mensaje = string.Empty;

            try 
            {
                string nuevacontraseña = BLL_Encriptacion.GenerarContraseña();
                bool resultado = ObjetoDAL.ReestablecerContraseña(idCliente, BLL_Encriptacion.ConvertirClave(nuevacontraseña), out Mensaje);

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
                // Bitacora de errores internos
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Cliente.ReestablecerContraseña()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error interno al reestablecer la contraseña.";
                return false;
            }

        }

        public bool Modificar(Cliente obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try 
            {
                if (string.IsNullOrEmpty(obj.Nombres))
                {
                    Mensaje = "El nombre del cliente está vacío.";
                }
                else if (string.IsNullOrEmpty(obj.Apellidos))
                {
                    Mensaje = "El apellido del cliente está vacío.";
                }
                else if (string.IsNullOrEmpty(obj.Correo))
                {
                    Mensaje = "El correo del cliente está vacío.";
                }

                if (!string.IsNullOrEmpty(Mensaje))
                {
                    return false;
                }

                // Cifro el correo antes de modificar
                obj.Correo = BLL_Encriptacion.EncriptarTexto(obj.Correo);
                bool resultado = ObjetoDAL.Modificar(obj, out Mensaje);

                if (resultado)
                {
                    try
                    {
                        BLL_DigitoVerificador dv = new BLL_DigitoVerificador();
                        dv.RecalcularDVH("Cliente", "IdCliente");
                        dv.RecalcularDVV("Cliente");
                    }
                    catch (Exception ex)
                    {
                        Mensaje += " | Error al recalcular DVH/DVV: " + ex.Message;
                        resultado = false;
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                // Bitacora de errores internos
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Cliente.Modificar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error interno al modificar el cliente.";
                return false;
            }
        }

        public List<PermisoComponente> ListarPermisosCliente(int idCliente)
        {
            return dal.ListarPermisosCliente(idCliente);
        }

        public bool AsignarPermisoACliente(int idCliente, int idPermiso)
        {
            return dal.AsignarPermisoACliente(idCliente, idPermiso);
        }

        public bool EliminarPermisosCliente(int idCliente)
        {
            return dal.EliminarPermisosCliente(idCliente);
        }
        public Cliente ObtenerPorId(int id)
        {
            return Listar().FirstOrDefault(c => c.IdCliente == id);
        }
    }
}
