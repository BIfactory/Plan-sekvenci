using Microsoft.AspNetCore.Authorization;

namespace PlanSekvenci.Api.Authorization;

// PRD sekce 3 / 8.1: approver = clen AD skupiny ApproverAdGroupId NEBO e-mail v
// ApproverEmailWhitelist. Logika viz ApproverService (sdilena i s AuthController).
public class ApproverAuthorizationHandler(IApproverService approverService)
    : AuthorizationHandler<ApproverRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ApproverRequirement requirement)
    {
        if (approverService.IsApprover(context.User))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
