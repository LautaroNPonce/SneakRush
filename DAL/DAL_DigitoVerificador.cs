using BE;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DAL_DigitoVerificador
    {
        private Conexion acceso = new Conexion(); // Tu clase actual de acceso a BD

        public DataTable LeerTabla(string nombreTabla)
        {
            string consulta = $"SELECT * FROM {nombreTabla}";
            return acceso.Leer(consulta);
        }

        public void ActualizarDVH(string tabla, string campoId, object id, string dvh)
        {
            string query = $"UPDATE {tabla} SET DVH = @DVH WHERE {campoId} = @Id";
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@DVH", dvh),
                new SqlParameter("@Id", id)
            };
            acceso.Escribir(query, false, parametros);
        }

        public string ObtenerDVV(string tabla)
        {
            string query = "SELECT DVV FROM DigitoVerificador WHERE Tabla = @Tabla";
            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@Tabla", tabla)
            };
            object resultado = acceso.LeerScalar(query, false, parametros);
            return resultado?.ToString();
        }

        public void GuardarDVV(string tabla, string dvv)
        {
            string queryExiste = "SELECT COUNT(*) FROM DigitoVerificador WHERE Tabla = @Tabla";
            List<SqlParameter> parametrosExiste = new List<SqlParameter>
            {
                new SqlParameter("@Tabla", tabla)
            };
            object result = acceso.LeerScalar(queryExiste, false, parametrosExiste);
            int existe = result != null ? Convert.ToInt32(result) : 0;

            string query = existe > 0
                ? "UPDATE DigitoVerificador SET DVV = @DVV WHERE Tabla = @Tabla"
                : "INSERT INTO DigitoVerificador (Tabla, DVV) VALUES (@Tabla, @DVV)";

            List<SqlParameter> parametros = new List<SqlParameter>
            {
                new SqlParameter("@Tabla", tabla),
                new SqlParameter("@DVV", dvv)
            };
            acceso.Escribir(query, false, parametros);
        }

    }
}

