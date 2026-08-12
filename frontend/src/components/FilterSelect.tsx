interface FilterSelectProps {
  label: string;
  value: string;
  options: string[];
  onChange: (value: string) => void;
  disabled?: boolean;
  disabledHint?: string;
}

// Filtr-dropdown s malym tlacitkem pro vymazani (viz Icon7/Icon7_1/... v
// Plán sekvencí.pa.yaml) - popisek nad selectem.
export default function FilterSelect({ label, value, options, onChange, disabled, disabledHint }: FilterSelectProps) {
  return (
    <label className="filter-select-field" title={disabled ? disabledHint : undefined}>
      <span className="filter-select-label">
        {label}
        {value && (
          <button type="button" className="filter-clear-button" onClick={() => onChange("")} aria-label={`Vymazat ${label}`}>
            ✕
          </button>
        )}
      </span>
      <select value={value} onChange={(e) => onChange(e.target.value)} disabled={disabled}>
        <option value="">Vše</option>
        {options.map((v) => (
          <option key={v} value={v}>
            {v}
          </option>
        ))}
      </select>
    </label>
  );
}
