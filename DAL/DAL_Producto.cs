using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using BE;
using System.Globalization;

namespace DAL
{
    public class DAL_Producto
    {
        public List<Producto> Listar()
        {
            List<Producto> Lista = new List<Producto>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();

                    sb.AppendLine("SELECT p.IdProducto, p.Nombre, p.Descripcion,");
                    sb.AppendLine("p.IdMarca, m.Descripcion AS DesMarca,");
                    sb.AppendLine("p.IdCategoria, c.Descripcion AS DesCategoria,");
                    sb.AppendLine("p.Precio, p.Stock, p.RutaImagen, p.NombreImagen, p.Activo");
                    sb.AppendLine("FROM PRODUCTO p");
                    sb.AppendLine("INNER JOIN MARCA m ON m.IdMarca = p.IdMarca");
                    sb.AppendLine("INNER JOIN CATEGORIA c ON c.IdCategoria = p.IdCategoria");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add(new Producto()
                            {
                                IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                Nombre = dr["Nombre"].ToString(),
                                Descripcion = dr["Descripcion"].ToString(),
                                oMarca = new Marca() { IdMarca = Convert.ToInt32(dr["IdMarca"]), Descripcion = dr["DesMarca"].ToString() },
                                oCategoria = new Categoria() { IdCategoria = Convert.ToInt32(dr["IdCategoria"]), Descripcion = dr["DesCategoria"].ToString() },
                                // Convertir el decimal
                                Precio = Convert.ToDecimal(dr["Precio"], new CultureInfo("en-US")), 
                                Stock = Convert.ToInt32(dr["Stock"]),
                                RutaImagen = dr["RutaImagen"].ToString(),
                                NombreImagen = dr["NombreImagen"].ToString(),
                                Activo = Convert.ToBoolean(dr["Activo"])

                            });
                        }
                    }
                }
            }
            catch
            {
                Lista = new List<Producto>();
            }
            return Lista;
        }

        public int Registrar(Producto obj, out string Mensaje)
        {
            int idautogenerado = 0;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_RegistrarProducto", OConexion);
                    comando.Parameters.AddWithValue("Nombre", obj.Nombre);
                    comando.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    comando.Parameters.AddWithValue("IdMarca", obj.oMarca.IdMarca);
                    comando.Parameters.AddWithValue("IdCategoria", obj.oCategoria.IdCategoria);
                    comando.Parameters.AddWithValue("Precio", obj.Precio);
                    comando.Parameters.AddWithValue("Stock", obj.Stock);
                    comando.Parameters.AddWithValue("Activo", obj.Activo);
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

        public bool Modificar(Producto obj, out string Mensaje)
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_ModificarProducto", OConexion);
                    comando.Parameters.AddWithValue("IdProducto", obj.IdProducto);
                    comando.Parameters.AddWithValue("Nombre", obj.Nombre);
                    comando.Parameters.AddWithValue("Descripcion", obj.Descripcion);
                    comando.Parameters.AddWithValue("IdMarca", obj.oMarca.IdMarca);
                    comando.Parameters.AddWithValue("IdCategoria", obj.oCategoria.IdCategoria);
                    comando.Parameters.AddWithValue("Precio", obj.Precio);
                    comando.Parameters.AddWithValue("Stock", obj.Stock);
                    comando.Parameters.AddWithValue("Activo", obj.Activo);
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


        public bool GuardarDatosImagen(Producto obj, out string Mensaje) 
        {
            bool resultado = false;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {

                    string query = "update producto set RutaImagen = @rutaimagen, NombreImagen = @nombreimagen where IdProducto = @idproducto ";

                    SqlCommand comando = new SqlCommand(query, OConexion);
                    comando.Parameters.AddWithValue("@rutaimagen", obj.RutaImagen);
                    comando.Parameters.AddWithValue("@nombreimagen", obj.NombreImagen);
                    comando.Parameters.AddWithValue("@idproducto", obj.IdProducto);
                    comando.CommandType = CommandType.Text;

                    OConexion.Open();
                    if (comando.ExecuteNonQuery() > 0)
                    {
                        resultado = true;
                    }
                    else 
                    {
                        Mensaje = "No se pudo actualizar";
                    }
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
                    SqlCommand comando = new SqlCommand("sp_EliminarProducto", OConexion);
                    comando.Parameters.AddWithValue("IdProducto", id);
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
