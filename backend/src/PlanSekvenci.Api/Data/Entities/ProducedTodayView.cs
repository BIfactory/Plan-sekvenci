using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Agregace dnesni produkce per node/rgid (PRD 4.2) - zdroj panelu "Aktualni data"
// (Odvedeno celkem) a vypoctu "Kapacita" na obrazovce Plan sekvenci.
[Table("v_produced_today")]
public class ProducedTodayView
{
    [Column("node")]
    public string? Node { get; set; }

    [Column("rgid")]
    public string? Rgid { get; set; }

    [Column("date")]
    public DateOnly? Date { get; set; }

    [Column("hod_produced")]
    public double? HodProduced { get; set; }

    [Column("qty_done")]
    public double? QtyDone { get; set; }

    [Column("status")]
    public int? Status { get; set; }
}
