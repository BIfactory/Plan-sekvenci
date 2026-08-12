namespace PlanSekvenci.Api.Dtos;

// Fáze 3 (PRD 2) - audit log editaci, ctecí DTO pro GET /api/audit-log.
public record AuditLogEntryDto(
    int Id,
    DateTime RecordDate,
    string User,
    string Action,
    string EntityType,
    string EntityId,
    string? Node,
    string? OldValue,
    string? NewValue
);
