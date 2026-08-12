import { useEffect, useState } from "react";
import { api } from "../api/client";
import type { ClientSettings } from "../api/types";

// Fallback, dokud se nenacte GET /api/config (nebo kdyz appsettings.json sekci
// ClientSettings neobsahuje) - stejne hodnoty jako drivejsi natvrdo zapsane konstanty,
// aby chovani zustalo shodne i bez konfigurace.
const DEFAULT_SETTINGS: ClientSettings = {
  autoRefreshIntervalSeconds: 60,
  filterCookieExpiryDays: 30,
};

// Faze 3 - auto-refresh interval a platnost cookie s filtry jsou konfigurovatelne
// v appsettings.json (sekce ClientSettings), aby je slo menit bez rebuildu SPA.
export function useClientSettings(): ClientSettings {
  const [settings, setSettings] = useState<ClientSettings>(DEFAULT_SETTINGS);

  useEffect(() => {
    let cancelled = false;
    api.clientSettings().then((result) => {
      if (!cancelled) setSettings(result);
    }).catch(() => {
      // appka zustane na vychozich hodnotach (napr. 401 mimo domenu/IIS - viz useMe.ts)
    });
    return () => {
      cancelled = true;
    };
  }, []);

  return settings;
}
