// api/tasksApi.js
const BASE_URL = import.meta.env.VITE_API_URL ?? "https://localhost:5001/api";

async function request(path, options = {}) {
  const response = await fetch(`${BASE_URL}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });

  if (!response.ok) {
    const error = await response.json().catch(() => ({}));
    throw new Error(error.message ?? `Error ${response.status}`);
  }

  if (response.status === 204) return null;
  return response.json();
}

// ── Tareas ───────────────────────────────────────────────────────────────────
export const getTasks    = (status) => request(`/tasks${status ? `?status=${status}` : ""}`);
export const getTaskById = (id)     => request(`/tasks/${id}`);
export const createTask  = (data)   => request("/tasks", { method: "POST", body: JSON.stringify(data) });
export const updateTask  = (id, data) => request(`/tasks/${id}`, { method: "PUT", body: JSON.stringify(data) });
export const deleteTask  = (id)     => request(`/tasks/${id}`, { method: "DELETE" });

// ── Usuarios ─────────────────────────────────────────────────────────────────
/** Obtiene todos los usuarios para poblar selectores en el frontend. */
export const getUsers = () => request("/users");