using BlankCanvasApp.Application.DTOs;
using BlankCanvasApp.Application.Interfaces;
using BlankCanvasApp.Domain.Models;
using BlankCanvasApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace BlankCanvasApp.Infrastructure.Repositories
{
    public class RepresentativeRepository : BaseRepository<Representative>, IRepresentativeRepository
    {
        private readonly BcDContext _context;
        public RepresentativeRepository(BcDContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<RepresentativeDto>> GetActiveRepresentativesAsync()
        {
            var representatives = await (from r in _context.Representative
                                         join u in _context.Users
                                            on r.UserId equals u.Id
                                         where r.IsActive && !r.IsDeleted
                                         select new RepresentativeDto
                                             {
                                                Id = r.Id,
                                                UserId = u.Id,
                                                Name = u.FirstName,
                                                LastName = u.LastName,
                                         }
                                         ).ToListAsync();

            return representatives;
        }
    }
}
