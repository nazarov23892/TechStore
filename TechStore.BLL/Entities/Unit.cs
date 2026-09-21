namespace TechStore.BLL.Entities;

/// <summary>
/// Единица измерения.
/// </summary>
/// <remarks>Примеры: сантиметр, грамм, гигабайт.</remarks>
public class Unit
{
    public long Id { get; set; }

    /// <summary>
    /// Обозначение.
    /// </summary>
    public required string Name { get; set; }
}
