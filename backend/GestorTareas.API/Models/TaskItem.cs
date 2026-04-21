namespace GestorTareas.API.Models;

/// <summary>
/// Representa una tarea del sistema.
/// Se mapea a la tabla dbo.Tasks de la base de datos.
/// Nota: se llama TaskItem para evitar conflicto con System.Threading.Tasks.Task.
/// </summary>
public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }

    /// <summary>
    /// Estado de la tarea. Valores posibles: pending | in_progress | completed
    /// </summary>
    public string Status { get; set; } = "pending";

    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Propiedad de navegación
    public User? User { get; set; }
}