// App.jsx
// Componente raíz de la aplicación.
// Responsabilidades:
//   - Orquestar el estado global (filtro activo, modo creación)
//   - Conectar el hook useTasks con los componentes de presentación
//   - Manejar errores globales de las operaciones mutables

import { useState } from "react";
import { useTasks } from "./hooks/useTasks";
import TaskFilter from "./components/TaskFilter";
import TaskList from "./components/TaskList";
import TaskForm from "./components/TaskForm";

export default function App() {
  const [statusFilter, setStatusFilter] = useState(null);
  const [showForm, setShowForm]         = useState(false);
  const [opError, setOpError]           = useState(null);

  const { tasks, loading, error, addTask, editTask, removeTask } = useTasks(statusFilter);

  // ── Handlers ──────────────────────────────────────────────────────────────

  const handleCreate = async (data) => {
    setOpError(null);
    try {
      await addTask(data);
      setShowForm(false);
    } catch (err) {
      setOpError(err.message);
    }
  };

  const handleEdit = async (id, data) => {
    setOpError(null);
    try {
      await editTask(id, data);
    } catch (err) {
      setOpError(err.message);
    }
  };

  const handleDelete = async (id) => {
    if (!confirm("¿Confirmás que querés eliminar esta tarea?")) return;
    setOpError(null);
    try {
      await removeTask(id);
    } catch (err) {
      setOpError(err.message);
    }
  };

  // ── Render ─────────────────────────────────────────────────────────────────

  return (
    <div style={styles.container}>
      <header style={styles.header}>
        <h1 style={styles.h1}>Gestor de Tareas</h1>
        <button onClick={() => setShowForm((v) => !v)} style={styles.btnNew}>
          {showForm ? "Cancelar" : "+ Nueva tarea"}
        </button>
      </header>

      {/* Formulario de creación (toggle) */}
      {showForm && (
        <div style={styles.formWrapper}>
          <TaskForm onSubmit={handleCreate} onCancel={() => setShowForm(false)} />
        </div>
      )}

      {/* Errores de operaciones mutables (crear, editar, eliminar) */}
      {opError && <p style={styles.error}>⚠ {opError}</p>}

      {/* Filtros */}
      <TaskFilter current={statusFilter} onChange={setStatusFilter} />

      {/* Estados de carga y error de fetch */}
      {loading && <p style={styles.loading}>Cargando tareas...</p>}
      {error   && <p style={styles.error}>Error al cargar tareas: {error}</p>}

      {/* Lista */}
      {!loading && !error && (
        <TaskList tasks={tasks} onEdit={handleEdit} onDelete={handleDelete} />
      )}
    </div>
  );
}

const styles = {
  container:   { maxWidth: "720px", margin: "0 auto", padding: "32px 16px", fontFamily: "system-ui, sans-serif" },
  header:      { display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "24px" },
  h1:          { margin: 0, fontSize: "22px" },
  btnNew:      { padding: "8px 18px", background: "#2563eb", color: "white", border: "none", borderRadius: "6px", cursor: "pointer", fontSize: "14px" },
  formWrapper: { marginBottom: "24px", padding: "20px", border: "1px solid #e5e7eb", borderRadius: "8px", background: "#f9fafb" },
  loading:     { color: "#888", fontSize: "14px" },
  error:       { color: "#dc2626", fontSize: "14px" },
};