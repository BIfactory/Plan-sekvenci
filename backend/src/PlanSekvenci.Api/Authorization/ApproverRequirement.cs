using Microsoft.AspNetCore.Authorization;

namespace PlanSekvenci.Api.Authorization;

public class ApproverRequirement : IAuthorizationRequirement
{
    public const string PolicyName = "Approver";
}
