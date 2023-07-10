using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApi.DataBaseMegaLinea.Context;
using WebApi.DataBaseMegaLinea.Models;
using WebApi.Services;


namespace WebApiFieldBots.Controllers
{
    [Authorize]
    public class UsuarioController : Controller
    {
        private readonly MegaLineaContext _context;

        public UsuarioController(MegaLineaContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene una lista de usuarios filtrada por los parámetros proporcionados.
        /// Los parámetros deben pasarse en la cadena de consulta de la URL.
        /// </summary>
        /// <remarks>
        /// Se hace uso del servicio "GetUsuarios", el cual devuelve una lista de usuarios obtenida a través del 
        /// procedimiento almacenado "SPConsultarUsuarios".
        /// </remarks>
        /// <param name="NtUser">Nt de usuario.</param>
        /// <param name="IdArea">ID del área.</param>
        /// <param name="Nombre">Nombre del usuario.</param>
        /// <param name="Activo">Estado usuario inactivo/activo.</param>
        [HttpGet]
        [Route("/GetUsuarios")]
        [ProducesResponseType(typeof(SPConsultarUsuariosResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<SPConsultarUsuariosResult>> GetUsuarios(string? NtUser, int? IdArea, string? Nombre, int? Activo)
        {
            try
            {
                var usuariosService = new UsuarioService(_context);
                var list = await usuariosService.GetUsuarios(NtUser, IdArea, Nombre, Activo);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }

        }
    }
}
