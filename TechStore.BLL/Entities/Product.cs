namespace TechStore.BLL.Entities;

/// <summary>
/// Товар.
/// </summary>
public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public required string Category { get; set; }
}