using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using PlanSekvenci.Api.Data;
using PlanSekvenci.Api.Data.Entities;

namespace PlanSekvenci.Api.Services;

// PRD 4.5: konsolidovany zapis chyb do t_log_powerapp misto rucniho logovani po
// kazdem Patch, jak to delala puvodni Power Apps appka.
// IExceptionHandler je registrovany jako singleton, proto scoped BiAppDbContext
// ziskavame pres IServiceScopeFactory misto konstruktorove injekce.
public class ErrorLoggingExceptionHandler(IServiceScopeFactory scopeFactory, ILogger<ErrorLoggingExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Neosetrena chyba na {Path}", httpContext.Request.Path);

        try
        {
            using var scope = scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<BiAppDbContext>();
            db.LogPowerapps.Add(new LogPowerapp
            {
                RecordDate = DateTime.Now,
                Date = DateOnly.FromDateTime(DateTime.Today),
                ObjectName = httpContext.Request.Path,
                ErrorMessage = exception.Message.Length > 4000 ? exception.Message[..4000] : exception.Message,
                User = httpContext.User.Identity?.Name,
            });
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (Exception logEx)
        {
            logger.LogError(logEx, "Zapis chyby do t_log_powerapp selhal");
        }

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Nastala neocekavana chyba.",
        }, cancellationToken);

        return true;
    }
}
