using System.Security.Claims;

namespace PlanSekvenci.Api.Authorization;

public interface IApproverService
{
    bool IsApprover(ClaimsPrincipal user);
}
