using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Ciselnik pracovist (PRD 4.2) - zdroj filtru plant/dept/team_leader/node (PRD 5.2).
[Table("v_node_definition")]
public class NodeDefinition
{
    [Column("node")]
    public string Node { get; set; } = string.Empty;

    [Column("plant")]
    public string? Plant { get; set; }

    [Column("dept")]
    public string? Dept { get; set; }

    [Column("team_leader")]
    public string? TeamLeader { get; set; }

    // Pouzito v panelu "Aktualni data" (Typ planu: Online plan / Fixni).
    [Column("online_workPlan")]
    public int? OnlineWorkPlan { get; set; }

    // Pouzito ve vypoctu "Kapacita" (rozliseni RGID- vs node-uroven kapacity).
    [Column("cnc")]
    public int? Cnc { get; set; }
}
