using BlankCanvasApp.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlankCanvasApp.Domain.Models
{
    public class Services : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        /// <summary>Color hex para mostrar en UI (ej: #6a3093)</summary>
        [MaxLength(7)]
        public string? Color { get; set; }

        // Relación muchos a muchos con Customer
        public ICollection<CustomerServices> CustomerServices { get; set; } = [];
    }

    /// <summary>
    /// Tabla intermedia Customer ↔ Service (muchos a muchos)
    /// </summary>
    public class CustomerServices : BaseEntity
    {
        public int CustomerId { get; set; }
        public virtual Customer Customer { get; set; } = null!;

        public int ServiceId { get; set; }
        public virtual Services Service { get; set; } = null!;
    }
}

