using Microsoft.EntityFrameworkCore;
using TechStore.AL.Abstractions;
using TechStore.AL.Catalog;
using TechStore.AL.Catalog.Concrete;
using TechStore.DAL.DbContexts;
using TechStore.DAL.SeedData;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TechStore");
builder.Services.AddDbContext<ApplicationDbContext>(
    opts =>
    {
        opts.UseSqlServer(connectionString);
    });

builder.Services.AddLogging(
    configure =>
    {
        configure.AddConsole();
    });

builder.Services.AddScoped<ICatalogService, CatalogService>();
builder.Services.AddScoped<IApplicationDbContext>(
    provider => provider.GetRequiredService<ApplicationDbContext>());

builder.Services.AddControllers();

var app = builder.Build();

app.MapDefaultControllerRoute();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await SeedData.RunSeed(dbContext, 100, logger);
}

await app.RunAsync();
