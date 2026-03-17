using DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using System.Security.Claims;
using System.Data.SqlClient;

namespace BLL
{
    public class BLL_DigitoVerificador
    {
        DAL_DigitoVerificador dal = new DAL_DigitoVerificador();

        public void RecalcularDVH(string tabla, string campoId)
        {
            try 
            {
                var dt = dal.LeerTabla(tabla);
                var ignorar = new List<string> { "DVH", "Reestablecer", "FechaRegistro", "FechaVenta", "Activo" };

                foreach (DataRow row in dt.Rows)
                {
                    string concatenado = "";

                    foreach (DataColumn col in dt.Columns)
                    {
                        if (!ignorar.Contains(col.ColumnName))
                        {
                            concatenado += row[col].ToString();
                        }
                    }

                    string dvh = BLL_Encriptacion.ConvertirClave(concatenado);
                    object id = row[campoId];
                    dal.ActualizarDVH(tabla, campoId, id, dvh);
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
                    Accion = $"Error en BLL_DigitoVerificador.RecalcularDVH() - Tabla: {tabla}",
                    Detalle = ex.Message
                });

                throw new Exception($"No se pudo recalcular el DVH para la tabla {tabla}. Verifique la Bitácora para más detalles.");
            }

        }


        public void RecalcularDVV(string tabla)
        {
            try 
            {
                DataTable dt = dal.LeerTabla(tabla);
                StringBuilder sb = new StringBuilder();

                foreach (DataRow fila in dt.Rows)
                {
                    sb.Append(fila["DVH"].ToString());
                }

                string dvv = BLL_Encriptacion.ConvertirClave(sb.ToString());
                dal.GuardarDVV(tabla, dvv);
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = $"Error en BLL_DigitoVerificador.RecalcularDVV() - Tabla: {tabla}",
                    Detalle = ex.Message
                });

                throw new Exception($"No se pudo recalcular el DVV para la tabla {tabla}. Verifique la Bitácora para más detalles.");
            }
        }

        public void RecalcularTodo()
        {
            try 
            {
                Dictionary<string, string> tablas = new Dictionary<string, string>
            {
                { "Usuario", "IdUsuario" },
                { "Cliente", "IdCliente" },
                { "Producto", "IdProducto" },
                { "Venta", "IdVenta" },
            };

                foreach (var kvp in tablas)
                {
                    RecalcularDVH(kvp.Key, kvp.Value); // Esto es de la tabla y campoId
                    RecalcularDVV(kvp.Key);
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
                    Accion = "Error en BLL_DigitoVerificador.RecalcularTodo()",
                    Detalle = ex.Message
                });

                throw new Exception("Ocurrió un error al recalcular los dígitos verificadores. Revise la Bitácora para más detalles.");
            }

        }

        public List<ErrorVerificacion> VerificarIntegridad()
        {
            var errores = new List<ErrorVerificacion>();

            try 
            {
                var tablas = new List<string> { "Usuario", "Cliente", "Producto", "Venta", };

                foreach (string tabla in tablas)
                {
                    var dt = dal.LeerTabla(tabla);
                    if (dt.Rows.Count == 0) continue;

                    string campoId = "Id" + tabla.Replace("_", "");
                    var ignorar = new List<string> { "DVH", "Reestablecer", "FechaRegistro", "FechaVenta", "Activo" };
                    var sbDVHConcat = new StringBuilder();

                    foreach (DataRow row in dt.Rows)
                    {
                        string concatenado = "";
                        foreach (DataColumn col in dt.Columns)
                        {
                            if (!ignorar.Contains(col.ColumnName))
                                concatenado += row[col].ToString();
                        }

                        string dvhCalculado = BLL_Encriptacion.ConvertirClave(concatenado);
                        string dvhActual = row["DVH"].ToString();

                        if (dvhCalculado != dvhActual)
                        {
                            errores.Add(new ErrorVerificacion
                            {
                                Tabla = tabla,
                                TipoError = "Modificación",
                                IdRegistro = Convert.ToInt32(row[campoId])
                            });
                        }

                        sbDVHConcat.Append(dvhActual);
                    }

                    string dvvCalculado = BLL_Encriptacion.ConvertirClave(sbDVHConcat.ToString());
                    string dvvActual = dal.ObtenerDVV(tabla);

                    if (dvvCalculado != dvvActual)
                    {
                        errores.Add(new ErrorVerificacion
                        {
                            Tabla = tabla,
                            TipoError = "Eliminación",
                            IdRegistro = null
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_DigitoVerificador.VerificarIntegridad()",
                    Detalle = ex.Message
                });
            }

            return errores;
        }
            
    }
}
