using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Radkovy zdroj gridu "Plan sekvenci" (PRD 5) - jen cteni, appka do view nezapisuje
// (zapis jde primo do t_workplan_input, viz WorkplanInput.cs).
[Table("v_workplan_input")]
public class WorkplanInputView
{
    [Column("id")]
    public int Id { get; set; }

    [Column("node")]
    public string? Node { get; set; }

    [Column("id_job_suffix")]
    public string? IdJobSuffix { get; set; }

    [Column("id_job_suffix_oper")]
    public string IdJobSuffixOper { get; set; } = string.Empty;

    [Column("job")]
    public string? Job { get; set; }

    [Column("suffix")]
    public string? Suffix { get; set; }

    [Column("oper_num")]
    public int? OperNum { get; set; }

    [Column("oper_desc")]
    public string? OperDesc { get; set; }

    [Column("oper_desc_full")]
    public string? OperDescFull { get; set; }

    [Column("item")]
    public string? Item { get; set; }

    [Column("item_desc")]
    public string? ItemDesc { get; set; }

    [Column("item_desc_full")]
    public string? ItemDescFull { get; set; }

    [Column("gun_family")]
    public string? GunFamily { get; set; }

    [Column("mprio")]
    public int? Mprio { get; set; }

    [Column("qty_received")]
    public double? QtyReceived { get; set; }

    [Column("qty_complete")]
    public double? QtyComplete { get; set; }

    [Column("qty_todo")]
    public double? QtyTodo { get; set; }

    [Column("last_tran")]
    public DateTime? LastTran { get; set; }

    [Column("sequence_date_time")]
    public DateTime? SequenceDateTime { get; set; }

    [Column("hod")]
    public double? Hod { get; set; }

    [Column("rank_total")]
    public int? RankTotal { get; set; }

    [Column("plant")]
    public string? Plant { get; set; }

    [Column("dept")]
    public string? Dept { get; set; }

    [Column("inf")]
    public string? Inf { get; set; }

    [Column("rgid")]
    public string? Rgid { get; set; }

    [Column("in_plan")]
    public int? InPlan { get; set; }

    [Column("first_time_in_plan")]
    public DateTime? FirstTimeInPlan { get; set; }

    [Column("fixed")]
    public int? Fixed { get; set; }

    [Column("waiting_to_move")]
    public int? WaitingToMove { get; set; }

    [Column("move_time")]
    public DateTime? MoveTime { get; set; }

    [Column("selected")]
    public int? Selected { get; set; }

    [Column("delay")]
    public int? Delay { get; set; }

    [Column("skluz")]
    public int? Skluz { get; set; }

    [Column("razeno")]
    public int? Razeno { get; set; }

    [Column("team_leader")]
    public string? TeamLeader { get; set; }

    [Column("reason_filled")]
    public int? ReasonFilled { get; set; }

    [Column("reason_text")]
    public string? ReasonText { get; set; }

    [Column("reason_note")]
    public string? ReasonNote { get; set; }

    [Column("next_radod_oper")]
    public string? NextRadodOper { get; set; }

    [Column("next_presun")]
    public int? NextPresun { get; set; }

    [Column("oper_to_do")]
    public int? OperToDo { get; set; }

    [Column("oper_group_color")]
    public string? OperGroupColor { get; set; }

    [Column("rank_all")]
    public int? RankAll { get; set; }

    // Fáze 2 - sloupec "Status" v gridu (divergence tlacitko, PRD 5.5).
    [Column("status")]
    public string? Status { get; set; }

    [Column("workflow_link")]
    public string? WorkflowLink { get; set; }

    // Z LEFT JOIN t_divergence_oper uvnitr view - barevne kodovani tlacitka Status.
    [Column("div_status")]
    public int? DivStatus { get; set; }
}
