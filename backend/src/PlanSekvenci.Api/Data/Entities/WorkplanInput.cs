using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Zapisovatelna cast t_workplan_input (PRD 4.2) - mapuje jen sloupce, do kterych appka
// skutecne zapisuje. PK odpovida realnemu DB constraintu (PK_workplan_input).
[Table("t_workplan_input")]
public class WorkplanInput
{
    [Column("id_job_suffix_oper")]
    public string IdJobSuffixOper { get; set; } = string.Empty;

    [Column("id")]
    public int Id { get; set; }

    [Column("node")]
    public string? Node { get; set; }

    [Column("fixed")]
    public int? Fixed { get; set; }

    [Column("fixed_RecordDate")]
    public DateTime? FixedRecordDate { get; set; }

    [Column("selected")]
    public int? Selected { get; set; }

    [Column("selected_RecordDate")]
    public DateTime? SelectedRecordDate { get; set; }

    [Column("reason_text")]
    public string? ReasonText { get; set; }

    [Column("reason_note")]
    public string? ReasonNote { get; set; }

    [Column("reason_filled")]
    public int? ReasonFilled { get; set; }
}
