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
        var cancellationToken = TestContext.Current.CancellationToken;
        long categoryId = DataForSeed.DomainModels.Category2_CPUs.Id;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, cancellationToken);
        }

        long productId = DataForSeed.DomainModels.Product1_Intel.Id;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var category = await dbContext.Categories
                .Include(c => c.Attributes).ThenInclude(a => a.ProductValues)
                .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken);
            Assert.NotNull(category);

            var attributesByKey = category.Attributes.ToDictionary(a => a.Key);
            var attrFrequency = attributesByKey[DataForSeed.DomainModels.Attribute1_Frequency.Key];
            var attrGpuIntegrated = attributesByKey[DataForSeed.DomainModels.Attribute2_GpuIntegrated.Key];
            var attrSocket = attributesByKey[DataForSeed.DomainModels.Attribute3_Socket.Key];
            var attrTdp = attributesByKey[DataForSeed.DomainModels.Attribute4_Tdp.Key];

            attrFrequency.ProductValues.Add(
                new()
                {
                    ProductId = productId,
                    Value = new AttributeValue()
                    {
                        NumericValue = 3.3M,
                    },
                });
            attrGpuIntegrated.ProductValues.Add(
                new()
                {
                    ProductId = productId,
                    Value = new AttributeValue()
                    {
                        BoolValue = true,
                    },
                });
            attrSocket.ProductValues.Add(
               new()
               {
                   ProductId = productId,
                   Value = new AttributeValue()
                   {
                       StringValue = "LGA1700",
                   },
               });
            attrTdp.ProductValues.Add(
                new()
                {
                    ProductId = productId,
                    Value = new AttributeValue()
                    {
                        NumericValue = 120,
                    },
                });
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        List<ProductAttributeListDto>? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new ProductsService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.GetProductAttributes(
                productId, cancellationToken);
            Assert.IsType<ActionResult<IEnumerable<ProductAttributeListDto>>>(actionResult);
            response = actionResult.Value?.OrderBy(a => a.Key).ToList();
        }
        Assert.NotNull(response);
        Assert.NotEmpty(response);

        Assert.NotNull(response);
        Assert.NotEmpty(response);

        Assert.Equal(DataForSeed.DomainModels.Attribute1_Frequency.Key, response[0].Key);
        Assert.Equal(DataForSeed.DomainModels.Attribute1_Frequency.DataType.ToString(), response[0].DataType);
        Assert.NotNull(response[0].Value.NumericValue);
        Assert.Null(response[0].Value.StringValue);
        Assert.Null(response[0].Value.BoolValue);
        Assert.Equal(3.3M, response[0].Value.NumericValue);

        Assert.Equal(DataForSeed.DomainModels.Attribute2_GpuIntegrated.Key, response[1].Key);
        Assert.Equal(DataForSeed.DomainModels.Attribute2_GpuIntegrated.DataType.ToString(), response[1].DataType);
        Assert.Null(response[1].Value.NumericValue);
        Assert.NotNull(response[1].Value.BoolValue);
        Assert.Null(response[1].Value.StringValue);
        Assert.Equal(true, response[1].Value.BoolValue);

        Assert.Equal(DataForSeed.DomainModels.Attribute3_Socket.Key, response[2].Key);
        Assert.Equal(DataForSeed.DomainModels.Attribute3_Socket.DataType.ToString(), response[2].DataType);
        Assert.Null(response[2].Value.NumericValue);
        Assert.Null(response[2].Value.BoolValue);
        Assert.NotNull(response[2].Value.StringValue);
        Assert.Equal("LGA1700", response[2].Value.StringValue);

        Assert.Equal(DataForSeed.DomainModels.Attribute4_Tdp.Key, response[3].Key);
        Assert.Equal(DataForSeed.DomainModels.Attribute4_Tdp.DataType.ToString(), response[3].DataType);
        Assert.NotNull(response[3].Value.NumericValue);
        Assert.Null(response[3].Value.BoolValue);
        Assert.Null(response[3].Value.StringValue);
        Assert.Equal(120, response[3].Value.NumericValue);
    }

    /// <remarks>
    /// Должен возвращать список атрибутов категории,
    /// даже если в самом товаре не назначены значения.
    /// </remarks>
    [Fact(DisplayName = "Товары: Атрибуты: получение списка: когда значения атрибутов не установлены: успешно.")]
    public async Task Attributes_GetList_WhenValuesNonExist_Successfully()
    {
        // Arrange.
        var cancellationToken = TestContext.Current.CancellationToken;
        long categoryId = DataForSeed.DomainModels.Category2_CPUs.Id;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, cancellationToken);
        }

        long productId = DataForSeed.DomainModels.Product1_Intel.Id;
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
        Assert.Equal(4, response.Count);

        Assert.Equal(DataForSeed.DomainModels.Attribute1_Frequency.Key, response[0].Key);
        Assert.Equal(DataForSeed.DomainModels.Attribute1_Frequency.DataType.ToString(), response[0].DataType);
        Assert.Null(response[0].Value.NumericValue);
        Assert.Null(response[0].Value.StringValue);
        Assert.Null(response[0].Value.BoolValue);

        Assert.Equal(DataForSeed.DomainModels.Attribute2_GpuIntegrated.Key, response[1].Key);
        Assert.Equal(DataForSeed.DomainModels.Attribute2_GpuIntegrated.DataType.ToString(), response[1].DataType);
        Assert.Null(response[1].Value.NumericValue);
        Assert.Null(response[1].Value.BoolValue);
        Assert.Null(response[1].Value.StringValue);

        Assert.Equal(DataForSeed.DomainModels.Attribute3_Socket.Key, response[2].Key);
        Assert.Equal(DataForSeed.DomainModels.Attribute3_Socket.DataType.ToString(), response[2].DataType);
        Assert.Null(response[2].Value.NumericValue);
        Assert.Null(response[2].Value.BoolValue);
        Assert.Null(response[2].Value.StringValue);

        Assert.Equal(DataForSeed.DomainModels.Attribute4_Tdp.Key, response[3].Key);
        Assert.Equal(DataForSeed.DomainModels.Attribute4_Tdp.DataType.ToString(), response[3].DataType);
        Assert.Null(response[3].Value.NumericValue);
        Assert.Null(response[3].Value.BoolValue);
        Assert.Null(response[3].Value.StringValue);
    }

    [Fact(DisplayName = "Товары: Атрибуты: обновление значений: успешно.")]
    public async Task Attributes_Values_Put_Successfully()
    {
        // Arrange.
        var cancellationToken = TestContext.Current.CancellationToken;
        long categoryId = DataForSeed.DomainModels.Category2_CPUs.Id;
        long productId = DataForSeed.DomainModels.Product1_Intel.Id;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, cancellationToken);
        }

        var request = new ProductAttributeValuesPutDto()
        {
            Values = [
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = DataForSeed.DomainModels.Attribute1_Frequency.Key,
                   NumericValue = 3.3M,
               },
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = DataForSeed.DomainModels.Attribute2_GpuIntegrated.Key,
                   BooleanValue = true,
               },
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = DataForSeed.DomainModels.Attribute3_Socket.Key,
                   StringValue = "LGA1700",
               },
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = DataForSeed.DomainModels.Attribute4_Tdp.Key,
                   NumericValue = 120,
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
                productId, request, cancellationToken);
            Assert.IsType<ActionResult<IEnumerable<ProductAttributeListDto>>>(actionResult);
            response = actionResult.Value?.OrderBy(attr => attr.Key).ToList();
        }

        Assert.NotNull(response);
        Assert.NotEmpty(response);

        var expectedAttributes = new ProductAttributeListDto[]
        {
            new()
            {
                Key = DataForSeed.DomainModels.Attribute1_Frequency.Key,
                DataType = DataForSeed.DomainModels.Attribute1_Frequency.DataType.ToString(),
                Unit = new UnitShortDto()
                {
                    Name = DataForSeed.DomainModels.Unit3_Mhz.Name,
                },
                Value = new ProductAttributeValueDto()
                {
                    NumericValue = 3.3M,
                }
            },
            new()
            {
                Key = DataForSeed.DomainModels.Attribute2_GpuIntegrated.Key,
                DataType = DataForSeed.DomainModels.Attribute2_GpuIntegrated.DataType.ToString(),
                Unit = null,
                Value = new ProductAttributeValueDto()
                {
                    BoolValue = true,
                }
            },
            new()
            {
                Key = DataForSeed.DomainModels.Attribute3_Socket.Key,
                DataType = DataForSeed.DomainModels.Attribute3_Socket.DataType.ToString(),
                Unit = null,
                Value = new ProductAttributeValueDto()
                {
                    StringValue = "LGA1700",
                }
            },
            new()
            {
                Key = DataForSeed.DomainModels.Attribute4_Tdp.Key,
                DataType = DataForSeed.DomainModels.Attribute4_Tdp.DataType.ToString(),
                Unit = new UnitShortDto()
                {
                    Name = DataForSeed.DomainModels.Unit4_W.Name,
                },
                Value = new ProductAttributeValueDto()
                {
                    NumericValue = 120,
                }
            },
        };

        Assert.Equal(expectedAttributes.Length, response.Count);

        for (var i = 0; i < expectedAttributes.Length; i++)
        {
            var expected = expectedAttributes[i];
            var actual = response[i];

            Assert.EquivalentWithExclusions(
                expected, actual,
                attr => attr.Id,
                attr => attr.Unit,
                attr => attr.Value);

            if (expected.Unit != null)
            {
                Assert.NotNull(actual.Unit);
                Assert.EquivalentWithExclusions(expected.Unit, actual.Unit, u => u.Id);
            }
            Assert.EquivalentWithExclusions(
                expected.Value, actual.Value);
        }
    }

    static async Task SeedData(
        ApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        dbContext.Units.AddRange(DataForSeed.DomainModels.Units);
        dbContext.Categories.Add(DataForSeed.DomainModels.Category1_Empty);
        dbContext.Categories.Add(DataForSeed.DomainModels.Category2_CPUs);
        dbContext.Products.Add(DataForSeed.DomainModels.Product1_Intel);
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
