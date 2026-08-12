import { useState } from "react";

interface ReasonPanelProps {
  title: string;
  targetLabel: string;
  reasons: string[];
  disabled?: boolean;
  disabledHint?: string;
  onSubmit: (reason: string, note: string) => Promise<void>;
  onClose: () => void;
}

// Panel "Zadani duvodu neplneni" (PRD 5.5 / 6.6) - dropdown s duvody + poznamka.
export default function ReasonPanel({
  title,
  targetLabel,
  reasons,
  disabled,
  disabledHint,
  onSubmit,
  onClose,
}: ReasonPanelProps) {
  const [reason, setReason] = useState("");
  const [note, setNote] = useState("");
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const canSubmit = !disabled && !submitting && targetLabel.length > 0;

  const handleSubmit = async () => {
    setSubmitting(true);
    setError(null);
    try {
      await onSubmit(reason, note);
      setReason("");
      setNote("");
      onClose();
    } catch (e) {
      setError(e instanceof Error ? e.message : String(e));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <aside className="reason-panel">
      <div className="reason-panel-header">
        <h3>{title}</h3>
        <button type="button" className="icon-button" onClick={onClose} aria-label="Zavřít">
          ✕
        </button>
      </div>

      <label className="reason-field">
        <span>JobSuffixOper:</span>
        <div className="reason-readonly">{targetLabel}</div>
      </label>

      <label className="reason-field">
        <span>Důvod:</span>
        <select value={reason} onChange={(e) => setReason(e.target.value)}>
          <option value="">Vyber důvod</option>
          {reasons.filter((r) => r !== "").map((r) => (
            <option key={r} value={r}>
              {r}
            </option>
          ))}
        </select>
      </label>

      <label className="reason-field">
        <span>Poznámka:</span>
        <textarea
          value={note}
          onChange={(e) => setNote(e.target.value)}
          placeholder="Místo pro poznámku"
          rows={3}
        />
      </label>

      {disabled && disabledHint && <p className="reason-hint">{disabledHint}</p>}
      {error && <p className="reason-error">{error}</p>}

      <button type="button" className="primary-button" disabled={!canSubmit} onClick={handleSubmit}>
        Uložit
      </button>
    </aside>
  );
}
