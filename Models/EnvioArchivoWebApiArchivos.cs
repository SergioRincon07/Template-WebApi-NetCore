namespace WebApi.Models
{
    public class EnvioArchivoWebApiArchivos
    {
        public IFormFile File { get; set; }
        public int IdAlmacenamiento { get; set; }
        public int IdAplicacion { get; set; }
        public string NombreCarpeta { get; set; }

        public EnvioArchivoWebApiArchivos()
        {
            File = new FormFile(Stream.Null, 0, 0, "filename", "filename.ext");
            NombreCarpeta = string.Empty;
        }
    }
}
