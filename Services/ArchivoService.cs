using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using WebApi.DataBaseMegaLinea.Context;
using WebApi.DataBaseMegaLinea.Models;
using WebApi.Models;

namespace WebApi.Services
{
    public class ArchivoService
    {
        private readonly MegaLineaContext _context;
        private readonly IConfiguration _configuration;

        public ArchivoService(MegaLineaContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<ObtenerArchivo> ObtenerArchivoWebApiArchivo(string IdArchivo)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var getArchivo = new ObtenerArchivo();
                    // Obtener el Token de la App WebApiArchivosMegaLinea
                    string urlWebApiArchivos = _configuration.GetSection("WebApiArchivos:UrlWebApiArchivos").Value ?? string.Empty;
                    string apiUrlLogin = $"{urlWebApiArchivos}/Login";
                    var usuarioLoginWebApiArchivos = new UsuarioLoginWebApiArchivos
                    {
                        Username = _configuration.GetSection("WebApiArchivos:Username").Value,
                        Password = _configuration.GetSection("WebApiArchivos:Password").Value,
                    };
                    string jsonUsuarioLogin = JsonConvert.SerializeObject(usuarioLoginWebApiArchivos);
                    HttpContent contenido = new StringContent(jsonUsuarioLogin, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseToken = await httpClient.PostAsync(apiUrlLogin, contenido);

                    responseToken.EnsureSuccessStatusCode(); // Lanza una excepción si la respuesta no es exitosa

                    string jsonRespuestaToken = await responseToken.Content.ReadAsStringAsync();
                    Token? token = JsonConvert.DeserializeObject<Token>(jsonRespuestaToken);

                    // Obtener el Archivo
                    string apiUrlArchivo = $"{urlWebApiArchivos}/GetArchivo?IdArchivo={IdArchivo}";

                    string tokenJWT = token == null ? "" : token.TokenJWT.ToString();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenJWT);
                    HttpResponseMessage responseArchivo = await httpClient.GetAsync(apiUrlArchivo);

                    if (responseArchivo.IsSuccessStatusCode)
                    {
                        byte[] archivoBytes = await responseArchivo.Content.ReadAsByteArrayAsync();
                        getArchivo.FileBytes = archivoBytes;
                        getArchivo.Exitoso = true;
                        getArchivo.Mensaje = null;

                        // Obtener el nombre del archivo del encabezado "Content-Disposition"
                        if (responseArchivo.Content.Headers.ContentDisposition != null)
                        {
                            var contentDisposition = responseArchivo.Content.Headers.ContentDisposition;
                            getArchivo.NombreArchivo = contentDisposition.FileNameStar;
                            return getArchivo;
                        }
                    }

                    string errorMessage = await responseArchivo.Content.ReadAsStringAsync();
                    getArchivo.FileBytes = null;
                    getArchivo.Exitoso = false;
                    getArchivo.Mensaje = errorMessage;
                    getArchivo.NombreArchivo = string.Empty;
                    return getArchivo;
                }
            }
            catch (Exception ex)
            {
                var getArchivo = new ObtenerArchivo
                {
                    FileBytes = null,
                    Exitoso = false,
                    Mensaje = ex.Message,
                    NombreArchivo = null,
                };

                return getArchivo;
            }
        }

        public async Task<ArchivoResponseWebApiArchivos> GuardarWebApiArchivo(EnvioArchivo setArchivo)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    // Obtener el Token de la App WebApiArchivosMegaLinea
                    string urlWebApiArchivos = _configuration.GetSection("WebApiArchivos:UrlWebApiArchivos").Value ?? string.Empty;
                    string apiUrlLogin = $"{urlWebApiArchivos}/Login";
                    var usuarioLoginWebApiArchivos = new UsuarioLoginWebApiArchivos
                    {
                        Username = _configuration.GetSection("WebApiArchivos:Username").Value,
                        Password = _configuration.GetSection("WebApiArchivos:Password").Value,
                    };
                    string jsonUsuarioLogin = JsonConvert.SerializeObject(usuarioLoginWebApiArchivos);
                    HttpContent contenidoToken = new StringContent(jsonUsuarioLogin, Encoding.UTF8, "application/json");
                    HttpResponseMessage responseToken = await httpClient.PostAsync(apiUrlLogin, contenidoToken);

                    responseToken.EnsureSuccessStatusCode(); // Lanza una excepción si la respuesta no es exitosa

                    string jsonRespuestaToken = await responseToken.Content.ReadAsStringAsync();
                    Token? token = JsonConvert.DeserializeObject<Token>(jsonRespuestaToken);

                    // Guardar el Archivo
                    string apiUrlSetArchivo = $"{urlWebApiArchivos}/SetArchivo";

                    var enviarArchivo = new EnvioArchivoWebApiArchivos
                    {
                        File = setArchivo.File,
                        IdAlmacenamiento = 1,
                        IdAplicacion = 9,
                        NombreCarpeta = "Cunstom Folder",
                    };

                    MultipartFormDataContent contenidoArchivo = new MultipartFormDataContent();
                    StreamContent archivoContent = new StreamContent(enviarArchivo.File.OpenReadStream());
                    contenidoArchivo.Add(archivoContent, "File", enviarArchivo.File.FileName);
                    contenidoArchivo.Add(new StringContent(enviarArchivo.IdAlmacenamiento.ToString()), "IdAlmacenamiento");
                    contenidoArchivo.Add(new StringContent(enviarArchivo.IdAplicacion.ToString()), "IdAplicacion");
                    contenidoArchivo.Add(new StringContent(enviarArchivo.NombreCarpeta), "NombreCarpeta");

                    string tokenJWT = token == null ? "" : token.TokenJWT.ToString();
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenJWT);
                    HttpResponseMessage responseSetArchivo = await httpClient.PostAsync(apiUrlSetArchivo, contenidoArchivo);

                    string jsonRespuestaSetArchivo = await responseSetArchivo.Content.ReadAsStringAsync();
                    ArchivoResponseWebApiArchivos setArchivoResponse = JsonConvert.DeserializeObject<ArchivoResponseWebApiArchivos>(jsonRespuestaSetArchivo) ?? new ArchivoResponseWebApiArchivos();

                    return setArchivoResponse;
                }
            }
            catch (Exception ex)
            {
                var setArchivoResponse = new ArchivoResponseWebApiArchivos
                {
                    IdArchivo = null,
                    Error = ex.Message,
                    Exitoso = false,
                };

                return setArchivoResponse;
            }
        }
    }
}
