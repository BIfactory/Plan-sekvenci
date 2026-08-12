using System.ComponentModel.DataAnnotations;

namespace PlanSekvenci.Api.Dtos;

// Radek gridu "Plan sekvenci" (PRD 5.4). Nazvy vlastnosti drzi puvodni nazvy sloupcu
// (camelCase), aby byla jasna navaznost na Power Fx/DB zdroj (viz CLAUDE.md - konvence).
public record WorkplanItemDto(
    int Id,
    string? Node,
    string? IdJobSuffix,
    string IdJobSuffixOper,
    string? Job,
    string? Suffix,
    int? OperNum,
    string? OperDesc,
    string? OperDescFull,
    string? Item,
    string? ItemDesc,
    string? ItemDescFull,
    string? GunFamily,
    int? Mprio,
    double? QtyReceived,
    double? QtyComplete,
    double? QtyTodo,
    DateTime? LastTran,
    DateTime? SequenceDateTime,
    double? Hod,
    string? Plant,
    string? Dept,
    string? Inf,
    string? Rgid,
    int? InPlan,
    DateTime? FirstTimeInPlan,
    bool Fixed,
    bool WaitingToMove,
    DateTime? MoveTime,
    bool Selected,
    int? Delay,
    int? Razeno,
    string? TeamLeader,
    int? ReasonFilled,
    string? ReasonText,
    string? ReasonNote,
    string? NextRadodOper,
    int? NextPresun,
    int? OperToDo,
    string? OperGroupColor,
    int? RankAll,
    // Fáze 2 - sloupec Status / tlacitko divergence (PRD 5.5).
    string? Status,
    string? WorkflowLink,
    int? DivStatus
);

public record WorkplanListResultDto(IReadOnlyList<WorkplanItemDto> Items, int TotalCount);

public record SetFixedRequest(bool Fixed);

public record SetSelectedRequest(bool Selected);

// PRD 4.5 - silnejsi validace: delky odpovidaji nejtesnejsimu ze sloupcu, do kterych se
// hodnota zapisuje (t_workplan_reasons.reason nvarchar(100), *.reason_note/note nvarchar(600)
// - viz schema.sql), aby zapis nikdy neselhal na oriznuti retezce v SQL Serveru. Obsah
// pole Reason navic musi odpovidat ciselniku dim_workplan_reasons (viz
// WorkplanService/ProducedService.SubmitReasonAsync - MaxLength sama o sobe neresi
// platnost hodnoty, jen jeji delku).
public record SubmitReasonRequest(
    [property: MaxLength(100)] string? Reason,
    [property: MaxLength(600)] string? Note);
