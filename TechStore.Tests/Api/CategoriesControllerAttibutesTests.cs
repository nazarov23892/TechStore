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
            var actionResult = await controller.GetCategoryAttributes(categoryId, default);
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

    [Fact(DisplayName = "Категории: Атрибуты: создание: успешно.")]
    public async Task Attributes_Create_Successfully()
    {
        var categoryId = DataForSeed.DomainModels.Category1_Empty.Id;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext);
        }

        var request = new CategoryAttributePostDto()
        {
            Key = "attribute1",
            DataType = "Numeric",
            UnitId = null,
        };
        CategoryAttributetDto? response;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var categoryService = new CategoryService(dbContext);
            var controller = new CategoriesController(categoryService);

            //Act.
            var actionResult = await controller.CreateCategoryAttribute(categoryId, request, default);
            Assert.IsType<ActionResult<CategoryAttributetDto>>(actionResult);
            response = actionResult.Value;
        }
        Assert.NotNull(response);
        Assert.Equal("attribute1", response.Key);
        Assert.Equal("Numeric", response.DataType);
        Assert.Null(response.UnitId);
        Assert.NotEqual(0, response.Id);

        CategoryAttribute? domainModel = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var category = await dbContext.Categories
                .Include(c => c.Attributes)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
            domainModel = category?.Attributes.SingleOrDefault();
        }
        Assert.NotNull(domainModel);
        Assert.Equal("attribute1", domainModel.Key);
        Assert.Equal(CategoryAttributeDataTypes.Numeric, domainModel.DataType);
        Assert.Equal(categoryId, domainModel.CategoryId);
    }

    [Fact(DisplayName = "Категории: Атрибуты: создание с единицей измерения: успешно.")]
    public async Task Attributes_Create_WithUnit_Successfully()
    {
        var categoryId = DataForSeed.DomainModels.Category1_Empty.Id;
        var unitId = DataForSeed.DomainModels.Unit1_Gb.Id;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext);
        }

        var request = new CategoryAttributePostDto()
        {
            Key = "attribute1",
            DataType = "Numeric",
            UnitId = unitId,
        };
        CategoryAttributetDto? response;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var categoryService = new CategoryService(dbContext);
            var controller = new CategoriesController(categoryService);

            //Act.
            var actionResult = await controller.CreateCategoryAttribute(categoryId, request, default);
            Assert.IsType<ActionResult<CategoryAttributetDto>>(actionResult);
            response = actionResult.Value;
        }
        Assert.NotNull(response);
        Assert.Equal("attribute1", response.Key);
        Assert.Equal("Numeric", response.DataType);
        Assert.NotEqual(0, response.Id);
        Assert.NotNull(response.UnitId);
        Assert.Equal(unitId, response.UnitId);

        CategoryAttribute? domainModel = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var category = await dbContext.Categories
                .Include(c => c.Attributes)
                .SingleOrDefaultAsync(c => c.Id == categoryId);
            domainModel = category?.Attributes.FirstOrDefault();
        }
        Assert.NotNull(domainModel);
        Assert.Equal("attribute1", domainModel.Key);
        Assert.Equal(CategoryAttributeDataTypes.Numeric, domainModel.DataType);
        Assert.Equal(categoryId, domainModel.CategoryId);
        Assert.NotNull(domainModel.UnitId);
        Assert.Equal(DataForSeed.DomainModels.Unit1_Gb.Id, domainModel.UnitId);
    }

    static async Task SeedData(ApplicationDbContext dbContext)
    {
        dbContext.Units.AddRange(DataForSeed.DomainModels.Units);
        dbContext.Categories.Add(DataForSeed.DomainModels.Category1_Empty);
        dbContext.Categories.Add(DataForSeed.DomainModels.Category2_CPUs);
        await dbContext.SaveChangesAsync();
    }
}
