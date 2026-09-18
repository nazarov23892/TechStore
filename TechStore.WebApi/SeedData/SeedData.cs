using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using TechStore.BLL.Entities;
using TechStore.DAL.DbContexts;

namespace TechStore.WebApi.SeedData;

public static class SeedData
{
    const string CategoriesFileName = "categories.json";
    const string ProductsFileName = "products.json";

    static readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = { new JsonStringEnumConverter() },
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
        ICollection<CategoryInfo>? items;
        try
        {
            var content = File.ReadAllText(fileName);
            items = JsonSerializer.Deserialize<ICollection<CategoryInfo>>(
                content, _jsonSerializerOptions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "error");
            return;
        }

        foreach (var item in items ?? Enumerable.Empty<CategoryInfo>())
        {
            dbContext.Categories.Add(
                new Category()
                {
                    Key = item.Key,
                    DisplayName = item.DisplayName,
                    Attributes = item.Attributes?.Select(
                        a => new CategoryAttribute()
                        {
                            Key = a.Key,
                            DataType = a.DataType,
                        }).ToList() ?? []
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
        ICollection<ProductInfo> items = [];
        try
        {
            var content = File.ReadAllText(fileName);
            items = JsonSerializer.Deserialize<ICollection<ProductInfo>>(
                content, _jsonSerializerOptions) ?? [];
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "error");
            return;
        }

        var categoryKeys = items.Select(p => p.CategoryKey).Distinct().ToList();
        var categoryMap = await dbContext.Categories
            .Include(c => c.Attributes)
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

            var categoryAttributeMap = categoryMap.Values
                .SelectMany(c => c.Attributes)
                .ToDictionary(c => c.Key);

            var product = new Product()
            {
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                CategoryId = category.Id,
            };
            dbContext.Products.Add(product);
            await dbContext.SaveChangesAsync(cancellationToken);

            var attributes = new List<ProductAttributeValue>();
            foreach (var attr in item.Attributes ?? [])
            {
                if (!categoryAttributeMap.TryGetValue(attr.Key, out var categoryAttr)
                    || categoryAttr == null)
                    continue;

                var attributeValue = new ProductAttributeValue()
                {
                    ProductId = product.Id,
                    Value = new AttributeValue()
                    {
                        BoolValue = attr.Value.BooleanValue,
                        NumericValue = attr.Value.NumericValue,
                        StringValue = attr.Value.Stringvalue,
                    },
                };
                categoryAttr.ProductValues.Add(attributeValue);
            }
        }
        var total = await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Seeding products done. Records: {Total}", total);
    }

    record CategoryInfo(string Key, string DisplayName, CategoryAttributeInfo[] Attributes);
    record CategoryAttributeInfo(string Key, CategoryAttributeDataTypes DataType);

    record ProductInfo(string Name, string Description, decimal Price, string CategoryKey, ProductAttributeInfo[] Attributes);
    record ProductAttributeInfo(string Key, ProductAttributeValueInfo Value);
    record ProductAttributeValueInfo(decimal? NumericValue, bool? BooleanValue, string? Stringvalue);
}
