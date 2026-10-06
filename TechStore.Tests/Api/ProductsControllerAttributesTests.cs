using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechStore.AL.Catalog.Concrete;
using TechStore.BLL.Entities;
using TechStore.Contracts.DTOs;
using TechStore.DAL.DbContexts;
using TechStore.Tests.TestData;
using TechStore.WebApi.Controllers;

namespace TechStore.Tests.Api;

public class ProductsControllerAttributesTests
{
    readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public ProductsControllerAttributesTests()
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

    [Fact(DisplayName = "Товары: Атрибуты: получение списка: успешно.")]
    public async Task Attributes_GetList_Successfully()
    {
        // Arrange.
        long categoryId = 0;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, TestContext.Current.CancellationToken);
            var category = await dbContext.Categories.FirstOrDefaultAsync(TestContext.Current.CancellationToken);
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

            await dbContext.SaveChangesAsync( TestContext.Current.CancellationToken);
            categoryId = category.Id;
        }

        long productId = 0;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var category = await dbContext.Categories
                .Include(c => c.Attributes.OrderBy(a => a.Key))
                .FirstOrDefaultAsync(c => c.Id == categoryId, TestContext.Current.CancellationToken);
            Assert.NotNull(category);

            var product = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.CategoryId == categoryId, TestContext.Current.CancellationToken);
            Assert.NotNull(product);
            productId = product.Id;

            var categoryAttributes = category.Attributes.ToArray();
            Assert.NotNull(categoryAttributes);
            Assert.Equal(3, categoryAttributes.Length);
            Assert.Equal("attr1_numeric", categoryAttributes[0].Key);
            Assert.Equal("attr2_string", categoryAttributes[1].Key);
            Assert.Equal("attr3_boolean", categoryAttributes[2].Key);

            categoryAttributes[0].ProductValues.Add(
                new ProductAttributeValue()
                {
                    ProductId = productId,
                    Value = new AttributeValue()
                    {
                        NumericValue = 1,
                    }
                });
            categoryAttributes[1].ProductValues.Add(
               new ProductAttributeValue()
               {
                   ProductId = productId,
                   Value = new AttributeValue()
                   {
                       StringValue = "string1",
                   }
               });
            categoryAttributes[2].ProductValues.Add(
              new ProductAttributeValue()
              {
                  ProductId = productId,
                  Value = new AttributeValue()
                  {
                      BoolValue = true,
                  }
              });
            await dbContext.SaveChangesAsync( TestContext.Current.CancellationToken);
        }

        List<ProductAttributeListDto>? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new ProductsService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.GetProductAttributes(
                productId, TestContext.Current.CancellationToken);
            Assert.IsType<ActionResult<IEnumerable<ProductAttributeListDto>>>(actionResult);
            response = actionResult.Value?.OrderBy(a => a.Key).ToList();
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
            await SeedData(dbContext, TestContext.Current.CancellationToken);
            var category = await dbContext.Categories.FirstOrDefaultAsync( TestContext.Current.CancellationToken);
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

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
            categoryId = category.Id;

            var product = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.CategoryId == categoryId, TestContext.Current.CancellationToken);
            Assert.NotNull(product);
            productId = product.Id;
        }

        List<ProductAttributeListDto>? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new ProductsService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.GetProductAttributes(
                productId, TestContext.Current.CancellationToken);
            Assert.IsType<ActionResult<IEnumerable<ProductAttributeListDto>>>(actionResult);
            response = actionResult.Value?.OrderBy(a => a.Key).ToList();
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

    [Fact(DisplayName = "Товары: Атрибуты: обновление значений: успешно.")]
    public async Task Attributes_Values_Put_Successfully()
    {
        // Arrange.
        long categoryId = 0;
        long productId = 0;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, TestContext.Current.CancellationToken);

            var category = await dbContext.Categories
                .FirstOrDefaultAsync(TestContext.Current.CancellationToken);
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

            await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
            categoryId = category.Id;

            var product = await dbContext.Products
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.CategoryId == categoryId, TestContext.Current.CancellationToken);
            Assert.NotNull(product);
            productId = product.Id;
        }

        var request = new ProductAttributeValuesPutDto()
        {
            Values = [
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = "attr1_numeric",
                   NumericValue = 57M,
               },
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = "attr2_string",
                   StringValue = "updated-string-value"
               },
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = "attr3_boolean",
                   BooleanValue = true,
               },
            ],
        };
        List<ProductAttributeListDto>? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new ProductsService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.UpdateProductAttributes(
                productId, request, TestContext.Current.CancellationToken);
            Assert.IsType<ActionResult<IEnumerable<ProductAttributeListDto>>>(actionResult);
            response = actionResult.Value?.OrderBy(attr => attr.Key).ToList();
        }

        Assert.NotNull(response);
        Assert.NotEmpty(response);

        Assert.Equal("attr1_numeric", response[0].Key);
        Assert.Equal(57M, response[0].Value.NumericValue);

        Assert.Equal("attr2_string", response[1].Key);
        Assert.Equal("updated-string-value", response[1].Value.StringValue);

        Assert.Equal("attr3_boolean", response[2].Key);
        Assert.Equal(true, response[2].Value.BoolValue);

        List<CategoryAttribute>? domainModel = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var category = await dbContext.Categories
                .AsNoTracking()
                .Include(c => c.Attributes)
                    .ThenInclude(a => a.ProductValues.Take(1))
                .FirstOrDefaultAsync(c => c.Id == categoryId, TestContext.Current.CancellationToken);
            domainModel = category?.Attributes.OrderBy(a => a.Key).ToList();
        }
        Assert.NotNull(domainModel);
        Assert.NotEmpty(domainModel);

        Assert.Equal("attr1_numeric", domainModel[0].Key);
        Assert.Equal(57M, domainModel[0].ProductValues.First().Value.NumericValue);

        Assert.Equal("attr2_string", domainModel[1].Key);
        Assert.Equal("updated-string-value", domainModel[1].ProductValues.First().Value.StringValue);

        Assert.Equal("attr3_boolean", domainModel[2].Key);
        Assert.Equal(true, domainModel[2].ProductValues.First().Value.BoolValue);
    }

    static async Task SeedData(
        ApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        dbContext.Units.AddRange(DataForSeed.DomainModels.Units);
        dbContext.Categories.Add(DataForSeed.DomainModels.Category1_Empty);
        dbContext.Categories.Add(DataForSeed.DomainModels.Category2_CPUs);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
    
    static async Task SeedAdditionalData(
        ApplicationDbContext dbContext, int count, CancellationToken cancellationToken = default)
    {
        for (var i = 0; i < count; i++)
        {
            dbContext.Products.Add(
                new Product()
                {
                    Id = 1 + i,
                    Name = $"product-{1 + i}",
                    Description = $"description-{1 + i}",
                    Price = 1 + i,
                    CategoryId = i <= count / 2 
                        ? DataForSeed.DomainModels.Category1_Empty.Id 
                        : DataForSeed.DomainModels.Category2_CPUs.Id,
                });
        }
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
