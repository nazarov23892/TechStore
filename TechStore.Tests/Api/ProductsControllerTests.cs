using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechStore.AL.Catalog.Concrete;
using TechStore.BLL.Entities;
using TechStore.Contracts.DTOs;
using TechStore.DAL.DbContexts;
using TechStore.WebApi.Controllers;

namespace TechStore.Tests.Api;

public class ProductsControllerTests
{
    readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public ProductsControllerTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceCollection.BuildServiceProvider());

        _dbContextOptions = builder.Options;
    }

    [Fact(DisplayName = "Товары: получение списка: по выбранной категории успешно.")]
    public async Task GetPagedProductsByCategory_ReturnsOk()
    {
        // Arrange.
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, 10);
        }
        var paging = new PagingRequestDto()
        {
            Page = 1,
            PerPage = 3,
        };
        var category = "category1";
        PagedListResponseDto<ProductListItemDto>? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new CatalogService(dbContext);
            var controller = new ProductsController(catalogService);
            
            //Act.
            var actionResult = await controller.GetProductList(
                paging, category, default);
            Assert.IsType<ActionResult<PagedListResponseDto<ProductListItemDto>>>(actionResult);
            response = actionResult.Value;
        }

        // Assert.
        Assert.NotNull(response);
        Assert.NotEmpty(response.Items);
        Assert.Equal(4, response.TotalPages);
        Assert.Equal(1, response.Page);
        Assert.Equal(3, response.PerPage);

        var items = response.Items.ToList();
        Assert.Equal(3, items.Count);
        Assert.Equal(1, items[0].Id);
        Assert.Equal(2, items[1].Id);
        Assert.Equal(3, items[2].Id);
    }

    [Fact(DisplayName = "Товары: получение списка: без выбранной категории пустой список.")]
    public async Task GetPagedProductsWithoutCategory_ReturnsEmpty()
    {
        // Arrange.
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, 10);
        }
        var paging = new PagingRequestDto()
        {
            Page = 1,
            PerPage = 3,
        };
        string? category = null; 
        PagedListResponseDto<ProductListItemDto>? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new CatalogService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.GetProductList(
                paging, category, default);
            Assert.IsType<ActionResult<PagedListResponseDto<ProductListItemDto>>>(actionResult);
            response = actionResult.Value;
        }

        // Assert.
        Assert.NotNull(response);
        Assert.Empty(response.Items);
    }

    static async Task SeedData(ApplicationDbContext dbContext, int count)
    {
        var category1 = new Category()
        {
            Name = "category1"
        };
        for (var i = 0; i < count; i++)
        {
            dbContext.Products.Add(
                new Product()
                {
                    Id = 1 + i,
                    Name = $"product-{1 + i}",
                    Description = $"description-{1 + i}",
                    Price = 1 + i,
                    Category = category1,
                });
        }
        await dbContext.SaveChangesAsync();
    }
}