using WebApi.DataBaseMegaLinea.Context;
using WebApi.DataBaseMegaLinea.Models;

namespace WebApi.Services
{
    public class UsuarioService
    {
        private readonly MegaLineaContext _context;

        public UsuarioService(MegaLineaContext context)
        {
            _context = context;
        }
        public async Task<List<SPConsultarUsuariosResult>> GetUsuarios(string? NtUser, int? IdArea, string? Nombre, int? Activo)
        {
            NtUser = NtUser != "0" ? NtUser : null;
            IdArea = IdArea != 0 ? IdArea : null;
            Nombre = Nombre != "0" ? Nombre : null;
            Activo = Activo != -1 ? Activo : null;
            List<SPConsultarUsuariosResult> ListUsuarios = await _context.GetProcedures().SPConsultarUsuariosAsync(NtUser, IdArea, Nombre, Activo);
            return ListUsuarios;
        }
    }
}
