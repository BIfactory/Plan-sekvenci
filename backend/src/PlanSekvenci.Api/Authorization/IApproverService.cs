using System.Security.Claims;

namespace PlanSekvenci.Api.Authorization;

public interface IApproverService
{
    Task<bool> IsApproverAsync(ClaimsPrincipal user);
}
