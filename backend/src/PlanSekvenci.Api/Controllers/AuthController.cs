using Microsoft.AspNetCore.Mvc;
using PlanSekvenci.Api.Authorization;
using PlanSekvenci.Api.Dtos;

namespace PlanSekvenci.Api.Controllers;

// Frontend potrebuje vedet, jestli ma zobrazit editacni ovladaci prvky (PRD 3) -
// server je autoritativni zdroj (skutecna autorizace je vynucena na jednotlivych
// PATCH/POST endpointech), tohle je jen pro UI.
[ApiController]
[Route("api/auth")]
public class AuthController(IApproverService approverService) : ControllerBase
{
    [HttpGet("me")]
    public ActionResult<MeDto> Me()
    {
        var isApprover = approverService.IsApprover(User);
        return Ok(new MeDto(User.Identity?.Name, isApprover));
    }
}
