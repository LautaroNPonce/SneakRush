using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using System.Data.SqlClient;
using System.Data;


namespace DAL
{
    public class DAL_Usuarios
    {
        public List<Usuario> Listar() 
        {
            List<Usuario> Lista = new List<Usuario>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    string query = "SELECT IdUsuario, Nombres, Apellidos, Correo, Clave, Reestablecer, Activo, Rol FROM Usuario";
                    SqlCommand comando = new SqlCommand(query, OConexion);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();
                    
                    using (SqlDataReader dr = comando.ExecuteReader() ) 
                    {
                        while (dr.Read())
                        {
                            Lista.Add
                                (
                                new Usuario()
                                {
                                    IdUsuario = Convert.ToInt32(dr["IdUsuario"]), 
                                    Nombres = dr["Nombres"].ToString(),
                                    Apellidos = dr["Apellidos"].ToString(),
                                    Correo = dr["Correo"].ToString(),
                                    Clave = dr["Clave"].ToString(),
                                    Reestablecer = Convert.ToBoolean(dr["Reestablecer"]),
                                    Activo = Convert.ToBoolean(dr["Activo"]),
                                    Rol = dr["Rol"].ToString(),
                                });
                        }
                    }
                }
            }
            catch 
            {
                Lista = new List<Usuario>();
            }

            return Lista;
        }

        public int Registrar(Usuario obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_RegistrarUsuario", OConexion);
                    comando.Parameters.AddWithValue("Nombres", obj.Nombres);
                    comando.Parameters.AddWithValue("Apellidos", obj.Apellidos);
                    comando.Parameters.AddWithValue("Correo", obj.Correo);
                    comando.Parameters.AddWithValue("Clave", obj.Clave);
                    comando.Parameters.AddWithValue("Activo", obj.Activo);
                    comando.Parameters.AddWithValue("Rol", obj.Rol);
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

        public bool Modificar(Usuario obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_ModificarUsuario", OConexion);
                    comando.Parameters.AddWithValue("IdUsuario", obj.IdUsuario);
                    comando.Parameters.AddWithValue("Nombres", obj.Nombres);
                    comando.Parameters.AddWithValue("Apellidos", obj.Apellidos);
                    comando.Parameters.AddWithValue("Correo", obj.Correo);
                    comando.Parameters.AddWithValue("Activo", obj.Activo);
                    comando.Parameters.AddWithValue("Rol", obj.Rol);
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

        public bool Eliminar (int id, out string Mensaje) 
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD)) 
                {
                    SqlCommand comando = new SqlCommand("delete top (1) from Usuario where IdUsuario = @id", OConexion);
                    comando.Parameters.AddWithValue("Id", id);
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

        public bool CambiarContraseña(int idusuario,string nuevacontraseña ,out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("update usuario set clave = @nuevacontraseña, reestablecer = 0 where idusuario = @id ", OConexion); // Aca lo que hacemos es actualizar la contraseña
                    comando.Parameters.AddWithValue("Id", idusuario);
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

        public bool ReestablecerContraseña(int idusuario, string contraseña, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("update usuario set clave = @contraseña, reestablecer = 1 where idusuario = @id", OConexion);
                    comando.Parameters.AddWithValue("Id", idusuario);
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

    }
}
