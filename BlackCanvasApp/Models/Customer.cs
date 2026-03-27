using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlackCanvasApp.Models
{
    [Table("Customer")]
    public class Customer
    {
        [Key,Column("id")]
        public int Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("lastname")]
        public string lastName { get; set; }

        [Column("phone")]
        public string Phone { get; set; }

        [Column("address")]
        public string Address { get; set; }

        [Column("isdeleted")]
        public bool IsDeleted { get; set; }

        public ICollection<InvoiceHeader> Invoices { get; set; }
    }
}
