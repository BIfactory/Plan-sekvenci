using Microsoft.EntityFrameworkCore;

namespace PlanSekvenci.Api.Data;

// Databaze BI_APP neni vlastnena touto appkou (PRD 4.1) - EF Core se pouziva jen jako
// pristupova vrstva nad existujicimi tabulkami/views, bez migraci.
public class BiAppDbContext(DbContextOptions<BiAppDbContext> options) : DbContext(options)
{
}
