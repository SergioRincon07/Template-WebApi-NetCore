using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using WebApi.DataBaseMegaLinea.Context;
using WebApi.DataBaseMegaLinea.Models;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [Authorize]
    public class ArchivoController : Controller
    {
        private readonly MegaLineaContext _context;
        private readonly IConfiguration _configuration;

        public ArchivoController(MegaLineaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene un archivo de WebApiArchivos
        /// </summary>
        /// <remarks>
        /// Se hace uso del servicio "ObtenerArchivoWebApiArchivo", el cual devuelveun Archivo si fue encontrado 
        /// </remarks>
        [HttpGet]
        [Route("/GetArchivo")]
        [ProducesResponseType(typeof(ActionResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult> GetArchivo(string IdArchivo)
        {
            try
            {
                var archivoService = new ArchivoService(_context, _configuration);
                var archivo = await archivoService.ObtenerArchivoWebApiArchivo(IdArchivo);

                if (archivo.Exitoso == false)
                    return BadRequest(archivo.Mensaje);

                if (archivo.FileBytes == null)
                    return BadRequest("El archivo no pudo ser encontrado.");

                return File(archivo.FileBytes, "application/octet-stream", archivo.NombreArchivo);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }


        /// <summary>
        /// Envía un archivo para ser guardado WebApiArchivos.
        /// </summary>
        /// <remarks>
        /// Se hace uso del servicio "GuardarWebApiArchivo", el cual devuelve un modelo que continene el identificador del 
        /// Archivo, si fue exito o si hubo un mensaje de error.
        /// </remarks>
        [HttpPost]
        [Route("/SetArchivo")]
        [ProducesResponseType(typeof(ArchivoResponseWebApiArchivos), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        public async Task<ActionResult<ArchivoResponseWebApiArchivos>> SetArchivo([FromForm] EnvioArchivo setArchivo)
        {
            try
            {
                var archivoService = new ArchivoService(_context, _configuration);
                var archivoResponse = await archivoService.GuardarWebApiArchivo(setArchivo);

                return Ok(archivoResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
                throw;
            }
        }
    }
}
