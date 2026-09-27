import { useRef } from "react";
import { useClickOutside } from "../hooks/useClickOutside";

interface SideListPanelProps {
  title: string;
  items: string[];
  loading: boolean;
  emptyText: string;
  onClose: () => void;
}

// Boční panel s jednoduchým seznamem (Sériová čísla / X-suffix, PRD 5.5). Zavira se
// krizkem i kliknutim kamkoliv mimo panel (stejne jako Detail operace).
export default function SideListPanel({ title, items, loading, emptyText, onClose }: SideListPanelProps) {
  const panelRef = useRef<HTMLElement>(null);
  useClickOutside(panelRef, onClose);

  return (
    <aside className="side-list-panel" ref={panelRef}>
      <div className="side-list-header">
        <h3>{title}</h3>
        <button type="button" className="icon-button" onClick={onClose} aria-label="Zavřít">
          ✕
        </button>
      </div>
      <ul className="side-list">
        {loading && <li className="side-list-empty">Načítání…</li>}
        {!loading && items.length === 0 && <li className="side-list-empty">{emptyText}</li>}
        {!loading && items.map((item, i) => <li key={i}>{item}</li>)}
      </ul>
    </aside>
  );
}
