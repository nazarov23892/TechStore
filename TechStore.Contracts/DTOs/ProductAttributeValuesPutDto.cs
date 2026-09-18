namespace TechStore.Contracts.DTOs;

/// <summary>
/// Модель запроса на обновление значений атрибутов товаров.
/// </summary>
public class ProductAttributeValuesPutDto
{
    /// <summary>
    /// Список моделей атрибутов со значениями.
    /// </summary>
    public IEnumerable<ProductAttributeValueListPutDto> Values { get; set; } = [];
}
