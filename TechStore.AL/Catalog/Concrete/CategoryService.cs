using Mapster;
using Microsoft.EntityFrameworkCore;
using TechStore.AL.Abstractions;
using TechStore.AL.Extensions;
using TechStore.BLL.Entities;
using TechStore.BLL.Exceptions;
using TechStore.Contracts.DTOs;

namespace TechStore.AL.Catalog.Concrete;

public class CategoryService : ICategoryService
{
    readonly IApplicationDbContext _context;

    public CategoryService(IApplicationDbContext context)
    {
        _context = context;
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
    public async Task<CategoryDto> UpdateCategoryAsync(
        long categoryId, CategoryPutDto value, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken)
            ?? throw new NotFoundException($"Category {categoryId} not found.");

        if (value.Key != null)
            category.Key = value.Key;
        if (value.DisplayName != null)
            category.DisplayName = value.DisplayName;

        await _context.SaveChangesAsync(cancellationToken);

        var dto = category.Adapt<CategoryDto>();
        return dto;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<CategoryAttributeListDto>> GetCategoryAttributesAsync(
        long categoryId, CancellationToken cancellationToken = default)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Include(c => c.Attributes.OrderBy(a => a.Key))
            .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken)
            ?? throw new NotFoundException($"Category {categoryId} not found.");

        var dtos = category.Attributes.Adapt<List<CategoryAttributeListDto>>();
        return dtos;
    }

    /// <inheritdoc/>
    public async Task<CategoryAttributetDto> CreateCategoryAtrributeAsync(
        long categoryId,
        CategoryAttributePostDto value,
        CancellationToken cancellationToken = default)
    {
        if (!Enum.TryParse<CategoryAttributeDataTypes>(value.DataType, out var dataType))
            throw new FailedPreconditionException(
                $"{nameof(value.DataType)} has invalid value: '{value.DataType}'");

        var category = await _context.Categories
            .Include(c => c.Attributes)
            .FirstOrDefaultAsync(c => c.Id == categoryId, cancellationToken)
            ?? throw new NotFoundException($"Category {categoryId} not found.");

        var attribute = new CategoryAttribute()
        {
            Key = value.Key,
            DataType = dataType,
        };
        category.Attributes.Add(attribute);
        await _context.SaveChangesAsync(cancellationToken);
        var dto = attribute.Adapt<CategoryAttributetDto>();
        return dto;
    }
}
