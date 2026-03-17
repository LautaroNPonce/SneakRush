using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using BE;
using System.Data.SqlClient;
using System.Data;

namespace BLL
{
    public class BLL_Carrito
    {
        private DAL_Carrito ObjetoDAL = new DAL_Carrito();

        public bool ExisteCarrito(int idcliente, int idproducto)
        {
            try
            {
                return ObjetoDAL.ExisteCarrito(idcliente, idproducto);
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Carrito.ExisteCarrito()",
                    Detalle = ex.Message
                });

                return false;
            }

        }

        public bool OperacionCarrito(int idcliente, int idproducto, bool sumar, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                return ObjetoDAL.OperacionCarrito(idcliente, idproducto, sumar, out Mensaje);
            }
            catch (Exception ex)
            {
                Mensaje = "Ocurrió un error interno al procesar la operación del carrito.";

                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Carrito.OperacionCarrito()",
                    Detalle = ex.Message
                });

                return false;
            }
        }

        public int CantidadCarrito(int idcliente)
        {
            try
            {
                return ObjetoDAL.CantidadCarrito(idcliente);
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Carrito.CantidadCarrito()",
                    Detalle = ex.Message
                });

                return 0;
            }
        }

        public List<Carrito> ListarProducto(int idcliente)
        {
            try
            {
                return ObjetoDAL.ListarProducto(idcliente);
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Carrito.ListarProducto()",
                    Detalle = ex.Message
                });

                return new List<Carrito>();
            }
        }

        public bool EliminarCarrito(int idcliente, int idproducto)
        {
            try
            {
                return ObjetoDAL.EliminarCarrito(idcliente, idproducto);
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Carrito.EliminarCarrito()",
                    Detalle = ex.Message
                });

                return false;
            }
        }
    }
}
