using BlankCanvasApp.Application.DTOs;
using BlankCanvasApp.Domain.Models;

namespace BlankCanvasApp.Application.Interfaces
{
    public interface ICustomerRepository : IBaseRepository<Customer>
    {
        Task<List<CustomerListDto>> GetCustomerListDtoAsync();
    }
}
