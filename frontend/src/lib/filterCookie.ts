const COOKIE_NAME = "ps_node_filters";

// Fallback, pokud volajici nepredhodi platnost z konfigurace (viz
// hooks/useClientSettings.ts, appsettings.json - ClientSettings.FilterCookieExpiryDays).
const DEFAULT_EXPIRY_DAYS = 30;

// Sdilene mezi obrazovkami Plan sekvenci a Vyhodnoceni - uzivatel chce mit stejne
// nastaveni Provoz/Dilna/Team Leader/Uzel/Skupina zdroju pri prechodu mezi strankami
// i po vikendu/dovolene (viz pozadavek: cookie s dlouhou platnosti, konfigurovatelnou
// v appsettings.json).
export interface PersistedNodeFilters {
  plant: string;
  dept: string;
  teamLeader: string;
  node: string;
  rgid: string;
}

const EMPTY: PersistedNodeFilters = { plant: "", dept: "", teamLeader: "", node: "", rgid: "" };

export function readPersistedNodeFilters(): PersistedNodeFilters {
  const match = document.cookie.match(new RegExp(`(?:^|; )${COOKIE_NAME}=([^;]*)`));
  if (!match) return { ...EMPTY };
  try {
    const parsed = JSON.parse(decodeURIComponent(match[1]));
    return {
      plant: typeof parsed.plant === "string" ? parsed.plant : "",
      dept: typeof parsed.dept === "string" ? parsed.dept : "",
      teamLeader: typeof parsed.teamLeader === "string" ? parsed.teamLeader : "",
      node: typeof parsed.node === "string" ? parsed.node : "",
      rgid: typeof parsed.rgid === "string" ? parsed.rgid : "",
    };
  } catch {
    return { ...EMPTY };
  }
}

export function writePersistedNodeFilters(filters: PersistedNodeFilters, expiryDays: number = DEFAULT_EXPIRY_DAYS): void {
  const value = encodeURIComponent(JSON.stringify(filters));
  const maxAgeSeconds = expiryDays * 24 * 60 * 60;
  document.cookie = `${COOKIE_NAME}=${value}; max-age=${maxAgeSeconds}; path=/; SameSite=Lax`;
}
