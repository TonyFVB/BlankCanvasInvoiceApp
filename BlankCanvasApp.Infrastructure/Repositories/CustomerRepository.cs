using BlankCanvasApp.Application.DTOs;
using BlankCanvasApp.Application.Interfaces;
using BlankCanvasApp.Domain.Models;
using BlankCanvasApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlankCanvasApp.Infrastructure.Repositories
{
    public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
    {
        private readonly BcDContext _context;

        public CustomerRepository(BcDContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<CustomerListDto>> GetCustomerListDtoAsync()
        {
            return await (from c in _context.Customer
                          join r in _context.Representative
                            on c.RepresentativeId equals r.Id into reps
                          from r in reps.DefaultIfEmpty()
                          join u in _context.Users
                            on r.UserId equals u.Id into users
                          from u in users.DefaultIfEmpty()
                          where !c.IsDeleted
                        select new CustomerListDto
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Contact = c.Contact,
                            Email = c.Email,
                            Status = c.Status,
                            ProjectStartDate = c.ProjectStartDate,
                            ProjectEndDate = c.ProjectEndDate,
                            Budget = c.Budget,

                            RepresentativeName =
                                u != null
                                    ? $"{u.FirstName} {u.LastName}"
                                    : null,
                            ServiceIds = _context.CustomerServices
                                          .Where(cs => cs.CustomerId == c.Id && !cs.IsDeleted)
                                          .Select(cs => cs.ServiceId)
                                          .ToList(),
                            Services = _context.CustomerServices
                                          .Where(cs => cs.CustomerId == c.Id && !cs.IsDeleted)
                                          .Select(cs => cs.Service.Name)
                                          .ToList()
                        }).ToListAsync();
        }

    }
}
