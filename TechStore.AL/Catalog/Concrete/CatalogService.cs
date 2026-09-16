using Mapster;
using Microsoft.EntityFrameworkCore;
using TechStore.AL.Abstractions;
using TechStore.AL.Extensions;
using TechStore.BLL.Entities;
using TechStore.BLL.Exceptions;
using TechStore.Contracts.DTOs;

namespace TechStore.AL.Catalog.Concrete;

/// <summary>
/// Сервис для работы с функционалом каталога товаров.
/// </summary>
public class CatalogService : ICatalogService
{
    readonly IApplicationDbContext _context;

    public CatalogService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<ProductDto> CreateProductAsync(
        ProductPostDto request, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == request.CategoryId, cancellationToken)
            ?? throw new NotFoundException($"Category {request.CategoryId} not found.");

        var product = new Product()
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Category = category,
        };
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        var dto = product.Adapt<ProductDto>();
        return dto;
    }

    /// <inheritdoc/>
    public async Task<PagedListResponseDto<ProductListItemDto>> GetProductPagedListAsync(
        PagingRequestDto pagingRequest,
        string? category = null,
        CancellationToken cancellationToken = default)
    {
        var pagingStartsZero = pagingRequest.Page - 1;
        var query = _context.Products
            .AsNoTracking()
            .Where(p => !string.IsNullOrEmpty(category) && p.Category!.Key == category);
        var totalCount = await query
            .CountAsync(cancellationToken);
        var products = await query
            .AsNoTracking()
            .Include(p => p.Category)
            .WithPaging(pagingStartsZero, pagingRequest.PerPage)
            .ToListAsync(cancellationToken);

        var dtos = products.Adapt<List<ProductListItemDto>>();
        var result = new PagedListResponseDto<ProductListItemDto>()
        {
            Items = dtos,
            Page = pagingRequest.Page,
            PerPage = pagingRequest.PerPage,
            TotalCount = totalCount,
        };
        return result;
    }

    /// <inheritdoc/>
    public async Task DeleteProductAsync(
        long id, CancellationToken cancellationToken = default)
    {
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken: cancellationToken)
            ?? throw new NotFoundException($"The product with Id {id} not found.");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public async Task<PagedListResponseDto<CategoryListDto>> GetCategoriesAsync(
        PagingRequestDto pagingRequest, CancellationToken cancellationToken = default)
    {
        var pagingStartsZero = pagingRequest.Page - 1;
        var totalCount = await _context.Categories
            .CountAsync(cancellationToken);
        var models = await _context.Categories
            .AsNoTracking()
            .WithPaging(pagingStartsZero, pagingRequest.PerPage)
            .ToListAsync(cancellationToken);

        var dtos = models.Adapt<List<CategoryListDto>>();
        var result = new PagedListResponseDto<CategoryListDto>()
        {
            Items = dtos,
            Page = pagingRequest.Page,
            PerPage = pagingRequest.PerPage,
            TotalCount = totalCount,
        };
        return result;
    }

    /// <inheritdoc/>
    public async Task<CategoryDto> GetCategyByIdAsync(
        long categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken)
            ?? throw new NotFoundException($"Category {categoryId} not found.");

        var dto = category.Adapt<CategoryDto>();
        return dto;
    }

    /// <inheritdoc/>
    public async Task<CategoryDto> CreateCategoryAsync(
        CategoryPostDto request, CancellationToken cancellationToken = default)
    {
        if (await _context.Categories.AnyAsync(c => c.Key == request.Key, cancellationToken))
            throw new FailedPreconditionException($"Category {request.Key} already exists.");

        var category = new Category()
        {
            Key = request.Key,
            DisplayName = request.DisplayName,
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        var dto = category.Adapt<CategoryDto>();
        return dto;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CategoryAttributeListDto>> GetCategyAtrributesAsync(
        long categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Include(c => c.Attributes)
            .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken)
            ?? throw new NotFoundException($"Category {categoryId} not found.");

        var attributes = category.Attributes
            .Select(
                a => new CategoryAttributeListDto()
                {
                    Id = a.Id,
                    DataType = a.DataType.ToString(),
                    Key = a.Key,
                }).ToList();

        return attributes;
    }
}
