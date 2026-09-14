using Azure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.NetworkInformation;
using TechStore.AL.Catalog.Concrete;
using TechStore.BLL.Entities;
using TechStore.Contracts.DTOs;
using TechStore.DAL.DbContexts;
using TechStore.WebApi.Controllers;

namespace TechStore.Tests.Api;


public class CategoriesControllerTest
{
    readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public CategoriesControllerTest()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceCollection.BuildServiceProvider());

        _dbContextOptions = builder.Options;
    }

    [Fact(DisplayName = "Получение постраничного списка категорий успешно.")]
    public async Task GetPagedCategories_ReturnsOk()
    {
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, 10);
        }

        var paging = new PagingRequestDto()
        {
            Page = 1,
            PerPage = 3,
        };
        PagedListResponseDto<CategoryListDto>? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new CatalogService(dbContext);
            var controller = new CategoriesController(catalogService);

            //Act.
            var actionResult = await controller.GetCategories(paging, default);
            Assert.IsType<ActionResult<PagedListResponseDto<CategoryListDto>>>(actionResult);
            response = actionResult.Value;
        }
        Assert.NotNull(response);
        Assert.NotEmpty(response.Items);
        Assert.Equal(4, response.TotalPages);
        Assert.Equal(1, response.Page);
        Assert.Equal(3, response.PerPage);
        var categories = response.Items.ToList();
        Assert.Equal("category-1", categories[0].Key);
        Assert.Equal("category-2", categories[1].Key);
        Assert.Equal("category-3", categories[2].Key);
        Assert.Equal("display-name-1", categories[0].DisplayName);
        Assert.Equal("display-name-2", categories[1].DisplayName);
        Assert.Equal("display-name-3", categories[2].DisplayName);
    }

    [Fact(DisplayName = "Категории: создание категории успешно.")]
    public async Task CreateCategory_Successfully()
    {
        var request = new CategoryPostDto()
        {
            Key = "cat1",
            DisplayName = "name1",
        };
        CategoryDto? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new CatalogService(dbContext);
            var controller = new CategoriesController(catalogService);

            //Act.
            var actionResult = await controller.CreateCategory(request, default);
            Assert.IsType<ActionResult<CategoryDto>>(actionResult);
            response = actionResult.Value;
        }
        Assert.NotNull(response);
        Assert.Equal("cat1", response.Key);
        Assert.Equal("name1", response.DisplayName);

        Category? domainModel = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            domainModel = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == response.Id);
        }
        Assert.NotNull(domainModel);
        Assert.Equal("cat1", domainModel.Key);
        Assert.Equal("name1", domainModel.DisplayName);
    }

    static async Task SeedData(ApplicationDbContext dbContext, int count)
    {
        for (var i = 0; i < count; i++)
        {
            dbContext.Categories.Add(
                new Category()
                {
                    Id = 1 + i,
                    Key = $"category-{1 + i}",
                    DisplayName = $"display-name-{1 + i}"
                });
        }
        await dbContext.SaveChangesAsync();
    }
}
