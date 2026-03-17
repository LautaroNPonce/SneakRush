using BE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DAL
{
    public class DAL_Patente
    {
        // Listar todas las patentes
        public List<Patente> Listar()
        {
            List<Patente> lista = new List<Patente>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT IdPermiso, Nombre, Codigo");
                    sb.AppendLine("FROM Permiso");
                    sb.AppendLine("WHERE EsFamilia = 0");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.CommandType = System.Data.CommandType.Text;
                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            lista.Add(new Patente()
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
                lista = new List<Patente>();
            }

            return lista;
        }

        // Registrar nueva patente
        public bool Registrar(Patente patente)
        {
            bool resultado = false;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("INSERT INTO Permiso (Nombre, Codigo, EsFamilia)");
                    sb.AppendLine("VALUES (@Nombre, @Codigo, 0)");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@Nombre", patente.Nombre);
                    comando.Parameters.AddWithValue("@Codigo",
                        string.IsNullOrEmpty(patente.Codigo) ? (object)DBNull.Value : patente.Codigo);
                    comando.CommandType = System.Data.CommandType.Text;

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

        // Modificar patente existente
        public bool Modificar(Patente patente)
        {
            bool resultado = false;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("UPDATE Permiso");
                    sb.AppendLine("SET Nombre = @Nombre, Codigo = @Codigo");
                    sb.AppendLine("WHERE IdPermiso = @Id AND EsFamilia = 0");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@Nombre", patente.Nombre);
                    comando.Parameters.AddWithValue("@Codigo",
                        string.IsNullOrEmpty(patente.Codigo) ? (object)DBNull.Value : patente.Codigo);
                    comando.Parameters.AddWithValue("@Id", patente.Id);
                    comando.CommandType = System.Data.CommandType.Text;

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

        // Eliminar patente
        public bool Eliminar(int idPatente)
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
                        // Eliminar relaciones con familias antes de borrar la patente
                        StringBuilder sb1 = new StringBuilder();
                        sb1.AppendLine("DELETE FROM FamiliaPermiso WHERE IdPermisoHijo = @IdPatente");

                        SqlCommand cmdRel = new SqlCommand(sb1.ToString(), OConexion, trans);
                        cmdRel.Parameters.AddWithValue("@IdPatente", idPatente);
                        cmdRel.ExecuteNonQuery();

                        // Eliminar la patente
                        StringBuilder sb2 = new StringBuilder();
                        sb2.AppendLine("DELETE FROM Permiso WHERE IdPermiso = @Id AND EsFamilia = 0");

                        SqlCommand cmdPat = new SqlCommand(sb2.ToString(), OConexion, trans);
                        cmdPat.Parameters.AddWithValue("@Id", idPatente);
                        cmdPat.ExecuteNonQuery();

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

        // Buscar patente por código
        public Patente BuscarPorCodigo(string codigo)
        {
            Patente patente = null;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT IdPermiso, Nombre, Codigo");
                    sb.AppendLine("FROM Permiso");
                    sb.AppendLine("WHERE EsFamilia = 0 AND Codigo = @Codigo");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@Codigo", codigo);
                    comando.CommandType = System.Data.CommandType.Text;

                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            patente = new Patente()
                            {
                                Id = Convert.ToInt32(dr["IdPermiso"]),
                                Nombre = dr["Nombre"].ToString(),
                                Codigo = dr["Codigo"].ToString()
                            };
                        }
                    }
                }
            }
            catch
            {
                patente = null;
            }

            return patente;
        }
    }
}
