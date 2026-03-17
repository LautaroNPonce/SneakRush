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
    public class DAL_Permiso
    {
        // Listar todos los permisos (familias y patentes)
        public List<PermisoComponente> Listar()
        {
            List<PermisoComponente> lista = new List<PermisoComponente>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT IdPermiso, Nombre, Codigo, EsFamilia");
                    sb.AppendLine("FROM Permiso");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.CommandType = CommandType.Text;

                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            bool esFamilia = Convert.ToBoolean(dr["EsFamilia"]);

                            if (esFamilia)
                            {
                                lista.Add(new Familia()
                                {
                                    Id = Convert.ToInt32(dr["IdPermiso"]),
                                    Nombre = dr["Nombre"].ToString(),
                                    Codigo = dr["Codigo"].ToString()
                                });
                            }
                            else
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
            }
            catch
            {
                lista = new List<PermisoComponente>();
            }

            return lista;
        }

        // listar todos los permisos asignados a un usuario
        public List<PermisoComponente> ListarPermisosUsuario(int idUsuario)
        {
            List<PermisoComponente> lista = new List<PermisoComponente>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT p.IdPermiso, p.Nombre, p.Codigo, p.EsFamilia");
                    sb.AppendLine("FROM UsuarioPermiso up");
                    sb.AppendLine("INNER JOIN Permiso p ON up.IdPermiso = p.IdPermiso");
                    sb.AppendLine("WHERE up.IdUsuario = @IdUsuario");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    comando.CommandType = System.Data.CommandType.Text;

                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (Convert.ToBoolean(dr["EsFamilia"]))
                            {
                                lista.Add(new Familia()
                                {
                                    Id = Convert.ToInt32(dr["IdPermiso"]),
                                    Nombre = dr["Nombre"].ToString(),
                                    Codigo = dr["Codigo"].ToString()
                                });
                            }
                            else
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
            }
            catch
            {
                lista = new List<PermisoComponente>();
            }

            return lista;
        }

        // eliminar todos los permisos asignados a un usuario
        public bool EliminarPermisosUsuario(int idUsuario)
        {
            bool resultado = false;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("DELETE FROM UsuarioPermiso WHERE IdUsuario = @IdUsuario");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    comando.CommandType = System.Data.CommandType.Text;

                    OConexion.Open();
                    resultado = comando.ExecuteNonQuery() >= 0;
                }
            }
            catch
            {
                resultado = false;
            }

            return resultado;
        }

        // Obtener hijos de una familia
        public List<PermisoComponente> ListarHijos(int idFamilia)
        {
            List<PermisoComponente> lista = new List<PermisoComponente>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT p.IdPermiso, p.Nombre, p.Codigo, p.EsFamilia");
                    sb.AppendLine("FROM FamiliaPermiso fp");
                    sb.AppendLine("INNER JOIN Permiso p ON p.IdPermiso = fp.IdPermisoHijo");
                    sb.AppendLine("WHERE fp.IdFamilia = @IdFamilia");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@IdFamilia", idFamilia);
                    comando.CommandType = CommandType.Text;

                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            bool esFamilia = Convert.ToBoolean(dr["EsFamilia"]);

                            if (esFamilia)
                            {
                                lista.Add(new Familia()
                                {
                                    Id = Convert.ToInt32(dr["IdPermiso"]),
                                    Nombre = dr["Nombre"].ToString(),
                                    Codigo = dr["Codigo"].ToString()
                                });
                            }
                            else
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
            }
            catch
            {
                lista = new List<PermisoComponente>();
            }

            return lista;
        }

        // Asignar permiso a usuario
        public bool AsignarPermisoAUsuario(int idUsuario, int idPermiso)
        {
            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    OConexion.Open();

                    // 1) verificar si ya existe
                    String checkQuery = @"SELECT COUNT(*) FROM UsuarioPermiso WHERE IdUsuario = @IdUsuario AND IdPermiso = @IdPermiso";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, OConexion))
                    {
                        checkCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        checkCmd.Parameters.AddWithValue("@IdPermiso", idPermiso);

                        int existe = (int)checkCmd.ExecuteScalar();
                        if (existe > 0)
                            return true; // ya existe, no inserto
                    }

                    // 2) insertar porque no existe
                    String insertQuery = @"INSERT INTO UsuarioPermiso (IdUsuario, IdPermiso) VALUES (@IdUsuario, @IdPermiso)";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, OConexion))
                    {
                        insertCmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                        insertCmd.Parameters.AddWithValue("@IdPermiso", idPermiso);

                        return insertCmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }

        // Quitar permiso de usuario
        public bool QuitarPermisoAUsuario(int idUsuario, int idPermiso)
        {
            bool resultado = false;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("DELETE FROM UsuarioPermiso");
                    sb.AppendLine("WHERE IdUsuario = @IdUsuario AND IdPermiso = @IdPermiso");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    comando.Parameters.AddWithValue("@IdPermiso", idPermiso);
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
        public List<PermisoComponente> ListarPermisosCliente(int idCliente)
        {
            List<PermisoComponente> lista = new List<PermisoComponente>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("SELECT p.IdPermiso, p.Nombre, p.Codigo, p.EsFamilia");
                    sb.AppendLine("FROM ClientePermiso cp");
                    sb.AppendLine("INNER JOIN Permiso p ON cp.IdPermiso = p.IdPermiso");
                    sb.AppendLine("WHERE cp.IdCliente = @IdCliente");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@IdCliente", idCliente);
                    comando.CommandType = CommandType.Text;

                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            if (Convert.ToBoolean(dr["EsFamilia"]))
                            {
                                lista.Add(new Familia()
                                {
                                    Id = Convert.ToInt32(dr["IdPermiso"]),
                                    Nombre = dr["Nombre"].ToString(),
                                    Codigo = dr["Codigo"].ToString()
                                });
                            }
                            else
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
            }
            catch
            {
                lista = new List<PermisoComponente>();
            }

            return lista;
        }

        // Eliminar todos los permisos de un cliente
        public bool EliminarPermisosCliente(int idCliente)
        {
            bool resultado = false;

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("DELETE FROM ClientePermiso WHERE IdCliente = @IdCliente");

                    SqlCommand comando = new SqlCommand(sb.ToString(), OConexion);
                    comando.Parameters.AddWithValue("@IdCliente", idCliente);
                    comando.CommandType = CommandType.Text;

                    OConexion.Open();
                    // >= 0 por si no tenía permisos (no es error)
                    resultado = comando.ExecuteNonQuery() >= 0;
                }
            }
            catch
            {
                resultado = false;
            }

            return resultado;
        }

        // Asignar un permiso a un cliente
        public bool AsignarPermisoACliente(int idCliente, int idPermiso)
        {
            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    OConexion.Open();

                    // 1) Verificar si ya existe
                    string checkQuery = @"SELECT COUNT(*) FROM ClientePermiso WHERE IdCliente = @IdCliente AND IdPermiso = @IdPermiso";

                    using (SqlCommand checkCmd = new SqlCommand(checkQuery, OConexion))
                    {
                        checkCmd.Parameters.AddWithValue("@IdCliente", idCliente);
                        checkCmd.Parameters.AddWithValue("@IdPermiso", idPermiso);

                        int existe = (int)checkCmd.ExecuteScalar();
                        if (existe > 0)
                            return true; // ya estaba asignado, no es error
                    }

                    // Insertar porque no existe
                    string insertQuery = @"INSERT INTO ClientePermiso (IdCliente, IdPermiso) VALUES (@IdCliente, @IdPermiso)";

                    using (SqlCommand insertCmd = new SqlCommand(insertQuery, OConexion))
                    {
                        insertCmd.Parameters.AddWithValue("@IdCliente", idCliente);
                        insertCmd.Parameters.AddWithValue("@IdPermiso", idPermiso);

                        return insertCmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
