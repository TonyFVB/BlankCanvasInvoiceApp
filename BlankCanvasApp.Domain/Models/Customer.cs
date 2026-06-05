using BlankCanvasApp.Domain.Common;
using BlankCanvasApp.Domain.Emuns;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;

namespace BlankCanvasApp.Domain.Models
{

    public class Customer : BaseEntity
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(200)]
        public string Name { get; set; }

        //public string LastName { get; set; }

        [MaxLength(256)]
        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        public string? Email { get; set; }

        [MaxLength(30)]
        public string? Contact { get; set; }
        public Constants.CustomerStatus Status { get; set; } =  Constants.CustomerStatus.Newlead;

        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Budget { get; set; }

        //[MaxLength(1000)]
        //public string? Notes { get; set; }

        public int? RepresentativeId { get; set; }

        public virtual Representative? Representative { get; set; }

        public ICollection<InvoiceHeader> Invoices { get; set; }

        public ICollection<CustomerServices> Services { get; set; }
    }
}
