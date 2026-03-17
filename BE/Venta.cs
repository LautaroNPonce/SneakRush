using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdCliente { get; set; }
        public int TotalProducto { get; set; }
        public decimal MontoTotal { get; set; }
        public string Contacto { get; set; }
        public string IdLocalidad { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public string IdTrasferencia { get; set; }
        public DateTime FechaVenta { get; set; }

        public List<DetalleVenta> oDetalleVentas { get; set; }

    }

    public class VentaExportXML
    {
        public DateTime FechaVenta { get; set; }
        public string Cliente { get; set; }
        public string IdTransaccion { get; set; }
        public List<DetalleVentaExportXML> Detalles { get; set; }
    }

    public class DetalleVentaExportXML
    {
        public string Producto { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
        public decimal Total => Precio * Cantidad;
    }
}
