using BlackCanvasApp.Data;
using BlackCanvasApp.Repositories;
using BlackCanvasApp.Services.Interfaces;
using BlackCanvasApp.Services.Services;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Configuration.AddEnvironmentVariables();
//var connString = builder.Configuration.GetConnectionString("DefaultConnection")
//                 ?? Environment.GetEnvironmentVariable("DATABASE_URL");
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
        TrustServerCertificate = true
    };

    connString = builderConn.ToString();
}
else
{
    connString = builder.Configuration.GetConnectionString("DefaultConnection");
}
builder.Services.AddDbContext<BcDContext>(options =>
    options.UseNpgsql(connString));

//var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
//builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

builder.Services.AddScoped<ICustomer, CustomerService>();
builder.Services.AddScoped<Iinvoice, InvoiceService>();

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
    //.WithStaticAssets();


app.Run();
