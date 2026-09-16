namespace TechStore.Contracts.DTOs;

/// <summary>
/// Модель запроса для обновления категории товара.
/// </summary>
public class CategoryPutDto
{
    /// <summary>
    /// Ключевое/техническое название (обозначение).
    /// </summary>
    public string? Key { get; set; }

    /// <summary>
    /// Отображаемое в интерфейсе название.
    /// </summary>
    public string? DisplayName { get; set; }
}
