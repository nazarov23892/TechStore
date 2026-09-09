using Microsoft.EntityFrameworkCore;
using TechStore.DAL.DbContexts;
using TechStore.DAL.SeedData;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TechStore");
builder.Services.AddDbContext<ApplicationDbContext>(
    opts =>
    {
        opts.UseSqlServer(connectionString);
    });

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await SeedData.RunSeed(dbContext, 100);
}

app.MapGet("/", () => "Hello World!");

app.Run();
