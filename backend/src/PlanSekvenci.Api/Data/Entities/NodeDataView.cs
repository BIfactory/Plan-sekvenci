using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Agregace hodin/skluzu per node (PRD 4.2) - zdroj panelu "Aktualni data" na
// obrazovce Plan sekvenci (hod_plan, hod_plan_skluz, hod_all, suma delay).
[Table("v_workplan_node_data")]
public class NodeDataView
{
    [Column("node")]
    public string? Node { get; set; }

    [Column("hod_plan")]
    public double? HodPlan { get; set; }

    [Column("hod_plan_skluz")]
    public double? HodPlanSkluz { get; set; }

    [Column("hod_all")]
    public double? HodAll { get; set; }

    [Column("delay")]
    public int? Delay { get; set; }
}
