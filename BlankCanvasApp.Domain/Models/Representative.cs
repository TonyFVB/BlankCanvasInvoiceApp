using BlankCanvasApp.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlankCanvasApp.Domain.Models
{
    public class Representative : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public virtual ICollection<Customer> Customers { get; set; } = [];
    }
}
