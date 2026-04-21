// components/TaskFilter.jsx
// Barra de filtros por estado. Componente sin estado propio:
// recibe el estado activo y notifica cambios al padre (App).

const STATUSES = [
  { value: null,         label: "Todas"       },
  { value: "pending",    label: "Pendientes"  },
  { value: "in_progress",label: "En progreso" },
  { value: "completed",  label: "Completadas" },
];

export default function TaskFilter({ current, onChange }) {
  return (
    <div style={styles.bar}>
      {STATUSES.map(({ value, label }) => (
        <button
          key={label}
          onClick={() => onChange(value)}
          style={{
            ...styles.btn,
            ...(current === value ? styles.active : {}),
          }}
        >
          {label}
        </button>
      ))}
    </div>
  );
}

const styles = {
  bar: {
    display: "flex",
    gap: "8px",
    marginBottom: "20px",
    flexWrap: "wrap",
  },
  btn: {
    padding: "6px 16px",
    border: "1px solid #ccc",
    borderRadius: "20px",
    background: "white",
    cursor: "pointer",
    fontSize: "14px",
  },
  active: {
    background: "#2563eb",
    color: "white",
    borderColor: "#2563eb",
  },
};