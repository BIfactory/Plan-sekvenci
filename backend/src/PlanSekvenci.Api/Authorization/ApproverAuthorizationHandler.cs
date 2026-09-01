using Microsoft.AspNetCore.Authorization;

namespace PlanSekvenci.Api.Authorization;

// PRD sekce 3 / 8.1: approver = clen AD skupiny ApproverAdGroupId NEBO e-mail v
// ApproverEmailWhitelist. Logika viz ApproverService (sdilena i s AuthController).
public class ApproverAuthorizationHandler(IApproverService approverService)
    : AuthorizationHandler<ApproverRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ApproverRequirement requirement)
    {
        if (await approverService.IsApproverAsync(context.User))
        {
            context.Succeed(requirement);
        }
    }
}
