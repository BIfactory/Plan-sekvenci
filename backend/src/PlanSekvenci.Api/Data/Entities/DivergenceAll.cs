using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Odchylky/divergence per job/suffix (PRD 4.2, Fáze 2 - "Divergence"), appka jen
// cte, filtrovano na job+suffix. V DB nema deklarovany primary key (viz schema2.sql).
[Table("t_divergence_all")]
public class DivergenceAll
{
    [Column("job")]
    public string Job { get; set; } = string.Empty;

    [Column("suffix")]
    public short Suffix { get; set; }

    [Column("div_num")]
    public string DivNum { get; set; } = string.Empty;

    [Column("workflow_link")]
    public string? WorkflowLink { get; set; }

    [Column("div_count")]
    public int? DivCount { get; set; }
}
