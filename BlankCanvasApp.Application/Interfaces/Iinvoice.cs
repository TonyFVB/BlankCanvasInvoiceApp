using BlankCanvasApp.Application.DTOs;
using BlankCanvasApp.Domain.Models;

namespace BlankCanvasApp.Application.Interfaces
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
