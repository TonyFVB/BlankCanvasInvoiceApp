using BlankCanvasApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlankCanvasApp.Application.Services
{
    public class ServicesService : IServices
    {
        public Task<List<Domain.Models.Services>> GetAllActiveAsync()
        {
            throw new NotImplementedException();
        }

        public Task<List<Domain.Models.Services>> GetAllAsync()
        {
            throw new NotImplementedException();
        }
    }
}
