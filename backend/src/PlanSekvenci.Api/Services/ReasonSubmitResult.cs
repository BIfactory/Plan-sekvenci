namespace PlanSekvenci.Api.Services;

// PRD 4.5 - silnejsi validace: SubmitReasonAsync (workplan i produced) uz nevraci jen
// bool (nalezeno/nenalezeno), ale rozlisuje i neplatnou hodnotu Reason (musi byt prazdna
// nebo odpovidat ciselniku dim_workplan_reasons), aby ji controller mohl mapovat na
// spravny HTTP status (404 vs. 400) misto vzdy stejneho vysledku.
public enum ReasonSubmitResult
{
    Success,
    NotFound,
    InvalidReason,
}
