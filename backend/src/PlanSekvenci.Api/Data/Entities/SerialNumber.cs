using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Seriova cisla per operace (PRD 4.2, Fáze 2 - "Seriova cisla"), appka jen cte,
// filtrovano na id_job_suffix_oper. Sloupec "ttem" je preklep v puvodnim DDL.
[Table("t_serial_numbers")]
public class SerialNumber
{
    [Column("ser_num")]
    public string SerNum { get; set; } = string.Empty;

    [Column("ttem")]
    public string? Item { get; set; }

    [Column("job")]
    public string Job { get; set; } = string.Empty;

    [Column("suffix")]
    public short Suffix { get; set; }

    [Column("oper_num")]
    public short OperNum { get; set; }

    [Column("id_job_suffix")]
    public string IdJobSuffix { get; set; } = string.Empty;

    [Column("id_job_suffix_oper")]
    public string IdJobSuffixOper { get; set; } = string.Empty;
}
