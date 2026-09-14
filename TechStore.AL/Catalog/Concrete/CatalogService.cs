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
        var dto = new ProductDto()
        {
            Id = product.Id,
            Name = request.Name,
            Description = product.Description,
            Category = new CategoryShortDto()
            {
                Id = product.CategoryId,
                Name = product.Category?.Key ?? string.Empty,
            },
            Price = product.Price,
        };
        return dto;
    }

    /// <inheritdoc/>
    public async Task<PagedListResponseDto<ProductListItemDto>> GetProductPagedListAsync(
        PagingRequestDto pagingRequest,
        string? category = null,
        CancellationToken cancellationToken = default)
    {
        var pagingStartsZero = pagingRequest.Page - 1;
        var totalCount = await _context.Products
            .CountAsync(cancellationToken);
        var products = await _context.Products
            .AsNoTracking()
            .Include(p => p.Category)
            .Where(p => !string.IsNullOrEmpty(category) && p.Category!.Key == category)
            .WithPaging(pagingStartsZero, pagingRequest.PerPage)
            .ToListAsync(cancellationToken);

        var dtos = products.Select(
            p => new ProductListItemDto()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Category = p.Category?.Key ?? string.Empty,
            }).ToList();

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

        var dtos = models.Select(
            m => new CategoryListDto()
            {
                Id = m.Id,
                Name = m.Key,
            }).ToList();

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
    public async Task<CategoryDto> CreateCategoryAsync(
        CategoryPostDto request, CancellationToken cancellationToken = default)
    {
        if (await _context.Categories.AnyAsync(c => c.Key == request.Name, cancellationToken))
            throw new FailedPreconditionException($"Category {request.Name} already exists.");

        var category = new Category()
        {
            Key = request.Name,
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);
        var dto = new CategoryDto()
        {
            Id = category.Id,
            Name = request.Name,
        };
        return dto;
    }
}
