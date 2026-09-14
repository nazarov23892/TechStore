using TechStore.Contracts.DTOs;

namespace TechStore.Client.HttpServices;

/// <summary>
/// Http-сервис категорий товаров.
/// </summary>
public interface ICategoriesHttpService
{
    /// <summary>
    /// Выполняет запрос получения постраничного списка категорий.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<PagedListResponseDto<CategoryListDto>?> GetPagedListAsync(
        int page, int perPage, CancellationToken cancellationToken = default);
}
