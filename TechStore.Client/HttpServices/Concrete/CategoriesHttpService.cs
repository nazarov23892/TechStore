using System.Net.Http.Json;
using TechStore.Contracts.Configuration;
using TechStore.Contracts.DTOs;

namespace TechStore.Client.HttpServices.Concrete;

/// <summary>
/// Http-сервис категорий товаров.
/// </summary>
public class CategoriesHttpService : ICategoriesHttpService
{
    readonly HttpClient _httpClient;
    const string BasePath = SharedConstants.BaseUri.Categories;

    public CategoriesHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public Task<PagedListResponseDto<CategoryListDto>?> GetPagedListAsync(
        int page, int perPage, CancellationToken cancellationToken = default)
    {
        var uri = $"{BasePath}?page={page}&perPage={perPage}";
        return _httpClient.GetFromJsonAsync<PagedListResponseDto<CategoryListDto>>(
            uri, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<CategoryDto?> GetByIdAsync(
        long id, CancellationToken cancellationToken = default)
    {
        var uri = $"{BasePath}/{id}";
        return _httpClient.GetFromJsonAsync<CategoryDto>(uri, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IEnumerable<CategoryAttributeListDto>?> GetAttributeListAsync(
        long id, CancellationToken cancellationToken = default)
    {
        var uri = $"{BasePath}/{id}/attributes";
        return _httpClient.GetFromJsonAsync<IEnumerable<CategoryAttributeListDto>>(uri, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<CategoryDto?> Update(
        long id, CategoryPutDto value, CancellationToken cancellationToken = default)
    {
        var uri = $"{BasePath}/{id}";
        using var response = await _httpClient.PutAsJsonAsync(uri, value, cancellationToken);
        response.EnsureSuccessStatusCode();
        var dto = await response.Content.ReadFromJsonAsync<CategoryDto>(cancellationToken);
        return dto;
    }
}
