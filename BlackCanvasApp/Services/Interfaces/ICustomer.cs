using BlackCanvasApp.DTOs;
using BlackCanvasApp.Models;

namespace BlackCanvasApp.Services.Interfaces
{
    public interface ICustomer
    {
        Task<List<CustomerDto>> GetAllAsync();
        Task<CustomerDto> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
    }
}
