namespace PlanSekvenci.Api.Authorization;

public class ApproverOptions
{
    public const string SectionName = "Authorization";

    public string ApproverAdGroupId { get; set; } = string.Empty;

    public string[] ApproverEmailWhitelist { get; set; } = [];

    // Jen pro lokalni vyvoj (viz DevApproverService) - AD/LDAP casto neni z vyvojoveho
    // stroje dosazitelne ani pres VPN. Vyzaduje soucasne IsDevelopment() prostredi,
    // takze v produkci nema efekt i kdyby se sem omylem dostalo true.
    public bool BypassAdInDevelopment { get; set; } = false;
}
