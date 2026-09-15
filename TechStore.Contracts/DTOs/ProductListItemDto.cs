namespace TechStore.Contracts.DTOs;

/// <summary>
/// Списочная модель товара.
/// </summary>
public class ProductListItemDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
}
