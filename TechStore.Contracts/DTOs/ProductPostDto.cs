using System.ComponentModel.DataAnnotations;

namespace TechStore.Contracts.DTOs;

/// <summary>
/// Модель запроса для создания товара.
/// </summary>
public class ProductPostDto
{
    [Required]
    public required string Name { get; set; }
    public string? Description { get; set; }

    [Range(0, 1_000_000)]
    public decimal Price { get; set; }

    [Required]
    public required string Category { get; set; }
}
