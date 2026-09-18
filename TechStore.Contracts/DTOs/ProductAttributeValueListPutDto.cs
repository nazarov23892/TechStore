using System.ComponentModel.DataAnnotations;

namespace TechStore.Contracts.DTOs;

/// <summary>
/// Списочная модель значения атрибута товара для обновления.
/// </summary>
public class ProductAttributeValueListPutDto
{
    [Required]
    public string AttributeKey { get; set; } = string.Empty;
    public decimal? NumericValue { get; set; }
    public string? StringValue { get; set; }
    public bool? BooleanValue { get; set; }
}


