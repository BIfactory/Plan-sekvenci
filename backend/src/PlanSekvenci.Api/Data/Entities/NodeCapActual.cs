using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Kapacita pracoviste, skutecnost (PRD 4.2) - soucast vypoctu "Kapacita" v panelu
// "Aktualni data" (rozliseni mezi node-uroven kapacitou a "bez limitu" = cap 10000).
[Table("t_node_cap_actual")]
public class NodeCapActual
{
    [Column("shift_day")]
    public DateOnly ShiftDay { get; set; }

    [Column("node")]
    public string Node { get; set; } = string.Empty;

    [Column("cap")]
    public double? Cap { get; set; }

    [Column("online_workplan")]
    public int OnlineWorkplan { get; set; }
}
