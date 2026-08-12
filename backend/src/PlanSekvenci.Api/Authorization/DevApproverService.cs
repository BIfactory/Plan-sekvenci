using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace PlanSekvenci.Api.Authorization;

// Nahrada za ApproverService pro lokalni vyvoj, kdy AD/LDAP neni dosazitelny
// (System.DirectoryServices hazi LdapException: "The LDAP server is unavailable").
// Registruje se v Program.cs jen kdyz IsDevelopment() A Authorization:BypassAdInDevelopment
// = true - v produkci se nepouzije za zadnych okolnosti. Kazdy prihlaseny uzivatel
// se chova jako approver, aby slo lokalne otestovat editacni funkce (fixed, reason).
public class DevApproverService(ILogger<DevApproverService> logger) : IApproverService
{
    private bool _warned;

    public bool IsApprover(ClaimsPrincipal user)
    {
        if (!_warned)
        {
            logger.LogWarning(
                "DevApproverService je aktivni (Authorization:BypassAdInDevelopment=true) - " +
                "vsichni prihlaseni uzivatele se chovaji jako approver. Nikdy nenasazovat takto do produkce.");
            _warned = true;
        }

        return true;
    }
}
