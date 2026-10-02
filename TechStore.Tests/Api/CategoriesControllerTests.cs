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

public class CategoriesControllerTests
{
    readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public CategoriesControllerTests()
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
            await SeedAdditionalCategories(dbContext, 10);
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
            var actionResult = await controller.GetCategories(
                paging, TestContext.Current.CancellationToken);
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

    [Theory(DisplayName = "Категории: получение по Id: успешно.")]
    [MemberData(nameof(GetInlineDataForGetById))]
    public async Task Categories_GetById_ReturnsOk(
        long categoryId,
        CategoryDto expectedResponse)
    {
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext);
        }

        CategoryDto? actualResponse = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var categoryService = new CategoryService(dbContext);
            var controller = new CategoriesController(categoryService);

            //Act.
            var actionResult = await controller.GetCategoryById(
                categoryId, TestContext.Current.CancellationToken);
            Assert.IsType<ActionResult<CategoryDto>>(actionResult);
            actualResponse = actionResult.Value;
        }
        Assert.NotNull(actualResponse);
        Assert.Equivalent(expectedResponse, actualResponse);
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
            var actionResult = await controller.CreateCategory(
                request, TestContext.Current.CancellationToken);
            Assert.IsType<ActionResult<CategoryDto>>(actionResult);
            response = actionResult.Value;
        }
        Assert.NotNull(response);
        Assert.Equal("cat1", response.Key);
        Assert.Equal("name1", response.DisplayName);

        Category? domainModel = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            domainModel = await dbContext.Categories.FirstOrDefaultAsync(
                c => c.Id == response.Id, TestContext.Current.CancellationToken);
        }
        Assert.NotNull(domainModel);
        Assert.Equal("cat1", domainModel.Key);
        Assert.Equal("name1", domainModel.DisplayName);
    }

    [Theory(DisplayName = "Категории: обновление: успешно.")]
    [MemberData(nameof(GetInlineDataForPut))]
    public async Task Categories_Update_Successfully(
        long categoryId,
        CategoryPutDto putRequest,
        CategoryDto expectedResponse,
        Category expectedDomainModel)
    {
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext);
        }

        CategoryDto? actualResponse = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var categoryService = new CategoryService(dbContext);
            var controller = new CategoriesController(categoryService);

            //Act.
            var actionResult = await controller.UpdateCategory(
                categoryId, putRequest, TestContext.Current.CancellationToken);
            Assert.IsType<ActionResult<CategoryDto>>(actionResult);
            actualResponse = actionResult.Value;
        }
        Assert.NotNull(actualResponse);
        Assert.Equivalent(expectedResponse, actualResponse);

        Category? actualDomainModel = null;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            actualDomainModel = await dbContext.Categories
                .FirstOrDefaultAsync(c => c.Id == actualResponse.Id, TestContext.Current.CancellationToken);
        }
        Assert.NotNull(actualDomainModel);
        Assert.Equivalent(expectedDomainModel, actualDomainModel);
    }

    public static IEnumerable<TheoryDataRow<long, CategoryPutDto, CategoryDto, Category>> GetInlineDataForPut()
    {
        yield return new TheoryDataRow<long, CategoryPutDto, CategoryDto, Category>(

            // categoryId.
            DataForSeed.DomainModels.Category1_Empty.Id,

            // putRequest.
            new CategoryPutDto()
            {
                Key = "new-key1",
                DisplayName = "new-display-name1",
            },

            // expectedResponse.
            new CategoryDto()
            {
                Id = DataForSeed.DomainModels.Category1_Empty.Id,
                Key = "new-key1",
                DisplayName = "new-display-name1",
            },

            // expectedDomainModel.
            new Category()
            {
                Id = DataForSeed.DomainModels.Category1_Empty.Id,
                Key = "new-key1",
                DisplayName = "new-display-name1",
            }
        );

        yield return new TheoryDataRow<long, CategoryPutDto, CategoryDto, Category>(

            // categoryId.
            DataForSeed.DomainModels.Category1_Empty.Id,

            // putRequest.
            new CategoryPutDto()
            {
                Key = null,
                DisplayName = "new-display-name1",
            },

            // expectedResponse.
            new CategoryDto()
            {
                Id = DataForSeed.DomainModels.Category1_Empty.Id,
                Key = DataForSeed.DomainModels.Category1_Empty.Key,
                DisplayName = "new-display-name1",
            },

            // expectedDomainModel.
            new Category()
            {
                Id = DataForSeed.DomainModels.Category1_Empty.Id,
                Key = DataForSeed.DomainModels.Category1_Empty.Key,
                DisplayName = "new-display-name1",
            }
        );

        yield return new TheoryDataRow<long, CategoryPutDto, CategoryDto, Category>(

            // categoryId.
            DataForSeed.DomainModels.Category1_Empty.Id,

            // putRequest.
            new CategoryPutDto()
            {
                Key = "new-key1",
                DisplayName = null,
            },

            // expectedResponse.
            new CategoryDto()
            {
                Id = DataForSeed.DomainModels.Category1_Empty.Id,
                Key = "new-key1",
                DisplayName = DataForSeed.DomainModels.Category1_Empty.DisplayName,
            },

            // expectedDomainModel.
            new Category()
            {
                Id = DataForSeed.DomainModels.Category1_Empty.Id,
                Key = "new-key1",
                DisplayName = DataForSeed.DomainModels.Category1_Empty.DisplayName,
            }
        );
    }

    public static IEnumerable<TheoryDataRow<long, CategoryDto>> GetInlineDataForGetById()
    {
        yield return new TheoryDataRow<long, CategoryDto>(

            // categoryId.
            DataForSeed.DomainModels.Category1_Empty.Id,

            // expectedResponse.
            new CategoryDto()
            {
                Id = DataForSeed.DomainModels.Category1_Empty.Id,
                Key = DataForSeed.DomainModels.Category1_Empty.Key,
                DisplayName = DataForSeed.DomainModels.Category1_Empty.DisplayName,
            }
        );

        yield return new TheoryDataRow<long, CategoryDto>(

            // categoryId.
            DataForSeed.DomainModels.Category2_CPUs.Id,

            // expectedResponse.
            new CategoryDto()
            {
                Id = DataForSeed.DomainModels.Category2_CPUs.Id,
                Key = DataForSeed.DomainModels.Category2_CPUs.Key,
                DisplayName = DataForSeed.DomainModels.Category2_CPUs.DisplayName,
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

    static async Task SeedAdditionalCategories(
        ApplicationDbContext dbContext, int count)
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
