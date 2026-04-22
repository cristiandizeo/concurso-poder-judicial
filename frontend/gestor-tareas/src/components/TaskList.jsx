// components/TaskList.jsx
// Renderiza la lista de tareas con acciones de editar y eliminar.
// Recibe los datos y callbacks desde App (a través del hook useTasks).
// No maneja estado propio ni lógica de negocio: solo presentación.

import { useState } from "react";
import TaskForm from "./TaskForm";

const STATUS_LABELS = {
  pending:     { label: "Pendiente",   color: "#f59e0b" },
  in_progress: { label: "En progreso", color: "#3b82f6" },
  completed:   { label: "Completada",  color: "#10b981" },
};

export default function TaskList({ tasks, onEdit, onDelete }) {
  // ID de la tarea que está siendo editada en este momento (null = ninguna)
  const [editingId, setEditingId] = useState(null);

  if (tasks.length === 0)
    return <p style={{ color: "#888", fontSize: "14px" }}>No hay tareas para mostrar.</p>;

  const handleEdit = async (id, data) => {
    await onEdit(id, data);
    setEditingId(null);
  };

  return (
    <ul style={styles.list}>
      {tasks.map((task) => {
        const statusInfo = STATUS_LABELS[task.status] ?? { label: task.status, color: "#888" };

        return (
          <li key={task.id} style={styles.card}>
            {editingId === task.id ? (
              // Modo edición inline: reemplaza la card por el formulario
              <TaskForm
                initialData={{
                  title:       task.title,
                  description: task.description ?? "",
                  status:      task.status,
                  userId:      task.userId,
                }}
                onSubmit={(data) => handleEdit(task.id, data)}
                onCancel={() => setEditingId(null)}
              />
            ) : (
              <>
                <div style={styles.cardHeader}>
                  <span style={styles.taskTitle}>{task.title}</span>
                  <span style={{ ...styles.badge, background: statusInfo.color }}>
                    {statusInfo.label}
                  </span>
                </div>

                {task.description && (
                  <p style={styles.description}>{task.description}</p>
                )}

                <div style={styles.meta}>
                  <span>👤 {task.userName}</span>
                  <span>{new Date(task.createdAt).toLocaleDateString("es-AR")}</span>
                </div>

                <div style={styles.actions}>
                  <button onClick={() => setEditingId(task.id)} style={styles.btnEdit}>
                    Editar
                  </button>
                  <button onClick={() => onDelete(task.id)} style={styles.btnDelete}>
                    Eliminar
                  </button>
                </div>
              </>
            )}
          </li>
        );
      })}
    </ul>
  );
}

const styles = {
  list:        { listStyle: "none", padding: 0, margin: 0, display: "flex", flexDirection: "column", gap: "12px" },
  card:        { border: "1px solid #e5e7eb", borderRadius: "8px", padding: "16px", background: "white" },
  cardHeader:  { display: "flex", justifyContent: "space-between", alignItems: "center", marginBottom: "6px" },
  taskTitle:   { fontWeight: 600, fontSize: "15px" },
  badge:       { color: "white", fontSize: "12px", padding: "2px 10px", borderRadius: "12px" },
  description: { fontSize: "14px", color: "#555", margin: "4px 0 8px" },
  meta:        { fontSize: "12px", color: "#888", display: "flex", gap: "16px", marginBottom: "10px" },
  actions:     { display: "flex", gap: "8px" },
  btnEdit:     { padding: "4px 14px", fontSize: "13px", border: "1px solid #2563eb", color: "#2563eb", background: "white", borderRadius: "6px", cursor: "pointer" },
  btnDelete:   { padding: "4px 14px", fontSize: "13px", border: "1px solid #dc2626", color: "#dc2626", background: "white", borderRadius: "6px", cursor: "pointer" },
};