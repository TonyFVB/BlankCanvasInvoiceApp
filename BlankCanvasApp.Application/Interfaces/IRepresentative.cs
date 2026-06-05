using BlankCanvasApp.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlankCanvasApp.Application.Interfaces
{
    public interface IRepresentative
    {
        Task<List<RepresentativeDto>> GetActiveRepresentativesAsync();
    }
}
