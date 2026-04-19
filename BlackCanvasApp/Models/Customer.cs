using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlackCanvasApp.Models
{

    public class Customer : BaseEntity
    {

        public string Name { get; set; }

        public string LastName { get; set; }

        public string? Email { get; set; }

        public string? Contact { get; set; }

        public ICollection<InvoiceHeader> Invoices { get; set; }
    }
}
