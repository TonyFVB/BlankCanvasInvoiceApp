using BlackCanvasApp.Data;
using BlackCanvasApp.DTOs;
using BlackCanvasApp.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BlackCanvasApp.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly BcDContext _context;

        public CustomerRepository(BcDContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllAsync()
        {
            var customer =  await _context.Customer.AsNoTracking().ToListAsync();
            return customer;
        }
        public async Task<List<Customer>> GetAllActiveAsync()
        {
            var customer = await _context.Customer.AsNoTracking().Where(e => !e.IsDeleted).ToListAsync();
            return customer;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            var customer = await _context.Customer.FindAsync(id);
            return customer;
        }

        public async Task<bool> AddAsync(Customer customer)
        {
            var result = await _context.Customer.AddAsync(customer);
            await _context.SaveChangesAsync();
            return result != null;
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            var result = _context.Customer.Update(customer);
            await _context.SaveChangesAsync();
            return result != null;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var customer = await _context.Customer.FindAsync(id);
            customer.IsDeleted = true;
            var result = _context.Customer.Update(customer);
            await _context.SaveChangesAsync();
            return result != null;

        }
    }
}
