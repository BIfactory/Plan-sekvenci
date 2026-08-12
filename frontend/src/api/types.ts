// Typy odpovídají DTO v backend/src/PlanSekvenci.Api/Dtos (PRD 5.4, 6.4).

export interface WorkplanItem {
  id: number;
  node: string | null;
  idJobSuffix: string | null;
  idJobSuffixOper: string;
  job: string | null;
  suffix: string | null;
  operNum: number | null;
  operDesc: string | null;
  operDescFull: string | null;
  item: string | null;
  itemDesc: string | null;
  itemDescFull: string | null;
  gunFamily: string | null;
  mprio: number | null;
  qtyReceived: number | null;
  qtyComplete: number | null;
  qtyTodo: number | null;
  lastTran: string | null;
  sequenceDateTime: string | null;
  hod: number | null;
  plant: string | null;
  dept: string | null;
  inf: string | null;
  rgid: string | null;
  inPlan: number | null;
  firstTimeInPlan: string | null;
  fixed: boolean;
  waitingToMove: boolean;
  moveTime: string | null;
  selected: boolean;
  delay: number | null;
  razeno: number | null;
  teamLeader: string | null;
  reasonFilled: number | null;
  reasonText: string | null;
  reasonNote: string | null;
  nextRadodOper: string | null;
  nextPresun: number | null;
  operToDo: number | null;
  operGroupColor: string | null;
  rankAll: number | null;
  // Fáze 2 - sloupec Status / tlačítko divergence.
  status: string | null;
  workflowLink: string | null;
  divStatus: number | null;
}

export interface WorkplanListResult {
  items: WorkplanItem[];
  totalCount: number;
}

export interface WorkplanFilters {
  plant?: string;
  dept?: string;
  teamLeader?: string;
  node?: string;
  rgid?: string;
  inf?: string;
  gunFamily?: string;
  inPlan?: number;
  nextPresun?: boolean;
  waitingToMove?: boolean;
  razeno?: boolean;
  jobSuffix?: string;
  item?: string;
  sort?: string;
  sortDir?: "asc" | "desc";
  page?: number;
  pageSize?: number;
}

export interface NodeDefinition {
  node: string;
  plant: string | null;
  dept: string | null;
  teamLeader: string | null;
}

export interface Me {
  userName: string | null;
  isApprover: boolean;
}

export interface LastSync {
  startTime: string | null;
}

// Faze 3 - konfigurace z appsettings.json (sekce ClientSettings), viz GET /api/config.
export interface ClientSettings {
  autoRefreshIntervalSeconds: number;
  filterCookieExpiryDays: number;
}

// Panel "Aktuální data" na obrazovce Plán sekvencí.
export interface Capacity {
  used: number | null;
  total: number | null;
  mode: "rgid" | "node" | "nocap";
}

export interface NodeSummary {
  hodPlanSkluz: number | null;
  hodPlan: number | null;
  hodAll: number | null;
  delaySum: number | null;
  producedTotal: number | null;
  producedStatus1Or3: number | null;
  producedStatus1: number | null;
  isOnlinePlan: boolean | null;
  capacity: Capacity | null;
}

// Panel "Historie" (Plnění plánu / Výkon).
export interface SaHistoryEntry {
  date: string | null;
  dateDesc: string | null;
  sa: number | null;
  performance: number | null;
}

export interface ProducedItem {
  id: number;
  date: string | null;
  idJobSuffixOper: string | null;
  item: string | null;
  qtyTodo: number | null;
  qtyDone: number | null;
  hodPlan: number | null;
  hodProduced: number | null;
  node: string | null;
  produced: number | null;
  sequenceDateTime: string | null;
  status: number | null;
  reason: string | null;
  reasonNote: string | null;
  reasonLastUpdateTime: string | null;
  reasonLastUpdateBy: string | null;
  reasonEnabled: number | null;
  rgid: string | null;
  selectedOper: number | null;
}

// Panel "Vyhodnocení nad plán nebo mimo plán" (druhý grid, PRD 6.5 souvislosti).
export interface OverPlanItem {
  id: number;
  idJobSuffixOper: string | null;
  item: string | null;
  status: number | null;
  statusDesc: string | null;
  sequenceDateTime: string | null;
  hodProduced: number | null;
  qtyDone: number | null;
}

// Panel "Vyhodnocení plánu:" + "Graf vyhodnocení".
export interface ProducedSummary {
  planHod: number | null;
  producedInPlanHod: number | null;
  producedOverPlanHod: number | null;
  producedOutOfPlanHod: number | null;
  capacityHod: number | null;
  sa: number | null;
  performance: number | null;
}

// Fáze 2 (PRD 5.5) - podgalerie otevirane z gridu "Plán sekvencí".

export interface SequenceDetailItem {
  idJobSuffixOper: string | null;
  operNum: number | null;
  operDesc: string | null;
  qtyReceived: number | null;
  qtyComplete: number | null;
  qtyScrapped: number | null;
  hod: number | null;
  node: string | null;
  delay: number | null;
  sequenceDateTime: string | null;
  complete: number | null;
  razeno: number | null;
}

export interface SequenceDetailHeader {
  idJobSuffix: string | null;
  jobStartDate: string | null;
  jobEndDate: string | null;
  steps: SequenceDetailItem[];
}

export interface SerialNumberItem {
  serNum: string;
  item: string | null;
}

export interface DivergenceItem {
  divNum: string;
  workflowLink: string | null;
}

export interface XSuffixItem {
  xJobSuffix: string | null;
  idJobSuffixOper: string | null;
}
