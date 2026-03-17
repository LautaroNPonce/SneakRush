using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Producto
    {
        private DAL_Producto ObjetoDAL = new DAL_Producto();

        public List<Producto> Listar()
        {
            try
            {
                return ObjetoDAL.Listar();
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Producto.Listar()",
                    Detalle = ex.Message
                });

                return new List<Producto>();
            }
        }

        public int Registrar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(obj.Nombre) || string.IsNullOrEmpty(obj.Nombre))
                {
                    Mensaje = "El nombre del Producto esta vacia. Completar Obligatoriamente ";
                }

                else if (string.IsNullOrEmpty(obj.Descripcion) || string.IsNullOrEmpty(obj.Descripcion))
                {
                    Mensaje = "La descripcion del Producto esta vacia. Completar Obligatoriamente ";
                }

                else if (obj.oMarca.IdMarca == 0)
                {
                    Mensaje = "Debe seleccionar una marca. Completar Obligatoriamente ";
                }

                else if (obj.oCategoria.IdCategoria == 0)
                {
                    Mensaje = "Debe seleccionar una categoria. Completar Obligatoriamente ";
                }

                else if (obj.Precio == 0)
                {
                    Mensaje = "Debe ingresar el precio del producto. Completar Obligatoriamente ";
                }

                else if (obj.Stock == 0)
                {
                    Mensaje = "Debe ingresar el stock del producto. Completar Obligatoriamente ";
                }

                if (string.IsNullOrEmpty(Mensaje))
                {
                    return 0;
                }

                int idGenerado = ObjetoDAL.Registrar(obj, out Mensaje);

                if (idGenerado > 0)
                {
                    try
                    {
                        BLL_DigitoVerificador dv = new BLL_DigitoVerificador();
                        dv.RecalcularDVH("Producto", "IdProducto");
                        dv.RecalcularDVV("Producto");
                    }
                    catch (Exception ex)
                    {
                        Mensaje += " | Error al actualizar los dígitos verificadores: " + ex.Message;
                    }
                }

                return idGenerado;
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Producto.Registrar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error interno al registrar el producto.";
                return 0;
            }
        }

        public bool Modificar(Producto obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(obj.Nombre))
                {
                    Mensaje = "El nombre del Producto está vacío.";
                }
                else if (string.IsNullOrEmpty(obj.Descripcion))
                {
                    Mensaje = "La descripción del Producto está vacía.";
                }
                else if (obj.oMarca.IdMarca == 0)
                {
                    Mensaje = "Debe seleccionar una marca.";
                }
                else if (obj.oCategoria.IdCategoria == 0)
                {
                    Mensaje = "Debe seleccionar una categoría.";
                }
                else if (obj.Precio == 0)
                {
                    Mensaje = "Debe ingresar el precio del producto.";
                }
                else if (obj.Stock == 0)
                {
                    Mensaje = "Debe ingresar el stock del producto.";
                }

                if (!string.IsNullOrEmpty(Mensaje))
                {
                    return false;
                }

                bool resultado = ObjetoDAL.Modificar(obj, out Mensaje);

                if (resultado)
                {
                    try
                    {
                        BLL_DigitoVerificador dv = new BLL_DigitoVerificador();
                        dv.RecalcularDVH("Producto", "IdProducto");
                        dv.RecalcularDVV("Producto");
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
                    Accion = "Error en BLL_Producto.Modificar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error interno al modificar el producto.";
                return false;
            }

        }

        public bool Eliminar(int id, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                return ObjetoDAL.Eliminar(id, out Mensaje);
            }
            catch (Exception ex)
            {
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    // Bitacora de error general
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Producto.Eliminar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error interno al eliminar el producto.";
                return false;
            }
        }

        public bool GuardarDatosImagen(Producto obj, out string Mensaje) 
        {
            Mensaje = string.Empty;

            try
            {
                return ObjetoDAL.GuardarDatosImagen(obj, out Mensaje);
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Producto.GuardarDatosImagen()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error interno al guardar los datos de la imagen.";
                return false;
            }
        }

        public List<ProductoStockDTO> ListarConStock()
        {
            try 
            {
                return Listar()
                .Where(p => p.Stock > 0)
                .Select(p => new ProductoStockDTO
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Marca = p.oMarca?.Descripcion ?? "",
                    Categoria = p.oCategoria?.Descripcion ?? "",
                    Precio = p.Precio,
                    Stock = p.Stock
                })
                .ToList();
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Producto.ListarConStock()",
                    Detalle = ex.Message
                });

                return new List<ProductoStockDTO>();
            }
        }
    }
}
