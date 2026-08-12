interface ToggleProps {
  label: string;
  checked: boolean;
  onChange: (checked: boolean) => void;
  disabled?: boolean;
}

// Prepinac odpovidajici puvodnimu Power Apps Toggle (Řazeno, Čeká na přesun,
// V plánu, Přesun - viz Plán sekvencí.pa.yaml) - popisek nad pilulkovym prepinacem.
export default function Toggle({ label, checked, onChange, disabled }: ToggleProps) {
  return (
    <label className="toggle-field">
      <span className="toggle-label">{label}</span>
      <button
        type="button"
        role="switch"
        aria-checked={checked}
        className={"toggle-switch" + (checked ? " on" : "")}
        disabled={disabled}
        onClick={() => onChange(!checked)}
      >
        <span className="toggle-knob" />
      </button>
    </label>
  );
}
