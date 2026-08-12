namespace PlanSekvenci.Api.Dtos;

public record NodeDefinitionDto(string Node, string? Plant, string? Dept, string? TeamLeader);

public record LastSyncDto(DateTime? StartTime);

public record MeDto(string? UserName, bool IsApprover);

// Panel "Aktualni data" na obrazovce Plan sekvenci - agregace za vybrany uzel
// (v_workplan_node_data, v_produced_today) + vypocet kapacity.
public record NodeSummaryDto(
    double? HodPlanSkluz,
    double? HodPlan,
    double? HodAll,
    int? DelaySum,
    double? ProducedTotal,
    double? ProducedStatus1Or3,
    double? ProducedStatus1,
    bool? IsOnlinePlan,
    CapacityDto? Capacity
);

// Mode: "rgid" (RGID-uroven kapacity u CNC pracovist), "node" (uroven pracoviste
// se znamym limitem), "nocap" (bez stanoveneho limitu - Total je null).
public record CapacityDto(double? Used, double? Total, string Mode);

// Panel "Historie" (Plneni planu / Vykon) - t_pwrapp_workplan_sa_last_valid.
public record SaHistoryEntryDto(DateOnly? Date, string? DateDesc, double? Sa, double? Performance);
