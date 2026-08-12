import { useEffect, useRef } from "react";

// Fáze 3 (PRD 2, 8.1) - automaticky background refresh dat namisto (puvodne) vyhradne
// rucniho tlacitka. Interval se spousti pres callbackRef, aby zmena `callback` (novy
// filtr apod.) nerestartovala odpocet - jinak by pri caste zmene filtru auto-refresh
// nikdy nestihl spustit. Kdyz je tab na pozadi (document.hidden), tick se preskoci -
// nema smysl zatezovat server refreshem, ktery uzivatel stejne nevidi.
// `intervalMs` prichazi z appsettings.json (viz hooks/useClientSettings.ts).
export function useAutoRefresh(callback: () => void, intervalMs: number) {
  const callbackRef = useRef(callback);

  useEffect(() => {
    callbackRef.current = callback;
  }, [callback]);

  useEffect(() => {
    const id = setInterval(() => {
      if (!document.hidden) {
        callbackRef.current();
      }
    }, intervalMs);
    return () => clearInterval(id);
  }, [intervalMs]);
}
