using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlanSekvenci.Api.Authorization;
using PlanSekvenci.Api.Dtos;
using PlanSekvenci.Api.Services;

namespace PlanSekvenci.Api.Controllers;

// Fáze 3 (PRD 2) - audit log editaci, jen pro approvery (stejny okruh lidi, kteri smi
// editovat - viz PRD sekce 3).
[ApiController]
[Route("api/audit-log")]
[Authorize(Policy = ApproverRequirement.PolicyName)]
public class AuditLogController(AuditLogService auditLogService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AuditLogEntryDto>>> Get(
        [FromQuery] string? entityId,
        [FromQuery] string? entityType,
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        CancellationToken ct) =>
        Ok(await auditLogService.GetAsync(entityId, entityType, from, to, ct));
}
