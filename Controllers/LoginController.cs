using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using WebApi.DataBaseMegaLinea.Context;
using WebApi.DataBaseMegaLinea.Models;
using WebApi.Models;
using WebApi.Services;

namespace WebApiFieldBots.Controllers
{
    public class LoginController : Controller
    {
        private readonly MegaLineaContext _context;
        private readonly IConfiguration _configuration;

        public LoginController(MegaLineaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene el Token para el acceso de las APIs. 
        /// </summary>
        /// <remarks>
        /// Se hace uso del servicio "LoginService", el cual devuelve un Token si el usuario tiene Acceso, 
        /// el procedimiento almacenado para validar los accesos es "spLogin".
        /// Si es usuario de MegaLinea el usuario por defecto es: User -> Admin Generico; Password -> 00000000
        /// </remarks>
        [HttpPost]
        [Route("/Login")]
        [ProducesResponseType(typeof(Token), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<Token>> IniciarSesionAsync([FromBody] UsuarioLogin usuarioLogin)
        {
            try
            {
                if (usuarioLogin == null)
                {
                    return BadRequest("Datos incorrectos");
                }
                var LoginService = new LoginService(_context);
                var listUserLogin = await LoginService.LoginAppMegaLinea(usuarioLogin);

                if (listUserLogin.Count > 0)
                {
                    var token = LoginService.GenerateToken(_configuration);

                    var responseToken = new Token
                    {
                        TokenJWT = token,
                        Acceso = true
                    };

                    return Ok(responseToken);
                }
                else
                {
                    var responseToken = new Token
                    {
                        TokenJWT = "",
                        Acceso = false
                    };

                    return Ok(responseToken);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }

        }
    }
}
