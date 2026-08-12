using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Detail kroku operace (PRD 4.2, Fáze 2 - "Detail operace") - JOIN t_sequences +
// t_jobroute, appka jen cte, filtrovano na id_job_suffix.
//
// Appka tyto "podkladové" tabulky (PRD 4.4) nikdy primo necetla, takze nezname
// jejich presne DDL - SqlDataReader je striktni na presny CLR typ (napr. decimal
// vs double zpusobi InvalidCastException az za behu na realne DB, ne pri buildu).
// Proto se tahle entita nacita vyhradne pres FromSqlInterpolated s explicitnim
// CAST() na kazdem cisleném sloupci (viz WorkplanService.GetSequenceDetailAsync) -
// typ tady v entite musi presne odpovidat cilovemu typu toho CASTu.
[Table("v_sequences_detail")]
public class SequencesDetail
{
    [Column("id_job_suffix_oper")]
    public string IdJobSuffixOper { get; set; } = string.Empty;

    [Column("oper_num")]
    public int? OperNum { get; set; }

    [Column("oper_desc")]
    public string? OperDesc { get; set; }

    [Column("node")]
    public string? Node { get; set; }

    [Column("complete")]
    public int? Complete { get; set; }

    [Column("qty_received")]
    public double? QtyReceived { get; set; }

    [Column("qty_complete")]
    public double? QtyComplete { get; set; }

    [Column("qty_scrapped")]
    public double? QtyScrapped { get; set; }

    [Column("hod")]
    public double? Hod { get; set; }

    [Column("delay")]
    public int? Delay { get; set; }

    [Column("sequence_date_time")]
    public DateTime? SequenceDateTime { get; set; }

    [Column("job_start_date")]
    public DateTime? JobStartDate { get; set; }

    [Column("job_end_date")]
    public DateTime? JobEndDate { get; set; }

    [Column("razeno")]
    public int? Razeno { get; set; }
}
