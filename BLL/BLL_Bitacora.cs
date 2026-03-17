using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using System.Security.Claims;

namespace BLL
{
    public class BLL_Bitacora
    {
        private DAL_Bitacora Dal = new DAL_Bitacora();

        public bool Registrar(Bitacora registro)
        {
            return Dal.Registrar(registro);
        }

        public List<Bitacora> Listar()
        {
            return Dal.Listar();
        }

        public List<Bitacora> Filtrar(string tipoUsuario, string fechaInicio, string fechaFin)
        {
            return Dal.Filtrar(tipoUsuario, fechaInicio, fechaFin);
        }
    }
}
