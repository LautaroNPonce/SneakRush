using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BE.Venta;

namespace DAL
{
    public class DAL_Venta
    {
        public bool Registrar(Venta obj, DataTable DetalleVenta, out int idVenta, out string Mensaje)
        {
            bool respuesta = false;
            Mensaje = string.Empty;
            idVenta = 0;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("usp_RegistrarVenta", OConexion);
                    comando.Parameters.AddWithValue("IdCliente", obj.IdCliente);
                    comando.Parameters.AddWithValue("TotalProducto", obj.TotalProducto);
                    comando.Parameters.AddWithValue("MontoTotal", obj.MontoTotal);
                    comando.Parameters.AddWithValue("Contacto", obj.Contacto);
                    comando.Parameters.AddWithValue("IdLocalidad", obj.IdLocalidad);
                    comando.Parameters.AddWithValue("Telefono", obj.Telefono);
                    comando.Parameters.AddWithValue("Direccion", obj.Direccion);
                    comando.Parameters.AddWithValue("IdTrasferencia", obj.IdTrasferencia);
                    comando.Parameters.AddWithValue("DetalleVenta", DetalleVenta);

                    comando.Parameters.Add("IdVenta", SqlDbType.Int).Direction = ParameterDirection.Output;
                    comando.Parameters.Add("Resultado", SqlDbType.Bit).Direction = ParameterDirection.Output;
                    comando.Parameters.Add("Mensaje", SqlDbType.VarChar, 500).Direction = ParameterDirection.Output;

                    comando.CommandType = CommandType.StoredProcedure;

                    OConexion.Open();
                    comando.ExecuteNonQuery();

                    respuesta = Convert.ToBoolean(comando.Parameters["Resultado"].Value);
                    Mensaje = comando.Parameters["Mensaje"].Value.ToString();

                    if (respuesta)
                    {
                        idVenta = Convert.ToInt32(comando.Parameters["IdVenta"].Value);
                    }
                }
            }
            catch (Exception ex)
            {
                respuesta = false;
                Mensaje = ex.Message;
            }

            return respuesta;
        }


        // Esto es para incrementar el ID de Transaccion: code0001 
        public int ObtenerUltimoIdVenta()
        {
            int id = 0;

            using (SqlConnection con = new SqlConnection(Conexion.BD))
            {
                string query = "SELECT ISNULL(MAX(IdVenta), 0) FROM Venta";

                SqlCommand cmd = new SqlCommand(query, con);
                con.Open();
                id = Convert.ToInt32(cmd.ExecuteScalar());
            }

            return id;
        }

        // Para Exportar el XML a mi Compu
        public List<VentaExportXML> ListarVentasConDetalles()
        {
            var listaVentas = new List<VentaExportXML>();

            using (SqlConnection conn = new SqlConnection(Conexion.BD))
            {
                string query = @" SELECT v.IdVenta, v.FechaVenta, v.IdTrasferencia, c.Nombres + ' ' + c.Apellidos AS Cliente, p.Nombre AS Producto, p.Precio, dv.Cantidad
                FROM Venta v
                INNER JOIN Cliente c ON c.IdCliente = v.IdCliente
                INNER JOIN Detalle_Venta dv ON dv.IdVenta = v.IdVenta
                INNER JOIN Producto p ON p.IdProducto = dv.IdProducto 
                ORDER BY v.IdVenta, p.Nombre";

                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                int? ventaActual = null;
                VentaExportXML venta = null;

                while (reader.Read())
                {
                    int idVenta = Convert.ToInt32(reader["IdVenta"]);

                    if (ventaActual == null || idVenta != ventaActual)
                    {
                        venta = new VentaExportXML
                        {
                            FechaVenta = Convert.ToDateTime(reader["FechaVenta"]),
                            Cliente = reader["Cliente"].ToString(),
                            IdTransaccion = reader["IdTrasferencia"].ToString(),
                            Detalles = new List<DetalleVentaExportXML>()
                        };

                        listaVentas.Add(venta);
                        ventaActual = idVenta;
                    }

                    venta.Detalles.Add(new DetalleVentaExportXML
                    {
                        Producto = reader["Producto"].ToString(),
                        Precio = Convert.ToDecimal(reader["Precio"]),
                        Cantidad = Convert.ToInt32(reader["Cantidad"])
                    });
                }
            }

            return listaVentas;
        }
    }
}
