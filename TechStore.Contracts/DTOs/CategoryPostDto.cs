using System.ComponentModel.DataAnnotations;

namespace TechStore.Contracts.DTOs;

/// <summary>
/// Модель запроса для создания категории товара.
/// </summary>
public class CategoryPostDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
