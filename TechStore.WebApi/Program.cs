using Microsoft.EntityFrameworkCore;
using TechStore.DAL.DbContexts;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("TechStore");
builder.Services.AddDbContext<ApplicationDbContext>(
    opts =>
    {
        opts.UseSqlServer(connectionString);
    });

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
