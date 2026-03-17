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
    public class DAL_Categoria
    {
        public List<Categoria> Listar()
        {
            List<Categoria> Lista = new List<Categoria>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    string query = "Select IdCategoria, Descripcion, Activo From Categoria";
                    SqlCommand comando = new SqlCommand(query, OConexion);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add(new Categoria()
                            {
                                IdCategoria = Convert.ToInt32(dr["idCategoria"]),
                                Descripcion = dr ["Descripcion"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"])
                            });
                        }
                    }
                }
            }
            catch
            {
                Lista = new List<Categoria>();
            }
            return Lista;
        }

        public int Registrar(Categoria obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_RegistrarCategoria", OConexion);
                    comando.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    comando.Parameters.AddWithValue("Activo", obj.Activo);
                    comando.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output; // Parametro de salida
                    comando.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output; // Parametro de salida
                    comando.CommandType = CommandType.StoredProcedure;

                    OConexion.Open();
                    comando.ExecuteNonQuery();

                    idautogenerado = Convert.ToInt32(comando.Parameters["Resultado"].Value); // Retorno de resultado
                    Mensaje = comando.Parameters["Mensaje"].Value.ToString();  // Devuelve el mensaje directamente del procedimiento almacenado
                }
            }
            catch (Exception ex)
            {
                idautogenerado = 0;
                Mensaje = ex.Message;
            }
            return idautogenerado;
        }

        public bool Modificar(Categoria obj, out string Mensaje)  // obj =  Objeto del tipo usuario
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_ModificarCategoria", OConexion);
                    comando.Parameters.AddWithValue("idCategoria", obj.IdCategoria);
                    comando.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    comando.Parameters.AddWithValue("Activo", obj.Activo);
                    comando.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output; // Parametro de salida
                    comando.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output; // Parametro de salida
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

        public bool Eliminar(int id, out string Mensaje)  // obj =  Objeto del tipo usuario
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_EliminarCategoria", OConexion);
                    comando.Parameters.AddWithValue("idCategoria", id);
                    comando.Parameters.Add("Resultado", SqlDbType.Int).Direction = ParameterDirection.Output; // Parametro de salida
                    comando.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output; // Parametro de salida
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
    }
}
