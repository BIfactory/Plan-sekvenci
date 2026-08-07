namespace PlanSekvenci.Api.Authorization;

public class ApproverOptions
{
    public const string SectionName = "Authorization";

    public string ApproverAdGroupId { get; set; } = string.Empty;

    public string[] ApproverEmailWhitelist { get; set; } = [];
}
