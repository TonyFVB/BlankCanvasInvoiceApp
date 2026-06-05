using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static BlankCanvasApp.Domain.Emuns.Constants;
using BlankCanvasApp.Domain.Models;
namespace BlankCanvasApp.Application.DTOs
{
    public class CustomerListDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Contact { get; set; }
        public string? Email { get; set; }
        public CustomerStatus Status { get; set; }
        public string StatusLabel => CustomerStatusMeta.GetLabel(Status);
        public string StatusCssClass => CustomerStatusMeta.GetCssClass(Status);
        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }
        public decimal? Budget { get; set; }
        public string? RepresentativeName { get; set; }
        public List<int> ServiceIds { get; set; } = [];
        public List<string> Services { get; set; } = [];
        public int InvoiceCount { get; set; }
    }

    public class CustomerServicesFormDto
    {
        public int CustomerId { get; set; }
        public List<int> SelectedServiceIds { get; set; } = new();
    }

    // ============================================================
    // FORM DTO — lo que recibe el modal de crear/editar
    // ============================================================
    public class CustomerFormDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(30)]
        public string? Contact { get; set; }

        [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
        [MaxLength(256)]
        public string? Email { get; set; }

        public CustomerStatus Status { get; set; } = CustomerStatus.Newlead;

        public DateTime? ProjectStartDate { get; set; }
        public DateTime? ProjectEndDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Budget { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        /// <summary>Username del representante asignado</summary>
        public int? RepresentativeId { get; set; }

        /// <summary>IDs de servicios seleccionados</summary>
        public List<int> SelectedServiceIds { get; set; } = new();
    }

    // ============================================================
    // DETAIL DTO — vista de detalles del cliente
    // ============================================================
    public class CustomerDetailDto : CustomerListDto
    {
        public string? Notes { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime? LastModificationTime { get; set; }
    }

    // ============================================================
    // MAPEO MANUAL — métodos de extensión
    // ============================================================
    public static class CustomerMappingExtensions
    {
        /// <summary>Entidad → ListDto</summary>
        public static CustomerListDto ToListDto(this Customer c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Contact = c.Contact,
            Email = c.Email,
            Status = c.Status,
            ProjectStartDate = c.ProjectStartDate,
            ProjectEndDate = c.ProjectEndDate,
            Budget = c.Budget,
            RepresentativeName = null,
            ServiceIds = c.Services?
                              .Select(cs => cs.ServiceId)
                              .ToList() ?? [],
            Services = c.Services?
                              .Select(cs => cs.Service.Name)
                              .ToList() ?? [],
            InvoiceCount = c.Invoices?.Count ?? 0,
        };
        public static List<CustomerListDto> ToListDto(this IEnumerable<Customer> customers)
                => customers.Select(c => c.ToListDto()).ToList();
        /// <summary>Entidad → FormDto (para editar)</summary>
        public static CustomerFormDto ToFormDto(this Customer c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Contact = c.Contact,
            Email = c.Email,
            Status = c.Status,
            ProjectStartDate = c.ProjectStartDate,
            ProjectEndDate = c.ProjectEndDate,
            Budget = c.Budget,
            //Notes = c.Notes,
            RepresentativeId = c.RepresentativeId,
            SelectedServiceIds = c.Services?
                                  .Select(s => s.ServiceId)
                                  .ToList() ?? [],
        };

        /// <summary>FormDto → Entidad nueva</summary>
        public static Customer ToEntity(this CustomerFormDto dto) => new()
        {
            Name = dto.Name.Trim(),
            Contact = dto.Contact,
            Email = dto.Email,
            Status = dto.Status,
            ProjectStartDate = dto.ProjectStartDate,
            ProjectEndDate = dto.ProjectEndDate,
            Budget = dto.Budget,
            //Notes = dto.Notes,
            RepresentativeId = dto.RepresentativeId,
            Services = dto.SelectedServiceIds
                            .Select(serviceId => new CustomerServices { ServiceId = serviceId })
                            .ToList()
        };

        /// <summary>FormDto → Entidad existente (actualizar)</summary>
        public static void ApplyTo(this CustomerFormDto dto, Customer entity)
        {
            entity.Name = dto.Name.Trim();
            entity.Contact = dto.Contact;
            entity.Email = dto.Email;
            entity.Status = dto.Status;
            entity.ProjectStartDate = dto.ProjectStartDate;
            entity.ProjectEndDate = dto.ProjectEndDate;
            entity.Budget = dto.Budget;
            //entity.Notes = dto.Notes;
            entity.RepresentativeId = dto.RepresentativeId;
            entity.Services ??= [];

            var selectedIds = dto.SelectedServiceIds.Distinct().ToList();

            var servicesToRemove = entity.Services
                .Where(cs => !selectedIds.Contains(cs.ServiceId))
                .ToList();

            foreach (var service in servicesToRemove)
            {
                entity.Services.Remove(service);
            }

            var existingServiceIds = entity.Services
                .Select(cs => cs.ServiceId)
                .ToHashSet();

            var servicesToAdd = selectedIds
                .Where(serviceId => !existingServiceIds.Contains(serviceId))
                .Select(serviceId => new CustomerServices
                {
                    CustomerId = entity.Id,
                    ServiceId = serviceId
                });

            foreach (var service in servicesToAdd)
            {
                entity.Services.Add(service);
            }
        }
    }
}
