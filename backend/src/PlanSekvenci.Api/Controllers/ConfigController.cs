using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PlanSekvenci.Api.Configuration;
using PlanSekvenci.Api.Dtos;

namespace PlanSekvenci.Api.Controllers;

// Faze 3 - appsettings.json (sekce ClientSettings) je jediny zdroj pravdy pro chovani
// frontendu, ktere ma jit menit bez rebuildu SPA (auto-refresh interval, platnost
// cookie s filtry) - staticky build appsettings.json primo necte, proto ho frontend
// nacita pres tento endpoint pri startu (viz frontend hooks/useClientSettings.ts).
[ApiController]
[Route("api/config")]
public class ConfigController(IOptions<ClientSettingsOptions> options) : ControllerBase
{
    [HttpGet]
    public ActionResult<ClientSettingsDto> Get()
    {
        var settings = options.Value;
        return Ok(new ClientSettingsDto(settings.AutoRefreshIntervalSeconds, settings.FilterCookieExpiryDays));
    }
}
