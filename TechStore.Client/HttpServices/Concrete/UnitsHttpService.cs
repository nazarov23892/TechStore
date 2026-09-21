using System.Net.Http.Json;
using TechStore.Contracts.Configuration;
using TechStore.Contracts.DTOs;

namespace TechStore.Client.HttpServices.Concrete;

/// <summary>
/// Http-сервис единиц измерения.
/// </summary>
public class UnitsHttpService : IUnitsHttpService
{
    readonly HttpClient _httpClient;
    const string BaseUri = SharedConstants.BaseUri.Units;

    public UnitsHttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<IEnumerable<UnitListDto>?> GetListAsync(CancellationToken cancellationToken = default)
        => _httpClient.GetFromJsonAsync<IEnumerable<UnitListDto>>(BaseUri, cancellationToken);
}
