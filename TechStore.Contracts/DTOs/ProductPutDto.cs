using System.ComponentModel.DataAnnotations;

namespace TechStore.Contracts.DTOs;

/// <summary>
/// Модель запроса для обновления товара.
/// </summary>
public class ProductPutDto
{
    public string? Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    [Range(0, 1_000_000)]
    public decimal? Price { get; set; }
}
