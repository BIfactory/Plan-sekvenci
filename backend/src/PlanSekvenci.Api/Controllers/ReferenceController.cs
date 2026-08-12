using Microsoft.AspNetCore.Mvc;
using PlanSekvenci.Api.Dtos;
using PlanSekvenci.Api.Services;

namespace PlanSekvenci.Api.Controllers;

[ApiController]
[Route("api/reference")]
public class ReferenceController(ReferenceService referenceService) : ControllerBase
{
    [HttpGet("nodes")]
    public async Task<ActionResult<IReadOnlyList<NodeDefinitionDto>>> GetNodes(CancellationToken ct) =>
        Ok(await referenceService.GetNodesAsync(ct));

    [HttpGet("reasons")]
    public async Task<ActionResult<IReadOnlyList<string>>> GetReasons(CancellationToken ct) =>
        Ok(await referenceService.GetReasonsAsync(ct));

    [HttpGet("last-sync")]
    public async Task<ActionResult<LastSyncDto>> GetLastSync(CancellationToken ct) =>
        Ok(await referenceService.GetLastSyncAsync(ct));

    [HttpGet("node-summary")]
    public async Task<ActionResult<NodeSummaryDto>> GetNodeSummary([FromQuery] string? node, [FromQuery] string? rgid, CancellationToken ct) =>
        Ok(await referenceService.GetNodeSummaryAsync(node, rgid, ct));

    [HttpGet("sa-history")]
    public async Task<ActionResult<IReadOnlyList<SaHistoryEntryDto>>> GetSaHistory([FromQuery] string? node, CancellationToken ct) =>
        Ok(await referenceService.GetSaHistoryAsync(node, ct));
}
