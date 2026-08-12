import { useEffect, useRef, useState } from "react";

const WARNING_AFTER_MS = 30 * 60 * 1000; // PRD 5.5: po 30 minutach bez refreshe
const REFRESH_COOLDOWN_MS = 5 * 60 * 1000; // puvodni appka blokovala tlacitko refresh 5 min

// Puvodne appka nemela auto-refresh dat (PRD 8.1), jen upozornovala uzivatele po 30 min
// bez refreshe. Faze 3 pridala automaticky background refresh (viz useAutoRefresh), ktery
// markRefreshed() vola po kazdem uspesnem tiku - toto upozorneni tak v beznem provozu
// nenaskoci, ale zustava jako fallback pro pripad, ze by auto-refresh dlouhodobe selhaval
// (napr. vypadek site) - stejny vizualni signal jako puvodni appka (Timer1/Timer2
// v Plán sekvencí.pa.yaml).
export function useStaleDataWarning() {
  const [lastRefresh, setLastRefresh] = useState(() => Date.now());
  const [showWarning, setShowWarning] = useState(false);
  const [canRefresh, setCanRefresh] = useState(false);
  const timerRef = useRef<ReturnType<typeof setInterval> | null>(null);

  useEffect(() => {
    const tick = () => {
      const elapsed = Date.now() - lastRefresh;
      setShowWarning(elapsed >= WARNING_AFTER_MS);
      setCanRefresh(elapsed >= REFRESH_COOLDOWN_MS);
    };
    tick();
    timerRef.current = setInterval(tick, 1000);
    return () => {
      if (timerRef.current) clearInterval(timerRef.current);
    };
  }, [lastRefresh]);

  const markRefreshed = () => setLastRefresh(Date.now());

  return { showWarning, canRefresh, markRefreshed };
}
