using BlankCanvasApp.Application.DTOs;
using BlankCanvasApp.Application.Interfaces;

namespace BlankCanvasApp.Application.Services
{
    public class InvoiceService : Iinvoice
    {
        public Task AddAsync(InvoiceDto invoice)
        {
            throw new NotImplementedException();
        }

        public Task DeleteInvoiceAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<InvoiceDto>> GetAllInvoicesAsync()
        {
            throw new NotImplementedException();
        }

        public Task<InvoiceDto> GetInvoiceByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateInvoiceAsync(int id, InvoiceDto invoiceDto)
        {
            throw new NotImplementedException();
        }
    }
}
