// api/tasksApi.js
// ─────────────────────────────────────────────────────────────────────────────
// Capa de acceso a la API REST.
// Centralizar todas las llamadas HTTP aquí permite:
//   - Cambiar la URL base en un solo lugar
//   - Reutilizar la lógica de fetch en cualquier hook o componente
//   - Facilitar el mockeo en tests
// ─────────────────────────────────────────────────────────────────────────────

const BASE_URL = import.meta.env.VITE_API_URL ?? "https://localhost:5001/api";

/**
 * Wrapper genérico sobre fetch.
 * Lanza un Error con el mensaje del servidor si la respuesta no es OK.
 */
async function request(path, options = {}) {
  const response = await fetch(`${BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({}));
    throw new Error(error.message ?? `Error ${response.status}`);
  }

  // 204 No Content no tiene body
  if (response.status === 204) return null;
  return response.json();
}

// ── Endpoints ────────────────────────────────────────────────────────────────

/** Obtiene todas las tareas. Acepta filtro opcional por estado. */
export const getTasks = (status) =>
  request(`/tasks${status ? `?status=${status}` : ""}`);

/** Obtiene una tarea por ID. */
export const getTaskById = (id) => request(`/tasks/${id}`);

/** Crea una nueva tarea. */
export const createTask = (data) =>
  request("/tasks", { method: "POST", body: JSON.stringify(data) });

/** Actualiza una tarea existente. */
export const updateTask = (id, data) =>
  request(`/tasks/${id}`, { method: "PUT", body: JSON.stringify(data) });

/** Elimina una tarea por ID. */
export const deleteTask = (id) =>
  request(`/tasks/${id}`, { method: "DELETE" });