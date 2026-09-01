using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using PlanSekvenci.Api.Authorization;
using PlanSekvenci.Api.Configuration;
using PlanSekvenci.Api.Data;
using PlanSekvenci.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BiAppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BiApp")));

// Windows Authentication (IIS/Negotiate) - appka je viditelna vsem AD uzivatelum,
// editace je dale omezena approver policy nize (PRD sekce 3, 8.1).
builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();

builder.Services.AddAuthorizationBuilder()
    .SetFallbackPolicy(new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build())
    .AddPolicy(ApproverRequirement.PolicyName, policy =>
        policy.Requirements.Add(new ApproverRequirement()));

builder.Services.Configure<ApproverOptions>(
    builder.Configuration.GetSection(ApproverOptions.SectionName));

builder.Services.Configure<AzureAdOptions>(
    builder.Configuration.GetSection(AzureAdOptions.SectionName));

builder.Services.Configure<ClientSettingsOptions>(
    builder.Configuration.GetSection(ClientSettingsOptions.SectionName));

builder.Services.Configure<AuditLogOptions>(
    builder.Configuration.GetSection(AuditLogOptions.SectionName));

// DevApproverService obchazi AD/LDAP (viz Authorization:BypassAdInDevelopment v
// appsettings.Development.json) - jen pro pripady, kdy AD neni z vyvojoveho stroje
// dosazitelny. Bezpecnostne neskodne, protoze vyzaduje i IsDevelopment().
var bypassAdInDevelopment = builder.Environment.IsDevelopment()
    && builder.Configuration.GetValue<bool>($"{ApproverOptions.SectionName}:BypassAdInDevelopment");
if (bypassAdInDevelopment)
{
    builder.Services.AddSingleton<IApproverService, DevApproverService>();
}
else
{
    builder.Services.AddHttpClient();
    builder.Services.AddSingleton<IGraphGroupMembershipChecker, GraphGroupMembershipChecker>();
    builder.Services.AddSingleton<IApproverService, ApproverService>();
}

builder.Services.AddSingleton<IAuthorizationHandler, ApproverAuthorizationHandler>();

builder.Services.AddScoped<AuditLogService>();
builder.Services.AddScoped<WorkplanService>();
builder.Services.AddScoped<ProducedService>();
builder.Services.AddScoped<ReferenceService>();

// PRD 4.5: centralizovany zapis chyb do t_log_powerapp misto rucniho logovani po
// kazdem Patch, jak to delala puvodni appka.
builder.Services.AddExceptionHandler<ErrorLoggingExceptionHandler>();
builder.Services.AddProblemDetails();

const string FrontendCorsPolicy = "FrontendDev";
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors(FrontendCorsPolicy);
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
