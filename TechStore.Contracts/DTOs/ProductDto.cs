namespace TechStore.Contracts.DTOs;

/// <summary>
/// Полная модель товара.
/// </summary>
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    
    /// <summary>
    /// Категория.
    /// </summary>
    public CategoryShortDto? Category { get; set; }
}
