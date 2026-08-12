using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Fáze 3 (PRD 2) - audit log editaci (fixed/selected/reason), vylepseni oproti puvodni
// appce (ktera zadny audit trail nemela). Appka tuto tabulku vlastni (podobne jako
// t_log_powerapp) - appka ji ale nevytvari sama (bez migraci, viz PRD 4.1), musi ji
// napred vytvorit DBA skriptem t_workplan_audit_log.sql vedle tohoto souboru.
[Table("t_workplan_audit_log")]
public class WorkplanAuditLog
{
    [Column("id")]
    public int Id { get; set; }

    [Column("record_date")]
    public DateTime RecordDate { get; set; }

    [Column("user")]
    public string User { get; set; } = string.Empty;

    // "fixed" | "selected" | "reason"
    [Column("action")]
    public string Action { get; set; } = string.Empty;

    // "workplan" | "produced"
    [Column("entity_type")]
    public string EntityType { get; set; } = string.Empty;

    [Column("entity_id")]
    public string EntityId { get; set; } = string.Empty;

    [Column("node")]
    public string? Node { get; set; }

    [Column("old_value")]
    public string? OldValue { get; set; }

    [Column("new_value")]
    public string? NewValue { get; set; }
}
