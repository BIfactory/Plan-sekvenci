# CLAUDE.md – frontend

> Platí navíc ke kořenovému `CLAUDE.md` (ten se načítá automaticky). Sem patří jen to,
> co je specifické pro frontend a neopakuje se jinde.

## Stack a struktura projektu

- Vite + React 18 + TypeScript, `react-router-dom` pro 2 routy (`/plan-sekvenci`,
  `/vyhodnoceni`), `base: "/plan_sekvenci/"` ve `vite.config.ts` (appka běží pod
  podcestou na IIS) a `basename="/plan_sekvenci"` v `BrowserRouter` (main.tsx).
- **Žádná UI komponentová knihovna** (žádné MUI/AntD) – čisté HTML prvky + vlastní CSS
  v `src/index.css`. Držet se toho, ať projekt zůstává lehký; pokud přibude potřeba
  něčeho složitějšího (např. virtualizovaný grid pro výkon), řešit cíleně, ne zavádět
  celou knihovnu kvůli jedné komponentě.
- Struktura: `src/api/` (typy + fetch klient), `src/hooks/` (`useMe`,
  `useStaleDataWarning`, `useAutoRefresh`), `src/components/` (sdílené kusy UI –
  `ReasonPanel`, `RefreshBar`), `src/pages/` (`PlanSekvenciPage`, `VyhodnoceniPage` –
  hlavní logika obrazovek, zatím bez dalšího rozpadu na menší komponenty).

## Konvence psaní kódu

- Typy v `src/api/types.ts` odpovídají 1:1 backendovým DTO (`Dtos/*.cs`) včetně
  camelCase názvů – při změně DTO na backendu je potřeba ručně promítnout sem
  (žádné generování klienta z OpenAPI zatím není zapojené).
- Žádný globální state management (Redux/Zustand) – stav filtrů a dat žije lokálně
  v `useState`/`useMemo` uvnitř stránek. Zvážit React Query až při reálné potřebě
  (cache invalidation, retry) – pro Fázi 1 je to zbytečná komplexita.

## Komunikace s API

- `src/api/client.ts` – `fetch` s `credentials: "include"` (nutné pro Windows
  Authentication v prohlížeči). Vite dev server proxuje `/api` na
  `http://localhost:5280` (viz `vite.config.ts`), takže frontend vždy volá relativní
  `/api/...`, nikdy absolutní URL na backend.
- Chyby: `client.ts` hodí `Error` se zprávou z `ProblemDetails.title`, pokud je k
  dispozici; stránky ji zobrazí v `<p class="reason-error">`. `AuthController`/`useMe`
  chybu nezobrazuje (401 bez Windows Auth kontextu je očekávaný stav mimo
  doménu/IIS – necháváme `me` na `null`, editační prvky se pak chovají jako pro
  neschváleného uživatele).
- Server je autoritativní zdroj pravdy pro `isApprover` – frontend skrývá/disabluje
  ovládací prvky podle `/api/auth/me`, ale skutečné vynucení je vždy na backendu
  (403 při zápisu bez oprávnění).

## UI/UX

- Cíl: zachovat rozložení a chování blízké originální Power App (PRD sekce 2) – filtry
  nahoře, grid pod nimi, barevné kódování delay (1–3 žlutá, >3 červená), panel
  "Zadání důvodu neplnění" jako postranní panel (`ReasonPanel`), refresh
  s 30min upozorněním na neaktuální data + 5min cooldown tlačítka (`useStaleDataWarning`,
  PRD 5.5). Pixel-přesná shoda se zdrojovým Power Fx layoutem není cíl (viz PRD 2 –
  prostor pro moderní úpravy je povolený, pokud se nezmění vnímané chování).
- **Fáze 3 – automatický background refresh** (PRD 2, 8.1): `useAutoRefresh` (polling,
  ne SignalR – zvoleno kvůli jednoduchosti nasazení na IIS bez závislosti na
  WebSockets modulu) volá stejný `handleRefresh`, jaký spouští ruční tlačítko, jednou
  za nakonfigurovaný interval, přeskočí tik, pokud je tab na pozadí (`document.hidden`).
  Ruční tlačítko Refresh i 30min stale warning zůstávají zachované jako fallback
  (vědomé rozhodnutí – ne doslovná náhrada, viz PRD 2 "místo ručního tlačítka") –
  auto-refresh markRefreshed() volá po každém tiku, takže stale warning v běžném
  provozu nenaskočí, ale zůstává jako signál, kdyby auto-refresh dlouhodobě selhával
  (např. výpadek sítě).
- **Konfigurace z backendu** (`useClientSettings`, `GET /api/config`): auto-refresh
  interval (`autoRefreshIntervalSeconds`) a platnost cookie s filtry
  (`filterCookieExpiryDays`) se načítají z `appsettings.json` (backend sekce
  `ClientSettings`), ne jsou natvrdo v kódu – jde je změnit editací configu bez
  rebuildu frontendu. `useClientSettings` má lokální fallback (60 s / 30 dní) pro dobu
  než se fetch vrátí (nebo pokud selže, stejně jako `useMe`) – stránky proto vždy mají
  rozumnou hodnotu i před prvním načtením configu. `usePersistedNodeFilters(days)` a
  `writePersistedNodeFilters(filters, days)` (`lib/filterCookie.ts`) přijímají platnost
  jako parametr místo natvrdo zapsané konstanty.
- Server-side stránkování (100 řádků/stránka) na obrazovce Plán sekvencí je odchylka
  od originálu (ten stránkoval kvůli limitům Power Apps a pak filtroval na klientovi)
  – viz `backend/CLAUDE.md`.

## Testování

- Zatím žádné automatizované testy (Fáze 1 MVP). `npm run lint` (ESLint) a
  `tsc -b` (typecheck) jsou jediná automatická kontrola.

## Časté příkazy

- `npm run dev` – Vite dev server na `:5173` (proxy `/api` → `:5280`).
- `npm run build` – `tsc -b && vite build` (typecheck musí projít, jinak build spadne).
- `npm run lint` – ESLint.

## Známé problémy / rozhodnutí

- V tomto vývojovém prostředí `node`/`npm`/`npx` nejsou v PATH (jsou v
  `C:\Program Files\nodejs`) – při spouštění přes obyčejný shell je potřeba buď plná
  cesta, nebo `PATH` dočasně rozšířit.
- Filtry Provoz/Dílna/Team Leader/Uzel se počítají na klientovi z `/api/reference/nodes`
  (malý číselník, stejně jako v originále přes `Dim_NodesDef`), zatímco RGID/Inf/
  Produktová rodina se počítají na serveru (`/api/workplan/rgids|infs|gun-families`),
  protože `v_workplan_input` může být velká tabulka (PRD 8.1) – neobracet tento poměr
  bez dobrého důvodu.
