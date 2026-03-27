namespace BlackCanvasApp.Models
{
    public class InvoiceLine
    {
        public int Id { get; set; }
        public int InvoiceHeaderId { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        public InvoiceHeader InvoiceHeader { get; set; }
    }
}
