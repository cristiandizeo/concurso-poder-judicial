// components/TaskFilter.jsx
import { TASK_STATUSES } from "../constants/taskStatus";

const ALL_OPTION = { value: null, label: "Todas" };
const OPTIONS    = [ALL_OPTION, ...TASK_STATUSES];

export default function TaskFilter({ current, onChange }) {
  return (
    <div style={styles.bar}>
      {OPTIONS.map(({ value, label }) => (
        <button
          key={label}
          onClick={() => onChange(value)}
          style={{ ...styles.btn, ...(current === value ? styles.active : {}) }}
        >
          {label}
        </button>
      ))}
    </div>
  );
}

const styles = {
  bar:    { display: "flex", gap: "8px", marginBottom: "20px", flexWrap: "wrap" },
  btn:    { padding: "6px 16px", border: "1px solid #ccc", borderRadius: "20px", background: "white", cursor: "pointer", fontSize: "14px" },
  active: { background: "#2563eb", color: "white", borderColor: "#2563eb" },
};