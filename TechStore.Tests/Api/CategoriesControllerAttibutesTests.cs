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

public class CategoriesControllerAttributesTests
{
    readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public CategoriesControllerAttributesTests()
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


    [Fact(DisplayName = "Категории: Атрибуты: получение списка: успешно.")]
    public async Task Attributes_GetList_Successfully()
    {
        var categoryId = DataForSeed.DomainModels.Category2_CPUs.Id;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext);
        }

        List<CategoryAttributeListDto>? response;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var categoryService = new CategoryService(dbContext);
            var controller = new CategoriesController(categoryService);

            //Act.
            var actionResult = await controller.GetCategoryAttributes(
                categoryId, TestContext.Current.CancellationToken);
            Assert.IsType<ActionResult<IEnumerable<CategoryAttributeListDto>>>(actionResult);
            response = actionResult.Value?.ToList();
        }
        Assert.NotNull(response);
        Assert.NotEmpty(response);
        Assert.Equal(4, response.Count);

        Assert.Equal(DataForSeed.DomainModels.Attribute1_Frequency.Key, response[0].Key);
        Assert.Equal(DataForSeed.DomainModels.Attribute1_Frequency.DataType.ToString(), response[0].DataType);

        Assert.Equal(DataForSeed.DomainModels.Attribute2_GpuIntegrated.Key, response[1].Key);
        Assert.Equal(DataForSeed.DomainModels.Attribute2_GpuIntegrated.DataType.ToString(), response[1].DataType);

        Assert.Equal(DataForSeed.DomainModels.Attribute3_Socket.Key, response[2].Key);
        Assert.Equal(DataForSeed.DomainModels.Attribute3_Socket.DataType.ToString(), response[2].DataType);

        Assert.Equal(DataForSeed.DomainModels.Attribute4_Tdp.Key, response[3].Key);
        Assert.Equal(DataForSeed.DomainModels.Attribute4_Tdp.DataType.ToString(), response[3].DataType);

    }

    [Theory(DisplayName = "Категории: Атрибуты: создание: успешно.")]
    [MemberData(nameof(GetInlineDataForCreate))]
    public async Task Attributes_Create_Successfully(
        long categoryId,
        CategoryAttributePostDto request,
        CategoryAttributetDto expectedResponse,
        CategoryAttribute expectedDomainModel)
    {
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext);
        }

        CategoryAttributetDto? actualResponse;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var categoryService = new CategoryService(dbContext);
            var controller = new CategoriesController(categoryService);

            //Act.
            var actionResult = await controller.CreateCategoryAttribute(
                categoryId, request, TestContext.Current.CancellationToken);
            Assert.IsType<ActionResult<CategoryAttributetDto>>(actionResult);
            actualResponse = actionResult.Value;
        }
        Assert.NotNull(actualResponse);
        Assert.NotEqual(0, actualResponse.Id);
        Assert.EquivalentWithExclusions(
            expectedResponse,
            actualResponse,
            exclusionExpressions: r => r.Id);

        CategoryAttribute? actualDomainModel = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var category = await dbContext.Categories
                .Include(c => c.Attributes)
                .FirstOrDefaultAsync(c => c.Id == categoryId, TestContext.Current.CancellationToken);
            actualDomainModel = category?.Attributes.SingleOrDefault();
        }
        Assert.NotNull(actualDomainModel);
        Assert.EquivalentWithExclusions(
            expectedDomainModel,
            actualDomainModel,
             r => r.Id,
             r => r.ProductValues,
             r => r.Unit);
    }

    public static IEnumerable<TheoryDataRow<long, CategoryAttributePostDto, CategoryAttributetDto, CategoryAttribute>> GetInlineDataForCreate()
    {
        yield return new TheoryDataRow<long, CategoryAttributePostDto, CategoryAttributetDto, CategoryAttribute>(

            // categoryId.
            DataForSeed.DomainModels.Category1_Empty.Id,

            // request.
            new CategoryAttributePostDto()
            {
                Key = "attribute1",
                DataType = "Numeric",
                UnitId = null,
            },

            // expectedResponse.
            new CategoryAttributetDto()
            {
                Key = "attribute1",
                DataType = "Numeric",
                UnitId = null,
            },

            // expectedDomainModel.
            new CategoryAttribute()
            {
                Key = "attribute1",
                DataType = CategoryAttributeDataTypes.Numeric,
                UnitId = null,
                CategoryId = DataForSeed.DomainModels.Category1_Empty.Id,
            }
        );

        yield return new TheoryDataRow<long, CategoryAttributePostDto, CategoryAttributetDto, CategoryAttribute>(

            // categoryId.
            DataForSeed.DomainModels.Category1_Empty.Id,

            // request.
            new CategoryAttributePostDto()
            {
                Key = "attribute1",
                DataType = "Numeric",
                UnitId = DataForSeed.DomainModels.Unit1_Gb.Id,
            },

            // expectedResponse.
            new CategoryAttributetDto()
            {
                Key = "attribute1",
                DataType = "Numeric",
                UnitId = DataForSeed.DomainModels.Unit1_Gb.Id,
            },

            // expectedDomainModel.
            new CategoryAttribute()
            {
                Key = "attribute1",
                DataType = CategoryAttributeDataTypes.Numeric,
                CategoryId = DataForSeed.DomainModels.Category1_Empty.Id,
                UnitId = DataForSeed.DomainModels.Unit1_Gb.Id,
            }
        );
    }

    static async Task SeedData(ApplicationDbContext dbContext)
    {
        dbContext.Units.AddRange(DataForSeed.DomainModels.Units);
        dbContext.Categories.Add(DataForSeed.DomainModels.Category1_Empty);
        dbContext.Categories.Add(DataForSeed.DomainModels.Category2_CPUs);
        await dbContext.SaveChangesAsync();
    }
}
