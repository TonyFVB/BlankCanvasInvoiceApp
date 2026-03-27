using BlackCanvasApp.DTOs;
using BlackCanvasApp.Models;

namespace BlackCanvasApp.Services.Interfaces
{
    public interface Iinvoice
    {
        Task AddAsync(InvoiceDto invoice);
        Task<InvoiceDto> GetInvoiceByIdAsync(int id);
        Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync();
        Task UpdateInvoiceAsync(int id, InvoiceDto invoiceDto);
        Task DeleteInvoiceAsync(int id);
    }
}
