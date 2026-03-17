using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

namespace DAL
{
    public class DAL_Carrito
    {
        public bool ExisteCarrito(int idcliente, int idproducto)
        {
            bool resultado  = true;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_ExisteCarrito", OConexion);
                    comando.Parameters.AddWithValue("IdCliente", idcliente);
                    comando.Parameters.AddWithValue("IdProducto", idproducto);
                    comando.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output; // Es el parametro de salida
                    comando.CommandType = CommandType.StoredProcedure;

                    OConexion.Open();
                    comando.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(comando.Parameters["Resultado"].Value); // Retorno de resultado
                }
            }
            catch (Exception ex)
            {
                resultado = false;
            }
            return resultado;
        }

        public bool OperacionCarrito(int idcliente, int idproducto, bool sumar, out string Mensaje)
        {
            bool resultado = true;
            Mensaje = string.Empty;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_OperacionCarrito", OConexion);
                    comando.Parameters.AddWithValue("IdCliente", idcliente);
                    comando.Parameters.AddWithValue("IdProducto", idproducto);
                    comando.Parameters.AddWithValue("Sumar", sumar);
                    comando.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    comando.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;
                    comando.CommandType = CommandType.StoredProcedure;

                    OConexion.Open();
                    comando.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(comando.Parameters["Resultado"].Value);
                    Mensaje = comando.Parameters["Mensaje"].Value.ToString();  // Devuelve el mensaje directamente del procedimiento almacenado
                }
            }
            catch (Exception ex)
            {
                resultado = false;
                Mensaje = ex.Message;
            }
            return resultado;
        }

        public int CantidadCarrito(int idcliente)
        {
            int resultado = 0;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("Select COUNT (*) from Carrito Where IdCliente = @idcliente", OConexion);
                    comando.Parameters.AddWithValue("@idcliente", idcliente);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();
                    resultado = Convert.ToInt32(comando.ExecuteScalar());
                }
            }
            catch (Exception ex)
            {
                resultado = 0;
            }
            return resultado;
        }

        public List<Carrito> ListarProducto(int idcliente)
        {
            List<Carrito> Lista = new List<Carrito>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();

                    string query = "Select * from fn_ObtenerCarritoCliente(@idcliente)";

                    SqlCommand comando = new SqlCommand(query, OConexion);
                    comando.Parameters.AddWithValue("@idcliente", idcliente);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add(new Carrito()
                            {
                                oProducto = new Producto() 
                                {
                                    IdProducto = Convert.ToInt32(dr["IdProducto"]),
                                    Nombre = dr["Nombre"].ToString(),
                                    Precio = Convert.ToDecimal(dr["Precio"], new CultureInfo("en-US")), // Lo uso para convertir el decimal
                                    RutaImagen = dr["RutaImagen"].ToString(),
                                    NombreImagen = dr["NombreImagen"].ToString(),
                                    oMarca = new Marca() { Descripcion = dr["DesMarca"].ToString() }
                                },
                                Cantidad = Convert.ToInt32(dr["Cantidad"])
                            });
                        }
                    }
                }
            }
            catch
            {
                Lista = new List<Carrito>();
            }
            return Lista;
        }

        public bool EliminarCarrito(int idcliente, int idproducto)
        {
            bool resultado = true;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_EliminarCarrito", OConexion);
                    comando.Parameters.AddWithValue("IdCliente", idcliente);
                    comando.Parameters.AddWithValue("IdProducto", idproducto);
                    comando.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    comando.CommandType = CommandType.StoredProcedure;

                    OConexion.Open();
                    comando.ExecuteNonQuery();

                    resultado = Convert.ToBoolean(comando.Parameters["Resultado"].Value);
                }
            }
            catch (Exception ex)
            {
                resultado = false;
            }
            return resultado;
        }
    }
}
