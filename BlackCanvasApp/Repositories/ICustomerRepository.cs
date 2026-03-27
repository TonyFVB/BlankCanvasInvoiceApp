using BlackCanvasApp.DTOs;
using BlackCanvasApp.Models;

namespace BlackCanvasApp.Repositories
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<Customer> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
    }
}
