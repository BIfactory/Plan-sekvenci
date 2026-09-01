using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Vyrobene polozky (PRD 4.2, 6) - zdroj gridu "Vyhodnoceni". Appka zapisuje jen reason_*
// pole, zbytek plni externi sync (PRD 4.1).
[Table("t_workplan_produced")]
public class WorkplanProduced
{
    [Column("id")]
    public long Id { get; set; }

    [Column("date")]
    public DateOnly? Date { get; set; }

    [Column("id_job_suffix_oper")]
    public string? IdJobSuffixOper { get; set; }

    [Column("item")]
    public string? Item { get; set; }

    [Column("qty_todo")]
    public double? QtyTodo { get; set; }

    [Column("qty_done")]
    public double? QtyDone { get; set; }

    [Column("hod_plan")]
    public double? HodPlan { get; set; }

    [Column("hod_produced")]
    public double? HodProduced { get; set; }

    [Column("node")]
    public string? Node { get; set; }

    [Column("produced")]
    public int? Produced { get; set; }

    [Column("sequence_date_time")]
    public DateTime? SequenceDateTime { get; set; }

    [Column("status")]
    public int? Status { get; set; }

    [Column("status_desc")]
    public string? StatusDesc { get; set; }

    [Column("reason")]
    public string? Reason { get; set; }

    [Column("reason_note")]
    public string? ReasonNote { get; set; }

    [Column("reason_last_update_time")]
    public DateTime? ReasonLastUpdateTime { get; set; }

    [Column("reason_last_update_by")]
    public string? ReasonLastUpdateBy { get; set; }

    [Column("reason_enabled")]
    public int? ReasonEnabled { get; set; }

    [Column("rgid")]
    public string? Rgid { get; set; }

    [Column("selected_oper")]
    public int? SelectedOper { get; set; }

    // "P" priznak ve sloupci Status (Vyhodnocení.pa.yaml: If(ThisItem.prio = 1, "P", "")).
    [Column("prio")]
    public int? Prio { get; set; }
}
