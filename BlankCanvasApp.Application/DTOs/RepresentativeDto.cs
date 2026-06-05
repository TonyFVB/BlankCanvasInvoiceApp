using System;
using System.Collections.Generic;
using System.Text;

namespace BlankCanvasApp.Application.DTOs
{
    public class RepresentativeDto
    {
        public int Id { get; set; }

        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string LastName { get; set; }  =string.Empty;

        public string FullName => string.IsNullOrWhiteSpace(LastName)
            ? Name
            : $"{Name} {LastName}".Trim();
    }
}
