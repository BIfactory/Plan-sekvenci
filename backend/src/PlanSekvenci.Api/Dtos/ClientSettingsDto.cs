namespace PlanSekvenci.Api.Dtos;

// Faze 3 - GET /api/config (viz Configuration/ClientSettingsOptions.cs).
public record ClientSettingsDto(int AutoRefreshIntervalSeconds, int FilterCookieExpiryDays);
