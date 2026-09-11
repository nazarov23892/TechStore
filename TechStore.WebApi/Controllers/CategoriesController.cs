using Microsoft.AspNetCore.Mvc;
using TechStore.AL.Catalog;
using TechStore.Contracts.DTOs;

namespace TechStore.WebApi.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController: ControllerBase
{
    readonly ICatalogService _catalogService;

    public CategoriesController(ICatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedListResponseDto<CategoryListDto>>> GetCategories(
    [FromQuery] PagingRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _catalogService.GetCategoriesAsync(request, cancellationToken);
        return result;
    }
}
