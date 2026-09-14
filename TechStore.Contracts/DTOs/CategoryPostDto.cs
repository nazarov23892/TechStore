using System.ComponentModel.DataAnnotations;

namespace TechStore.Contracts.DTOs;

/// <summary>
/// Модель запроса для создания категории товара.
/// </summary>
public class CategoryPostDto
{
    /// <summary>
    /// Ключевое/техническое название (обозначение).
    /// </summary>
    [Required]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Отображаемое в интерфейсе название.
    /// </summary>
    public string? DisplayName { get; set; }
}
