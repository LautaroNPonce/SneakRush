using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using BE;

namespace BLL
{
    public class BLL_Reporte
    {
        private DAL_Reporte ObjetoDAL = new DAL_Reporte();

        public List<Reporte> Ventas(string fechainicio, string fechafin, string idtrasferencia) 
        {
            return ObjetoDAL.Ventas(fechainicio, fechafin, idtrasferencia);
        }

        public DashBoard VerDashBoards()
        {
            return ObjetoDAL.VerDashBoards();
        }
    }
}
