using BlackCanvasApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace BlackCanvasApp.Data
{
    public class BcDContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<InvoiceHeader> InvoiceHeaders { get; set; }
        public DbSet<InvoiceLine> InvoiceLines { get; set; }

        public BcDContext(DbContextOptions<BcDContext> options)
            : base(options)
        {
        }
    }
}
