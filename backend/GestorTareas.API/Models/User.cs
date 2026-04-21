namespace GestorTareas.API.Models;

/// <summary>
/// Representa un usuario del sistema.
/// Se mapea a la tabla dbo.Users de la base de datos.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    // Propiedad de navegación: un usuario tiene muchas tareas
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}