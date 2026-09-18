using Mapster;
using Microsoft.EntityFrameworkCore;
using TechStore.AL.Abstractions;
using TechStore.AL.Catalog;
using TechStore.AL.Catalog.Concrete;
using TechStore.DAL.DbContexts;
using TechStore.WebApi.ExceptionHandlers;
using TechStore.WebApi.SeedData;

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

builder.Services.AddProblemDetails();

// Exception handlers.
builder.Services.AddExceptionHandler<NotFoundExceptionHandler>();

// Services.
builder.Services.AddScoped<IProductsService, ProductsService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IApplicationDbContext>(
    provider => provider.GetRequiredService<ApplicationDbContext>());

// Mapster.
TypeAdapterConfig.GlobalSettings.Scan(typeof(TechStore.AL.Configuration.AppConstants).Assembly);

// CORS.
builder.Services.AddCors(
    options =>
    {
        options.AddPolicy(
            "AllowBlazorClient", 
            policy =>
            {
                policy.WithOrigins("http://localhost:5005")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
            });
    });
builder.Services.AddControllers();

var app = builder.Build();

app.UseCors("AllowBlazorClient"); // Между UseRouting и UseAuthorization.

app.MapDefaultControllerRoute();
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
    var seedDataDir = Path.Combine(env.WebRootPath, "SeedData");

    await SeedData.RunSeed(dbContext, seedDataDir, 100, logger);
}

await app.RunAsync();
