// components/TaskForm.jsx
// Formulario para crear y editar tareas.
// En modo creación carga los usuarios desde la API para mostrar un selector.
// En modo edición no se permite cambiar el usuario asignado.

import { useState } from "react";
import { TASK_STATUSES } from "../constants/taskStatus";
import { useUsers } from "../hooks/useUsers";

const EMPTY_FORM = { title: "", description: "", status: "pending", userId: "" };

export default function TaskForm({ initialData = null, onSubmit, onCancel }) {
  const [form, setForm]         = useState(initialData ?? EMPTY_FORM);
  const [submitting, setSubmitting] = useState(false);
  const [error, setError]       = useState(null);

  // Solo cargamos usuarios en modo creación
  const isEditing               = Boolean(initialData);
  const { users, loading: loadingUsers } = useUsers();

  const handleChange = (e) =>
    setForm((prev) => ({ ...prev, [e.target.name]: e.target.value }));

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    setError(null);
    try {
      await onSubmit({ ...form, userId: Number(form.userId) });
    } catch (err) {
      setError(err.message);
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit} style={styles.form}>
      <h3 style={styles.title}>{isEditing ? "Editar tarea" : "Nueva tarea"}</h3>

      {error && <p style={styles.error}>{error}</p>}

      <label style={styles.label}>
        Título *
        <input
          name="title"
          value={form.title}
          onChange={handleChange}
          required
          maxLength={200}
          style={styles.input}
        />
      </label>

      <label style={styles.label}>
        Descripción
        <textarea
          name="description"
          value={form.description}
          onChange={handleChange}
          maxLength={1000}
          rows={3}
          style={styles.input}
        />
      </label>

      <label style={styles.label}>
        Estado *
        <select name="status" value={form.status} onChange={handleChange} style={styles.input}>
          {TASK_STATUSES.map(({ value, label }) => (
            <option key={value} value={value}>{label}</option>
          ))}
        </select>
      </label>

      {/* Selector de usuario — solo en modo creación */}
      {!isEditing && (
        <label style={styles.label}>
          Usuario *
          <select
            name="userId"
            value={form.userId}
            onChange={handleChange}
            required
            disabled={loadingUsers}
            style={styles.input}
          >
            <option value="">
              {loadingUsers ? "Cargando usuarios..." : "— Seleccioná un usuario —"}
            </option>
            {users.map((u) => (
              <option key={u.id} value={u.id}>
                {u.name} ({u.email})
              </option>
            ))}
          </select>
        </label>
      )}

      <div style={styles.actions}>
        <button type="submit" disabled={submitting} style={styles.btnPrimary}>
          {submitting ? "Guardando..." : isEditing ? "Guardar cambios" : "Crear tarea"}
        </button>
        {onCancel && (
          <button type="button" onClick={onCancel} style={styles.btnSecondary}>
            Cancelar
          </button>
        )}
      </div>
    </form>
  );
}

const styles = {
  form:         { display: "flex", flexDirection: "column", gap: "12px", maxWidth: "480px" },
  title:        { margin: "0 0 4px", fontSize: "16px" },
  label:        { display: "flex", flexDirection: "column", gap: "4px", fontSize: "14px" },
  input:        { padding: "8px", border: "1px solid #ccc", borderRadius: "6px", fontSize: "14px" },
  error:        { color: "#dc2626", fontSize: "13px", margin: 0 },
  actions:      { display: "flex", gap: "8px", marginTop: "4px" },
  btnPrimary:   { padding: "8px 20px", background: "#2563eb", color: "white", border: "none", borderRadius: "6px", cursor: "pointer", fontSize: "14px" },
  btnSecondary: { padding: "8px 20px", background: "white", border: "1px solid #ccc", borderRadius: "6px", cursor: "pointer", fontSize: "14px" },
};