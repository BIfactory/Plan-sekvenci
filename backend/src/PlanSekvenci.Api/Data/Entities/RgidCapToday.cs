using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Kapacita rgid dnes, pwrapp-specific (PRD 4.2) - soucast vypoctu "Kapacita"
// v panelu "Aktualni data" (kdyz je vybrano CNC pracoviste + konkretni RGID).
[Table("t_pwrapp_rgid_cap_today")]
public class RgidCapToday
{
    [Column("date")]
    public DateOnly? Date { get; set; }

    [Column("rgid")]
    public string? Rgid { get; set; }

    [Column("cap")]
    public double? Cap { get; set; }

    [Column("date_rgid")]
    public string? DateRgid { get; set; }
}
