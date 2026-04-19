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
        public async Task<List<Customer>> GetAllAsync()
        {
            var customers = await _customerRepository.GetAllAsync();
            var customerDto =  customers.Select(c => new CustomerDto
            {
                Name = c.Name,
                lastName = c.LastName,
                Email = c.Email,
                Contact = c.Contact
            }).ToList();

            return customers;
        }
        public async Task<List<Customer>> GetAllActiveAsync()
        {
            var customer = await _customerRepository.GetAllActiveAsync();
            var customerDto = customer.Select(c => new CustomerDto
            {
                Name = c.Name,
                lastName = c.LastName,
                Email = c?.Email,
                Contact = c?.Contact
            }).ToList();

            return customer;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {

            var customer = await _customerRepository.GetByIdAsync(id);
            var customerDto = new CustomerDto
            {
                Name = customer.Name,
                lastName = customer.LastName,
                Email = customer?.Email,
                Contact = customer?.Contact,
            };
            return customer;
        }

        public async Task<bool> AddAsync(Customer customer)
        {
            var result =  await _customerRepository.AddAsync(customer);
            return result;
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            var result = await _customerRepository.UpdateAsync(customer);
            return result;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _customerRepository.DeleteAsync(id);
            return result;
        }
    }
}
