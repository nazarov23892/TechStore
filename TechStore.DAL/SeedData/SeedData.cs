using Microsoft.EntityFrameworkCore;
using TechStore.BLL.Entities;
using TechStore.DAL.DbContexts;

namespace TechStore.DAL.SeedData;

public static class SeedData
{
    public static async Task RunSeed(
        ApplicationDbContext dbContext, int count, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Products.AnyAsync(cancellationToken))
            return;

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
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
