using System.Net.Http.Json;
using TechStore.Contracts.Configuration;
using TechStore.Contracts.DTOs;

namespace TechStore.Client.HttpServices.Concrete;

/// <summary>
/// Http-сервис товаров.
/// </summary>
public class ProductHttpService : IProductHttpService
{
    readonly HttpClient _http;
    const string BasePath = SharedConstants.BaseUri.Products;

    public ProductHttpService(HttpClient http)
    {
        _http = http;
    }

    /// <inheritdoc/>
    public Task<PagedListResponseDto<ProductListItemDto>?> GetPagedListAsync(
        int page, int perPage, CancellationToken cancellationToken = default)
    {
        var uri = $"{BasePath}?page={page}&perPage={perPage}";
        return _http.GetFromJsonAsync<PagedListResponseDto<ProductListItemDto>>(
            uri, cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<ProductDto?> CreateAsync(
        ProductPostDto createRequest, CancellationToken cancellationToken = default)
    {
        var response = await _http.PostAsJsonAsync(BasePath, createRequest, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken);
        return result;
    }
}
