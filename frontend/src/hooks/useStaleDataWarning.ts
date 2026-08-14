import { useEffect, useRef, useState } from "react";

const WARNING_AFTER_MS = 30 * 60 * 1000; // PRD 5.5: po 30 minutach bez refreshe

// Puvodne appka nemela auto-refresh dat (PRD 8.1), jen upozornovala uzivatele po 30 min
// bez refreshe. Faze 3 pridala automaticky background refresh (viz useAutoRefresh), ktery
// markRefreshed() vola po kazdem uspesnem tiku - toto upozorneni tak v beznem provozu
// nenaskoci, ale zustava jako fallback pro pripad, ze by auto-refresh dlouhodobe selhaval
// (napr. vypadek site) - stejny vizualni signal jako puvodni appka (Timer1/Timer2
// v Plán sekvencí.pa.yaml).
//
// Puvodni 5min cooldown rucniho tlacitka (blokovani proti castemu zatezovani DB) se sem
// zamerne nepresunul - auto-refresh uz beh v pravidelnem intervalu resi sam, takze 5min
// cooldown pocitany od stejneho `lastRefresh` (ktery auto-refresh kazdych ~60s resetuje)
// by tlacitko drzel trvale disabled. Kratky anti-dvojklik debounce ma misto toho primo
// RefreshBar (viz komponenta) - nezavisly na tomto hooku.
export function useStaleDataWarning() {
  const [lastRefresh, setLastRefresh] = useState(() => Date.now());
  const [showWarning, setShowWarning] = useState(false);
  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);

  useEffect(() => {
    const tick = () => {
      setShowWarning(Date.now() - lastRefresh >= WARNING_AFTER_MS);
    };
    tick();
    timerRef.current = setInterval(tick, 1000);
    return () => {
      if (timerRef.current) clearInterval(timerRef.current);
    };
  }, [lastRefresh]);

  const markRefreshed = () => setLastRefresh(Date.now());

  return { showWarning, markRefreshed };
}
