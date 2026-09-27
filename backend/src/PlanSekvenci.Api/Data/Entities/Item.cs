using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Ciselnik polozek (t_item) - jen cteni, pouzito pro sloupec "Popis" na Vyhodnoceni
// (t_workplan_produced nema vlastni item_desc, na rozdil od t_workplan_input).
[Table("t_item")]
public class Item
{
    [Column("item")]
    public string ItemCode { get; set; } = string.Empty;

    [Column("description")]
    public string? Description { get; set; }
}
