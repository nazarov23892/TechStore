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

    [Fact(DisplayName = "Товары: создание: успешно.")]
    public async Task CreateProduct_Successfully()
    {
        // Arrange.
        long categoryId = 0;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var category1 = new Category()
            {
                Key = "cat1",
                DisplayName = "category1",
            };
            dbContext.Categories.Add(
                category1);
            await dbContext.SaveChangesAsync();
            categoryId = category1.Id;
        }

        var request = new ProductPostDto()
        {
            Name = "product1",
            Description = "description1",
            Price = 999.9M,
            CategoryId = categoryId,
        };
        ProductDto? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new CatalogService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.CreateProduct(request, default);
            Assert.IsType<ActionResult<ProductDto>>(actionResult);
            response = actionResult.Value;
        }

        // Assert.
        Assert.NotNull(response);
        Assert.Equal("product1", response.Name);
        Assert.Equal("description1", response.Description);
        Assert.Equal(999.9M, response.Price);
        Assert.NotNull(response.Category);
        Assert.Equal("cat1", response.Category!.Name);

        Product? domainModel = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            domainModel = await dbContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == response.Id);
        }
        Assert.NotNull(domainModel);
        Assert.Equal("product1", domainModel.Name);
        Assert.Equal("description1", domainModel.Description);
        Assert.Equal(999.9M, domainModel.Price);
        Assert.NotNull(domainModel.Category);
        Assert.Equal("cat1", domainModel.Category!.Key);
    }

    static async Task SeedData(ApplicationDbContext dbContext, int count)
    {
        var category1 = new Category()
        {
            Key = "category1"
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