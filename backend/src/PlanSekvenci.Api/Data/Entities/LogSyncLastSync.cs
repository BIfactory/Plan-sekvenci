using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Cas posledniho behu sync procesu (PRD 4.2) - zobrazuje se u tlacitka refresh.
[Table("v_log_sync_last_sync_dbs10")]
public class LogSyncLastSync
{
    [Column("start_time")]
    public DateTime? StartTime { get; set; }
}
