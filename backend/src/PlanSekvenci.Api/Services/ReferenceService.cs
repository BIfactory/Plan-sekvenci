using Microsoft.EntityFrameworkCore;
using PlanSekvenci.Api.Data;
using PlanSekvenci.Api.Dtos;

namespace PlanSekvenci.Api.Services;

public class ReferenceService(BiAppDbContext db)
{
    public async Task<IReadOnlyList<NodeDefinitionDto>> GetNodesAsync(CancellationToken ct) =>
        await db.NodeDefinitions.AsNoTracking()
            .Select(x => new NodeDefinitionDto(x.Node, x.Plant, x.Dept, x.TeamLeader))
            .ToListAsync(ct);

    // Prazdna volba se pridava na frontendu (PRD 5.5 / App.pa.yaml: Collect(Reasons, {desc: ""})).
    public async Task<IReadOnlyList<string>> GetReasonsAsync(CancellationToken ct) =>
        await db.WorkplanReasonDims.AsNoTracking()
            .Select(x => x.Desc)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);

    public async Task<LastSyncDto> GetLastSyncAsync(CancellationToken ct)
    {
        var row = await db.LogSyncLastSync.AsNoTracking().FirstOrDefaultAsync(ct);
        return new LastSyncDto(row?.StartTime);
    }

    // Panel "Aktualni data" (viz Plán sekvencí.pa.yaml, Group1) - agregace za vybrany
    // uzel + vypocet kapacity (RGID-uroven u CNC pracovist, jinak node-uroven).
    public async Task<NodeSummaryDto> GetNodeSummaryAsync(string? node, string? rgid, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(node))
        {
            return new NodeSummaryDto(null, null, null, null, null, null, null, null, null);
        }

        var nodeData = await db.NodeDataView.AsNoTracking().FirstOrDefaultAsync(x => x.Node == node, ct);

        var producedTodayForNode = db.ProducedTodayView.AsNoTracking().Where(x => x.Node == node);
        var producedTotal = await producedTodayForNode.SumAsync(x => (double?)x.HodProduced, ct);
        var producedStatus1Or3 = await producedTodayForNode.Where(x => x.Status == 1 || x.Status == 3).SumAsync(x => (double?)x.HodProduced, ct);
        var producedStatus1 = await producedTodayForNode.Where(x => x.Status == 1).SumAsync(x => (double?)x.HodProduced, ct);

        var nodeDef = await db.NodeDefinitions.AsNoTracking().FirstOrDefaultAsync(x => x.Node == node, ct);
        var isOnlinePlan = nodeDef is null ? (bool?)null : nodeDef.OnlineWorkPlan == 1;

        var capacity = await GetCapacityAsync(node, rgid, nodeDef?.Cnc == 1, producedStatus1 ?? 0, ct);

        return new NodeSummaryDto(
            nodeData?.HodPlanSkluz, nodeData?.HodPlan, nodeData?.HodAll, nodeData?.Delay,
            producedTotal, producedStatus1Or3, producedStatus1, isOnlinePlan, capacity);
    }

    private async Task<CapacityDto?> GetCapacityAsync(string node, string? rgid, bool isCnc, double producedStatus1, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        if (isCnc && !string.IsNullOrEmpty(rgid))
        {
            var dateRgid = $"{today:yyyy-MM-dd}-{rgid.Trim()}";
            var cap = await db.RgidCapToday.AsNoTracking().Where(x => x.DateRgid == dateRgid).Select(x => x.Cap).FirstOrDefaultAsync(ct);
            if (cap is null) return null;

            var used = await db.ProducedTodayView.AsNoTracking()
                .Where(x => x.Rgid == rgid.Trim() && x.Status == 1)
                .SumAsync(x => (double?)x.HodProduced, ct) ?? 0;
            return new CapacityDto(Math.Round(cap.Value - used, 1), Math.Round(cap.Value, 1), "rgid");
        }

        var dateNode = $"{today:yyyy-MM-dd}-{node}";
        var nodeCapTodayMax = await db.NodeCapToday.AsNoTracking().Where(x => x.DateNode == dateNode).MaxAsync(x => (double?)x.Cap, ct);
        var nodeCapActualMax = await db.NodeCapActual.AsNoTracking().Where(x => x.Node == node).MaxAsync(x => (double?)x.Cap, ct);

        if (nodeCapTodayMax is > 0 && nodeCapActualMax is < 1000)
        {
            return new CapacityDto(Math.Round(nodeCapTodayMax.Value - producedStatus1, 1), Math.Round(nodeCapTodayMax.Value, 1), "node");
        }

        if (nodeCapActualMax == 10000)
        {
            return new CapacityDto(Math.Round((nodeCapTodayMax ?? 0) - producedStatus1, 1), null, "nocap");
        }

        return null;
    }

    // Panel "Historie" (Plneni planu / Vykon po dnech, PRD 4.2 t_pwrapp_workplan_sa_last_valid).
    public async Task<IReadOnlyList<SaHistoryEntryDto>> GetSaHistoryAsync(string? node, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(node))
        {
            return [];
        }

        var today = DateOnly.FromDateTime(DateTime.Today);
        var rows = await db.SaLastValid.AsNoTracking()
            .Where(x => x.Node == node && x.Date < today)
            .OrderByDescending(x => x.Date)
            .ToListAsync(ct);

        return rows.Select(x => new SaHistoryEntryDto(x.Date, x.DateDesc, x.Sa, x.Performance)).ToList();
    }
}
