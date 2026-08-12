using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Mapovani na navazujici suffix (PRD 4.2, Fáze 2 - "X-suffix (návaznost)") - JOIN
// t_workplan_input + t_job, appka jen cte, filtrovano na id_job_suffix_oper.
[Table("v_x_suffix")]
public class XSuffix
{
    [Column("X_Job_Suffix")]
    public string? XJobSuffix { get; set; }

    [Column("id_job_suffix_oper")]
    public string? IdJobSuffixOper { get; set; }
}
