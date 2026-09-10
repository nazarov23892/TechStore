using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TechStore.AL.Catalog.Concrete;
using TechStore.BLL.Entities;
using TechStore.Contracts.DTOs;
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
        PagedListResponseDto<ProductListItemDto>? result;
        using (var dbContext = new ApplicationDbContext(_dbContextOptions))
        {
            var productService = new CatalogService(dbContext);

            // Act.
            result = await productService.GetProductPagedListAsync(pagingDto);
        }

        // Assert.
        Assert.NotNull(result);
        var items = result.Items.ToList();
        Assert.NotEmpty(items);
        Assert.Equal(4, items.Count);
        Assert.Equal(1, items[0].Id);
        Assert.Equal(2, items[1].Id);
        Assert.Equal(3, items[2].Id);
        Assert.Equal(4, items[3].Id);
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(3, result.TotalPages);
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