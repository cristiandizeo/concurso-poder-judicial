using System.ComponentModel.DataAnnotations;

namespace GestorTareas.API.DTOs;

/// <summary>
/// DTO de salida: lo que la API devuelve al cliente cuando consulta tareas.
/// Expone solo los campos necesarios, sin exponer el modelo interno.
/// </summary>
public class TaskResponseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Status { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// DTO de entrada para crear una nueva tarea (POST /api/tasks).
/// Las validaciones con DataAnnotations se evalúan automáticamente
/// por el model binding de ASP.NET Core antes de llegar al controller.
/// </summary>
public class CreateTaskDto
{
    [Required(ErrorMessage = "El título es obligatorio.")]
    [MaxLength(200, ErrorMessage = "El título no puede superar los 200 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000, ErrorMessage = "La descripción no puede superar los 1000 caracteres.")]
    public string? Description { get; set; }

    [Required(ErrorMessage = "El estado es obligatorio.")]
    [RegularExpression("^(pending|in_progress|completed)$",
        ErrorMessage = "Estado inválido. Valores permitidos: pending, in_progress, completed.")]
    public string Status { get; set; } = "pending";

    [Required(ErrorMessage = "El id de usuario es obligatorio.")]
    public int UserId { get; set; }
}

/// <summary>
/// DTO de entrada para actualizar una tarea existente (PUT /api/tasks/{id}).
/// Todos los campos son opcionales: se actualiza solo lo que se envía.
/// </summary>
public class UpdateTaskDto
{
    [MaxLength(200)]
    public string? Title { get; set; }

    [MaxLength(1000)]
    public string? Description { get; set; }

    [RegularExpression("^(pending|in_progress|completed)$",
        ErrorMessage = "Estado inválido. Valores permitidos: pending, in_progress, completed.")]
    public string? Status { get; set; }
}