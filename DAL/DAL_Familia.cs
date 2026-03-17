using BE;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_Familia
    {
        // Listar todas las familias
        public List<Familia> Listar()
        {
            List<Familia> lista = new List<Familia>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT IdPermiso, Nombre, Codigo");
                    sb.AppendLine("FROM Permiso");
                    sb.AppendLine("WHERE EsFamilia = 1");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Familia()
                            {
                                Id = Convert.ToInt32(dr["IdPermiso"]),
                                Nombre = dr["Nombre"].ToString(),
                                Codigo = dr["Codigo"].ToString()
                            });
                        }
                    }
                }
            }
            catch
            {
                lista = new List<Familia>();
            }

            return lista;
        }

        // 2️⃣ Registrar nueva familia
        public bool Registrar(Familia familia)
        {
            bool resultado = false;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("INSERT INTO Permiso (Nombre, Codigo, EsFamilia)");
                    sb.AppendLine("VALUES (@Nombre, @Codigo, 1)");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@Nombre", familia.Nombre);
                    comando.Parameters.AddWithValue("@Codigo", string.IsNullOrEmpty(familia.Codigo) ? (object)DBNull.Value : familia.Codigo);
                    comando.CommandType = CommandType.Text;

                    OConexion.Open();
                    resultado = comando.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                resultado = false;
            }

            return resultado;
        }

        // Modificar familia existente
        public bool Modificar(Familia familia)
        {
            bool resultado = false;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("UPDATE Permiso");
                    sb.AppendLine("SET Nombre = @Nombre, Codigo = @Codigo");
                    sb.AppendLine("WHERE IdPermiso = @Id AND EsFamilia = 1");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@Nombre", familia.Nombre);
                    comando.Parameters.AddWithValue("@Codigo", string.IsNullOrEmpty(familia.Codigo) ? (object)DBNull.Value : familia.Codigo);
                    comando.Parameters.AddWithValue("@Id", familia.Id);
                    comando.CommandType = CommandType.Text;

                    OConexion.Open();
                    resultado = comando.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                resultado = false;
            }

            return resultado;
        }

        // Eliminar familia (y sus relaciones)
        public bool Eliminar(int idFamilia)
        {
            bool resultado = false;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    OConexion.Open();
                    SqlTransaction trans = OConexion.BeginTransaction();

                    try
                    {
                        // Primero eliminar relaciones en FamiliaPermiso
                        StringBuilder sb1 = new StringBuilder();
                        sb1.AppendLine("DELETE FROM FamiliaPermiso WHERE IdFamilia = @IdFamilia OR IdPermisoHijo = @IdFamilia");

                        SqlCommand cmdRel = new SqlCommand(sb1.ToString(), OConexion, trans);
                        cmdRel.Parameters.AddWithValue("@IdFamilia", idFamilia);
                        cmdRel.ExecuteNonQuery();

                        // Luego eliminar la familia
                        StringBuilder sb2 = new StringBuilder();
                        sb2.AppendLine("DELETE FROM Permiso WHERE IdPermiso = @Id AND EsFamilia = 1");

                        SqlCommand cmdFam = new SqlCommand(sb2.ToString(), OConexion, trans);
                        cmdFam.Parameters.AddWithValue("@Id", idFamilia);
                        cmdFam.ExecuteNonQuery();

                        trans.Commit();
                        resultado = true;
                    }
                    catch
                    {
                        trans.Rollback();
                        resultado = false;
                    }
                }
            }
            catch
            {
                resultado = false;
            }

            return resultado;
        }

        // Agregar permiso (familia o patente) a una familia
        public bool AgregarPermisoAFamilia(int idFamilia, int idPermisoHijo)
        {
            bool resultado = false;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("INSERT INTO FamiliaPermiso (IdFamilia, IdPermisoHijo)");
                    sb.AppendLine("VALUES (@IdFamilia, @IdPermisoHijo)");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@IdFamilia", idFamilia);
                    comando.Parameters.AddWithValue("@IdPermisoHijo", idPermisoHijo);
                    comando.CommandType = CommandType.Text;

                    OConexion.Open();
                    resultado = comando.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                resultado = false;
            }

            return resultado;
        }

        // Quitar permiso (familia o patente) de una familia
        public bool QuitarPermisoDeFamilia(int idFamilia, int idPermisoHijo)
        {
            bool resultado = false;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("DELETE FROM FamiliaPermiso");
                    sb.AppendLine("WHERE IdFamilia = @IdFamilia AND IdPermisoHijo = @IdPermisoHijo");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@IdFamilia", idFamilia);
                    comando.Parameters.AddWithValue("@IdPermisoHijo", idPermisoHijo);
                    comando.CommandType = CommandType.Text;

                    OConexion.Open();
                    resultado = comando.ExecuteNonQuery() > 0;
                }
            }
            catch
            {
                resultado = false;
            }

            return resultado;
        }
    }
}
