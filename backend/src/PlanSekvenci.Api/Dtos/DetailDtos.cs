namespace PlanSekvenci.Api.Dtos;

// Fáze 2 (PRD 5.5) - podgalerie otevirane z gridu "Plan sekvenci".

public record SequenceDetailDto(
    string? IdJobSuffixOper,
    int? OperNum,
    string? OperDesc,
    double? QtyReceived,
    double? QtyComplete,
    double? QtyScrapped,
    double? Hod,
    string? Node,
    int? Delay,
    DateTime? SequenceDateTime,
    int? Complete,
    int? Razeno
);

public record SequenceDetailHeaderDto(
    string? IdJobSuffix,
    DateTime? JobStartDate,
    DateTime? JobEndDate,
    IReadOnlyList<SequenceDetailDto> Steps
);

public record SerialNumberDto(string SerNum, string? Item);

public record DivergenceDto(string DivNum, string? WorkflowLink);

public record XSuffixDto(string? XJobSuffix, string? IdJobSuffixOper);
