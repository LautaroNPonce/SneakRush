using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_Categoria
    {
        private DAL_Categoria ObjetoDAL = new DAL_Categoria();

        public List<Categoria> Listar()
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
                    Accion = "Error en BLL_Categoria.Listar()",
                    Detalle = ex.Message
                });

                return new List<Categoria>();
            }
        }

        public int Registrar(Categoria obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(obj.Descripcion))
                {
                    Mensaje = "La descripcion de la categoría esta vacia. Completar obligatoriamente.";
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
                    Accion = "Error en BLL_Categoria.Registrar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrio un error interno al registrar la categoria.";
                return 0;
            }
        }

        public bool Modificar(Categoria obj, out string Mensaje)
        {
            Mensaje = string.Empty;

            try
            {
                if (string.IsNullOrEmpty(obj.Descripcion))
                {
                    Mensaje = "La descripción de la categoría está vacía. Completar obligatoriamente.";
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
                    Accion = "Error en BLL_Categoria.Modificar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error interno al modificar la categoría.";
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
                    Accion = "Error en BLL_Categoria.Eliminar()",
                    Detalle = ex.Message
                });

                Mensaje = "Ocurrió un error interno al eliminar la categoría.";
                return false;
            }
        }

    }
}


