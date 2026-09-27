using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using System.Security.Claims;
using Microsoft.Extensions.Options;

namespace PlanSekvenci.Api.Authorization;

// PRD sekce 3 / 8.1: approver = clen AD skupiny ApproverAdGroupId NEBO e-mail v
// ApproverEmailWhitelist. Sdileno mezi ApproverAuthorizationHandler (policy na
// [Authorize]) a AuthController (/api/auth/me pro frontend).
//
// ApproverAdGroupId je objectGUID on-prem AD bezpecnostni skupiny "USERS_Plan_sekvenci"
// (potvrzeno se zakaznikem u dodavatele AD) - cela kontrola tak jde cistě pres LDAP,
// bez zavislosti na Microsoft Graph/Azure AD App Registration. Puvodni appka volala
// Office365Groups.ListGroupMembers() s Azure AD Object ID skupiny, ktery se na tento
// on-prem objectGUID vubec nemapuje - prvni pokus o migraci proto omylem hledal
// spatny identifikator (a pak docasne resil pres Graph API, nez se potvrdilo, ze
// skupina ma i on-prem ekvivalent).
[SupportedOSPlatform("windows")]
public class ApproverService(IOptions<ApproverOptions> options) : IApproverService
{
    private readonly ApproverOptions _options = options.Value;

    public Task<bool> IsApproverAsync(ClaimsPrincipal user)
    {
        var identityName = user.Identity?.Name;
        if (string.IsNullOrEmpty(identityName))
        {
            return Task.FromResult(false);
        }

        using var domainContext = new PrincipalContext(ContextType.Domain);
        using var userPrincipal = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, identityName);
        if (userPrincipal is null)
        {
            return Task.FromResult(false);
        }

        var isWhitelisted = !string.IsNullOrEmpty(userPrincipal.EmailAddress)
            && _options.ApproverEmailWhitelist.Contains(userPrincipal.EmailAddress, StringComparer.OrdinalIgnoreCase);

        if (isWhitelisted)
        {
            return Task.FromResult(true);
        }

        if (string.IsNullOrEmpty(_options.ApproverAdGroupId))
        {
            return Task.FromResult(false);
        }

        using var group = GroupPrincipal.FindByIdentity(domainContext, IdentityType.Guid, _options.ApproverAdGroupId);
        var isGroupMember = group is not null && userPrincipal.IsMemberOf(group);

        return Task.FromResult(isGroupMember);
    }
}
