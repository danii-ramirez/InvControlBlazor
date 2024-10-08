using InvControl.Shared.Models;

namespace InvControl.Server.Helpers
{
    internal static class Functions
    {
        internal static List<Permiso> ConstruirArbolPermiso(List<Permiso> permisos, int? idpadre = null)
        {
            var permisosHijos = permisos.Where(p => p.IdPadre == idpadre).ToList();
            foreach (var permiso in permisosHijos)
            {
                permiso.Permisos = ConstruirArbolPermiso(permisos, permiso.IdPermiso);
            }
            return permisosHijos;
        }
    }
}
