using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanSekvenci.Api.Authorization;
using PlanSekvenci.Api.Dtos;
using PlanSekvenci.Api.Services;

namespace PlanSekvenci.Api.Controllers;

[ApiController]
[Route("api/workplan")]
public class WorkplanController(WorkplanService workplanService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<WorkplanListResultDto>> Get(
        [FromQuery] string? plant,
        [FromQuery] string? dept,
        [FromQuery] string? teamLeader,
        [FromQuery] string? node,
        [FromQuery] string? rgid,
        [FromQuery] string? inf,
        [FromQuery] string? gunFamily,
        [FromQuery] int? inPlan,
        [FromQuery] bool? nextPresun,
        [FromQuery] bool? waitingToMove,
        [FromQuery] bool? razeno,
        [FromQuery] string? jobSuffix,
        [FromQuery] string? item,
        [FromQuery] string? sort,
        [FromQuery] string? sortDir,
        [FromQuery] int page,
        [FromQuery] int pageSize,
        CancellationToken ct)
    {
        var filter = new WorkplanFilter(plant, dept, teamLeader, node, rgid, inf, gunFamily,
            inPlan, nextPresun, waitingToMove, razeno, jobSuffix, item, sort, sortDir, page, pageSize);
        return Ok(await workplanService.GetAsync(filter, ct));
    }

    [HttpGet("rgids")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetRgids(
        [FromQuery] string? plant, [FromQuery] string? dept, [FromQuery] string? node, [FromQuery] string? teamLeader,
        CancellationToken ct) =>
        Ok(await workplanService.GetRgidsAsync(plant, dept, node, teamLeader, ct));

    [HttpGet("infs")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetInfs(CancellationToken ct) =>
        Ok(await workplanService.GetInfsAsync(ct));

    [HttpGet("gun-families")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetGunFamilies(CancellationToken ct) =>
        Ok(await workplanService.GetGunFamiliesAsync(ct));

    // Fáze 2 (PRD 5.5, 7) - podgalerie otevirane z gridu.
    [HttpGet("{idJobSuffix}/detail")]
    public async Task<ActionResult<SequenceDetailHeaderDto>> GetDetail(string idJobSuffix, CancellationToken ct) =>
        Ok(await workplanService.GetSequenceDetailAsync(idJobSuffix, ct));

    [HttpGet("{idJobSuffixOper}/serial-numbers")]
    public async Task<ActionResult<IReadOnlyList<SerialNumberDto>>> GetSerialNumbers(string idJobSuffixOper, CancellationToken ct) =>
        Ok(await workplanService.GetSerialNumbersAsync(idJobSuffixOper, ct));

    [HttpGet("{job}/{suffix:int}/divergence")]
    public async Task<ActionResult<IReadOnlyList<DivergenceDto>>> GetDivergence(string job, short suffix, CancellationToken ct) =>
        Ok(await workplanService.GetDivergenceAsync(job, suffix, ct));

    [HttpGet("{idJobSuffixOper}/x-suffix")]
    public async Task<ActionResult<IReadOnlyList<XSuffixDto>>> GetXSuffix(string idJobSuffixOper, CancellationToken ct) =>
        Ok(await workplanService.GetXSuffixAsync(idJobSuffixOper, ct));

    [HttpPatch("{idJobSuffixOper}/fixed")]
    [Authorize(Policy = ApproverRequirement.PolicyName)]
    public async Task<IActionResult> SetFixed(string idJobSuffixOper, [FromBody] SetFixedRequest request, CancellationToken ct)
    {
        var userName = User.Identity?.Name ?? "unknown";
        var found = await workplanService.SetFixedAsync(idJobSuffixOper, request.Fixed, userName, ct);
        return found ? NoContent() : NotFound();
    }

    [HttpPatch("{idJobSuffixOper}/selected")]
    public async Task<IActionResult> SetSelected(string idJobSuffixOper, [FromBody] SetSelectedRequest request, CancellationToken ct)
    {
        var userName = User.Identity?.Name ?? "unknown";
        var found = await workplanService.SetSelectedAsync(idJobSuffixOper, request.Selected, userName, ct);
        return found ? NoContent() : NotFound();
    }

    [HttpPost("{idJobSuffixOper}/reason")]
    [Authorize(Policy = ApproverRequirement.PolicyName)]
    public async Task<IActionResult> SubmitReason(string idJobSuffixOper, [FromQuery] string? node, [FromBody] SubmitReasonRequest request, CancellationToken ct)
    {
        var userName = User.Identity?.Name ?? "unknown";
        var result = await workplanService.SubmitReasonAsync(idJobSuffixOper, node, request.Reason, request.Note, userName, ct);
        return result switch
        {
            ReasonSubmitResult.Success => NoContent(),
            ReasonSubmitResult.NotFound => NotFound(),
            ReasonSubmitResult.InvalidReason => BadRequest(new ProblemDetails { Title = "Neplatný důvod - musí odpovídat číselníku." }),
            _ => throw new InvalidOperationException(),
        };
    }
}
