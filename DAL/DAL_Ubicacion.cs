using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using System.Data.SqlClient;
using System.Data;

namespace DAL
{
    public class DAL_Ubicacion
    {
        public List<Provincia> ObtenerProvincia()
        {
            List<Provincia> Lista = new List<Provincia>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    string query = "Select * from Provincia";
                    SqlCommand comando = new SqlCommand(query, OConexion);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add
                                (
                                new Provincia()
                                {
                                    IdProvincia = dr["IdProvincia"].ToString(),
                                    Descripcion = dr["Descripcion"].ToString(),
                                }
                                );
                        }
                    }
                }
            }
            catch
            {
                Lista = new List<Provincia>();
            }

            return Lista;
        }

        public List<Partido> ObtenerPartido(string idprovincia)
        {
            List<Partido> Lista = new List<Partido>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    string query = "Select * from Partido where idprovincia = @idprovincia";
                    SqlCommand comando = new SqlCommand(query, OConexion);
                    comando.Parameters.AddWithValue("@idprovincia", idprovincia);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add
                                (
                                new Partido()
                                {
                                    IdPartido = dr["IdPartido"].ToString(),
                                    Descripcion = dr["Descripcion"].ToString(),
                                }
                                );
                        }
                    }
                }
            }
            catch
            {
                Lista = new List<Partido>();
            }

            return Lista;
        }

        public List<Localidad> ObtenerLocalidad(string idprovincia, string idpartido)
        {
            List<Localidad> Lista = new List<Localidad>();

            try
            {
                using (SqlConnection OConexion = new SqlConnection(Conexion.BD))
                {
                    string query = "Select * from Localidad where IdPartido = @idpartido and IdProvincia = @idprovincia";
                    SqlCommand comando = new SqlCommand(query, OConexion);
                    comando.Parameters.AddWithValue("@idpartido", idpartido);
                    comando.Parameters.AddWithValue("@idprovincia", idprovincia);
                    comando.CommandType = CommandType.Text;
                    OConexion.Open();

                    using (SqlDataReader dr = comando.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Lista.Add
                                (
                                new Localidad()
                                {
                                    IdLocalidad = dr["IdLocalidad"].ToString(),
                                    Descripcion = dr["Descripcion"].ToString(),
                                }
                                );
                        }
                    }
                }
            }
            catch
            {
                Lista = new List<Localidad>();
            }

            return Lista;
        }
    }
}
