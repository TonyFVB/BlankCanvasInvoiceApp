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
            var customer = await _context.Customers.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync();
            return customer;
        }
        public async Task<List<Customer>> GetAllActiveAsync()
        {
            var customer = await _context.Customers.AsNoTracking().ToListAsync();
            return customer;
        }

        public async Task<Customer> GetByIdAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            return customer;
        }

        public async Task AddAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var customer = await _context.Customers.FindAsync(id);
            customer.IsDeleted = true;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
            
        }
    }
}
