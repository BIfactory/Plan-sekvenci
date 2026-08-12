using Microsoft.EntityFrameworkCore;
using PlanSekvenci.Api.Data.Entities;

namespace PlanSekvenci.Api.Data;

// Databaze BI_APP neni vlastnena touto appkou (PRD 4.1) - EF Core se pouziva jen jako
// pristupova vrstva nad existujicimi tabulkami/views, bez migraci.
public class BiAppDbContext(DbContextOptions<BiAppDbContext> options) : DbContext(options)
{
    public DbSet<WorkplanInput> WorkplanInputs => Set<WorkplanInput>();
    public DbSet<WorkplanInputView> WorkplanInputView => Set<WorkplanInputView>();
    public DbSet<NodeDefinition> NodeDefinitions => Set<NodeDefinition>();
    public DbSet<WorkplanReasonDim> WorkplanReasonDims => Set<WorkplanReasonDim>();
    public DbSet<WorkplanReason> WorkplanReasons => Set<WorkplanReason>();
    public DbSet<LogPowerapp> LogPowerapps => Set<LogPowerapp>();
    public DbSet<WorkplanProduced> WorkplanProduced => Set<WorkplanProduced>();
    public DbSet<LogSyncLastSync> LogSyncLastSync => Set<LogSyncLastSync>();
    public DbSet<NodeDataView> NodeDataView => Set<NodeDataView>();
    public DbSet<ProducedTodayView> ProducedTodayView => Set<ProducedTodayView>();
    public DbSet<NodeCapToday> NodeCapToday => Set<NodeCapToday>();
    public DbSet<RgidCapToday> RgidCapToday => Set<RgidCapToday>();
    public DbSet<NodeCapActual> NodeCapActual => Set<NodeCapActual>();
    public DbSet<SaLastValid> SaLastValid => Set<SaLastValid>();
    public DbSet<SaRgidLastValid> SaRgidLastValid => Set<SaRgidLastValid>();
    public DbSet<SequencesDetail> SequencesDetail => Set<SequencesDetail>();
    public DbSet<SerialNumber> SerialNumbers => Set<SerialNumber>();
    public DbSet<DivergenceAll> DivergenceAll => Set<DivergenceAll>();
    public DbSet<XSuffix> XSuffix => Set<XSuffix>();

    // Fáze 3 - audit log editaci (viz WorkplanAuditLog.cs, backend/sql/t_workplan_audit_log.sql).
    public DbSet<WorkplanAuditLog> WorkplanAuditLogs => Set<WorkplanAuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkplanInput>(e =>
        {
            e.HasKey(x => x.IdJobSuffixOper);
        });

        // Views - keyless, jen cteni. id_job_suffix_oper neni jedinecny klic pro EF zde
        // zamerne (agregacni/join views mohou vracet duplicity pri LEFT JOINu).
        modelBuilder.Entity<WorkplanInputView>(e =>
        {
            e.HasNoKey();
            e.ToView("v_workplan_input");
        });

        modelBuilder.Entity<NodeDefinition>(e =>
        {
            e.HasNoKey();
            e.ToView("v_node_definition");
        });

        modelBuilder.Entity<LogSyncLastSync>(e =>
        {
            e.HasNoKey();
            e.ToView("v_log_sync_last_sync_dbs10");
        });

        modelBuilder.Entity<WorkplanReasonDim>(e =>
        {
            e.HasKey(x => x.Desc);
        });

        modelBuilder.Entity<WorkplanReason>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<LogPowerapp>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<WorkplanProduced>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<NodeDataView>(e =>
        {
            e.HasNoKey();
            e.ToView("v_workplan_node_data");
        });

        modelBuilder.Entity<ProducedTodayView>(e =>
        {
            e.HasNoKey();
            e.ToView("v_produced_today");
        });

        // t_pwrapp_node_cap_today / t_pwrapp_rgid_cap_today / t_pwrapp_workplan_sa_last_valid
        // nemaji v DB deklarovany primary key (viz schema2.sql) - mapovat jako keyless.
        modelBuilder.Entity<NodeCapToday>().HasNoKey();
        modelBuilder.Entity<RgidCapToday>().HasNoKey();
        modelBuilder.Entity<SaLastValid>().HasNoKey();
        modelBuilder.Entity<SaRgidLastValid>().HasNoKey();

        modelBuilder.Entity<NodeCapActual>(e =>
        {
            e.HasKey(x => new { x.ShiftDay, x.Node, x.OnlineWorkplan });
        });

        // Fáze 2 - detail operace / seriova cisla / divergence / x-suffix (PRD 5.5).
        modelBuilder.Entity<SequencesDetail>(e =>
        {
            e.HasNoKey();
            e.ToView("v_sequences_detail");
        });

        modelBuilder.Entity<SerialNumber>(e =>
        {
            e.HasKey(x => new { x.SerNum, x.IdJobSuffixOper });
        });

        // t_divergence_all nema v DB deklarovany primary key (viz schema2.sql).
        modelBuilder.Entity<DivergenceAll>().HasNoKey();

        modelBuilder.Entity<XSuffix>(e =>
        {
            e.HasNoKey();
            e.ToView("v_x_suffix");
        });

        // Fáze 3 - audit log (backend/sql/t_workplan_audit_log.sql).
        modelBuilder.Entity<WorkplanAuditLog>(e =>
        {
            e.HasKey(x => x.Id);
            e.Property(x => x.Id).ValueGeneratedOnAdd();
        });
    }
}
