namespace TechStore.Contracts.DTOs;

/// <summary>
/// Списочная Модель единицы измерения.
/// </summary>
public class UnitListDto
{
    public long Id { get; set; }

    /// <summary>
    /// Обозначение.
    /// </summary>
    public string Name { get; set; } = string.Empty;
}
