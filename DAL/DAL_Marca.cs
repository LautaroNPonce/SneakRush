using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using BE;

namespace DAL
{
    public class DAL_Marca
    {
        public List<Marca> Listar()
        {
            List<Marca> Lista = new List<Marca>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    string query = "Select IdMarca, Descripcion, Activo From Marca";
                    SqlCommand comando = new SqlCommand(query, OConexion);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add(new Marca()
                            {
                                IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                Descripcion = dr["Descripcion"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"])
                            });
                        }
                    }
                }
            }
            catch
            {
                Lista = new List<Marca>();
            }
            return Lista;
        }

        public int Registrar(Marca obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_RegistrarMarca", OConexion);
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

        public bool Modificar(Marca obj, out string Mensaje)  // obj =  Objeto del tipo usuario
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_ModificarMarca", OConexion);
                    comando.Parameters.AddWithValue("IdMarca", obj.IdMarca);
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
                    SqlCommand comando = new SqlCommand("sp_EliminarMarca", OConexion);
                    comando.Parameters.AddWithValue("IdMarca", id);
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

        public List<Marca> ListarMarcaPorCategoria(int idcategoria)
        {
            List<Marca> lista = new List<Marca>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("select distinct m.IdMarca, m.Descripcion from PRODUCTO p");
                    sb.AppendLine("inner join CATEGORIA c on c.IdCategoria = p.IdCategoria");
                    sb.AppendLine("inner join MARCA m on m.idMarca = p.idmarca and m.Activo = 1");
                    sb.AppendLine("where c.IdCategoria = iif(@idcategoria = 0, c.IdCategoria, @idcategoria)");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@idcategoria", idcategoria);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Marca()
                            {
                                IdMarca = Convert.ToInt32(dr["IdMarca"]),
                                Descripcion = dr["Descripcion"].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Marca>();
            }
            return lista;
        }
    }
}
