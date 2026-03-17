using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;

namespace BLL
{
    public class BLL_Patente
    {
        private DAL_Patente dal = new DAL_Patente();

        // Listar todas las patentes
        public List<Patente> ListarPatentes()
        {
            return dal.Listar();
        }

        // Registrar nueva patente
        public bool RegistrarPatente(Patente patente)
        {
            if (string.IsNullOrWhiteSpace(patente.Nombre))
                throw new Exception("El nombre de la patente no puede estar vacío.");

            if (string.IsNullOrWhiteSpace(patente.Codigo))
                throw new Exception("El código de la patente no puede estar vacío.");

            return dal.Registrar(patente);
        }

        // Modificar patente existente
        public bool ModificarPatente(Patente patente)
        {
            if (patente.Id <= 0)
                throw new Exception("El ID de la patente no es válido.");

            return dal.Modificar(patente);
        }

        // Eliminar patente
        public bool EliminarPatente(int idPatente)
        {
            if (idPatente <= 0)
                throw new Exception("El ID de la patente no es válido.");

            return dal.Eliminar(idPatente);
        }

        // Buscar patente por código (opcional, útil para validaciones)
        public Patente BuscarPorCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            return dal.BuscarPorCodigo(codigo);
        }
    }
}
