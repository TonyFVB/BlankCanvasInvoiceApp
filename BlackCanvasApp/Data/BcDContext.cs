using BlackCanvasApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace BlackCanvasApp.Data
{
    public class BcDContext : DbContext
    {
        public DbSet<Customer> Customer { get; set; }
        public DbSet<InvoiceHeader> InvoiceHeader { get; set; }
        public DbSet<InvoiceLine> InvoiceLine { get; set; }

        public BcDContext(DbContextOptions<BcDContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Configuración de la relación entre Customer e InvoiceHeader
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var entity = modelBuilder.Entity(entityType.ClrType);

                    // Mapeo de nombres de columna (Igual que DataAnnotations pero global)
                    entity.Property("Id").HasColumnName("id");
                    entity.Property("CreationTime").HasColumnName("creationtime");
                    entity.Property("LastModificationTime").HasColumnName("lastmodificationtime");
                    entity.Property("IsDeleted").HasColumnName("isdeleted");
                    // Definir la Primary Key explícitamente
                    entity.HasKey("Id");
                }
            }
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Convertir nombre de tabla a minúsculas
                entity.SetTableName(entity.GetTableName().ToLower());
                entity.GetProperties().ToList().ForEach(p => p.SetColumnName(p.GetColumnName().ToLower()));
            }
        }

    }
}
