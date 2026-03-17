using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BE;
using DAL;

namespace BLL
{
    public class BLL_Familia
    {
        private DAL_Familia dal = new DAL_Familia();
        private DAL_Permiso dalPermiso = new DAL_Permiso();

        // Listar todas las familias existentes
        public List<Familia> ListarFamilias()
        {
            return dal.Listar();
        }

        // Crear una nueva familia
        public bool RegistrarFamilia(Familia familia)
        {
            if (string.IsNullOrWhiteSpace(familia.Nombre))
                throw new Exception("El nombre de la familia no puede estar vacío.");

            return dal.Registrar(familia);
        }

        // Modificar una familia existente
        public bool ModificarFamilia(Familia familia)
        {
            if (familia.Id <= 0)
                throw new Exception("El ID de la familia no es válido.");

            return dal.Modificar(familia);
        }

        // Eliminar familia
        public bool EliminarFamilia(int idFamilia)
        {
            return dal.Eliminar(idFamilia);
        }

        // Asignar permiso (familia o patente) a una familia
        public bool AgregarPermisoAFamilia(int idFamilia, int idPermisoHijo)
        {
            return dal.AgregarPermisoAFamilia(idFamilia, idPermisoHijo);
        }

        // Quitar permiso (familia o patente) de una familia
        public bool QuitarPermisoDeFamilia(int idFamilia, int idPermisoHijo)
        {
            return dal.QuitarPermisoDeFamilia(idFamilia, idPermisoHijo);
        }

        // Obtener familia con toda su jerarquía de permisos
        public Familia ObtenerFamiliaCompleta(int idFamilia)
        {
            Familia familia = dal.Listar().FirstOrDefault(f => f.Id == idFamilia);
            if (familia == null)
                return null;

            List<PermisoComponente> hijos = dalPermiso.ListarHijos(idFamilia);
            foreach (var hijo in hijos)
            {
                if (hijo is Familia f)
                    familia.AgregarHijo(ObtenerFamiliaCompleta(f.Id)); // recursión
                else
                    familia.AgregarHijo(hijo);
            }

            return familia;
        }
    }
}
