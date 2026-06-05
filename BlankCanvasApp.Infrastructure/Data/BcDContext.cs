using BlankCanvasApp.Application.Interfaces;
using BlankCanvasApp.Domain.Common;
using BlankCanvasApp.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace BlankCanvasApp.Infrastructure.Data
{
    public class BcDContext : IdentityDbContext<User, IdentityRole, string>
    {
        public DbSet<Customer> Customer { get; set; }
        public DbSet<InvoiceHeader> InvoiceHeader { get; set; }
        public DbSet<InvoiceLine> InvoiceLine { get; set; }

        public DbSet<Services> Services { get; set; }

        public DbSet<CustomerServices> CustomerServices { get; set; }

        public DbSet<Representative> Representative { get; set; }

        public BcDContext(DbContextOptions<BcDContext> options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.LastName).HasMaxLength(100).IsRequired();
                entity.Property(u => u.CreatedAt).HasDefaultValueSql("NOW()");
                entity.Ignore(u => u.FullName);
                entity.HasDiscriminator<string>("discriminator")
                    .HasValue<User>("User");
            });
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(u => u.ProjectStartDate).HasColumnType("timestamp without time zone");
                entity.Property(u => u.ProjectEndDate).HasColumnType("timestamp without time zone");
            });
            modelBuilder.Entity<Representative>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.UserId)
                      .IsRequired();

                entity.HasIndex(x => x.UserId)
                      .IsUnique();
            });

            modelBuilder.Entity<CustomerServices>(entity =>
            {
                entity.HasOne(cs => cs.Customer)
                      .WithMany(c => c.Services)
                      .HasForeignKey(cs => cs.CustomerId);

                entity.HasOne(cs => cs.Service)
                      .WithMany(s => s.CustomerServices)
                      .HasForeignKey(cs => cs.ServiceId);
            });
            //// Configuración de la relación entre Customer e InvoiceHeader
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
                {
                    var entity = modelBuilder.Entity(entityType.ClrType);

                    // Mapeo de nombres de columna (Igual que DataAnnotations pero global)
                    entity.Property("Id").HasColumnName("id");
                    entity.Property("CreationTime").HasColumnType("timestamp with time zone");
                    entity.Property("LastModificationTime").HasColumnType("timestamp with time zone");
                    entity.Property("IsDeleted").HasColumnName("isdeleted");
                    // Definir la Primary Key explícitamente
                    entity.HasKey("Id");
                }
            }
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                // Convertir nombre de tabla a minúsculas
                var tableName = entity.GetTableName();
                if (tableName != null && !tableName.StartsWith("AspNet"))
                    entity.SetTableName(tableName.ToLower());

                entity.GetProperties()
                      .ToList()
                      .ForEach(p => p.SetColumnName(p.GetColumnName().ToLower()));
            }
        }
        public override int SaveChanges()
        {
            ApplyAuditInfo();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ApplyAuditInfo();
            return await base.SaveChangesAsync(cancellationToken);
        }
        private void ApplyAuditInfo()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity &&
                           (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (BaseEntity)entry.Entity;

                if (entry.State == EntityState.Added)
                {
                    entity.CreationTime = DateTime.UtcNow;
                }

                if (entry.State == EntityState.Modified)
                {
                    entity.LastModificationTime = DateTime.UtcNow;

                    // 🔥 IMPORTANTE: evitar que se modifique CreationTime
                    entry.Property(nameof(BaseEntity.CreationTime)).IsModified = false;
                }
            }
        }

    }
}
