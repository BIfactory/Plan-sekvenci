using Microsoft.EntityFrameworkCore;
using PlanSekvenci.Api.Data;
using PlanSekvenci.Api.Data.Entities;
using PlanSekvenci.Api.Dtos;

namespace PlanSekvenci.Api.Services;

// Fáze 3 (PRD 2) - audit log editaci (fixed/selected/reason), viz
// backend/sql/t_workplan_audit_log.sql, Data/Entities/WorkplanAuditLog.cs.
public class AuditLogService(BiAppDbContext db)
{
    // Jen prida zaznam do change trackeru - volajici (WorkplanService/ProducedService)
    // ho uklada spolecne s vlastni zmenou v jedne transakci (PRD 4.5 - transakcni zapis).
    public void Log(string user, string action, string entityType, string entityId, string? node, string? oldValue, string? newValue)
    {
        db.WorkplanAuditLogs.Add(new WorkplanAuditLog
        {
            RecordDate = DateTime.Now,
            User = user,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Node = node,
            OldValue = oldValue,
            NewValue = newValue,
        });
    }

    public async Task<IReadOnlyList<AuditLogEntryDto>> GetAsync(
        string? entityId, string? entityType, DateTime? from, DateTime? to, CancellationToken ct)
    {
        var query = db.WorkplanAuditLogs.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(entityId))
        {
            query = query.Where(x => x.EntityId == entityId);
        }
        if (!string.IsNullOrEmpty(entityType))
        {
            query = query.Where(x => x.EntityType == entityType);
        }
        if (from.HasValue)
        {
            query = query.Where(x => x.RecordDate >= from.Value);
        }
        if (to.HasValue)
        {
            query = query.Where(x => x.RecordDate <= to.Value);
        }

        var rows = await query
            .OrderByDescending(x => x.RecordDate)
            .Take(500)
            .ToListAsync(ct);

        return rows.Select(x => new AuditLogEntryDto(
            x.Id, x.RecordDate, x.User, x.Action, x.EntityType, x.EntityId, x.Node, x.OldValue, x.NewValue
        )).ToList();
    }
}
