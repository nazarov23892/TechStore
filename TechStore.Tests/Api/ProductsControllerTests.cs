using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechStore.AL.Catalog.Concrete;
using TechStore.BLL.Entities;
using TechStore.BLL.Exceptions;
using TechStore.Contracts.DTOs;
using TechStore.DAL.DbContexts;
using TechStore.Tests.TestData;
using TechStore.WebApi.Controllers;

namespace TechStore.Tests.Api;

public class ProductsControllerTests
{
    readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public ProductsControllerTests()
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

    [Fact(DisplayName = "Товары: получение списка: по выбранной категории успешно.")]
    public async Task Products_GetList_Paged_ByCategory_ReturnsOk()
    {
        // Arrange.
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, TestContext.Current.CancellationToken);
            await SeedProducts(dbContext, count: 20, TestContext.Current.CancellationToken);
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
            var catalogService = new ProductsService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.GetProductList(
                paging, category, TestContext.Current.CancellationToken);
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
    public async Task Products_GetList_Paged_WithoutCategory_ReturnsEmpty()
    {
        // Arrange.
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, TestContext.Current.CancellationToken);
            await SeedProducts(dbContext, count: 10, TestContext.Current.CancellationToken);
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
            var catalogService = new ProductsService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.GetProductList(
                paging, category, TestContext.Current.CancellationToken);
            Assert.IsType<ActionResult<PagedListResponseDto<ProductListItemDto>>>(actionResult);
            response = actionResult.Value;
        }

        // Assert.
        Assert.NotNull(response);
        Assert.Empty(response.Items);
    }

    [Fact(DisplayName = "Товары: создание: успешно.")]
    public async Task Products_Create_Successfully()
    {
        // Arrange.
        long categoryId = DataForSeed.DomainModels.Category1_Empty.Id;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, TestContext.Current.CancellationToken);
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
            var catalogService = new ProductsService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.CreateProduct(
                request, TestContext.Current.CancellationToken);
            Assert.IsType<ActionResult<ProductDto>>(actionResult);
            response = actionResult.Value;
        }

        // Assert.
        Assert.NotNull(response);
        Assert.Equal("product1", response.Name);
        Assert.Equal("description1", response.Description);
        Assert.Equal(999.9M, response.Price);
        Assert.NotNull(response.Category);
        Assert.Equal(DataForSeed.DomainModels.Category1_Empty.Key, response.Category.Key);
        Assert.Equal(DataForSeed.DomainModels.Category1_Empty.DisplayName, response.Category.DisplayName);

        Product? domainModel = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            domainModel = await dbContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == response.Id, TestContext.Current.CancellationToken);
        }
        Assert.NotNull(domainModel);
        Assert.Equal("product1", domainModel.Name);
        Assert.Equal("description1", domainModel.Description);
        Assert.Equal(999.9M, domainModel.Price);
        Assert.Equal(categoryId, domainModel.CategoryId);
    }

    [Theory(DisplayName = "Товары: обновление: успешно.")]
    [MemberData(nameof(GetInlineDataForPut))]
    public async Task Products_Update_Successfully(
        long productId,
        ProductPutDto request,
        ProductDto expectedResponse,
        Product expectedDomainModel)
    {
        // Arrange.
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, TestContext.Current.CancellationToken);
            await SeedProducts(dbContext, count: 1, TestContext.Current.CancellationToken);
        }

        ProductDto? actualResponse = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new ProductsService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.UpdateProduct(
                productId, request, TestContext.Current.CancellationToken);
            Assert.IsType<ActionResult<ProductDto>>(actionResult);
            actualResponse = actionResult.Value;
        }

        // Assert.
        Assert.NotNull(actualResponse);
        Assert.Equivalent(expectedResponse, actualResponse);

        Product? actualDomainModel = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            actualDomainModel = await dbContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == actualResponse.Id, TestContext.Current.CancellationToken);
        }
        Assert.NotNull(actualDomainModel);
        Assert.EquivalentWithExclusions(expectedDomainModel, actualDomainModel,
            p => p.Category);
    }

    [Fact(DisplayName = "Товары: создание: в несуществующей категории: ошибка.")]
    public async Task Products_Create_NonExistingCategory_Error()
    {
        // Arrange.
        long nonExistingCategoryId = 501;
        var request = new ProductPostDto()
        {
            Name = "product1",
            Description = "description1",
            Price = 999.9M,
            CategoryId = nonExistingCategoryId,
        };
        using var dbContext = new ApplicationDbContext(_dbContextOptions);
        var catalogService = new ProductsService(dbContext);
        var controller = new ProductsController(catalogService);

        //Act.
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => controller.CreateProduct(request, TestContext.Current.CancellationToken));
        Assert.NotNull(exception);
    }

    public static IEnumerable<TheoryDataRow<long, ProductPutDto, ProductDto, Product>> GetInlineDataForPut()
    {
        yield return new TheoryDataRow<long, ProductPutDto, ProductDto, Product>(

            // productId.
            1L,

            // putRequest.
            new ProductPutDto()
            {
                Name = "new-product1",
                Description = "new-description1",
                Price = 42M,
            },

            // expectedResponse.
            new ProductDto()
            {
                Id = 1,
                Name = "new-product1",
                Description = "new-description1",
                Price = 42M,
                Category = new()
                {
                    Id = 101,
                    Key = "category1",
                    DisplayName = "displayname1",
                },
            },

            // expectedDomainModel.
            new Product()
            {
                Id = 1,
                Name = "new-product1",
                Description = "new-description1",
                Price = 42M,
                CategoryId = 101,
            }
        );
    }

    static async Task SeedData(
        ApplicationDbContext dbContext, CancellationToken cancellationToken)
    {
        dbContext.Units.AddRange(DataForSeed.DomainModels.Units);
        dbContext.Categories.Add(DataForSeed.DomainModels.Category1_Empty);
        dbContext.Categories.Add(DataForSeed.DomainModels.Category2_CPUs);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    static async Task SeedProducts(
        ApplicationDbContext dbContext, int count, CancellationToken cancellationToken)
    {
        var category1 = new Category()
        {
            Key = "category1",
            DisplayName = "displayname1",
        };
        var category2 = new Category()
        {
            Key = "category2",
            DisplayName = "displayname2",
        };
        for (var i = 0; i < count; i++)
        {
            var product = new Product()
            {
                Id = 1 + i,
                Name = $"product-{1 + i}",
                Description = $"description-{1 + i}",
                Price = 1 + i,
                Category = i <= count / 2 ? category1 : category2,
            };
            dbContext.Products.Add(product);
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}