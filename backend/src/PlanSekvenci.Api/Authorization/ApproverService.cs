using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace PlanSekvenci.Api.Authorization;

// PRD sekce 3 / 8.1: approver = clen AD skupiny ApproverAdGroupId NEBO e-mail v
// ApproverEmailWhitelist. Sdileno mezi ApproverAuthorizationHandler (policy na
// [Authorize]) a AuthController (/api/auth/me pro frontend).
[SupportedOSPlatform("windows")]
public class ApproverService(IOptions<ApproverOptions> options) : IApproverService
{
    private readonly ApproverOptions _options = options.Value;

    public bool IsApprover(ClaimsPrincipal user)
    {
        var identityName = user.Identity?.Name;
        if (string.IsNullOrEmpty(identityName))
        {
            return false;
        }

        using var domainContext = new PrincipalContext(ContextType.Domain);
        using var userPrincipal = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, identityName);
        if (userPrincipal is null)
        {
            return false;
        }

        var isWhitelisted = !string.IsNullOrEmpty(userPrincipal.EmailAddress)
            && _options.ApproverEmailWhitelist.Contains(userPrincipal.EmailAddress, StringComparer.OrdinalIgnoreCase);

        var isGroupMember = false;
        if (!string.IsNullOrEmpty(_options.ApproverAdGroupId))
        {
            using var group = GroupPrincipal.FindByIdentity(domainContext, IdentityType.Guid, _options.ApproverAdGroupId);
            isGroupMember = group is not null && userPrincipal.IsMemberOf(group);
        }

        return isWhitelisted || isGroupMember;
    }
}
