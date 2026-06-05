using BlankCanvasApp.Application.Interfaces;
using BlankCanvasApp.Application.Services;
using BlankCanvasApp.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace BlankCanvasApp.Infrastructure.Data
{
    /// <summary>
    /// Extension method que registra todos los servicios de Infrastructure.
    /// Program.cs solo llama: builder.Services.AddInfrastructure(builder.Configuration)
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ── Conexión PostgreSQL ──────────────────────────────────
            var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            string connString;

            if (!string.IsNullOrEmpty(databaseUrl))
            {
                var uri = new Uri(databaseUrl);
                var userInfo = uri.UserInfo.Split(':');

                var builderConn = new NpgsqlConnectionStringBuilder
                {
                    Host = uri.Host,
                    Port = uri.Port,
                    Username = userInfo[0],
                    Password = userInfo[1],
                    Database = uri.AbsolutePath.Trim('/'),
                    SslMode = SslMode.Require,
                    TrustServerCertificate = true,
                };
                connString = builderConn.ToString();
            }
            else
            {
                connString = configuration.GetConnectionString("DefaultConnection")!;
            }

            // ── DbContext ────────────────────────────────────────────
            services.AddDbContext<BcDContext>(options =>
                options.UseNpgsql(connString));

            //var port = EnvironmentGetEnvironmentVariable("PORT") ?? "8080";
            //builder .WebHost.UseUrls($"http://0.0.0.0:{port}");

            // ── Servicios ─────────────────────────────────────────────
            services.AddScoped<ICustomer, CustomerService>();
            services.AddScoped<Iinvoice, InvoiceService>();
            services.AddScoped<IServices, ServicesService>();
            services.AddScoped<IRepresentative, RepresentativeService>();
            // ── Repositorios ───────────────────────────────────────────
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IServicesRepository, ServiceRepository>();
            services.AddScoped(typeof(IBaseRepository<>), typeof(BaseRepository<>));
            services.AddScoped<IRepresentativeRepository, RepresentativeRepository>();

            return services;
        }
    }
}