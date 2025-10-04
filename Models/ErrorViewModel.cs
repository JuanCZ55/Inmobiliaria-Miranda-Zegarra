namespace Inmobiliaria.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public string? ErrorMessage { get; set; } // <-- AÑADE ESTA LÍNEA

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
