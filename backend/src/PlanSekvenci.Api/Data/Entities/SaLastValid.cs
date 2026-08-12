using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Posledni platna "SA" data per node (PRD 4.2) - zdroj panelu "Historie"
// (sloupce Plneni planu / Vykon) na obrazovce Plan sekvenci.
[Table("t_pwrapp_workplan_sa_last_valid")]
public class SaLastValid
{
    [Column("Date")]
    public DateOnly? Date { get; set; }

    [Column("date_desc")]
    public string? DateDesc { get; set; }

    [Column("node")]
    public string? Node { get; set; }

    [Column("sa")]
    public double? Sa { get; set; }

    [Column("performance")]
    public double? Performance { get; set; }
}
