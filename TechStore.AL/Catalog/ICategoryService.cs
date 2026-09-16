using TechStore.Contracts.DTOs;

namespace TechStore.AL.Catalog;

/// <summary>
/// Сервис для работы с функционалом категорий товаров.
/// </summary>
public interface ICategoryService
{
    /// <summary>
    /// Возвращает ответ со списком моделей категорий.
    /// </summary>
    /// <param name="pagingRequest">Модель постраничной навигации.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param> 
    Task<PagedListResponseDto<CategoryListDto>> GetCategoriesAsync(
        PagingRequestDto pagingRequest, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает категорию по Id.
    /// </summary>
    /// <param name="categoryId">Идентификатор категории.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<CategoryDto> GetCategyByIdAsync(
        long categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создает новую категорию товаров.
    /// </summary>
    /// <param name="request">Модель запроса на создание.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param> 
    Task<CategoryDto> CreateCategoryAsync(
        CategoryPostDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет категорию.
    /// </summary>
    /// <param name="categoryId">Идентификатор категории.</param>
    /// <param name="value">Модель запроса на обновление..</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<CategoryDto> UpdateCategoryAsync(
        long categoryId,
        CategoryPutDto value,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает список атрибутов категории товаров.
    /// </summary>
    /// <param name="categoryId">Идентификатор категории.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<IEnumerable<CategoryAttributeListDto>> GetCategoryAttributesAsync(
        long categoryId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создает новый атрибут в категории товаров.
    /// </summary>
    /// <param name="categoryId">Идентификатор категории.</param>
    /// <param name="value">Модель запроса на создание.</param>
    /// <param name="cancellationToken">Токен отмены.</param>
    Task<CategoryAttributetDto> CreateCategoryAtrributeAsync(
        long categoryId,
        CategoryAttributePostDto value,
        CancellationToken cancellationToken = default);
}
