import type { SequenceDetailHeader } from "../api/types";

interface DetailOperacePanelProps {
  item: string;
  itemDesc: string;
  activeOperId: string;
  header: SequenceDetailHeader | null;
  loading: boolean;
  onClose: () => void;
}

function formatDate(value: string | null): string {
  if (!value) return "";
  return new Date(value).toLocaleDateString("cs-CZ");
}

function round(value: number | null, digits = 0): string {
  if (value === null || value === undefined) return "";
  return value.toFixed(digits);
}

function delayClass(delay: number | null): string {
  if (delay === null || delay === undefined) return "";
  if (delay > 3) return "delay-red";
  if (delay > 1) return "delay-yellow";
  return "";
}

// Panel "Detail operace" (podgalerie nad v_sequences_detail, PRD 5.5) - přehled
// kroků operace pro dané JobSuffix, řazeno vzestupně dle čísla operace. Fixní
// overlay přes celou stránku (jinak by se panel vykreslil jen jako blok v běžném
// dokumentovém toku pod tabulkou a stránkováním a mohl by zůstat mimo viditelnou
// oblast bez scrollu).
export default function DetailOperacePanel({ item, itemDesc, activeOperId, header, loading, onClose }: DetailOperacePanelProps) {
  return (
    <div className="detail-vp-backdrop" onClick={onClose}>
      <div className="detail-vp-overlay" onClick={(e) => e.stopPropagation()}>
        <div className="detail-vp-header">
          <div>
            <span className="detail-vp-label">VP:</span> <strong>{header?.idJobSuffix}</strong>
          </div>
          <div>
            <span className="detail-vp-label">Položka:</span> <strong>{item}</strong> {itemDesc}
          </div>
          <div>
            <span className="detail-vp-label">Začátek VP:</span> {formatDate(header?.jobStartDate ?? null)}
          </div>
          <div>
            <span className="detail-vp-label">Konec VP:</span> {formatDate(header?.jobEndDate ?? null)}
          </div>
          <button type="button" className="icon-button" onClick={onClose} aria-label="Zavřít">
            ✕
          </button>
        </div>

        <div className="table-scroll">
          <table className="workplan-table">
            <thead>
              <tr>
                <th>Operace</th>
                <th className="detail-desc-col">Popis operace</th>
                <th>Přijato [ks]</th>
                <th>Hotovo [ks]</th>
                <th>Zmetky [ks]</th>
                <th>Hod</th>
                <th>Node</th>
                <th>Sequence date</th>
                <th>Delay</th>
              </tr>
            </thead>
            <tbody>
              {loading && (
                <tr>
                  <td colSpan={9} className="empty-row">Načítání…</td>
                </tr>
              )}
              {!loading && header?.steps.map((step) => (
                <tr
                  key={step.idJobSuffixOper}
                  className={step.idJobSuffixOper === activeOperId ? "row-detail-active" : ""}
                >
                  <td className="num" style={step.razeno === 1 ? { backgroundColor: "#ff9d00" } : undefined} title={step.razeno === 1 ? "Řazeno na DS" : undefined}>
                    {step.operNum}
                  </td>
                  <td className="detail-desc-col">{step.operDesc}</td>
                  <td className="num">{round(step.qtyReceived)}</td>
                  <td className="num">{round(step.qtyComplete)}</td>
                  <td className="num">{round(step.qtyScrapped)}</td>
                  <td className="num">{round(step.hod, 2)}</td>
                  <td>{step.node}</td>
                  <td className={"num" + (step.complete === 1 ? " detail-dimmed" : "")}>{formatDate(step.sequenceDateTime)}</td>
                  <td className={"num delay-cell " + delayClass(step.delay)}>{step.delay ? step.delay : ""}</td>
                </tr>
              ))}
              {!loading && header?.steps.length === 0 && (
                <tr>
                  <td colSpan={9} className="empty-row">Žádné kroky operace.</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
