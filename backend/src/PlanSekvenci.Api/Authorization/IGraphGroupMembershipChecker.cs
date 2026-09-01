namespace PlanSekvenci.Api.Authorization;

public interface IGraphGroupMembershipChecker
{
    // True, pokud je uzivatel (podle e-mailu) clenem AD skupiny groupId (Azure AD
    // Object ID) - vcetne vnorenych skupin (Graph checkMemberGroups je tranzitivni).
    Task<bool> IsMemberOfGroupAsync(string userEmail, string groupId, CancellationToken ct = default);
}
