// constants/taskStatus.js
// ─────────────────────────────────────────────────────────────────────────────
// Fuente única de verdad para los estados de tarea.
// Centralizar aquí evita que cada componente defina sus propias etiquetas
// y garantiza consistencia visual en toda la app.
// ─────────────────────────────────────────────────────────────────────────────

export const TASK_STATUSES = [
  { value: "pending",     label: "Pendiente",   color: "#f59e0b" },
  { value: "in_progress", label: "En progreso", color: "#3b82f6" },
  { value: "completed",   label: "Completada",  color: "#10b981" },
];

/** Devuelve la etiqueta en español para un valor de estado dado. */
export const getStatusLabel = (value) =>
  TASK_STATUSES.find((s) => s.value === value)?.label ?? value;

/** Devuelve el color asociado a un estado dado. */
export const getStatusColor = (value) =>
  TASK_STATUSES.find((s) => s.value === value)?.color ?? "#888";