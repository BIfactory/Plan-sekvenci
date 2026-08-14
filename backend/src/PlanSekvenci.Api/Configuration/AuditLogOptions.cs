namespace PlanSekvenci.Api.Configuration;

// Faze 3 - audit log editaci (fixed/selected/reason) jde vypnout bez rebuildu,
// viz Services/AuditLogService.cs. Vychozi stav je zapnuto.
public class AuditLogOptions
{
    public const string SectionName = "AuditLog";

    public bool Enabled { get; set; } = true;
}
