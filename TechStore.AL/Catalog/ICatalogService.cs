using TechStore.AL.DTOs;

namespace TechStore.AL.Catalog;

/// <summary>
/// Сервис для работы с функционалом каталога товаров.
/// </summary>
public interface ICatalogService
{
    /// <summary>
    /// Возвращает модель для списка товаров каталога.
    /// </summary>
    /// <param name="pagingRequest">Модель постраничной навигации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param> 
    Task<PagedListResponseDto<ProductCatalogListItemDto>> GetProductPagedListAsync(
        PagingRequestDto pagingRequest, CancellationToken cancellationToken = default);
}
