using BlankCanvasApp.Application.Interfaces;
using BlankCanvasApp.Domain.Models;
using BlankCanvasApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlankCanvasApp.Infrastructure.Repositories
{
    public class ServiceRepository : BaseRepository<Services>, IServicesRepository
    {
        private readonly BcDContext _context;

        public ServiceRepository(BcDContext context) : base(context)
        {
            _context = context;
        }
    }
}
