namespace BlackCanvasApp.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public string? Message { get; set; }
        public string? Details { get; set; }
        public int StatusCode { get; set; }
        public bool ShowDetails { get; set; }
        public string? Path { get; set; }

    }
}
