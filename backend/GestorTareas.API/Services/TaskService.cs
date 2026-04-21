using GestorTareas.API.DTOs;
using GestorTareas.API.Models;
using GestorTareas.API.Repositories.Interfaces;
using GestorTareas.API.Services.Interfaces;

namespace GestorTareas.API.Services;

/// <summary>
/// Capa de servicio: contiene la lógica de negocio y actúa como
/// intermediario entre el Controller y el Repository.
/// Responsabilidades:
///   - Mapear entre DTOs y modelos de dominio
///   - Aplicar reglas de negocio antes de persistir
///   - Retornar DTOs al controller (nunca modelos internos)
/// </summary>
public class TaskService(ITaskRepository repository) : ITaskService
{
    private readonly ITaskRepository _repository = repository;

    /// <inheritdoc/>
    public async Task<IEnumerable<TaskResponseDto>> GetAllAsync(string? status = null)
    {
        var tasks = await _repository.GetAllAsync(status);
        return tasks.Select(MapToDto);
    }

    /// <inheritdoc/>
    public async Task<TaskResponseDto?> GetByIdAsync(int id)
    {
        var task = await _repository.GetByIdAsync(id);
        return task is null ? null : MapToDto(task);
    }

    /// <inheritdoc/>
    public async Task<TaskResponseDto> CreateAsync(CreateTaskDto dto)
    {
        var task = new TaskItem
        {
            Title       = dto.Title,
            Description = dto.Description,
            Status      = dto.Status,
            UserId      = dto.UserId,
        };

        var created = await _repository.CreateAsync(task);
        return MapToDto(created);
    }

    /// <inheritdoc/>
    public async Task<TaskResponseDto?> UpdateAsync(int id, UpdateTaskDto dto)
    {
        var patch = new TaskItem
        {
            Title       = dto.Title ?? string.Empty,
            Description = dto.Description,
            Status      = dto.Status ?? string.Empty,
        };

        var updated = await _repository.UpdateAsync(id, patch);
        return updated is null ? null : MapToDto(updated);
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id) =>
        await _repository.DeleteAsync(id);

    // ── Helpers privados ───────────────────────────────────────────

    /// <summary>
    /// Mapea un modelo de dominio a un DTO de respuesta.
    /// Centralizar el mapeo aquí evita duplicar lógica en cada método.
    /// </summary>
    private static TaskResponseDto MapToDto(TaskItem task) => new()
    {
        Id          = task.Id,
        Title       = task.Title,
        Description = task.Description,
        Status      = task.Status,
        UserId      = task.UserId,
        UserName    = task.User?.Name ?? string.Empty,
        CreatedAt   = task.CreatedAt,
    };
}