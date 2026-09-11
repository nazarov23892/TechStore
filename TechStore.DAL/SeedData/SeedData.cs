using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TechStore.BLL.Entities;
using TechStore.DAL.DbContexts;

namespace TechStore.DAL.SeedData;

public static class SeedData
{
    public static async Task RunSeed(
        ApplicationDbContext dbContext, int count, ILogger logger, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Products.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Seedeng books was skipped.");
            return;
        }

        var category1 = new Category() 
        { 
            Name = "category1" 
        };
        for (var i = 0; i < count; i++)
        {
            dbContext.Products.Add(
                new Product()
                {
                    Name = $"product-{1 + i}",
                    Description = $"description-{1 + i}",
                    Price = 1 + i,
                    Category = category1,
                });
        }
        var total = await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeding data done. Records: {Total}", total);
    }
}
