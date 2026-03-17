using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;


namespace DAL
{
    public class DAL_BackupRestore
    {
        private readonly SqlConnection conexion;
        public DAL_BackupRestore()
        {
            conexion = new SqlConnection(ConfigurationManager.ConnectionStrings["ConexionBD"].ToString());
        }

        public void RealizarBackup(string backupPath)
        {
            try
            {
                string comandoBackup = $"BACKUP DATABASE [SneakRush] TO DISK='{backupPath}'";

                using (SqlCommand cmd = new SqlCommand(comandoBackup, conexion))
                {
                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al realizar el backup: " + ex.Message, ex);
            }
            finally
            {
                conexion.Close();
            }
        }

        public void RealizarRestore(string backupFilePath)
        {
            // Esto lo uso para crear una conexión nueva a la base master
            var cadenaMaster = conexion.ConnectionString.Replace("Initial Catalog=SneakRush", "Initial Catalog=master");

            using (var conexionMaster = new SqlConnection(cadenaMaster))
            {
                try
                {
                    conexionMaster.Open();

                    using (SqlCommand setSingleUser = new SqlCommand("ALTER DATABASE [SneakRush] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;", conexionMaster))
                        setSingleUser.ExecuteNonQuery();

                    string query = $"RESTORE DATABASE [SneakRush] FROM DISK = '{backupFilePath}' WITH REPLACE;";
                    using (SqlCommand cmd = new SqlCommand(query, conexionMaster))
                        cmd.ExecuteNonQuery();

                    using (SqlCommand setMultiUser = new SqlCommand("ALTER DATABASE [SneakRush] SET MULTI_USER;", conexionMaster))
                        setMultiUser.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw new Exception("Error al restaurar la base de datos: " + ex.Message, ex);
                }
            }
        }
    }
}