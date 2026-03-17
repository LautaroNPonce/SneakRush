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
    public class DAL_Reporte
    {
        public List<Reporte> Ventas(string fechainicio, string fechafin, string idtrasferencia)
        {
            List<Reporte> Lista = new List<Reporte>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    SqlCommand comando = new SqlCommand("sp_ReporteVentas", OConexion);
                    comando.Parameters.AddWithValue("fechainicio", fechainicio);
                    comando.Parameters.AddWithValue("fechafin", fechafin);
                    comando.Parameters.AddWithValue("idtrasferencia", idtrasferencia);
                    comando.CommandType = CommandType.StoredProcedure;

                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add
                                (
                                new Reporte()
                                {
                                    FechaVenta = dr["FechaVenta"].ToString(),
                                    Cliente = dr["Cliente"].ToString(),
                                    Producto = dr["Producto"].ToString(),
                                    Precio = Convert.ToDecimal(dr["Precio"], new CultureInfo("en-US")),
                                    Cantidad = Convert.ToInt32(dr["Cantidad"].ToString()),
                                    Total = Convert.ToDecimal(dr["Total"], new CultureInfo("en-US")),
                                    IdTrasferencia = dr["IdTrasferencia"].ToString()
                                }
                                );
                        }
                    }
                }
            }
            catch
            {
                Lista = new List<Reporte>();
            }

            return Lista;
        }

        public DashBoard VerDashBoards()
        {
            DashBoard objeto = new DashBoard();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {

                    SqlCommand comando = new SqlCommand("sp_ReporteDashboard", OConexion);
                    comando.CommandType = CommandType.StoredProcedure;
                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            objeto = new DashBoard()
                            {
                                TotalCliente = Convert.ToInt32(dr["TotalCliente"]),
                                TotalVenta = Convert.ToInt32(dr["TotalVenta"]),
                                TotalProducto = Convert.ToInt32(dr["TotalProducto"]),
                            };

                        }
                    }
                }
            }
            catch
            {
                objeto = new DashBoard();
            }

            return objeto;
        }
    }
}
