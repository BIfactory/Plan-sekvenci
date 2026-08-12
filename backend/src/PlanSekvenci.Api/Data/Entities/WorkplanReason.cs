using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Historie zapsanych duvodu/poznamek (PRD 4.2) - appka jen vklada (insert), nikdy needituje.
[Table("t_workplan_reasons")]
public class WorkplanReason
{
    [Column("id")]
    public int Id { get; set; }

    [Column("TimeStamp")]
    public DateTime TimeStamp { get; set; }

    [Column("id_job_suffix_oper")]
    public string? IdJobSuffixOper { get; set; }

    [Column("node")]
    public string? Node { get; set; }

    [Column("reason")]
    public string? Reason { get; set; }

    [Column("note")]
    public string? Note { get; set; }

    [Column("user")]
    public string User { get; set; } = string.Empty;

    [Column("date")]
    public DateOnly? Date { get; set; }
}
