// hooks/useUsers.js
// ─────────────────────────────────────────────────────────────────────────────
// Hook que carga la lista de usuarios una sola vez al montarse.
// Se usa para poblar el selector de usuario en TaskForm sin que el
// componente sepa nada de cómo se obtienen los datos.
// ─────────────────────────────────────────────────────────────────────────────

import { useState, useEffect } from "react";
import { getUsers } from "../api/tasksApi";

export function useUsers() {
  const [users, setUsers]     = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError]     = useState(null);

  useEffect(() => {
    let cancelled = false; // evitar setState si el componente se desmonta antes

    const fetch = async () => {
      setLoading(true);
      try {
        const data = await getUsers();
        if (!cancelled) setUsers(data);
      } catch (err) {
        if (!cancelled) setError(err.message);
      } finally {
        if (!cancelled) setLoading(false);
      }
    };

    fetch();
    return () => { cancelled = true; };
  }, []); // sin dependencias: se ejecuta solo al montar

  return { users, loading, error };
}