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
        var categoryId = 0L;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, 10);
            var category = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Key == "category-1");
            Assert.NotNull(category);
            categoryId = category.Id;
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
        Assert.Equal("attr1_numeric", response[0].Key);
        Assert.Equal("Numeric", response[0].DataType);

        Assert.Equal("attr2_string", response[1].Key);
        Assert.Equal("String", response[1].DataType);

        Assert.Equal("attr3_boolean", response[2].Key);
        Assert.Equal("Boolean", response[2].DataType);
    }

    [Fact(DisplayName = "Категории: Атрибуты: создание: успешно.")]
    public async Task Attributes_Create_Successfully()
    {
        var categoryId = 0L;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, 10);
            var category = await dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Key == "category-1");

            Assert.NotNull(category);
            categoryId = category.Id;
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
        Assert.NotEqual(0, response.Id);

        CategoryAttribute? domainModel = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var category = await dbContext.Categories
                .Include(c => c.Attributes)
                .FirstOrDefaultAsync(c => c.Id == categoryId);
            domainModel = category?.Attributes.FirstOrDefault(a => a.Id == categoryId);
        }
        Assert.NotNull(domainModel);
        Assert.Equal("attribute1", domainModel.Key);
        Assert.Equal(CategoryAttributeDataTypes.Numeric, domainModel.DataType);
        Assert.Equal(categoryId, domainModel.CategoryId);
    }

    [Fact(DisplayName = "Категории: Атрибуты: создание с единицей измерения: успешно.")]
    public async Task Attributes_Create_WithUnit_Successfully()
    {
        var categoryId = 0L;
        var unitId = 0L;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, 10);
            var category = await dbContext.Categories
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == DataForSeed.DomainModels.Category1_Empty.Id);

            Assert.NotNull(category);
            categoryId = category.Id;

            var unit = await dbContext.Units
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == DataForSeed.DomainModels.Unit1_Gb.Id);
            Assert.NotNull(unit);
            unitId = unit.Id;
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

    static async Task SeedData(ApplicationDbContext dbContext, int count)
    {
        dbContext.Units.AddRange(DataForSeed.DomainModels.Units);
        dbContext.Categories.Add(DataForSeed.DomainModels.Category1_Empty);

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
