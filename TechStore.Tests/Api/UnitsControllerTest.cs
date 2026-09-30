using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechStore.AL.Catalog.Concrete;
using TechStore.Contracts.DTOs;
using TechStore.DAL.DbContexts;
using TechStore.Tests.TestData;
using TechStore.WebApi.Controllers;

namespace TechStore.Tests.Api;

public class UnitsControllerTest
{
    readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public UnitsControllerTest()
    {
        TypeAdapterConfig.GlobalSettings.Scan(typeof(AL.Configuration.AppConstants).Assembly);

        var serviceCollection = new ServiceCollection();
        serviceCollection.AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceCollection.BuildServiceProvider());

        _dbContextOptions = builder.Options;
    }

    [Fact(DisplayName = "Единицы измерения: получение списка: успешно.")]
    public async Task Units_GetList_Successfully()
    {
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext);
        }

        List<UnitListDto>? response;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var categoryService = new UnitsService(dbContext);
            var controller = new UnitsController(categoryService);

            //Act.
            var actionResult = await controller.GetUnitsAsync(default);
            Assert.IsType<ActionResult<IEnumerable<UnitListDto>>>(actionResult);
            response = actionResult.Value?.OrderBy(u => u.Name).ToList();
        }
        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Assert.Equal(6, response.Count);
        Assert.Equal("1-GB", response[0].Name);
        Assert.Equal("2-kB", response[1].Name);
        Assert.Equal("3-MHz", response[2].Name);
        Assert.Equal("4-W", response[3].Name);
        Assert.Equal("5-kg", response[4].Name);
        Assert.Equal("6-cm", response[5].Name);
    }

    static async Task SeedData(ApplicationDbContext dbContext)
    {
        dbContext.Units.AddRange(DataForSeed.DomainModels.Units);

        await dbContext.SaveChangesAsync();
    }
}
