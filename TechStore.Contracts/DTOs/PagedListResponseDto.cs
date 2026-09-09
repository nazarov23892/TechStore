namespace TechStore.Contracts.DTOs;

/// <summary>
/// Списочный ответ с данными постраничной навигации.
/// </summary>
/// <typeparam name="T">Тип элементов списка.</typeparam>
public class PagedListResponseDto<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PerPage { get; set; }
    public int TotalPages => (TotalCount / PerPage) + (TotalCount % PerPage > 0 ? 1 : 0);
}
