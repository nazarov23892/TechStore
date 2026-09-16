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

    /// <summary>
    /// Выполняет запрос получения категории по Id.
    /// </summary>
    /// <param name="id">Id.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task<CategoryDto?> GetByIdAsync(long id, CancellationToken cancellationToken = default);
}
