namespace WebApi.Models
{
    public class ObtenerArchivo
    {
        public byte[]? FileBytes { get; set; }
        public Boolean Exitoso { get; set; }
        public string? Mensaje { get; set; }
        public string? NombreArchivo { get; set; }
    }
}
