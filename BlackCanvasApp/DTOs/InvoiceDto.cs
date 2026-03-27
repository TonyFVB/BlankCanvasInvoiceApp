using BlackCanvasApp.Models;

namespace BlackCanvasApp.DTOs
{
    public class InvoiceDto
    {
        public string InvoiceNum { get; set; }

        public DateTime InvoiceDate { get; set; }

        public string CustomerName { get; set; }

        public ICollection<InvoiceLine> Lines { get; set; }
    }
}
