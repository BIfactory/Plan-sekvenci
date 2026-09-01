using System.ComponentModel.DataAnnotations.Schema;

namespace PlanSekvenci.Api.Data.Entities;

// Rozsah datumu pro filtr "Datum" na Vyhodnoceni (PRD 6.2, Vyhodnocení.pa.yaml -
// filter_date.StartDate/EndDate). View vraci promenlivy pocet radku (3 az 5 dle dne
// v tydnu), aby pokryla posledni 3 skutecne pracovni dny i pres vikend/svatek
// (viz used tabs views2.sql - CTE Workdays + Limits).
[Table("v_calendar_last_3_workdays")]
public class CalendarLastWorkday
{
    [Column("date")]
    public DateOnly Date { get; set; }
}
