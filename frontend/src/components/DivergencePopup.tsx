import type { DivergenceItem } from "../api/types";

interface DivergencePopupProps {
  items: DivergenceItem[];
  loading: boolean;
  onClose: () => void;
}

// Popup "Seznam odchylek" (podgalerie nad t_divergence_all, PRD 5.5) - otevírá se
// z tlačítka Status, když je jich pro danou job/suffix víc.
export default function DivergencePopup({ items, loading, onClose }: DivergencePopupProps) {
  return (
    <div className="divergence-backdrop" onClick={onClose}>
      <div className="divergence-popup" onClick={(e) => e.stopPropagation()}>
        <div className="divergence-header">
          <h3>Seznam odchylek:</h3>
          <button type="button" className="icon-button" onClick={onClose} aria-label="Zavřít">
            ✕
          </button>
        </div>
        <ul className="divergence-list">
          {loading && <li className="side-list-empty">Načítání…</li>}
          {!loading && items.length === 0 && <li className="side-list-empty">Žádné odchylky.</li>}
          {!loading && items.map((d, i) => (
            <li key={i}>
              <span className="divergence-num">{d.divNum}</span>
              {d.workflowLink ? (
                <a href={d.workflowLink} target="_blank" rel="noreferrer">{d.workflowLink}</a>
              ) : (
                <span className="ps-summary-muted">bez odkazu</span>
              )}
            </li>
          ))}
        </ul>
      </div>
    </div>
  );
}
