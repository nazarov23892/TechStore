namespace TechStore.Contracts.DTOs;

/// <summary>
/// Полная модель категории товара.
/// </summary>
public class CategoryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
