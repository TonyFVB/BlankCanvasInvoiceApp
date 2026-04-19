using BlackCanvasApp.DTOs;
using BlackCanvasApp.Models;

namespace BlackCanvasApp.Repositories
{
    public interface ICustomerRepository
    {
        Task<List<Customer>> GetAllAsync();
        Task<List<Customer>> GetAllActiveAsync();
        Task<Customer> GetByIdAsync(int id);
        Task<bool> AddAsync(Customer customer);
        Task<bool> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(int id);
    }
}
