using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Marca
    {
        private DAL_Marca ObjetoDAL = new DAL_Marca();

        public List<Marca> Listar()
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
                    Accion = "Error en BLL_Marca.Listar()",
                    Detalle = ex.Message
                });

                return new List<Marca>();
            }
        }

        public int Registrar(Marca obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(obj.Descripcion))
                {
                    Mensaje = "La descripción de la marca está vacía. Completar obligatoriamente.";
                    return 0;
                }

                return ObjetoDAL.Registrar(obj, out Mensaje);
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Marca.Registrar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error interno al registrar la marca.";
                return 0;
            }
        }

        public bool Modificar(Marca obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(obj.Descripcion))
                {
                    Mensaje = "La descripción de la marca esta vacia. Completar obligatoriamente.";
                    return false;
                }

                return ObjetoDAL.Modificar(obj, out Mensaje);
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Marca.Modificar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrio un error interno al modificar la marca.";
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
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Marca.Eliminar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error interno al eliminar la marca.";
                return false;
            }
        }

        public List<Marca> ListarMarcaPorCategoria(int idcategoria)
        {
            try
            {
                return ObjetoDAL.ListarMarcaPorCategoria(idcategoria);
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_Marca.ListarMarcaPorCategoria()",
                    Detalle = ex.Message
                });

                return new List<Marca>();
            }
        }
    }
}
