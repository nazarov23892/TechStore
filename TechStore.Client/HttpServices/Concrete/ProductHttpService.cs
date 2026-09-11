using System.Net.Http.Json;
using TechStore.Contracts.Configuration;
using TechStore.Contracts.DTOs;

namespace TechStore.Client.HttpServices.Concrete;

/// <summary>
/// Http-сервис товаров.
/// </summary>
public class ProductHttpService : IProductHttpService
{
    readonly HttpClient _httpClient;
    const string BasePath = SharedConstants.BaseUri.Products;

    public ProductHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <inheritdoc/>
    public Task<PagedListResponseDto<ProductListItemDto>?> GetPagedListAsync(
        int page, int perPage, CancellationToken cancellationToken = default)
    {
        var uri = $"{BasePath}?page={page}&perPage={perPage}";
        return _httpClient.GetFromJsonAsync<PagedListResponseDto<ProductListItemDto>>(
            uri, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ProductDto?> CreateAsync(
        ProductPostDto createRequest, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(BasePath, createRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken);
        return result;
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"{BasePath}/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
