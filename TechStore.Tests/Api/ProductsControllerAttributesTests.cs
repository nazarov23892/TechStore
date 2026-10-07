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
        long productId = DataForSeed.DomainModels.Product1_IntelCore_i3.Id;

        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, cancellationToken);
        }

        ProductAttributeListDto[]? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new ProductsService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.GetProductAttributes(
                productId, cancellationToken);
            Assert.IsType<ActionResult<IEnumerable<ProductAttributeListDto>>>(actionResult);
            response = actionResult.Value?.OrderBy(a => a.Key).ToArray();
        }
        Assert.NotNull(response);

        var expectedResponseItems = new ProductAttributeListDto[]
        {
            new()
            {
                Id = DataForSeed.DomainModels.Attribute1_Frequency.Id,
                Key = DataForSeed.DomainModels.Attribute1_Frequency.Key,
                DataType = DataForSeed.DomainModels.Attribute1_Frequency.DataType.ToString(),
                Unit = new UnitShortDto() { Name = DataForSeed.DomainModels.Unit3_Mhz.Name, },
                Value = new ProductAttributeValueDto() { NumericValue =  3.3M },
            },
            new()
            {
                Id = DataForSeed.DomainModels.Attribute2_GpuIntegrated.Id,
                Key = DataForSeed.DomainModels.Attribute2_GpuIntegrated.Key,
                DataType = DataForSeed.DomainModels.Attribute2_GpuIntegrated.DataType.ToString(),
                Unit = null,
                Value = new ProductAttributeValueDto() { BoolValue = true, },
            },
            new()
            {
                Id = DataForSeed.DomainModels.Attribute3_Socket.Id,
                Key = DataForSeed.DomainModels.Attribute3_Socket.Key,
                DataType = DataForSeed.DomainModels.Attribute3_Socket.DataType.ToString(),
                Unit = null,
                Value = new ProductAttributeValueDto() { StringValue = "LGA1700", },
            },
            new()
            {
                Id = DataForSeed.DomainModels.Attribute4_Tdp.Id,
                Key = DataForSeed.DomainModels.Attribute4_Tdp.Key,
                DataType = DataForSeed.DomainModels.Attribute4_Tdp.DataType.ToString(),
                Unit = new UnitShortDto() { Name = DataForSeed.DomainModels.Unit4_W.Name, },
                Value = new ProductAttributeValueDto() { NumericValue = 120, },
            },
        };
        Assert.Equal(expectedResponseItems.Length, response.Length);
        for (var i = 0; i < response.Length; i++)
        {
            var expected = expectedResponseItems[i];
            var actual = response[i];
            Assert.EquivalentWithExclusions(expected, actual,
                attr => attr.Unit,
                attr => attr.Value);
            Assert.Equivalent(expected.Value, actual.Value);
            if (expected.Unit == null)
            {
                Assert.Null(actual.Unit);
            }
            else
            {
                Assert.NotNull(actual.Unit);
                Assert.Equivalent(expected.Unit, actual.Unit);
            }
        }
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
        long productId = DataForSeed.DomainModels.Product2_IntelCore_i5.Id;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, cancellationToken);
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

    [Theory(DisplayName = "Товары: Атрибуты: обновление значений: успешно.")]
    [MemberData(nameof(GetInlineDataForPut))]
    public async Task Attributes_Values_Put_Successfully(
        long productId,
        ProductAttributeValueListPutDto[] requestItems,
        ProductAttributeListDto[] expectedResponseItems,
        ProductAttributeValue[] expectedDomainItems)
    {
        // Arrange.
        var cancellationToken = TestContext.Current.CancellationToken;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, cancellationToken);
        }

        var request = new ProductAttributeValuesPutDto()
        {
            Values = requestItems,
        };
        ProductAttributeListDto[]? responseValues = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var catalogService = new ProductsService(dbContext);
            var controller = new ProductsController(catalogService);

            //Act.
            var actionResult = await controller.UpdateProductAttributes(
                productId, request, cancellationToken);
            Assert.IsType<ActionResult<IEnumerable<ProductAttributeListDto>>>(actionResult);
            responseValues = actionResult.Value?.OrderBy(attr => attr.Key).ToArray();
        }

        Assert.NotNull(responseValues);
        Assert.Equal(expectedResponseItems.Length, responseValues.Length);

        for (var i = 0; i < expectedResponseItems.Length; i++)
        {
            var expected = expectedResponseItems[i];
            var actual = responseValues[i];

            Assert.EquivalentWithExclusions(
                expected, actual,
                attr => attr.Id,
                attr => attr.Unit,
                attr => attr.Value);

            if (expected.Unit == null)
            {
                Assert.Null(actual.Unit);
            }
            else
            {
                Assert.NotNull(actual.Unit);
                Assert.EquivalentWithExclusions(expected.Unit, actual.Unit, u => u.Id);
            }
            Assert.Equivalent(expected.Value, actual.Value);
        }

        ProductAttributeValue[]? actualDomainItems;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            actualDomainItems = await dbContext.ProductAttributeValues
                .AsNoTracking()
                .Include(av => av.CategoryAttribute)
                .Where(av => av.ProductId == productId)
                .ToArrayAsync(cancellationToken);
        }
        Assert.NotNull(actualDomainItems);
        Assert.Equal(expectedDomainItems.Length, actualDomainItems.Length);
        for (var i = 0; i < expectedDomainItems.Length; i++)
        {
            var expected = expectedDomainItems[i];
            var actual = actualDomainItems[i];
            Assert.EquivalentWithExclusions(actual, expected,
                av => av.Id,
                av => av.Value,
                av => av.CategoryAttribute,
                av => av.Product
                );
            Assert.Equivalent(expected.Value, actual.Value);
        }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "xUnit1047:Avoid using TheoryDataRow arguments that might not be serializable", Justification = "<Ожидание>")]
    public static IEnumerable<TheoryDataRow<long, ProductAttributeValueListPutDto[], ProductAttributeListDto[], ProductAttributeValue[]>> GetInlineDataForPut()
    {
        yield return new TheoryDataRow<long, ProductAttributeValueListPutDto[], ProductAttributeListDto[], ProductAttributeValue[]>(

            // productId.
            DataForSeed.DomainModels.Product1_IntelCore_i3.Id,

            // requestValues.
            [
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = DataForSeed.DomainModels.Attribute1_Frequency.Key,
                   NumericValue = 3.7M,
               },
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = DataForSeed.DomainModels.Attribute2_GpuIntegrated.Key,
                   BooleanValue = true,
               },
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = DataForSeed.DomainModels.Attribute3_Socket.Key,
                   StringValue = "LGA1701",
               },
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = DataForSeed.DomainModels.Attribute4_Tdp.Key,
                   NumericValue = 121,
               },
            ],

            // expectedResponseItems.
            [
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
                        NumericValue = 3.7M,
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
                        StringValue = "LGA1701",
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
                        NumericValue = 121,
                    }
                },
            ],

            // expectedDomainItems.
            [
                new ProductAttributeValue()
                {
                    ProductId = DataForSeed.DomainModels.Product1_IntelCore_i3.Id,
                    CategoryAttributeId = DataForSeed.DomainModels.Attribute1_Frequency.Id,
                    Value = new AttributeValue()
                    {
                        NumericValue = 3.7M,
                    },
                },
                new ProductAttributeValue()
                {
                    ProductId = DataForSeed.DomainModels.Product1_IntelCore_i3.Id,
                    CategoryAttributeId = DataForSeed.DomainModels.Attribute2_GpuIntegrated.Id,
                    Value = new AttributeValue()
                    {
                        BoolValue = true,
                    },
                },
                new ProductAttributeValue()
                {
                    ProductId = DataForSeed.DomainModels.Product1_IntelCore_i3.Id,
                    CategoryAttributeId = DataForSeed.DomainModels.Attribute3_Socket.Id,
                    Value = new AttributeValue()
                    {
                        StringValue = "LGA1701",
                    },
                },
                new ProductAttributeValue()
                {
                    ProductId = DataForSeed.DomainModels.Product1_IntelCore_i3.Id,
                    CategoryAttributeId = DataForSeed.DomainModels.Attribute4_Tdp.Id,
                    Value = new AttributeValue()
                    {
                        NumericValue = 121,
                    },
                },
            ]
        );

        //********************************************************************

        yield return new TheoryDataRow<long, ProductAttributeValueListPutDto[], ProductAttributeListDto[], ProductAttributeValue[]>(

            // productId.
            DataForSeed.DomainModels.Product2_IntelCore_i5.Id,

            // requestValues.
            [
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = DataForSeed.DomainModels.Attribute1_Frequency.Key,
                   NumericValue = 4.33M,
               },
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = DataForSeed.DomainModels.Attribute2_GpuIntegrated.Key,
                   BooleanValue = false,
               },
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = DataForSeed.DomainModels.Attribute3_Socket.Key,
                   StringValue = "LGA1702",
               },
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = DataForSeed.DomainModels.Attribute4_Tdp.Key,
                   NumericValue = 132,
               },
            ],

            // expectedResponseItems.
            [
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
                        NumericValue = 4.33M,
                    }
                },
                new()
                {
                    Key = DataForSeed.DomainModels.Attribute2_GpuIntegrated.Key,
                    DataType = DataForSeed.DomainModels.Attribute2_GpuIntegrated.DataType.ToString(),
                    Unit = null,
                    Value = new ProductAttributeValueDto()
                    {
                        BoolValue = false,
                    }
                },
                new()
                {
                    Key = DataForSeed.DomainModels.Attribute3_Socket.Key,
                    DataType = DataForSeed.DomainModels.Attribute3_Socket.DataType.ToString(),
                    Unit = null,
                    Value = new ProductAttributeValueDto()
                    {
                        StringValue = "LGA1702",
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
                        NumericValue = 132,
                    }
                },
            ],

            // expectedDomainItems.
            [
                new ProductAttributeValue()
                {
                    ProductId = DataForSeed.DomainModels.Product2_IntelCore_i5.Id,
                    CategoryAttributeId = DataForSeed.DomainModels.Attribute1_Frequency.Id,
                    Value = new AttributeValue()
                    {
                        NumericValue = 4.33M,
                    },
                },
                new ProductAttributeValue()
                {
                    ProductId = DataForSeed.DomainModels.Product2_IntelCore_i5.Id,
                    CategoryAttributeId = DataForSeed.DomainModels.Attribute2_GpuIntegrated.Id,
                    Value = new AttributeValue()
                    {
                        BoolValue = false,
                    },
                },
                new ProductAttributeValue()
                {
                    ProductId = DataForSeed.DomainModels.Product2_IntelCore_i5.Id,
                    CategoryAttributeId = DataForSeed.DomainModels.Attribute3_Socket.Id,
                    Value = new AttributeValue()
                    {
                        StringValue = "LGA1702",
                    },
                },
                new ProductAttributeValue()
                {
                    ProductId = DataForSeed.DomainModels.Product2_IntelCore_i5.Id,
                    CategoryAttributeId = DataForSeed.DomainModels.Attribute4_Tdp.Id,
                    Value = new AttributeValue()
                    {
                        NumericValue = 132,
                    },
                },
            ]
        );

        //********************************************************************

        yield return new TheoryDataRow<long, ProductAttributeValueListPutDto[], ProductAttributeListDto[], ProductAttributeValue[]>(

            // productId.
            DataForSeed.DomainModels.Product3_IntelCore_i7.Id,

            // requestValues.
            [
               new ProductAttributeValueListPutDto()
               {
                   AttributeKey = DataForSeed.DomainModels.Attribute3_Socket.Key,
                   StringValue = "LGA1703",
               },
            ],

            // expectedResponseItems.
            [
                new()
                {
                    Key = DataForSeed.DomainModels.Attribute1_Frequency.Key,
                    DataType = DataForSeed.DomainModels.Attribute1_Frequency.DataType.ToString(),
                    Unit = new UnitShortDto() { Name = DataForSeed.DomainModels.Unit3_Mhz.Name },
                    Value = new ProductAttributeValueDto() { }
                },
                new()
                {
                    Key = DataForSeed.DomainModels.Attribute2_GpuIntegrated.Key,
                    DataType = DataForSeed.DomainModels.Attribute2_GpuIntegrated.DataType.ToString(),
                    Unit = null,
                    Value = new ProductAttributeValueDto() { }
                },
                new()
                {
                    Key = DataForSeed.DomainModels.Attribute3_Socket.Key,
                    DataType = DataForSeed.DomainModels.Attribute3_Socket.DataType.ToString(),
                    Unit = null,
                    Value = new ProductAttributeValueDto() { StringValue = "LGA1703" },
                },
                new()
                {
                    Key = DataForSeed.DomainModels.Attribute4_Tdp.Key,
                    DataType = DataForSeed.DomainModels.Attribute4_Tdp.DataType.ToString(),
                    Unit = new UnitShortDto() { Name = DataForSeed.DomainModels.Unit4_W.Name, },
                    Value = new ProductAttributeValueDto() { }
                },
            ],

            // expectedDomainItems.
            [
                new ProductAttributeValue()
                {
                    ProductId = DataForSeed.DomainModels.Product3_IntelCore_i7.Id,
                    CategoryAttributeId = DataForSeed.DomainModels.Attribute3_Socket.Id,
                    Value = new AttributeValue() { StringValue = "LGA1703" },
                },
            ]
        );
    }

    static async Task SeedData(
        ApplicationDbContext dbContext, CancellationToken cancellationToken = default)
    {
        dbContext.Units.AddRange(DataForSeed.DomainModels.Units);
        dbContext.Categories.Add(DataForSeed.DomainModels.Category1_Empty);
        dbContext.Categories.Add(DataForSeed.DomainModels.Category2_CPUs);
        dbContext.Products.Add(DataForSeed.DomainModels.Product1_IntelCore_i3);
        dbContext.Products.Add(DataForSeed.DomainModels.Product2_IntelCore_i5);
        dbContext.Products.Add(DataForSeed.DomainModels.Product3_IntelCore_i7);
        dbContext.ProductAttributeValues.AddRange(DataForSeed.DomainModels.Product1_IntelCore_i3_AttributeValues);
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
