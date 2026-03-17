using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BE.Venta;


namespace BLL
{
    public class BLL_Venta
    {
        private DAL_Venta ObjetoDAL = new DAL_Venta();

        public bool Registrar(Venta obj, DataTable DetalleVenta, out string Mensaje)
        {
            // Cifro los datos sensibles antes de guardar
            if (!string.IsNullOrEmpty(obj.Telefono))
                obj.Telefono = BLL_Encriptacion.EncriptarTexto(obj.Telefono);

            if (!string.IsNullOrEmpty(obj.Direccion))
                obj.Direccion = BLL_Encriptacion.EncriptarTexto(obj.Direccion);


            bool resultado = ObjetoDAL.Registrar(obj, DetalleVenta, out int idVenta, out Mensaje);

            try 
            {
                if (resultado)
                {
                    try
                    {
                        BLL_DigitoVerificador dv = new BLL_DigitoVerificador();
                        dv.RecalcularDVH("Venta", "IdVenta");
                        dv.RecalcularDVV("Venta");
                    }
                    catch (Exception ex)
                    {
                        Mensaje += " | Error al actualizar los dígitos verificadores: " + ex.Message;
                        resultado = false;
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
                    Accion = "Error en BLL_Venta.Registrar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error interno al registrar la venta.";
                return false;
            }
        }

        // Esto es para incrementar el ID de Transaccion: code0001 
        public int ObtenerUltimoIdVenta()
        {
            try
            {
                return new DAL_Venta().ObtenerUltimoIdVenta();
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Venta.ObtenerUltimoIdVenta()",
                    Detalle = ex.Message
                });

                return 0;
            }
        }

        public List<Venta> ListarPorCliente(int idCliente)
        {
            try 
            {
                List<Venta> lista = new List<Venta>();

                using (SqlConnection conn = new SqlConnection(Conexion.BD))
                {
                    string query = @"SELECT IdVenta, MontoTotal, FechaVenta, IdTrasferencia
                         FROM Venta
                         WHERE IdCliente = @IdCliente
                         ORDER BY FechaVenta DESC";

                    SqlCommand cmd = new SqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@IdCliente", idCliente);

                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        lista.Add(new Venta
                        {
                            IdVenta = Convert.ToInt32(reader["IdVenta"]),
                            MontoTotal = Convert.ToDecimal(reader["MontoTotal"]),
                            FechaVenta = Convert.ToDateTime(reader["FechaVenta"]),
                            IdTrasferencia = reader["IdTrasferencia"].ToString()
                        });
                    }
                }

                // Desifro los datos sensibles antes de devolver la lista
                foreach (var v in lista)
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(v.Telefono) && BLL_Encriptacion.EsBase64(v.Telefono)) 
                        {
                            v.Telefono = BLL_Encriptacion.DesencriptarTexto(v.Telefono);
                        }

                        if (!string.IsNullOrEmpty(v.Direccion) && BLL_Encriptacion.EsBase64(v.Direccion)) 
                        {
                            v.Direccion = BLL_Encriptacion.DesencriptarTexto(v.Direccion);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Si hay error al desencriptar, lo dejamos como está y registramos en bitácora
                        new BLL_Bitacora().Registrar(new Bitacora
                        {
                            Fecha = DateTime.Now,
                            Usuario = "Sistema",
                            TipoUsuario = "Error interno",
                            Accion = "Error al desencriptar datos sensibles en BLL_Venta.ListarPorCliente()",
                            Detalle = $"IdVenta: {v.IdVenta}, Error: {ex.Message}"
                        });
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
                    Accion = "Error en BLL_Venta.ListarPorCliente()",
                    Detalle = ex.Message
                });

                return new List<Venta>();
            }

        }

        // Para Exportar el XML a mi Compu
        public List<VentaExportXML> ListarVentasConDetalles()
        {
            try
            {
                return ObjetoDAL.ListarVentasConDetalles();
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Venta.ListarVentasConDetalles()",
                    Detalle = ex.Message
                });

                return new List<VentaExportXML>();
            }
        }

        // Para Importar el XML al programa
        public void ImportarDesdeXML(VentaExportXML ventaXml)
        {
            try 
            {
                // Buscar cliente
                Cliente cliente = new BLL_Cliente().Listar().FirstOrDefault(
                    c => (c.Nombres + " " + c.Apellidos).ToLower() == ventaXml.Cliente.ToLower());

                if (cliente == null) throw new Exception("Cliente no encontrado: " + ventaXml.Cliente);

                // Armar venta
                Venta venta = new Venta
                {
                    IdCliente = cliente.IdCliente,
                    FechaVenta = ventaXml.FechaVenta,
                    IdTrasferencia = ventaXml.IdTransaccion,
                    TotalProducto = ventaXml.Detalles.Count,
                    MontoTotal = ventaXml.Detalles.Sum(x => x.Total),
                    Contacto = "", // o null si tu base lo permite
                    IdLocalidad = "", // por defecto si no tenés el dato en XML
                    Telefono = "",
                    Direccion = ""
                };

                // Crear DataTable para detalles
                DataTable detalleTable = new DataTable();
                detalleTable.Columns.Add("IdProducto", typeof(int));
                detalleTable.Columns.Add("Precio", typeof(decimal));
                detalleTable.Columns.Add("Cantidad", typeof(int));

                foreach (var item in ventaXml.Detalles)
                {
                    var producto = new BLL_Producto().Listar().FirstOrDefault(
                        p => p.Nombre.ToLower() == item.Producto.ToLower());

                    if (producto == null) throw new Exception("Producto no encontrado: " + item.Producto);

                    detalleTable.Rows.Add(producto.IdProducto, item.Precio, item.Cantidad);
                }

                string mensaje;
                int idVenta;

                bool exito = new DAL_Venta().Registrar(venta, detalleTable, out idVenta, out mensaje);

                if (!exito)
                {
                    throw new Exception("Error al registrar venta desde XML: " + mensaje);
                }
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Venta.ImportarDesdeXML()",
                    Detalle = ex.Message
                });

                throw new Exception("Ocurrió un error interno al importar la venta desde XML: " + ex.Message);
            }

        }
    }
}
