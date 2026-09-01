namespace PlanSekvenci.Api.Configuration;

// Slouzi jen k overeni clenstvi v Authorization:ApproverAdGroupId pres Microsoft Graph
// (viz Authorization/GraphGroupMembershipChecker.cs) - ApproverAdGroupId je Azure AD
// Object ID (puvodni appka ho pouzivala v Office365Groups.ListGroupMembers()), ne
// on-prem AD objectGUID, takze LDAP dotaz (System.DirectoryServices.AccountManagement)
// ho nikdy nenajde. TenantId/ClientId nejsou tajne, ale ClientSecret ano - patri jen
// do appsettings.Development.json (mimo git) nebo produkcniho secret store, nikdy do
// appsettings.json (stejne pravidlo jako u ConnectionStrings:BiApp, viz PRD 8.1).
//
// Vyzadovana Azure AD App Registration s Application permissions (admin-consented):
// User.Read.All (vyhledani uzivatele podle e-mailu) + GroupMember.Read.All
// (checkMemberGroups).
public class AzureAdOptions
{
    public const string SectionName = "AzureAd";

    public string TenantId { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;
}
