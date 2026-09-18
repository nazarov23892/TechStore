using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechStore.AL.Catalog.Concrete;
using TechStore.BLL.Entities;
using TechStore.BLL.Exceptions;
using TechStore.Contracts.DTOs;
using TechStore.DAL.DbContexts;
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
            await SeedData(dbContext, 20);
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
    public async Task Products_GetList_Paged_WithoutCategory_ReturnsEmpty()
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
            var catalogService = new ProductsService(dbContext);
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
    public async Task Products_Create_Successfully()
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
            dbContext.Categories.Add(category1);
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
            var catalogService = new ProductsService(dbContext);
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
        Assert.Equal("cat1", response.Category!.Key);
        Assert.Equal("category1", response.Category.DisplayName);

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

    [Fact(DisplayName = "Товары: обновление: успешно.")]
    public async Task Products_Update_Successfully()
    {
        // Arrange.
        long productId = 0;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, count: 1);
            var product = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync();
            Assert.NotNull(product);
            productId = product.Id;
        }

        var request = new ProductPutDto()
        {
            Name = "new-product1",
            Description = "new-description1",
            Price = 42M,
        };
        ProductDto? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new ProductsService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.UpdateProduct(productId, request, default);
            Assert.IsType<ActionResult<ProductDto>>(actionResult);
            response = actionResult.Value;
        }

        // Assert.
        Assert.NotNull(response);

        Assert.Equal("new-product1", response.Name);
        Assert.Equal("new-description1", response.Description);
        Assert.Equal(42M, response.Price);
        Assert.NotNull(response.Category);
        Assert.Equal("category1", response.Category.Key);
        Assert.Equal("displayname1", response.Category.DisplayName);

        Product? domainModel = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            domainModel = await dbContext.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == response.Id);
        }
        Assert.NotNull(domainModel);
        Assert.Equal("new-product1", domainModel.Name);
        Assert.Equal("new-description1", domainModel.Description);
        Assert.Equal(42M, domainModel.Price);
        Assert.NotNull(domainModel.Category);
        Assert.Equal("category1", domainModel.Category.Key);
        Assert.Equal("displayname1", domainModel.Category.DisplayName);
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
            () => controller.CreateProduct(request, default));
        Assert.NotNull(exception);
    }

    [Fact(DisplayName = "Товары: Атрибуты: получение списка: успешно.")]
    public async Task Attributes_GetList_Successfully()
    {
        // Arrange.
        long categoryId = 0;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, 2);
            var category = await dbContext.Categories.FirstOrDefaultAsync();
            Assert.NotNull(category);

            category.Attributes.Add(
                new CategoryAttribute()
                {
                    Key = "attr1_numeric",
                    DataType = CategoryAttributeDataTypes.Numeric,
                });
            category.Attributes.Add(
                new CategoryAttribute()
                {
                    Key = "attr2_string",
                    DataType = CategoryAttributeDataTypes.String,
                });
            category.Attributes.Add(
               new CategoryAttribute()
               {
                   Key = "attr3_boolean",
                   DataType = CategoryAttributeDataTypes.Boolean,
               });

            await dbContext.SaveChangesAsync();
            categoryId = category.Id;
        }

        long productId = 0;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var category = await dbContext.Categories
                .AsNoTracking()
                .Include(c => c.Attributes.OrderBy(a => a.Key))
                .FirstOrDefaultAsync(c => c.Id == categoryId);
            Assert.NotNull(category);

            var product = await dbContext.Products
                .FirstOrDefaultAsync(p => p.CategoryId == categoryId);
            Assert.NotNull(product);

            var categoryAttributes = category.Attributes.ToArray();
            Assert.NotNull(categoryAttributes);
            Assert.Equal(3, categoryAttributes.Length);
            Assert.Equal("attr1_numeric", categoryAttributes[0].Key);
            Assert.Equal("attr2_string", categoryAttributes[1].Key);
            Assert.Equal("attr3_boolean", categoryAttributes[2].Key);

            product.Attributes.Add(
                new ProductAttribute()
                {
                    CategoryAttributeId = categoryAttributes[0].Id,
                    Value = new AttributeValue()
                    {
                        NumericValue = 1,
                    }
                });
            product.Attributes.Add(
               new ProductAttribute()
               {
                   CategoryAttributeId = categoryAttributes[1].Id,
                   Value = new AttributeValue()
                   {
                       StringValue = "string1",
                   }
               });
            product.Attributes.Add(
              new ProductAttribute()
              {
                  CategoryAttributeId = categoryAttributes[2].Id,
                  Value = new AttributeValue()
                  {
                      BoolValue = true,
                  }
              });
            await dbContext.SaveChangesAsync();
            productId = product.Id;
        }

        List<ProductAttributeListDto>? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new ProductsService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.GetProductAttributes(productId, default);
            Assert.IsType<ActionResult<IEnumerable<ProductAttributeListDto>>>(actionResult);
            response = actionResult.Value?.ToList();
        }
        Assert.NotNull(response);
        Assert.NotEmpty(response);

        Assert.NotNull(response);
        Assert.NotEmpty(response);

        Assert.Equal("attr1_numeric", response[0].Key);
        Assert.Equal("Numeric", response[0].DataType);
        Assert.NotNull(response[0].Value.NumericValue);
        Assert.Null(response[0].Value.StringValue);
        Assert.Null(response[0].Value.BoolValue);

        Assert.Equal("attr2_string", response[1].Key);
        Assert.Equal("String", response[1].DataType);
        Assert.Null(response[1].Value.NumericValue);
        Assert.NotNull(response[1].Value.StringValue);
        Assert.Null(response[1].Value.BoolValue);

        Assert.Equal("attr3_boolean", response[2].Key);
        Assert.Equal("Boolean", response[2].DataType);
        Assert.Null(response[2].Value.NumericValue);
        Assert.Null(response[2].Value.StringValue);
        Assert.NotNull(response[2].Value.BoolValue);
    }

    /// <remarks>
    /// Должен возвращать список атрибутов категории,
    /// даже если в самом товаре не назначены значения.
    /// </remarks>
    [Fact(DisplayName = "Товары: Атрибуты: получение списка: когда значения атрибутов не установлены: успешно.")]
    public async Task Attributes_GetList_WhenValuesNonExist_Successfully()
    {
        // Arrange.
        long categoryId = 0;
        long productId = 0;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, 2);
            var category = await dbContext.Categories.FirstOrDefaultAsync();
            Assert.NotNull(category);

            category.Attributes.Add(
                new CategoryAttribute()
                {
                    Key = "attr1_numeric",
                    DataType = CategoryAttributeDataTypes.Numeric,
                });
            category.Attributes.Add(
                new CategoryAttribute()
                {
                    Key = "attr2_string",
                    DataType = CategoryAttributeDataTypes.String,
                });
            category.Attributes.Add(
               new CategoryAttribute()
               {
                   Key = "attr3_boolean",
                   DataType = CategoryAttributeDataTypes.Boolean,
               });

            await dbContext.SaveChangesAsync();
            categoryId = category.Id;

            var product = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.CategoryId == categoryId);
            Assert.NotNull(product);
            productId = product.Id;
        }

        List<ProductAttributeListDto>? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new ProductsService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.GetProductAttributes(productId, default);
            Assert.IsType<ActionResult<IEnumerable<ProductAttributeListDto>>>(actionResult);
            response = actionResult.Value?.ToList();
        }
        Assert.NotNull(response);
        Assert.NotEmpty(response);

        Assert.NotNull(response);
        Assert.NotEmpty(response);

        Assert.Equal("attr1_numeric", response[0].Key);
        Assert.Equal("Numeric", response[0].DataType);
        Assert.Null(response[0].Value.NumericValue);
        Assert.Null(response[0].Value.StringValue);
        Assert.Null(response[0].Value.BoolValue);

        Assert.Equal("attr2_string", response[1].Key);
        Assert.Equal("String", response[1].DataType);
        Assert.Null(response[1].Value.NumericValue);
        Assert.Null(response[1].Value.StringValue);
        Assert.Null(response[1].Value.BoolValue);

        Assert.Equal("attr3_boolean", response[2].Key);
        Assert.Equal("Boolean", response[2].DataType);
        Assert.Null(response[2].Value.NumericValue);
        Assert.Null(response[2].Value.StringValue);
        Assert.Null(response[2].Value.BoolValue);
    }

    static async Task SeedData(ApplicationDbContext dbContext, int count)
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
            dbContext.Products.Add(
                new Product()
                {
                    Id = 1 + i,
                    Name = $"product-{1 + i}",
                    Description = $"description-{1 + i}",
                    Price = 1 + i,
                    Category = i <= count / 2 ? category1 : category2,
                });
        }
        await dbContext.SaveChangesAsync();
    }
}