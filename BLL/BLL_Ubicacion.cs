using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using BE;
using System.Security.Claims;

namespace BLL
{
    public class BLL_Ubicacion
    {
        private DAL_Ubicacion ObjetoDAL = new DAL_Ubicacion();

        public List<Provincia> ObtenerProvincia() 
        {
            return ObjetoDAL.ObtenerProvincia();
        }

        public List<Partido> ObtenerPartido(string idprovincia)
        {
            return ObjetoDAL.ObtenerPartido(idprovincia);
        }

        public List<Localidad> ObtenerLocalidad(string idprovincia, string idpartido)
        {
            return ObjetoDAL.ObtenerLocalidad(idprovincia,idpartido);
        }

    }
}
