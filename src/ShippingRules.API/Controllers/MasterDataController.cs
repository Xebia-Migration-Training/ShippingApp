using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShippingRules.Application.Services;
using ShippingRules.Infrastructure.Data;

namespace ShippingRules.API.Controllers;

[ApiController]
[Route("api/master")]
public class MasterDataController : ControllerBase
{
    private readonly ShippingRulesDbContext _db;
    private readonly ExchangeRateService _fx;

    public MasterDataController(ShippingRulesDbContext db, ExchangeRateService fx)
    {
        _db = db;
        _fx = fx;
    }

    [HttpGet("countries")]
    public async Task<ActionResult<IEnumerable<LookupItem>>> GetCountries()
        => Ok(await _db.Countries.Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new LookupItem(x.Id, x.Name, x.Code)).ToListAsync());

    [HttpGet("ports")]
    public async Task<ActionResult<IEnumerable<LookupItem>>> GetPorts([FromQuery] Guid? countryId = null)
        => Ok(await _db.Ports.Where(x => x.IsActive)
            .Where(x => !countryId.HasValue || x.CountryId == countryId.Value)
            .OrderBy(x => x.Name)
            .Select(x => new LookupItem(x.Id, x.Name, x.Code)).ToListAsync());

    [HttpGet("principals")]
    public async Task<ActionResult<IEnumerable<LookupItem>>> GetPrincipals()
        => Ok(await _db.Principals.Where(x => x.IsActive)
            .OrderBy(x => x.Name)
            .Select(x => new LookupItem(x.Id, x.Name, x.Code)).ToListAsync());

    [HttpGet("vessels")]
    public async Task<ActionResult<IEnumerable<LookupItem>>> GetVessels([FromQuery] Guid? principalId = null)
        => Ok(await _db.Vessels.Where(x => x.IsActive)
            .Where(x => !principalId.HasValue || x.PrincipalId == principalId.Value)
            .OrderBy(x => x.Name)
            .Select(x => new LookupItem(x.Id, x.Name, x.IMONumber)).ToListAsync());

    [HttpGet("fx-rate")]
    public async Task<ActionResult<object>> GetFxRate(
        [FromQuery] string from,
        [FromQuery] string to,
        [FromQuery] DateTime? at,
        CancellationToken ct)
    {
        var date = at ?? DateTime.UtcNow;
        var rate = await _fx.TryGetRateAsync(from, to, date, ct);
        if (rate is null)
        {
            return NotFound(new { message = "FX rate not found" });
        }

        return Ok(new
        {
            from = from.Trim().ToUpperInvariant(),
            to = to.Trim().ToUpperInvariant(),
            at = date,
            rate
        });
    }
}

public record LookupItem(Guid Id, string Name, string Code);