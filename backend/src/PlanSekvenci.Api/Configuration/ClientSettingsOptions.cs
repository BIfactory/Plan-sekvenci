namespace PlanSekvenci.Api.Configuration;

// Faze 3 - chovani frontendu, ktere ma jit menit bez rebuildu SPA (appsettings.json
// je jediny zdroj pravdy, viz Controllers/ConfigController.cs).
public class ClientSettingsOptions
{
    public const string SectionName = "ClientSettings";

    // Interval automatickeho background refreshe dat (PRD 2 - "misto rucniho tlacitka").
    public int AutoRefreshIntervalSeconds { get; set; } = 60;

    // Platnost cookie s ulozenymi filtry Provoz/Dilna/Team Leader/Uzel/Skupina zdroju
    // (viz frontend/src/lib/filterCookie.ts).
    public int FilterCookieExpiryDays { get; set; } = 30;
}
