namespace PlanSekvenci.Api.Dtos;

// Radek gridu "Vyhodnoceni" (PRD 6.4).
public record ProducedItemDto(
    long Id,
    DateOnly? Date,
    string? IdJobSuffixOper,
    string? Item,
    double? QtyTodo,
    double? QtyDone,
    double? HodPlan,
    double? HodProduced,
    string? Node,
    int? Produced,
    DateTime? SequenceDateTime,
    int? Status,
    string? Reason,
    string? ReasonNote,
    DateTime? ReasonLastUpdateTime,
    string? ReasonLastUpdateBy,
    int? ReasonEnabled,
    string? Rgid,
    int? SelectedOper
);

// Panel "Vyhodnoceni nad plan nebo mimo plan" (druhy grid, Gallery3_2 v puvodni
// appce) - polozky vyrobene nad ramec nebo mimo dnesni plan (status 3/4).
public record OverPlanItemDto(
    long Id,
    string? IdJobSuffixOper,
    string? Item,
    int? Status,
    string? StatusDesc,
    DateTime? SequenceDateTime,
    double? HodProduced,
    double? QtyDone
);

// Panel "Vyhodnoceni planu:" + "Graf vyhodnoceni" - souhrn za vybrany den/uzel(/rgid).
public record ProducedSummaryDto(
    double? PlanHod,
    double? ProducedInPlanHod,
    double? ProducedOverPlanHod,
    double? ProducedOutOfPlanHod,
    double? CapacityHod,
    double? Sa,
    double? Performance
);
