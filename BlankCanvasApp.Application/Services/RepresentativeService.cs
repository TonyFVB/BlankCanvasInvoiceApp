using BlankCanvasApp.Application.DTOs;
using BlankCanvasApp.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlankCanvasApp.Application.Services
{
    public class RepresentativeService : IRepresentative
    {
        private readonly IRepresentativeRepository _representativeRepository;

        public RepresentativeService(IRepresentativeRepository representativeRepository)
        {
            _representativeRepository = representativeRepository;
        }

        public async Task<List<RepresentativeDto>> GetActiveRepresentativesAsync()
        {
            var representatives = await _representativeRepository.GetActiveRepresentativesAsync();

            return representatives;
        }
    }
}
