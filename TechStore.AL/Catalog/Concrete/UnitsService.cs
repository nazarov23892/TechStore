using Mapster;
using Microsoft.EntityFrameworkCore;
using TechStore.AL.Abstractions;
using TechStore.Contracts.DTOs;

namespace TechStore.AL.Catalog.Concrete;

/// <summary>
/// Сервис для работы с функционалом единиц измерения.
/// </summary>
public class UnitsService : IUnitsService
{
    readonly IApplicationDbContext _context;

    public UnitsService(IApplicationDbContext context)
    {
        _context = context;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UnitListDto>> GetUnitsAsync(
        CancellationToken cancellationToken = default)
    {
        var units = await _context.Units
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        var dtos = units.Adapt<List<UnitListDto>>();
        return dtos;
    }
}
