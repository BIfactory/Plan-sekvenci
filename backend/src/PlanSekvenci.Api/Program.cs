using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using PlanSekvenci.Api.Authorization;
using PlanSekvenci.Api.Data;

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

builder.Services.AddSingleton<IAuthorizationHandler, ApproverAuthorizationHandler>();

builder.Services.Configure<ApproverOptions>(
    builder.Configuration.GetSection(ApproverOptions.SectionName));

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
