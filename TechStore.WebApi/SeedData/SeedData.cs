using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using TechStore.BLL.Entities;
using TechStore.DAL.DbContexts;

namespace TechStore.DAL.SeedData;

public static class SeedData
{
    const string CategoriesFileName = "categories.json";
    const string ProductsFileName = "products.json";

    static JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    public static async Task RunSeed(
        ApplicationDbContext dbContext, string filesDirPath, int count, ILogger logger, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Products.AnyAsync(cancellationToken))
        {
            logger.LogInformation("Seedeng was skipped.");
            return;
        }
        await SeedCategories(dbContext, filesDirPath, logger, cancellationToken);
        await SeedProducts(dbContext, filesDirPath, logger, cancellationToken);
    }

    static async Task SeedCategories(
        ApplicationDbContext dbContext, string filesDirPath, ILogger logger, CancellationToken cancellationToken)
    {
        var fileName = Path.Combine(filesDirPath, CategoriesFileName);
        if (!File.Exists(fileName))
        {
            logger.LogInformation("Seedeng failed. The file '{File}' not exists.", fileName);
            return;
        }
        ICollection<CategoryInfoJson>? items;
        try
        {
            var content = File.ReadAllText(fileName);
            items = JsonSerializer.Deserialize<ICollection<CategoryInfoJson>>(
                content, _jsonSerializerOptions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "error");
            return;
        }

        foreach (var item in items ?? Enumerable.Empty<CategoryInfoJson>())
        {
            dbContext.Categories.Add(
                new Category()
                {
                    Key = item.Key,
                    DisplayName = item.DisplayName,
                });
        }
        var total = await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeding categories done. Records: {Total}", total);
    }

    static async Task SeedProducts(
        ApplicationDbContext dbContext, string filesDirPath, ILogger logger, CancellationToken cancellationToken)
    {
        var fileName = Path.Combine(filesDirPath, ProductsFileName);
        if (!File.Exists(fileName))
        {
            logger.LogInformation("Seedeng failed. The file '{File}' not exists.", fileName);
            return;
        }
        ICollection<ProductInfoJson> items = [];
        try
        {
            var content = File.ReadAllText(fileName);
            items = JsonSerializer.Deserialize<ICollection<ProductInfoJson>>(
                content, _jsonSerializerOptions) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "error");
            return;
        }

        var categoryKeys = items.Select(p => p.CategoryKey).Distinct().ToList();
        var categoryMap = await dbContext.Categories
            .Where(c => categoryKeys.Contains(c.Key))
            .ToDictionaryAsync(
                c => c.Key,
                c => c,
                cancellationToken);

        foreach (var item in items)
        {
            if (!categoryMap.TryGetValue(item.CategoryKey, out var category)
                || category == null)
                continue;

            dbContext.Products.Add(
                new Product()
                {
                    Name = item.Name,
                    Description = item.Description,
                    Price = item.Price,
                    CategoryId = category.Id,
                });
        }
        var total = await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeding products done. Records: {Total}", total);
    }

    record CategoryInfoJson(string Key, string DisplayName);

    record ProductInfoJson(string Name, string Description, decimal Price, string CategoryKey);
}
