using Mapster;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechStore.AL.Catalog.Concrete;
using TechStore.BLL.Entities;
using TechStore.Contracts.DTOs;
using TechStore.DAL.DbContexts;
using TechStore.WebApi.Controllers;

namespace TechStore.Tests.Api;

public class CategoriesControllerTest
{
    readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public CategoriesControllerTest()
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

    [Fact(DisplayName = "Категории: получение списка: успешно.")]
    public async Task Categories_GetList_Paged_ReturnsOk()
    {
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, 10);
        }

        var paging = new PagingRequestDto()
        {
            Page = 1,
            PerPage = 3,
        };
        PagedListResponseDto<CategoryListDto>? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var categoryService = new CategoryService(dbContext);
            var controller = new CategoriesController(categoryService);

            //Act.
            var actionResult = await controller.GetCategories(paging, default);
            Assert.IsType<ActionResult<PagedListResponseDto<CategoryListDto>>>(actionResult);
            response = actionResult.Value;
        }
        Assert.NotNull(response);
        Assert.NotEmpty(response.Items);
        Assert.Equal(4, response.TotalPages);
        Assert.Equal(1, response.Page);
        Assert.Equal(3, response.PerPage);
        var categories = response.Items.ToList();
        Assert.Equal("category-1", categories[0].Key);
        Assert.Equal("category-2", categories[1].Key);
        Assert.Equal("category-3", categories[2].Key);
        Assert.Equal("display-name-1", categories[0].DisplayName);
        Assert.Equal("display-name-2", categories[1].DisplayName);
        Assert.Equal("display-name-3", categories[2].DisplayName);
    }

    [Fact(DisplayName = "Категории: создание: успешно.")]
    public async Task Categories_Create_Successfully()
    {
        var request = new CategoryPostDto()
        {
            Key = "cat1",
            DisplayName = "name1",
        };
        CategoryDto? response = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var categoryService = new CategoryService(dbContext);
            var controller = new CategoriesController(categoryService);

            //Act.
            var actionResult = await controller.CreateCategory(request, default);
            Assert.IsType<ActionResult<CategoryDto>>(actionResult);
            response = actionResult.Value;
        }
        Assert.NotNull(response);
        Assert.Equal("cat1", response.Key);
        Assert.Equal("name1", response.DisplayName);

        Category? domainModel = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            domainModel = await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == response.Id);
        }
        Assert.NotNull(domainModel);
        Assert.Equal("cat1", domainModel.Key);
        Assert.Equal("name1", domainModel.DisplayName);
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
            categoryId = category!.Id;
        }

        var request = new CategoryAttributePostDto()
        {
            Key = "attribute1",
            DataType = "Numeric",
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

    static async Task SeedData(ApplicationDbContext dbContext, int count)
    {
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
