namespace TechStore.BLL.Entities;

/// <summary>
/// Категория товаров.
/// </summary>
public class Category
{
    public long Id { get; set; }
    public required string Name { get; set; }
}