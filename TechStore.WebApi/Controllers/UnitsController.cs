using Microsoft.AspNetCore.Mvc;
using TechStore.AL.Catalog;
using TechStore.Contracts.DTOs;

namespace TechStore.WebApi.Controllers;

[ApiController]
[Route("api/units")]
public class UnitsController : ControllerBase
{
    readonly IUnitsService _unitsService;

    public UnitsController(
        IUnitsService unitsService)
    {
        _unitsService = unitsService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UnitListDto>>> GetUnitsAsync(
        CancellationToken cancellationToken)
    {
        var dtos = await _unitsService.GetUnitsAsync(cancellationToken);
        return dtos.ToList();
    }
}
