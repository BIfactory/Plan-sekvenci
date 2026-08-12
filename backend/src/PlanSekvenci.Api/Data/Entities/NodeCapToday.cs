using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Kapacita pracoviste dnes, pwrapp-specific (PRD 4.2) - soucast vypoctu "Kapacita"
// v panelu "Aktualni data".
[Table("t_pwrapp_node_cap_today")]
public class NodeCapToday
{
    [Column("date")]
    public DateOnly? Date { get; set; }

    [Column("node")]
    public string? Node { get; set; }

    [Column("cap")]
    public double? Cap { get; set; }

    [Column("date_node")]
    public string? DateNode { get; set; }
}
