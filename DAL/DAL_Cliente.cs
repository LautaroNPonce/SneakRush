using BE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_Cliente
    {
        public List<Cliente> Listar()
        {
            List<Cliente> Lista = new List<Cliente>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    string query = "Select IdCliente,Nombres,Apellidos,Correo,Clave,Reestablecer From Cliente";
                    SqlCommand comando = new SqlCommand(query, OConexion);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add
                                (
                                new Cliente()
                                {
                                    IdCliente = Convert.ToInt32(dr["IdCliente"]),
                                    Nombres = dr["Nombres"].ToString(),
                                    Apellidos = dr["Apellidos"].ToString(),
                                    Correo = dr["Correo"].ToString(),
                                    Clave = dr["Clave"].ToString(),
                                    Reestablecer = Convert.ToBoolean(dr["Reestablecer"]),
                                }
                                );
                        }
                    }
                }
            }
            catch
            {
                Lista = new List<Cliente>();
            }

            return Lista;
        }

        public int Registrar(Cliente obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_RegistrarCliente", OConexion);
                    comando.Parameters.AddWithValue("Nombres", obj.Nombres);
                    comando.Parameters.AddWithValue("Apellidos", obj.Apellidos);
                    comando.Parameters.AddWithValue("Correo", obj.Correo);
                    comando.Parameters.AddWithValue("Clave", obj.Clave);
                    comando.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    comando.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    comando.CommandType = CommandType.StoredProcedure;

                    OConexion.Open();
                    comando.ExecuteNonQuery();

                    idautogenerado = Convert.ToInt32(comando.Parameters["Resultado"].Value);
                    Mensaje = comando.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                idautogenerado = 0;
                Mensaje = ex.Message;
            }

            return idautogenerado;
        }

        public bool CambiarContraseña(int idCliente, string nuevacontraseña, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("update cliente set clave = @nuevacontraseña, reestablecer = 0 where idcliente = @id ", OConexion); // Aca lo que hacemos es actualizar la contraseña
                    comando.Parameters.AddWithValue("Id", idCliente);
                    comando.Parameters.AddWithValue("@nuevacontraseña", nuevacontraseña);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();
                    resultado = comando.ExecuteNonQuery() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }

        public bool ReestablecerContraseña(int idCliente, string contraseña, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("update Cliente set clave = @contraseña, reestablecer = 1 where idCliente = @id", OConexion);
                    comando.Parameters.AddWithValue("Id", idCliente);
                    comando.Parameters.AddWithValue("@contraseña", contraseña);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();
                    resultado = comando.ExecuteNonQuery() > 0 ? true : false;
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }

        public bool Modificar(Cliente obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_ModificarCliente", OConexion);
                    comando.Parameters.AddWithValue("IdCliente", obj.IdCliente);
                    comando.Parameters.AddWithValue("Nombres", obj.Nombres);
                    comando.Parameters.AddWithValue("Apellidos", obj.Apellidos);
                    comando.Parameters.AddWithValue("Correo", obj.Correo);
                    comando.Parameters.AddWithValue("Clave", obj.Clave);
                    comando.Parameters.AddWithValue("Reestablecer", obj.Reestablecer);
                    comando.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output;
                    comando.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    comando.CommandType = CommandType.StoredProcedure;

                    OConexion.Open();
                    comando.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(comando.Parameters["Resultado"].Value);
                    Mensaje = comando.Parameters["Mensaje"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }

            return resultado;
        }

        public List<PermisoComponente> ListarPermisosCliente(int idCliente)
        {
            List<PermisoComponente> lista = new List<PermisoComponente>();

            try
            {
                using (SqlConnection oConexion = new SqlConnection(Conexion.BD))
                {
                    string query = @"SELECT P.IdPermiso, P.Nombre, P.Codigo, P.EsFamilia FROM ClientePermiso CP INNER JOIN Permiso P ON P.IdPermiso = CP.IdPermiso WHERE CP.IdCliente = @idCliente";

                    SqlCommand cmd = new SqlCommand(query, oConexion);
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    cmd.CommandType = CommandType.Text;
                    oConexion.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            bool esFamilia = Convert.ToBoolean(dr["EsFamilia"]);

                            if (esFamilia)
                            {
                                lista.Add(new Familia
                                {
                                    Id = Convert.ToInt32(dr["IdPermiso"]),
                                    Nombre = dr["Nombre"].ToString(),
                                    Codigo = dr["Codigo"].ToString()
                                });
                            }
                            else
                            {
                                lista.Add(new Patente
                                {
                                    Id = Convert.ToInt32(dr["IdPermiso"]),
                                    Nombre = dr["Nombre"].ToString(),
                                    Codigo = dr["Codigo"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch
            {
                lista = new List<PermisoComponente>();
            }

            return lista;
        }

        public bool AsignarPermisoACliente(int idCliente, int idPermiso)
        {
            try
            {
                using (SqlConnection oConexion = new SqlConnection(Conexion.BD))
                {
                    string query = "INSERT INTO ClientePermiso (IdCliente, IdPermiso) VALUES (@idCliente, @idPermiso)";
                    SqlCommand cmd = new SqlCommand(query, oConexion);
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);
                    cmd.Parameters.AddWithValue("@idPermiso", idPermiso);

                    oConexion.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                return false;
            }
        }

        public bool EliminarPermisosCliente(int idCliente)
        {
            try
            {
                using (SqlConnection oConexion = new SqlConnection(Conexion.BD))
                {
                    string query = "DELETE FROM ClientePermiso WHERE IdCliente = @idCliente";
                    SqlCommand cmd = new SqlCommand(query, oConexion);
                    cmd.Parameters.AddWithValue("@idCliente", idCliente);

                    oConexion.Open();
                    return cmd.ExecuteNonQuery() >= 0;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
