namespace PlanSekvenci.Api.Services;

// Kombinovatelne filtry gridu "Plan sekvenci", AND logika (PRD 5.2).
public record WorkplanFilter(
    string? Plant,
    string? Dept,
    string? TeamLeader,
    string? Node,
    string? Rgid,
    string? Inf,
    string? GunFamily,
    int? InPlan,
    bool? NextPresun,
    bool? WaitingToMove,
    bool? Razeno,
    string? JobSuffix,
    string? Item,
    string? Sort,
    string? SortDir,
    int Page,
    int PageSize
)
{
    public bool HasAnyFilter =>
        !string.IsNullOrEmpty(Plant)
        || !string.IsNullOrEmpty(Dept)
        || !string.IsNullOrEmpty(TeamLeader)
        || !string.IsNullOrEmpty(Node)
        || !string.IsNullOrEmpty(Rgid)
        || !string.IsNullOrEmpty(Inf)
        || !string.IsNullOrEmpty(GunFamily)
        || InPlan.HasValue
        || NextPresun.HasValue
        || WaitingToMove.HasValue
        || Razeno.HasValue
        || !string.IsNullOrEmpty(JobSuffix)
        || !string.IsNullOrEmpty(Item);
}
