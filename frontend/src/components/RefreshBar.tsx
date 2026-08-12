interface RefreshBarProps {
  lastSync: string | null;
  onRefresh: () => void;
  canRefresh: boolean;
  showWarning: boolean;
  refreshing: boolean;
}

// Rucni refresh + upozorneni na neaktualni data (PRD 5.5, 8.1) - appka nema auto-refresh.
export default function RefreshBar({ lastSync, onRefresh, canRefresh, showWarning, refreshing }: RefreshBarProps) {
  return (
    <div className="refresh-bar">
      <button
        type="button"
        className="icon-button refresh-button"
        onClick={onRefresh}
        disabled={!canRefresh || refreshing}
        title="Refresh"
      >
        ⟳ Refresh
      </button>
      {lastSync && (
        <span className="last-sync">Poslední sync dat: {new Date(lastSync).toLocaleString("cs-CZ")}</span>
      )}
      {showWarning && (
        <span className="stale-warning">⚠ Aktualizuj aplikaci — klikni na refresh</span>
      )}
    </div>
  );
}
