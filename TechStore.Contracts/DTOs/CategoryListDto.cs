namespace TechStore.Contracts.DTOs;

/// <summary>
/// Списочная модель категории товара.
/// </summary>
public class CategoryListDto
{
    /// <summary>
    /// Идентификатор.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Название.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
