using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace PlanSekvenci.Api.Authorization;

// PRD sekce 3 / 8.1: approver = clen AD skupiny ApproverAdGroupId NEBO e-mail v
// ApproverEmailWhitelist. Sdileno mezi ApproverAuthorizationHandler (policy na
// [Authorize]) a AuthController (/api/auth/me pro frontend).
//
// Identita (email) se resi pres on-prem AD/LDAP (System.DirectoryServices), protoze
// z toho appka uz potrebuje UserPrincipal kvuli whitelist kontrole a Windows identity
// (DOMAIN\login) sama o sobe email neobsahuje. Samotna kontrola AD skupiny ale jde
// pres Microsoft Graph (viz GraphGroupMembershipChecker) - ApproverAdGroupId je Azure
// AD Object ID (puvodni appka volala Office365Groups.ListGroupMembers()), na LDAP
// (on-prem objectGUID) se vubec nenajde.
[SupportedOSPlatform("windows")]
public class ApproverService(IOptions<ApproverOptions> options, IGraphGroupMembershipChecker groupChecker) : IApproverService
{
    private readonly ApproverOptions _options = options.Value;

    public async Task<bool> IsApproverAsync(ClaimsPrincipal user)
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

        if (isWhitelisted)
        {
            return true;
        }

        if (string.IsNullOrEmpty(_options.ApproverAdGroupId) || string.IsNullOrEmpty(userPrincipal.EmailAddress))
        {
            return false;
        }

        return await groupChecker.IsMemberOfGroupAsync(userPrincipal.EmailAddress, _options.ApproverAdGroupId);
    }
}
