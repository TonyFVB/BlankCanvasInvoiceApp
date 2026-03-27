namespace BlackCanvasApp.Models
{
    public class InvoiceHeader
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int CustomerId { get; set; }
        public decimal Total { get; set; }

        public Customer Customer { get; set; }
        public ICollection<InvoiceLine> Lines { get; set; }
    }
}
