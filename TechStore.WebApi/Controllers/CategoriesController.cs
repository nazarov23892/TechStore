using Microsoft.AspNetCore.Mvc;
using TechStore.AL.Catalog;
using TechStore.Contracts.DTOs;

namespace TechStore.WebApi.Controllers;

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    readonly ICategoryService _categoryService;

    public CategoriesController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedListResponseDto<CategoryListDto>>> GetCategories(
        [FromQuery] PagingRequestDto request, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetCategoriesAsync(request, cancellationToken);
        return result;
    }

    [HttpGet("{id:long}")]
    public async Task<ActionResult<CategoryDto>> GetCategoryById(
        long id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetCategyByIdAsync(id, cancellationToken);
        return result;
    }

    [HttpPut("{id:long}")]
    public async Task<ActionResult<CategoryDto>> UpdateCategory(
        long id, 
        CategoryPutDto value,
        CancellationToken cancellationToken)
    {
        var result = await _categoryService.UpdateCategoryAsync(id, value, cancellationToken);
        return result;
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> CreateCategory(
        CategoryPostDto category, CancellationToken cancellationToken)
    {
        var result = await _categoryService.CreateCategoryAsync(category, cancellationToken);
        return result;
    }

    [HttpGet("{id:long}/attributes")]
    public async Task<ActionResult<IEnumerable<CategoryAttributeListDto>>> GetCategoryAttributes(
        long id, CancellationToken cancellationToken)
    {
        var result = await _categoryService.GetCategoryAttributesAsync(id, cancellationToken);
        return result.ToList();
    }

    [HttpPost("{id:long}/attributes")]
    public async Task<ActionResult<CategoryAttributetDto>> CreateCategoryAttribute(
        long id,
        CategoryAttributePostDto value, 
        CancellationToken cancellationToken = default)
    {
        var result = await _categoryService.CreateCategoryAtrributeAsync(
            id, value, cancellationToken);

        return result;
    }
}
