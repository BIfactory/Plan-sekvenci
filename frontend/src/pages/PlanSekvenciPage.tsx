import { useCallback, useEffect, useMemo, useRef, useState } from "react";
import { createPortal } from "react-dom";
import { api } from "../api/client";
import type {
  DivergenceItem,
  NodeDefinition,
  NodeSummary,
  SaHistoryEntry,
  SequenceDetailHeader,
  WorkplanItem,
} from "../api/types";
import { useMe } from "../hooks/useMe";
import { useStaleDataWarning } from "../hooks/useStaleDataWarning";
import { useAutoRefresh } from "../hooks/useAutoRefresh";
import { useClientSettings } from "../hooks/useClientSettings";
import { useNavSlot } from "../hooks/useNavSlot";
import { usePersistedNodeFilters } from "../hooks/usePersistedNodeFilters";
import RefreshBar from "../components/RefreshBar";
import ReasonPanel from "../components/ReasonPanel";
import Toggle from "../components/Toggle";
import FilterSelect from "../components/FilterSelect";
import DetailOperacePanel from "../components/DetailOperacePanel";
import SideListPanel from "../components/SideListPanel";
import DivergencePopup from "../components/DivergencePopup";

const DIVERGENCE_MULTI_STATUS = "více odchylek";
const STATUS_DISABLED = new Set(["waiting", "new", "uncompleted", "out"]);

const PAGE_SIZE = 100;

function distinctSorted(values: (string | null | undefined)[]): string[] {
  return Array.from(new Set(values.filter((v): v is string => !!v))).sort((a, b) => a.localeCompare(b));
}

export default function PlanSekvenciPage() {
  const me = useMe();
  const stale = useStaleDataWarning();
  const clientSettings = useClientSettings();
  const navSlot = useNavSlot();

  const [nodes, setNodes] = useState<NodeDefinition[]>([]);
  const [reasons, setReasons] = useState<string[]>([]);
  const [lastSync, setLastSync] = useState<string | null>(null);
  const [nodeSummary, setNodeSummary] = useState<NodeSummary | null>(null);
  const [saHistory, setSaHistory] = useState<SaHistoryEntry[]>([]);

  const { plant, setPlant, dept, setDept, teamLeader, setTeamLeader, node, setNode, rgid, setRgid } =
    usePersistedNodeFilters(clientSettings.filterCookieExpiryDays);
  const [inf, setInf] = useState("");
  const [gunFamily, setGunFamily] = useState("");
  const [inPlanOnly, setInPlanOnly] = useState(false);
  const [nextPresun, setNextPresun] = useState(false);
  const [waitingToMove, setWaitingToMove] = useState(false);
  const [razeno, setRazeno] = useState(false);
  const [jobSuffix, setJobSuffix] = useState("");
  const [item, setItem] = useState("");
  const [sort, setSort] = useState("");
  const [sortDir, setSortDir] = useState<"asc" | "desc">("desc");
  // Nekonecny seznam (bez strankovani v UI) - "page" jen rika, kolik davek uz je
  // nactenych a prirustaji se k sobe (viz handleLoadMore); pri zmene filtru/razeni
  // se resetuje na 1 a seznam se nahradi od zacatku.
  const [page, setPage] = useState(1);

  const [rgidOptions, setRgidOptions] = useState<string[]>([]);
  const [infOptions, setInfOptions] = useState<string[]>([]);
  const [gunFamilyOptions, setGunFamilyOptions] = useState<string[]>([]);

  const [items, setItems] = useState<WorkplanItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(false);
  const [loadingMore, setLoadingMore] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const tableScrollRef = useRef<HTMLDivElement>(null);

  const [reasonTarget, setReasonTarget] = useState<WorkplanItem | null>(null);

  // Fáze 2 (PRD 5.5) - podgalerie Detail operace / Sériová čísla / Divergence / X-suffix.
  const [detailRow, setDetailRow] = useState<WorkplanItem | null>(null);
  const [detailHeader, setDetailHeader] = useState<SequenceDetailHeader | null>(null);
  const [detailLoading, setDetailLoading] = useState(false);

  const [serialTarget, setSerialTarget] = useState<WorkplanItem | null>(null);
  const [serialNumbers, setSerialNumbers] = useState<string[]>([]);
  const [serialLoading, setSerialLoading] = useState(false);

  const [xSuffixTarget, setXSuffixTarget] = useState<WorkplanItem | null>(null);
  const [xSuffixItems, setXSuffixItems] = useState<string[]>([]);
  const [xSuffixLoading, setXSuffixLoading] = useState(false);

  const [divergenceRow, setDivergenceRow] = useState<WorkplanItem | null>(null);
  const [divergenceItems, setDivergenceItems] = useState<DivergenceItem[]>([]);
  const [divergenceLoading, setDivergenceLoading] = useState(false);

  const filterEnabled = plant !== "" || dept !== "" || node !== "";

  // Filtry/razeni bez "page" - zmena kterehokoliv z nich znamena novy vysledek
  // (viz efekt nize), ne pokracovani stavajiciho nekonecneho seznamu.
  const baseFilters = useMemo(
    () => ({
      plant: plant || undefined,
      dept: dept || undefined,
      teamLeader: teamLeader || undefined,
      node: node || undefined,
      rgid: rgid || undefined,
      inf: inf || undefined,
      gunFamily: gunFamily || undefined,
      inPlan: inPlanOnly ? 1 : undefined,
      nextPresun: nextPresun || undefined,
      waitingToMove: waitingToMove || undefined,
      razeno: razeno || undefined,
      jobSuffix: jobSuffix || undefined,
      item: item || undefined,
      sort: sort || undefined,
      sortDir: sort ? sortDir : undefined,
    }),
    [plant, dept, teamLeader, node, rgid, inf, gunFamily, inPlanOnly, nextPresun, waitingToMove, razeno, jobSuffix, item, sort, sortDir],
  );

  // Nacte danou davku - "replace" nahradi seznam od zacatku (novy filtr/razeni nebo
  // rucni refresh), "append" prida dalsi davku na konec (nekonecny scroll).
  const loadPage = useCallback(
    (pageToLoad: number, mode: "replace" | "append") => {
      if (mode === "replace") setLoading(true);
      else setLoadingMore(true);
      setError(null);
      return api
        .workplan({ ...baseFilters, page: pageToLoad, pageSize: PAGE_SIZE })
        .then((r) => {
          setItems((prev) => (mode === "replace" ? r.items : [...prev, ...r.items]));
          setTotalCount(r.totalCount);
          setPage(pageToLoad);
        })
        .catch((e) => setError(e instanceof Error ? e.message : String(e)))
        .finally(() => {
          setLoading(false);
          setLoadingMore(false);
        });
    },
    [baseFilters],
  );

  const loadWorkplan = useCallback(() => {
    loadPage(1, "replace");
  }, [loadPage]);

  const loadNodeSummary = useCallback(() => {
    if (!node) {
      setNodeSummary(null);
      return;
    }
    api.nodeSummary(node, rgid || undefined).then(setNodeSummary);
  }, [node, rgid]);

  const loadSaHistory = useCallback(() => {
    if (!node) {
      setSaHistory([]);
      return;
    }
    api.saHistory(node).then(setSaHistory);
  }, [node]);

  useEffect(() => {
    api.nodes().then(setNodes);
    api.reasons().then(setReasons);
    api.lastSync().then((r) => setLastSync(r.startTime));
  }, []);

  useEffect(() => {
    loadWorkplan();
  }, [loadWorkplan]);

  useEffect(() => {
    loadNodeSummary();
  }, [loadNodeSummary]);

  useEffect(() => {
    loadSaHistory();
  }, [loadSaHistory]);

  useEffect(() => {
    api.workplanRgids({ plant: plant || undefined, dept: dept || undefined, node: node || undefined, teamLeader: teamLeader || undefined }).then(setRgidOptions);
  }, [plant, dept, node, teamLeader]);

  useEffect(() => {
    api.workplanInfs().then(setInfOptions);
    api.workplanGunFamilies().then(setGunFamilyOptions);
  }, []);

  const plantOptions = useMemo(() => distinctSorted(nodes.map((n) => n.plant)), [nodes]);
  const deptOptions = useMemo(
    () => distinctSorted(nodes.filter((n) => !plant || n.plant === plant).map((n) => n.dept)),
    [nodes, plant],
  );
  const teamLeaderOptions = useMemo(
    () => distinctSorted(nodes.filter((n) => (!plant || n.plant === plant) && (!dept || n.dept === dept)).map((n) => n.teamLeader)),
    [nodes, plant, dept],
  );
  const nodeOptions = useMemo(
    () =>
      distinctSorted(
        nodes
          .filter((n) => (!plant || n.plant === plant) && (!dept || n.dept === dept) && (!teamLeader || n.teamLeader === teamLeader))
          .map((n) => n.node),
      ),
    [nodes, plant, dept, teamLeader],
  );

  const handleRefresh = () => {
    loadWorkplan();
    loadNodeSummary();
    loadSaHistory();
    api.lastSync().then((r) => setLastSync(r.startTime));
    stale.markRefreshed();
  };

  // Fáze 3 (PRD 2, 8.1) - automaticky background refresh misto (puvodne) vyhradne
  // rucniho tlacitka; tlacitko + 30min upozorneni na neaktualni data zustavaji jako
  // fallback (viz useStaleDataWarning). Interval z appsettings.json (useClientSettings).
  useAutoRefresh(handleRefresh, clientSettings.autoRefreshIntervalSeconds * 1000);

  // Nekonecny seznam misto strankovani - dalsi davka se dotahne, kdyz uzivatel
  // doscrolluje blizko ke spodku .table-scroll (viz onScroll na tom divu nize).
  const handleLoadMore = useCallback(() => {
    if (loading || loadingMore) return;
    if (items.length >= totalCount) return;
    loadPage(page + 1, "append");
  }, [loading, loadingMore, items.length, totalCount, page, loadPage]);

  const handleTableScroll = () => {
    const el = tableScrollRef.current;
    if (!el) return;
    if (el.scrollHeight - el.scrollTop - el.clientHeight < 200) {
      handleLoadMore();
    }
  };

  // Klik na sloupec: prvni klik seradi sestupne, dalsi klik na stejny sloupec
  // prehodi na vzestupne a zpet (bez navratu do vychoziho neseřazeneho stavu).
  const handleToggleSort = (column: string) => {
    if (sort !== column) {
      setSort(column);
      setSortDir("desc");
    } else {
      setSortDir((current) => (current === "desc" ? "asc" : "desc"));
    }
  };

  const handleToggleFixed = async (row: WorkplanItem) => {
    try {
      await api.setFixed(row.idJobSuffixOper, !row.fixed);
      loadWorkplan();
    } catch (e) {
      alert(e instanceof Error ? e.message : String(e));
    }
  };

  const handleToggleSelected = async (row: WorkplanItem) => {
    try {
      await api.setSelected(row.idJobSuffixOper, !row.selected);
      loadWorkplan();
    } catch (e) {
      alert(e instanceof Error ? e.message : String(e));
    }
  };

  // Fáze 2 (PRD 5.5): otevre panel "Detail operace" nad v_sequences_detail pro dane JobSuffix.
  const openDetail = (row: WorkplanItem) => {
    if (!row.idJobSuffix) return;
    setDetailRow(row);
    setDetailLoading(true);
    api.workplanDetail(row.idJobSuffix)
      .then(setDetailHeader)
      .finally(() => setDetailLoading(false));
  };

  // Fáze 2 (PRD 5.5): otevre boční panel "Sériová čísla" nad t_serial_numbers.
  const openSerialNumbers = (row: WorkplanItem) => {
    setSerialTarget(row);
    setSerialLoading(true);
    api.workplanSerialNumbers(row.idJobSuffixOper)
      .then((items) => setSerialNumbers(items.map((i) => i.serNum)))
      .finally(() => setSerialLoading(false));
  };

  // Fáze 2 (PRD 5.5): otevre boční panel "X-suffix (návaznost)" nad v_x_suffix.
  const openXSuffix = (row: WorkplanItem) => {
    setXSuffixTarget(row);
    setXSuffixLoading(true);
    api.workplanXSuffix(row.idJobSuffixOper)
      .then((items) => setXSuffixItems(items.map((i) => i.xJobSuffix ?? "").filter(Boolean)))
      .finally(() => setXSuffixLoading(false));
  };

  // Fáze 2 (PRD 5.5): tlačítko Status - vice odchylek otevre popup, jinak proklik na workflow_link.
  const handleStatusClick = (row: WorkplanItem) => {
    if (row.status === DIVERGENCE_MULTI_STATUS) {
      setDivergenceRow(row);
      setDivergenceLoading(true);
      const suffixNum = Number(row.suffix);
      if (row.job && Number.isFinite(suffixNum)) {
        api.workplanDivergence(row.job, suffixNum)
          .then(setDivergenceItems)
          .finally(() => setDivergenceLoading(false));
      } else {
        setDivergenceItems([]);
        setDivergenceLoading(false);
      }
    } else if (row.workflowLink) {
      window.open(row.workflowLink, "_blank", "noopener,noreferrer");
    }
  };

  const rowClass = (row: WorkplanItem) => {
    if ((row.nextPresun === 1 || row.operToDo === 0) && row.waitingToMove) return "row-move-blue";
    if (row.waitingToMove) return "row-move-orange";
    return "";
  };

  const capacityText = formatCapacity(nodeSummary);
  const planTypeText = nodeSummary?.isOnlinePlan === null || nodeSummary?.isOnlinePlan === undefined
    ? ""
    : nodeSummary.isOnlinePlan
      ? "Online plán"
      : "Fixní";

  return (
    <section className="plan-page">
      {navSlot && createPortal(
        <RefreshBar
          lastSync={lastSync}
          onRefresh={handleRefresh}
          showWarning={stale.showWarning}
          refreshing={loading}
        />,
        navSlot,
      )}
      <div className="ps-header-grid">
        <div className="ps-card ps-filters-card">
          <div className="ps-title-search-row">
            <h1>Plán sekvencí</h1>
            <label className="search-field">
              <span className="search-field-label">
                🔍 Položka
                {item && (
                  <button type="button" className="filter-clear-button" onClick={() => setItem("")} aria-label="Vymazat Položka">
                    ✕
                  </button>
                )}
              </span>
              <input type="text" value={item} onChange={(e) => setItem(e.target.value)} />
            </label>
            <label className="search-field">
              <span className="search-field-label">
                🔍 JobSuffix
                {jobSuffix && (
                  <button type="button" className="filter-clear-button" onClick={() => setJobSuffix("")} aria-label="Vymazat JobSuffix">
                    ✕
                  </button>
                )}
              </span>
              <input type="text" value={jobSuffix} onChange={(e) => setJobSuffix(e.target.value)} />
            </label>
            <div className="ps-toggle-row">
              <Toggle label="Řazeno" checked={razeno} onChange={setRazeno} />
              <Toggle label="Čeká na přesun" checked={waitingToMove} onChange={setWaitingToMove} />
              <Toggle label="V plánu" checked={inPlanOnly} onChange={setInPlanOnly} />
              <Toggle label="Přesun" checked={nextPresun} onChange={setNextPresun} />
            </div>
          </div>

          <div className="ps-filter-grid">
            <FilterSelect label="Provoz" value={plant} options={plantOptions} onChange={setPlant} />
            <FilterSelect label="Team Leader" value={teamLeader} options={teamLeaderOptions} onChange={setTeamLeader} />
            <FilterSelect label="Inf" value={inf} options={infOptions} onChange={setInf} disabled={!filterEnabled} disabledHint="Filtr je dostupný, pokud je vybrán alespoň provoz" />
            <FilterSelect label="Produktová rodina" value={gunFamily} options={gunFamilyOptions} onChange={setGunFamily} disabled={!filterEnabled} disabledHint="Filtr je dostupný, pokud je vybrán alespoň provoz" />
            <FilterSelect label="Dílna" value={dept} options={deptOptions} onChange={setDept} />
            <FilterSelect label="Uzel" value={node} options={nodeOptions} onChange={setNode} />
            <FilterSelect label="Skupina zdrojů" value={rgid} options={rgidOptions} onChange={setRgid} />
          </div>
        </div>

        <div className="ps-card ps-summary-card">
          <div className="ps-card-header">
            <h2>Aktuální data</h2>
          </div>
          <div className="ps-summary-grid">
            <ArrowCombo colors={["pink", "orange"]} value={formatHod(nodeSummary?.hodPlanSkluz)} />
            <div className="ps-summary-cell">
              <span className="ps-summary-label">Odvedeno celkem</span>
              <span>: {formatHod(nodeSummary?.producedTotal)}</span>
            </div>
            <ArrowCombo colors={["pink"]} value={formatHod(nodeSummary?.hodPlan)} />
            <ArrowCombo colors={["pink", "orange"]} value={formatHod(nodeSummary?.producedStatus1Or3)} />
            <div className="ps-summary-cell">
              <span className="ps-summary-label">Celková zásoba</span>
              <span className="ps-summary-muted">: {formatHod(nodeSummary?.hodAll)}</span>
            </div>
            <ArrowCombo colors={["pink"]} value={formatHod(nodeSummary?.producedStatus1)} />
          </div>
          <div className="ps-summary-footer">
            <span>Kapacita: {capacityText}</span>
            <span>Suma delay: {nodeSummary?.delaySum ?? ""}</span>
          </div>
          <div className="ps-summary-plantype">
            <span>Typ plánu</span>
            <span className="ps-summary-muted">{planTypeText}</span>
          </div>
        </div>

        <div className="ps-card ps-history-card">
          <h2>Historie</h2>
          <table className="history-table">
            <thead>
              <tr>
                <th></th>
                <th>Plnění plánu</th>
                <th>Výkon</th>
              </tr>
            </thead>
            <tbody>
              {saHistory.map((row) => (
                <tr key={row.date}>
                  <td>{row.dateDesc}</td>
                  <td className={pctClass(row.sa, 0.5, 0.95)}>
                    {formatPct(row.sa)} {row.sa !== null && row.sa >= 0.93 && "🙂"}
                  </td>
                  <td className={pctClass(row.performance, 0.5, 0.85)}>
                    {formatPct(row.performance)} {row.performance !== null && row.performance >= 0.85 && "🙂"}
                  </td>
                </tr>
              ))}
              {saHistory.length === 0 && (
                <tr>
                  <td colSpan={3} className="empty-row">—</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>

      {error && <p className="reason-error">{error}</p>}

      <div className="table-scroll" ref={tableScrollRef} onScroll={handleTableScroll}>
        <table className="workplan-table">
          <thead>
            <tr>
              <th>Fixovat</th>
              <th className="sortable" onClick={() => handleToggleSort("rank_all")}>
                Plán <SortIcon active={sort === "rank_all"} dir={sortDir} />
              </th>
              <th>JobSuffix</th>
              <th>Inf</th>
              <th>Položka</th>
              <th>Popis</th>
              <th className="sortable" onClick={() => handleToggleSort("oper_num")}>
                Operace <SortIcon active={sort === "oper_num"} dir={sortDir} />
              </th>
              <th>Hod</th>
              <th>Popis operace</th>
              <th>Přijato [ks]</th>
              <th>Hotovo [ks]</th>
              <th>Zbývá [ks]</th>
              <th>Sequence date</th>
              <th className="sortable" onClick={() => handleToggleSort("delay")}>
                Delay [dny] <SortIcon active={sort === "delay"} dir={sortDir} />
              </th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            {items.map((row) => (
              <tr key={row.idJobSuffixOper} className={rowClass(row)}>
                <td className="center fixovat-cell">
                  {row.mprio !== null && row.mprio >= 100 && row.mprio <= 110 && <span className="mprio-flag">P</span>}
                  {row.inPlan === 1 && !row.waitingToMove && (
                    <input
                      type="checkbox"
                      checked={row.fixed}
                      disabled={!me?.isApprover}
                      onChange={() => handleToggleFixed(row)}
                    />
                  )}
                </td>
                <td className="center">
                  {/* Tlacitko prepinajici "selected" musi byt klikatelne i kdyz sipka
                      neni zobrazena (row.inPlan neni 1/2) - viz pozadavek na sjednocenou
                      klikaci plochu ve sloupci Plán. */}
                  <button
                    type="button"
                    className={
                      "icon-button plan-arrow " +
                      (row.inPlan === 1 ? "plan-arrow-pink" : row.inPlan === 2 ? "plan-arrow-orange" : "plan-arrow-empty")
                    }
                    title="Přepnout výběr"
                    onClick={() => handleToggleSelected(row)}
                  >
                    {(row.inPlan === 1 || row.inPlan === 2) ? "➜" : ""}
                  </button>
                </td>
                <td className={row.selected ? "text-selected" : ""}>
                  {/* Detail operace a JobSuffix jsou samostatne blokove (div) elementy,
                      ne button+span - jinak trojklik na text JobSuffix oznaci i tlacitko
                      vedle nej (bezici jako jedna "odstavcova" jednotka pro selekci). */}
                  <div className="jobsuffix-cell-row">
                    <div className="doc-icon-wrap">
                      <button type="button" className="doc-icon-button" title="Detail operace" onClick={() => openDetail(row)}>📄</button>
                    </div>
                    <div className="jobsuffix-link" title="Sériová čísla" onClick={() => openSerialNumbers(row)}>
                      {row.idJobSuffix}
                    </div>
                    {row.razeno === 1 && (
                      <div className="razeno-flag" title={`DS: ${row.nextRadodOper ?? ""}`}>T</div>
                    )}
                  </div>
                </td>
                <td>{row.inf}</td>
                <td className="item-link" title="X-suffix (návaznost)" onClick={() => openXSuffix(row)}>{row.item}</td>
                <td title={row.itemDescFull ?? undefined}>{row.itemDesc}</td>
                <td
                  className="oper-num-cell"
                  style={row.operGroupColor ? { backgroundColor: fade(row.operGroupColor) } : undefined}
                  title={`SZ: ${row.rgid ?? ""}`}
                  onClick={() => row.inPlan === 1 && setReasonTarget(row)}
                >
                  {row.operNum}
                </td>
                <td className="num">{round(row.hod, 2)}</td>
                <td title={row.operDescFull ?? undefined}>{row.operDesc}</td>
                <td className="num">{round(row.qtyReceived)}</td>
                <td className="num" title={row.lastTran ? `Poslední transakce na vp: ${row.lastTran}` : undefined}>
                  {round(row.qtyComplete)}
                </td>
                <td className="num">{round(row.qtyTodo)}</td>
                <td
                  className="num"
                  title={row.firstTimeInPlan ? `Poprvé v plánu: ${row.firstTimeInPlan}` : undefined}
                >
                  {formatDate(row.sequenceDateTime)}
                </td>
                <td className={"num delay-cell " + delayClass(row.delay, row.sequenceDateTime)} title={row.moveTime ? `Čas přesunu: ${row.moveTime}` : undefined}>
                  {row.delay && row.delay > 0 ? row.delay : ""}
                </td>
                <td className="center">
                  {row.status && (
                    <button
                      type="button"
                      className={"status-button " + statusButtonClass(row.divStatus)}
                      disabled={STATUS_DISABLED.has(row.status)}
                      onClick={() => handleStatusClick(row)}
                    >
                      {row.status}
                    </button>
                  )}
                </td>
              </tr>
            ))}
            {!loading && items.length === 0 && (
              <tr>
                <td colSpan={15} className="empty-row">Žádná data pro zvolené filtry.</td>
              </tr>
            )}
            {loadingMore && (
              <tr>
                <td colSpan={15} className="empty-row">Načítám další záznamy…</td>
              </tr>
            )}
          </tbody>
        </table>
      </div>

      <div className="result-count">{totalCount} záznamů{items.length < totalCount ? ` (zobrazeno ${items.length})` : ""}</div>

      {reasonTarget && (
        <ReasonPanel
          title="Zadání důvodu neplnění"
          targetLabel={reasonTarget.idJobSuffixOper}
          reasons={reasons}
          disabled={!me?.isApprover}
          disabledHint="Zápis důvodu je dostupný jen pro approvery."
          onSubmit={async (reason, note) => {
            await api.submitWorkplanReason(reasonTarget.idJobSuffixOper, reasonTarget.node ?? undefined, reason, note);
            loadWorkplan();
          }}
          onClose={() => setReasonTarget(null)}
        />
      )}

      {detailRow && (
        <DetailOperacePanel
          item={detailRow.item ?? ""}
          itemDesc={detailRow.itemDesc ?? ""}
          activeOperId={detailRow.idJobSuffixOper}
          header={detailHeader}
          loading={detailLoading}
          onClose={() => setDetailRow(null)}
        />
      )}

      {serialTarget && (
        <SideListPanel
          title="Sériová čísla"
          items={serialNumbers}
          loading={serialLoading}
          emptyText="Žádná sériová čísla."
          onClose={() => setSerialTarget(null)}
        />
      )}

      {xSuffixTarget && (
        <SideListPanel
          title="X-suffix (návaznost)"
          items={xSuffixItems}
          loading={xSuffixLoading}
          emptyText="Žádné navazující suffixy."
          onClose={() => setXSuffixTarget(null)}
        />
      )}

      {divergenceRow && (
        <DivergencePopup
          items={divergenceItems}
          loading={divergenceLoading}
          onClose={() => setDivergenceRow(null)}
        />
      )}
    </section>
  );
}

// Barva tlacitka Status podle div_status (1=oranzova, 2=cervena) - viz PRD 5.5.
function statusButtonClass(divStatus: number | null): string {
  if (divStatus === 1) return "status-button-orange";
  if (divStatus === 2) return "status-button-red";
  return "";
}

// Serazeni sloupce: sedá šipka jako neaktivní stav, po kliknutí černá šipka dolů
// (sestupně), dalším kliknutím černá šipka nahoru (vzestupně) - viz handleToggleSort.
function SortIcon({ active, dir }: { active: boolean; dir: "asc" | "desc" }) {
  if (!active) {
    return <span className="sort-icon sort-icon-inactive">⇅</span>;
  }
  return <span className="sort-icon sort-icon-active">{dir === "desc" ? "▼" : "▲"}</span>;
}

function ArrowCombo({ colors, value }: { colors: ("pink" | "orange")[]; value: string }) {
  return (
    <div className="ps-summary-cell">
      <span className="ps-arrow-combo">
        {colors.map((c, i) => (
          <span key={i} className={"arrow-icon arrow-" + c}>➜</span>
        ))}
      </span>
      <span>: {value}</span>
    </div>
  );
}

function formatHod(value: number | null | undefined): string {
  if (value === null || value === undefined || value <= 0) return "";
  return value.toFixed(2) + " hod";
}

function formatPct(value: number | null): string {
  if (value === null || value === undefined) return "";
  return (value * 100).toFixed(2).replace(".", ",") + "%";
}

function pctClass(value: number | null, lowThreshold: number, highThreshold: number): string {
  if (value === null || value === undefined) return "";
  if (value <= lowThreshold) return "pct-red";
  if (value < highThreshold) return "pct-yellow";
  return "pct-green";
}

function formatCapacity(summary: NodeSummary | null): string {
  const cap = summary?.capacity;
  if (!cap) return "";
  const used = cap.used !== null && cap.used !== undefined ? cap.used.toFixed(1) : "";
  if (cap.mode === "nocap" || cap.total === null || cap.total === undefined) {
    return `${used} hod`;
  }
  return `${used} / ${cap.total.toFixed(1)} hod`;
}

function round(value: number | null, digits = 0): string {
  if (value === null || value === undefined) return "";
  return value.toFixed(digits);
}

function formatDate(value: string | null): string {
  if (!value) return "";
  const d = new Date(value);
  return d.toLocaleDateString("cs-CZ");
}

// Barevne pozadi bunky Delay se pouzije jen u polozek, jejichz Sequence date uz je
// v minulosti (< dnesek) - u budoucich datumu zustava bunka bez barvy bez ohledu na
// hodnotu delay (upresneno uzivatelem, puvodni appka toto rozliseni nemela).
function delayClass(delay: number | null, sequenceDateTime: string | null): string {
  if (delay === null || delay === undefined) return "";
  if (!isBeforeToday(sequenceDateTime)) return "";
  if (delay > 3) return "delay-red";
  if (delay > 1) return "delay-yellow";
  return "";
}

function isBeforeToday(dateStr: string | null): boolean {
  if (!dateStr) return false;
  const d = new Date(dateStr);
  if (Number.isNaN(d.getTime())) return false;
  const dateOnly = new Date(d.getFullYear(), d.getMonth(), d.getDate());
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return dateOnly < today;
}

function fade(hex: string): string {
  // Priblizna nahrada Power Fx ColorFade(..., 0.7) - zesvetleni barvy pro pozadi bunky.
  const c = hex.replace("#", "");
  if (c.length !== 6) return hex;
  const r = parseInt(c.slice(0, 2), 16);
  const g = parseInt(c.slice(2, 4), 16);
  const b = parseInt(c.slice(4, 6), 16);
  const mix = (channel: number) => Math.round(channel + (255 - channel) * 0.7);
  return `rgb(${mix(r)}, ${mix(g)}, ${mix(b)})`;
}
