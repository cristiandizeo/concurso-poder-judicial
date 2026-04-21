// hooks/useTasks.js
// ─────────────────────────────────────────────────────────────────────────────
// Hook personalizado que encapsula toda la lógica de estado y comunicación
// con la API para las tareas.
//
// Decisión de diseño: mantener este hook como única fuente de verdad evita
// que los componentes manejen estado propio de fetching, reduciendo duplicación
// y facilitando el testing unitario del comportamiento de la UI.
// ─────────────────────────────────────────────────────────────────────────────

import { useState, useEffect, useCallback } from "react";
import { getTasks, createTask, updateTask, deleteTask } from "../api/tasksApi";

/**
 * @param {string|null} statusFilter - Filtro de estado inicial (opcional)
 */
export function useTasks(statusFilter = null) {
  const [tasks, setTasks]     = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError]     = useState(null);

  // ── Fetch ──────────────────────────────────────────────────────────────────
  // useCallback evita que fetchTasks se recree en cada render,
  // lo que provocaría un loop infinito en el useEffect que depende de ella.
  const fetchTasks = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const data = await getTasks(statusFilter);
      setTasks(data);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  }, [statusFilter]);

  // Refrescar cuando cambia el filtro
  useEffect(() => {
    fetchTasks();
  }, [fetchTasks]);

  // ── Mutaciones ────────────────────────────────────────────────────────────

  /** Crea una tarea y actualiza la lista local sin re-fetch completo. */
  const addTask = async (data) => {
    const created = await createTask(data);
    setTasks((prev) => [created, ...prev]);
    return created;
  };

  /** Actualiza una tarea y refleja el cambio en el estado local. */
  const editTask = async (id, data) => {
    const updated = await updateTask(id, data);
    setTasks((prev) => prev.map((t) => (t.id === id ? updated : t)));
    return updated;
  };

  /** Elimina una tarea del estado local tras confirmar el delete en la API. */
  const removeTask = async (id) => {
    await deleteTask(id);
    setTasks((prev) => prev.filter((t) => t.id !== id));
  };

  return { tasks, loading, error, addTask, editTask, removeTask, refetch: fetchTasks };
}