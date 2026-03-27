using BlackCanvasApp.Data;
using BlackCanvasApp.DTOs;
using BlackCanvasApp.Models;
using BlackCanvasApp.Repositories;
using BlackCanvasApp.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BlackCanvasApp.Services.Services
{
    public class CustomerService : ICustomer
    {
        private ICustomerRepository _customerRepository;
        public CustomerService(ICustomerRepository customerRepository)
        {
            _customerRepository = customerRepository;
        }
        public async Task<List<CustomerDto>> GetAllAsync()
        {
            var customers = _customerRepository.GetAllAsync();
            var customerDto =  customers.Result.Select(c => new CustomerDto
            {
                Name = c.Name,
                lastName = c.lastName,
                Phone = c.Phone
            }).ToList();

            return customerDto;
        }
        public async Task<List<CustomerDto>> GetAllActiveAsync()
        {
            var customer = _customerRepository.GetAllAsync().Result.Where(c => !c.IsDeleted);
            var customerDto = customer.Select(c => new CustomerDto
            {
                Name = c.Name,
                lastName = c.lastName,
                Phone = c.Phone
            }).ToList();

            return customerDto;
        }

        public async Task<CustomerDto> GetByIdAsync(int id)
        {
            var customer = _customerRepository.GetByIdAsync(id).Result;
            var customerDto = new CustomerDto
            {
                Name = customer.Name,
                lastName = customer.lastName,
                Phone = customer.Phone,
                Address = customer.Address
            };
            return customerDto;
        }

        public async Task AddAsync(Customer customer)
        {
            var result =  _customerRepository.AddAsync(customer);
            await result;
        }

        public async Task UpdateAsync(Customer customer)
        {
            var result = _customerRepository.UpdateAsync(customer);
            await result;
        }

        public async Task DeleteAsync(int id)
        {
            var result = _customerRepository.DeleteAsync(id);
            await result;
        }
    }
}
