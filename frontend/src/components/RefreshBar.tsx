import { useRef, useState } from "react";

const CLICK_COOLDOWN_MS = 3000;

interface RefreshBarProps {
  lastSync: string | null;
  onRefresh: () => void;
  showWarning: boolean;
  refreshing: boolean;
}

// Rucni refresh (okamzite vynuceni, napr. hned po vlastni editaci) + upozorneni na
// neaktualni data (PRD 5.5, 8.1). Kratky cooldown po kliknuti je jen anti-dvojklik
// debounce - nezavisly na auto-refresh intervalu (viz useStaleDataWarning.ts), takze
// automaticky background refresh (kazdych ~60s) tlacitko trvale neblokuje.
export default function RefreshBar({ lastSync, onRefresh, showWarning, refreshing }: RefreshBarProps) {
  const [cooling, setCooling] = useState(false);
  const timerRef = useRef<ReturnType<typeof setTimeout> | null>(null);

  const handleClick = () => {
    onRefresh();
    setCooling(true);
    timerRef.current = setTimeout(() => setCooling(false), CLICK_COOLDOWN_MS);
  };

  return (
    <div className="refresh-bar">
      <button
        type="button"
        className="icon-button refresh-button"
        onClick={handleClick}
        disabled={cooling || refreshing}
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
