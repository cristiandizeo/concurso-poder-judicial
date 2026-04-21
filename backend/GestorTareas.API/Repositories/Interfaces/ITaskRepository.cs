using GestorTareas.API.Models;

namespace GestorTareas.API.Repositories.Interfaces;

/// <summary>
/// Define el contrato de acceso a datos para las tareas.
/// Separar la interfaz de la implementación permite:
///   - Inyectar dependencias sin acoplarse a EF Core
///   - Mockear el repositorio en los tests unitarios
/// </summary>
public interface ITaskRepository
{
    Task<IEnumerable<TaskItem>> GetAllAsync(string? status = null);
    Task<TaskItem?> GetByIdAsync(int id);
    Task<TaskItem> CreateAsync(TaskItem task);
    Task<TaskItem?> UpdateAsync(int id, TaskItem updatedTask);
    Task<bool> DeleteAsync(int id);
}