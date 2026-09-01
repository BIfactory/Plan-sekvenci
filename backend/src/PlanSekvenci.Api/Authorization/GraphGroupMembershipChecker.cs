using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using PlanSekvenci.Api.Configuration;

namespace PlanSekvenci.Api.Authorization;

// PRD sekce 3 / 8.1: ApproverAdGroupId je Azure AD Object ID (puvodni appka volala
// Office365Groups.ListGroupMembers() - Power Apps konektor jde pres Microsoft Graph),
// ne on-prem AD objectGUID. Overeni clenstvi proto musi jit primo pres Graph API,
// LDAP (System.DirectoryServices.AccountManagement) tuhle skupinu nikdy nenajde.
//
// Dva kroky, protoze checkMemberGroups vyzaduje AAD object id uzivatele (ne kazdy
// email odpovida userPrincipalName, takze primo email do cesty /users/{id} nejde
// spolehat): 1) najit uzivatele podle e-mailu (User.Read.All), 2) checkMemberGroups
// pro jeho id (GroupMember.Read.All). Oba typy jsou Application permissions (admin
// consent), appka bezi bez prihlaseneho uzivatele (client credentials flow).
//
// Singleton - IConfidentialClientApplication drzi vlastni token cache, opakovane
// AcquireTokenForClient v ramci platnosti tokenu nevola znovu Azure AD.
public class GraphGroupMembershipChecker(
    IOptions<AzureAdOptions> options,
    IHttpClientFactory httpClientFactory,
    ILogger<GraphGroupMembershipChecker> logger) : IGraphGroupMembershipChecker
{
    private static readonly string[] GraphScopes = ["https://graph.microsoft.com/.default"];
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly AzureAdOptions _options = options.Value;
    private IConfidentialClientApplication? _app;

    public async Task<bool> IsMemberOfGroupAsync(string userEmail, string groupId, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(_options.TenantId) || string.IsNullOrEmpty(_options.ClientId) || string.IsNullOrEmpty(_options.ClientSecret))
        {
            logger.LogWarning("AzureAd konfigurace neni kompletni (TenantId/ClientId/ClientSecret) - kontrola AD skupiny se preskakuje.");
            return false;
        }

        try
        {
            var token = await GetApp().AcquireTokenForClient(GraphScopes).ExecuteAsync(ct);

            using var client = httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

            var userId = await FindUserIdByEmailAsync(client, userEmail, ct);
            if (userId is null)
            {
                logger.LogWarning("Graph nenasel uzivatele s e-mailem {Email} (User.Read.All permission?).", userEmail);
                return false;
            }

            return await CheckMemberGroupsAsync(client, userId, groupId, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Overeni clenstvi v AD skupine pres Graph API selhalo pro {Email}", userEmail);
            return false;
        }
    }

    private IConfidentialClientApplication GetApp() =>
        _app ??= ConfidentialClientApplicationBuilder
            .Create(_options.ClientId)
            .WithClientSecret(_options.ClientSecret)
            .WithAuthority(new Uri($"https://login.microsoftonline.com/{_options.TenantId}"))
            .Build();

    private static async Task<string?> FindUserIdByEmailAsync(HttpClient client, string email, CancellationToken ct)
    {
        var url = $"https://graph.microsoft.com/v1.0/users?$filter=mail eq '{Uri.EscapeDataString(email)}'&$select=id";
        var response = await client.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var payload = await response.Content.ReadFromJsonAsync<GraphUsersResponse>(JsonOptions, ct);
        return payload?.Value?.FirstOrDefault()?.Id;
    }

    private static async Task<bool> CheckMemberGroupsAsync(HttpClient client, string userId, string groupId, CancellationToken ct)
    {
        var url = $"https://graph.microsoft.com/v1.0/users/{Uri.EscapeDataString(userId)}/checkMemberGroups";
        var response = await client.PostAsJsonAsync(url, new CheckMemberGroupsRequest([groupId]), JsonOptions, ct);
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var payload = await response.Content.ReadFromJsonAsync<CheckMemberGroupsResponse>(JsonOptions, ct);
        return payload?.Value?.Contains(groupId, StringComparer.OrdinalIgnoreCase) ?? false;
    }

    private record GraphUsersResponse(List<GraphUser>? Value);

    private record GraphUser(string Id);

    private record CheckMemberGroupsRequest(string[] GroupIds);

    private record CheckMemberGroupsResponse(List<string>? Value);
}
