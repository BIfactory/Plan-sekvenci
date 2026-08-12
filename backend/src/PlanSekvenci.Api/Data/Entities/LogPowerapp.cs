using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Log chyb appky (PRD 4.2, 4.5) - centralizovany zapis chyb z API (middleware/filter),
// nahrazuje puvodni rucni logovani po kazdem Patch v Power Apps.
[Table("t_log_powerapp")]
public class LogPowerapp
{
    [Column("id")]
    public int Id { get; set; }

    [Column("RecordDate")]
    public DateTime? RecordDate { get; set; }

    [Column("date")]
    public DateOnly? Date { get; set; }

    [Column("object_name")]
    public string? ObjectName { get; set; }

    [Column("error_message")]
    public string? ErrorMessage { get; set; }

    [Column("user")]
    public string? User { get; set; }

    [Column("node")]
    public string? Node { get; set; }
}
