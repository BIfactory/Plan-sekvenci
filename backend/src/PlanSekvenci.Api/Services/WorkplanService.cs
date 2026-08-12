using Microsoft.EntityFrameworkCore;
using PlanSekvenci.Api.Data;
using PlanSekvenci.Api.Data.Entities;
using PlanSekvenci.Api.Dtos;

namespace PlanSekvenci.Api.Services;

public class WorkplanService(BiAppDbContext db, AuditLogService auditLog)
{
    // Sloupce, na ktere smi uzivatel kliknout pro razeni (PRD 5.3) - vzdy sestupne.
    private static readonly HashSet<string> SortableColumns = new(StringComparer.OrdinalIgnoreCase)
    {
        "skluz", "delay", "oper_num", "mprio", "hod", "sequence_date_time", "qty_todo", "in_plan"
    };

    public async Task<WorkplanListResultDto> GetAsync(WorkplanFilter filter, CancellationToken ct)
    {
        var query = db.WorkplanInputView.AsNoTracking().AsQueryable();

        if (!string.IsNullOrEmpty(filter.Plant))
        {
            query = query.Where(x => x.Plant == filter.Plant);
        }
        if (!string.IsNullOrEmpty(filter.Dept))
        {
            query = query.Where(x => x.Dept == filter.Dept);
        }
        if (!string.IsNullOrEmpty(filter.TeamLeader))
        {
            query = query.Where(x => x.TeamLeader == filter.TeamLeader);
        }
        if (!string.IsNullOrEmpty(filter.Node))
        {
            query = query.Where(x => x.Node == filter.Node);
        }
        if (!string.IsNullOrEmpty(filter.Rgid))
        {
            query = query.Where(x => x.Rgid == filter.Rgid);
        }
        if (!string.IsNullOrEmpty(filter.Inf))
        {
            query = query.Where(x => x.Inf == filter.Inf);
        }
        if (!string.IsNullOrEmpty(filter.GunFamily))
        {
            query = query.Where(x => x.GunFamily == filter.GunFamily);
        }
        if (filter.InPlan.HasValue)
        {
            query = query.Where(x => x.InPlan == filter.InPlan.Value);
        }
        if (filter.NextPresun.HasValue)
        {
            // PRD 5.2: "next_presun" filtr = next_presun = 1 AND waiting_to_move = 1
            query = query.Where(x => x.NextPresun == 1 && x.WaitingToMove == 1);
        }
        if (filter.WaitingToMove.HasValue)
        {
            var value = filter.WaitingToMove.Value ? 1 : 0;
            query = query.Where(x => x.WaitingToMove == value);
        }
        if (filter.Razeno.HasValue)
        {
            var value = filter.Razeno.Value ? 1 : 0;
            query = query.Where(x => x.Razeno == value);
        }
        if (!string.IsNullOrEmpty(filter.JobSuffix))
        {
            query = query.Where(x => x.IdJobSuffix != null && x.IdJobSuffix.Contains(filter.JobSuffix));
        }
        if (!string.IsNullOrEmpty(filter.Item))
        {
            query = query.Where(x => x.Item != null && x.Item.Contains(filter.Item));
        }

        // Razeni dle PRD 5.3: bez filtru a bez zvoleneho sloupce -> skluz sestupne;
        // zvoleny sloupec -> ten sestupne/vzestupne dle SortDir (uzivatel prepina klikem);
        // jinak (filtr bez zvoleneho sloupce) -> rank_all vzestupne.
        var sortColumn = !string.IsNullOrEmpty(filter.Sort) && SortableColumns.Contains(filter.Sort)
            ? filter.Sort.ToLowerInvariant()
            : null;

        if (!filter.HasAnyFilter && sortColumn is null)
        {
            query = query.OrderByDescending(x => x.Skluz);
        }
        else if (sortColumn is not null)
        {
            var ascending = string.Equals(filter.SortDir, "asc", StringComparison.OrdinalIgnoreCase);
            query = (sortColumn, ascending) switch
            {
                ("skluz", false) => query.OrderByDescending(x => x.Skluz),
                ("skluz", true) => query.OrderBy(x => x.Skluz),
                ("delay", false) => query.OrderByDescending(x => x.Delay),
                ("delay", true) => query.OrderBy(x => x.Delay),
                ("oper_num", false) => query.OrderByDescending(x => x.OperNum),
                ("oper_num", true) => query.OrderBy(x => x.OperNum),
                ("mprio", false) => query.OrderByDescending(x => x.Mprio),
                ("mprio", true) => query.OrderBy(x => x.Mprio),
                ("hod", false) => query.OrderByDescending(x => x.Hod),
                ("hod", true) => query.OrderBy(x => x.Hod),
                ("sequence_date_time", false) => query.OrderByDescending(x => x.SequenceDateTime),
                ("sequence_date_time", true) => query.OrderBy(x => x.SequenceDateTime),
                ("qty_todo", false) => query.OrderByDescending(x => x.QtyTodo),
                ("qty_todo", true) => query.OrderBy(x => x.QtyTodo),
                ("in_plan", false) => query.OrderByDescending(x => x.InPlan),
                ("in_plan", true) => query.OrderBy(x => x.InPlan),
                _ => query.OrderByDescending(x => x.Skluz),
            };
        }
        else
        {
            query = query.OrderBy(x => x.RankAll);
        }

        var totalCount = await query.CountAsync(ct);

        var page = Math.Max(filter.Page, 1);
        var pageSize = Math.Clamp(filter.PageSize <= 0 ? 200 : filter.PageSize, 1, 2000);

        var rows = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = rows.Select(ToDto).ToList();
        return new WorkplanListResultDto(items, totalCount);
    }

    public async Task<IReadOnlyList<string>> GetRgidsAsync(string? plant, string? dept, string? node, string? teamLeader, CancellationToken ct)
    {
        var query = db.WorkplanInputView.AsNoTracking().Where(x => x.Rgid != null && x.Rgid != "");
        if (!string.IsNullOrEmpty(plant)) query = query.Where(x => x.Plant == plant);
        if (!string.IsNullOrEmpty(dept)) query = query.Where(x => x.Dept == dept);
        if (!string.IsNullOrEmpty(node)) query = query.Where(x => x.Node == node);
        if (!string.IsNullOrEmpty(teamLeader)) query = query.Where(x => x.TeamLeader == teamLeader);

        return await query.Select(x => x.Rgid!).Distinct().OrderBy(x => x).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<string>> GetInfsAsync(CancellationToken ct) =>
        await db.WorkplanInputView.AsNoTracking()
            .Where(x => x.Inf != null && x.Inf != "")
            .Select(x => x.Inf!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);

    // Fáze 2 (PRD 5.5) - "Detail operace": kroky operace pro dany id_job_suffix,
    // razeno vzestupne dle oper_num, + zacatek/konec VP z prvniho radku (jako
    // First(Filter(v_sequences_detail, id_job_suffix = ...)) v puvodni appce).
    //
    // Explicitni CAST() na kazdem cislenem sloupci - t_sequences/t_jobroute jsou
    // "podkladove" tabulky (PRD 4.4), jejich skutecne DB typy nezname a hadani
    // (double vs decimal) uz jednou spadlo za behu na realne DB. CAST v SQL je vzdy
    // platny bez ohledu na puvodni typ, takze se timhle problem odstranuje napevno.
    public async Task<SequenceDetailHeaderDto> GetSequenceDetailAsync(string idJobSuffix, CancellationToken ct)
    {
        var rows = await db.SequencesDetail
            .FromSqlInterpolated($"""
                SELECT
                    id_job_suffix_oper,
                    CAST(oper_num AS int) AS oper_num,
                    oper_desc,
                    node,
                    CAST(complete AS int) AS complete,
                    CAST(qty_received AS float) AS qty_received,
                    CAST(qty_complete AS float) AS qty_complete,
                    CAST(qty_scrapped AS float) AS qty_scrapped,
                    CAST(hod AS float) AS hod,
                    CAST(delay AS int) AS delay,
                    sequence_date_time,
                    job_start_date,
                    job_end_date,
                    CAST(razeno AS int) AS razeno
                FROM v_sequences_detail
                WHERE id_job_suffix = {idJobSuffix}
                ORDER BY oper_num
                """)
            .AsNoTracking()
            .ToListAsync(ct);

        var steps = rows.Select(x => new SequenceDetailDto(
            x.IdJobSuffixOper, x.OperNum, x.OperDesc, x.QtyReceived, x.QtyComplete, x.QtyScrapped,
            x.Hod, x.Node, x.Delay, x.SequenceDateTime, x.Complete, x.Razeno
        )).ToList();

        var first = rows.FirstOrDefault();
        return new SequenceDetailHeaderDto(idJobSuffix, first?.JobStartDate, first?.JobEndDate, steps);
    }

    // Fáze 2 (PRD 5.5) - "Sériová čísla": seriova cisla pro danou operaci.
    public async Task<IReadOnlyList<SerialNumberDto>> GetSerialNumbersAsync(string idJobSuffixOper, CancellationToken ct)
    {
        var rows = await db.SerialNumbers.AsNoTracking()
            .Where(x => x.IdJobSuffixOper == idJobSuffixOper)
            .OrderBy(x => x.SerNum)
            .ToListAsync(ct);

        return rows.Select(x => new SerialNumberDto(x.SerNum, x.Item)).ToList();
    }

    // Fáze 2 (PRD 5.5) - "Divergence": seznam odchylek pro job+suffix.
    public async Task<IReadOnlyList<DivergenceDto>> GetDivergenceAsync(string job, short suffix, CancellationToken ct)
    {
        var rows = await db.DivergenceAll.AsNoTracking()
            .Where(x => x.Job == job && x.Suffix == suffix)
            .ToListAsync(ct);

        return rows.Select(x => new DivergenceDto(x.DivNum, x.WorkflowLink)).ToList();
    }

    // Fáze 2 (PRD 5.5) - "X-suffix (návaznost)": navazujici suffixy pro danou operaci.
    public async Task<IReadOnlyList<XSuffixDto>> GetXSuffixAsync(string idJobSuffixOper, CancellationToken ct)
    {
        var rows = await db.XSuffix.AsNoTracking()
            .Where(x => x.IdJobSuffixOper == idJobSuffixOper)
            .OrderBy(x => x.XJobSuffix)
            .ToListAsync(ct);

        return rows.Select(x => new XSuffixDto(x.XJobSuffix, x.IdJobSuffixOper)).ToList();
    }

    public async Task<IReadOnlyList<string>> GetGunFamiliesAsync(CancellationToken ct) =>
        await db.WorkplanInputView.AsNoTracking()
            .Where(x => x.GunFamily != null && x.GunFamily != "")
            .Select(x => x.GunFamily!)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(ct);

    // Faze 3 - zapis + audit log v jedne transakci (predtim jediny SaveChanges bez auditu).
    public async Task<bool> SetFixedAsync(string idJobSuffixOper, bool value, string userName, CancellationToken ct)
    {
        var entity = await db.WorkplanInputs.FirstOrDefaultAsync(x => x.IdJobSuffixOper == idJobSuffixOper, ct);
        if (entity is null)
        {
            return false;
        }

        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        var oldValue = entity.Fixed == 1;
        entity.Fixed = value ? 1 : 0;
        entity.FixedRecordDate = DateTime.Now;
        auditLog.Log(userName, "fixed", "workplan", idJobSuffixOper, entity.Node, oldValue.ToString(), value.ToString());

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return true;
    }

    public async Task<bool> SetSelectedAsync(string idJobSuffixOper, bool value, string userName, CancellationToken ct)
    {
        var entity = await db.WorkplanInputs.FirstOrDefaultAsync(x => x.IdJobSuffixOper == idJobSuffixOper, ct);
        if (entity is null)
        {
            return false;
        }

        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        var oldValue = entity.Selected == 1;
        entity.Selected = value ? 1 : 0;
        entity.SelectedRecordDate = value ? DateTime.Now : null;
        auditLog.Log(userName, "selected", "workplan", idJobSuffixOper, entity.Node, oldValue.ToString(), value.ToString());

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return true;
    }

    // PRD 4.5 + 5.5: insert do t_workplan_reasons, update t_workplan_input a zapis do
    // audit logu v jedne transakci. Faze 3 - Reason musi byt prazdny nebo odpovidat
    // ciselniku dim_workplan_reasons (predtim appka prijala jakykoliv retezec bez
    // overeni, ze skutecne existuje v dropdownu).
    public async Task<ReasonSubmitResult> SubmitReasonAsync(string idJobSuffixOper, string? node, string? reason, string? note, string userName, CancellationToken ct)
    {
        var entity = await db.WorkplanInputs.FirstOrDefaultAsync(x => x.IdJobSuffixOper == idJobSuffixOper, ct);
        if (entity is null)
        {
            return ReasonSubmitResult.NotFound;
        }

        if (!string.IsNullOrEmpty(reason) && !await db.WorkplanReasonDims.AsNoTracking().AnyAsync(x => x.Desc == reason, ct))
        {
            return ReasonSubmitResult.InvalidReason;
        }

        await using var transaction = await db.Database.BeginTransactionAsync(ct);

        var oldValue = $"reason: {entity.ReasonText ?? "-"} | note: {entity.ReasonNote ?? ""}";
        var newValue = $"reason: {(string.IsNullOrEmpty(reason) ? "-" : reason)} | note: {note ?? ""}";

        db.WorkplanReasons.Add(new WorkplanReason
        {
            TimeStamp = DateTime.Now,
            IdJobSuffixOper = idJobSuffixOper,
            Node = node ?? entity.Node,
            Reason = string.IsNullOrEmpty(reason) ? "-" : reason,
            Note = note,
            User = userName,
            Date = DateOnly.FromDateTime(DateTime.Today),
        });

        entity.ReasonText = reason;
        entity.ReasonNote = note;
        entity.ReasonFilled = string.IsNullOrEmpty(reason) ? 0 : 1;

        auditLog.Log(userName, "reason", "workplan", idJobSuffixOper, node ?? entity.Node, oldValue, newValue);

        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return ReasonSubmitResult.Success;
    }

    private static WorkplanItemDto ToDto(WorkplanInputView x) => new(
        x.Id,
        x.Node,
        x.IdJobSuffix,
        x.IdJobSuffixOper,
        x.Job,
        x.Suffix,
        x.OperNum,
        x.OperDesc,
        x.OperDescFull,
        x.Item,
        x.ItemDesc,
        x.ItemDescFull,
        x.GunFamily,
        x.Mprio,
        x.QtyReceived,
        x.QtyComplete,
        x.QtyTodo,
        x.LastTran,
        x.SequenceDateTime,
        x.Hod,
        x.Plant,
        x.Dept,
        x.Inf,
        x.Rgid,
        x.InPlan,
        x.FirstTimeInPlan,
        x.Fixed == 1,
        x.WaitingToMove == 1,
        x.MoveTime,
        x.Selected == 1,
        x.Delay,
        x.Razeno,
        x.TeamLeader,
        x.ReasonFilled,
        x.ReasonText,
        x.ReasonNote,
        x.NextRadodOper,
        x.NextPresun,
        x.OperToDo,
        x.OperGroupColor,
        x.RankAll,
        x.Status,
        x.WorkflowLink,
        x.DivStatus
    );
}
