namespace WebApi.Models
{
    public class EnvioArchivo
    {
        public IFormFile File { get; set; }

        public EnvioArchivo()
        {
            File = new FormFile(Stream.Null, 0, 0, "filename", "filename.ext");
        }
    }
}
