using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Ciselnik duvodu pro dropdown pri zapisu duvodu (PRD 4.2) - jen cteni.
[Table("dim_workplan_reasons")]
public class WorkplanReasonDim
{
    [Column("desc")]
    public string Desc { get; set; } = string.Empty;
}
