using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Posledni platna "SA" data per rgid (PRD 4.2) - zdroj panelu "Historie" na
// obrazovce Vyhodnoceni, kdyz je vybrano konkretni RGID (jinak se pouziva
// node-uroven, viz SaLastValid).
[Table("t_pwrapp_workplan_sa_rgid_last_valid")]
public class SaRgidLastValid
{
    [Column("date")]
    public DateOnly? Date { get; set; }

    [Column("date_desc")]
    public string? DateDesc { get; set; }

    [Column("node")]
    public string? Node { get; set; }

    [Column("rgid")]
    public string? Rgid { get; set; }

    [Column("sa")]
    public double? Sa { get; set; }

    [Column("performance")]
    public double? Performance { get; set; }
}
