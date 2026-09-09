using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechStore.AL.Catalog.Concrete;
using TechStore.AL.DTOs;
using TechStore.BLL.Entities;
using TechStore.DAL.DbContexts;

namespace TechStore.Tests.Services;

public class CatalogServiceTests
{
    readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

    public CatalogServiceTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        builder.UseInMemoryDatabase(Guid.NewGuid().ToString())
            .UseInternalServiceProvider(serviceCollection.BuildServiceProvider());

        _dbContextOptions = builder.Options;
    }

    [Fact(DisplayName = "Формирование постраничного списка товаров успешно.")]
    public async Task Can_Form_PagedProductList()
    {
        // Arrange.
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            await SeedData(dbContext, 10);
        }
        var pagingDto = new PagingRequestDto()
        {
            Page = 1,
            PerPage = 4,
        };
        List<ProductCatalogListItemDto>? result;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var productService = new CatalogService(dbContext);

            // Act.
            result = (await productService.GetProductListAsync(pagingDto)).ToList();
        }

        // Assert.
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(4, result.Count);
    }

    static async Task SeedData(ApplicationDbContext dbContext, int count)
    {
        var category1 = "category1";
        for (var i = 0; i < count; i++)
        {
            dbContext.Products.Add(
            new Product()
            {
                Id = 1 + i,
                Name = $"product-{1 + i}",
                Description = $"description-{1 + i}",
                Price = 1 + i,
                Category = category1,
            });
        }
        await dbContext.SaveChangesAsync();
    }
}