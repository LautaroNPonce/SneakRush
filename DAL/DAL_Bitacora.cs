using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;

namespace DAL
{
    public class DAL_Bitacora
    {
        public bool Registrar(Bitacora registro)
        {
            bool respuesta = false;

            using (SqlConnection con = new SqlConnection(Conexion.BD))
            {
                SqlCommand cmd = new SqlCommand("sp_RegistrarBitacora", con);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@Fecha",registro.Fecha < new DateTime(1753, 1, 1) ? DateTime.Now : registro.Fecha);
                cmd.Parameters.AddWithValue("@TipoUsuario", registro.TipoUsuario);
                cmd.Parameters.AddWithValue("@Accion", registro.Accion);
                cmd.Parameters.AddWithValue("@Detalle", registro.Detalle);
                cmd.Parameters.AddWithValue("@Usuario", registro.Usuario);

                con.Open();
                int filas = cmd.ExecuteNonQuery();
                respuesta = filas > 0;
            }

            return respuesta;
        }


        public List<Bitacora> Listar()
        {
            List<Bitacora> lista = new List<Bitacora>();

            using (SqlConnection cn = new SqlConnection(Conexion.BD))
            {
                SqlCommand cmd = new SqlCommand("SELECT * FROM Bitacora ORDER BY Fecha DESC", cn);
                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Bitacora
                        {
                            IdBitacora = Convert.ToInt32(dr["IdBitacora"]),
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Accion = dr["Accion"].ToString(),
                            Detalle = dr["Detalle"].ToString(),
                            Usuario = dr["Usuario"].ToString(),
                            TipoUsuario = dr["TipoUsuario"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

        public List<Bitacora> Filtrar(string tipoUsuario, string fechaInicio, string fechaFin)
        {
            List<Bitacora> lista = new List<Bitacora>();

            using (SqlConnection cn = new SqlConnection(Conexion.BD))
            {
                SqlCommand cmd = new SqlCommand("sp_FiltrarBitacora", cn);
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@TipoUsuario", tipoUsuario ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@FechaInicio", string.IsNullOrEmpty(fechaInicio) ? (object)DBNull.Value : fechaInicio);
                cmd.Parameters.AddWithValue("@FechaFin", string.IsNullOrEmpty(fechaFin) ? (object)DBNull.Value : fechaFin);

                cn.Open();

                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        lista.Add(new Bitacora
                        {
                            IdBitacora = Convert.ToInt32(dr["IdBitacora"]),
                            Fecha = Convert.ToDateTime(dr["Fecha"]),
                            Accion = dr["Accion"].ToString(),
                            Detalle = dr["Detalle"].ToString(),
                            Usuario = dr["Usuario"].ToString(),
                            TipoUsuario = dr["TipoUsuario"].ToString()
                        });
                    }
                }
            }

            return lista;
        }

    }
}
