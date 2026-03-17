using BE;
using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class BLL_BackupRestore
    {
        private readonly DAL_BackupRestore dal = new DAL_BackupRestore();

        public void RealizarBackup(string backupPath)
        {
            try
            {
                dal.RealizarBackup(backupPath);
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_BackupRestore.RealizarBackup()",
                    Detalle = ex.Message
                });

                // Lanza un error controlado para el Controller
                throw new Exception("No se pudo realizar el backup correctamente. Verifique la Bitácora para más detalles.");
            }
        }
        public void RealizarRestore(string ruta)
        {
            try
            {
                dal.RealizarRestore(ruta);
            }
            catch (Exception ex)
            {
                // Bitacora de error general
                new BLL_Bitacora().Registrar(new Bitacora
                {
                    Fecha = DateTime.Now,
                    Usuario = "Sistema",
                    TipoUsuario = "Error interno",
                    Accion = "Error en BLL_BackupRestore.RealizarRestore()",
                    Detalle = ex.Message
                });

                // Lanza un error controlado
                throw new Exception("No se pudo restaurar la base de datos. Consulte la Bitácora para más información.");
            }
        }
    }
}
