using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace DAL
{
    public class Conexion
    {
        public static string BD = ConfigurationManager.ConnectionStrings["ConexionBD"].ToString(); // Conexion de la BD en Web.Config


        public DataTable Leer(string consulta, bool esProcedimiento = false, List<SqlParameter> parametros = null)
        {
            using (SqlConnection con = new SqlConnection(BD)) 
            using (SqlCommand cmd = new SqlCommand(consulta, con))
            {
                cmd.CommandType = esProcedimiento ? CommandType.StoredProcedure : CommandType.Text;
                if (parametros != null)
                    cmd.Parameters.AddRange(parametros.ToArray());

                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool Escribir(string consulta, bool esProcedimiento = false, List<SqlParameter> parametros = null)
        {
            using (SqlConnection con = new SqlConnection(BD))
            using (SqlCommand cmd = new SqlCommand(consulta, con))
            {
                cmd.CommandType = esProcedimiento ? CommandType.StoredProcedure : CommandType.Text;
                if (parametros != null)
                    cmd.Parameters.AddRange(parametros.ToArray());

                con.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public object LeerScalar(string consulta, bool esProcedimiento = false, List<SqlParameter> parametros = null)
        {
            using (SqlConnection con = new SqlConnection(BD)) 
            using (SqlCommand cmd = new SqlCommand(consulta, con))
            {
                cmd.CommandType = esProcedimiento ? CommandType.StoredProcedure : CommandType.Text;
                if (parametros != null)
                    cmd.Parameters.AddRange(parametros.ToArray());

                con.Open();
                return cmd.ExecuteScalar();
            }
        }
    }
}