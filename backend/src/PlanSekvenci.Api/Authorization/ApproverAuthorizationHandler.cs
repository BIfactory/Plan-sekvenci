using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace PlanSekvenci.Api.Authorization;

// PRD sekce 3 / 8.1: approver = clen AD skupiny ApproverAdGroupId NEBO e-mail v
// ApproverEmailWhitelist. Bezi jen na Windows/IIS (System.DirectoryServices), coz
// odpovida planovanemu nasazeni appky.
[SupportedOSPlatform("windows")]
public class ApproverAuthorizationHandler(IOptions<ApproverOptions> options)
    : AuthorizationHandler<ApproverRequirement>
{
    private readonly ApproverOptions _options = options.Value;

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ApproverRequirement requirement)
    {
        var identityName = context.User.Identity?.Name;
        if (string.IsNullOrEmpty(identityName))
        {
            return Task.CompletedTask;
        }

        using var domainContext = new PrincipalContext(ContextType.Domain);
        using var userPrincipal = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, identityName);
        if (userPrincipal is null)
        {
            return Task.CompletedTask;
        }

        var isWhitelisted = !string.IsNullOrEmpty(userPrincipal.EmailAddress)
            && _options.ApproverEmailWhitelist.Contains(userPrincipal.EmailAddress, StringComparer.OrdinalIgnoreCase);

        var isGroupMember = false;
        if (!string.IsNullOrEmpty(_options.ApproverAdGroupId))
        {
            using var group = GroupPrincipal.FindByIdentity(domainContext, IdentityType.Guid, _options.ApproverAdGroupId);
            isGroupMember = group is not null && userPrincipal.IsMemberOf(group);
        }

        if (isWhitelisted || isGroupMember)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
