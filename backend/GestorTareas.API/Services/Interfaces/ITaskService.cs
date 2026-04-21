using GestorTareas.API.DTOs;

namespace GestorTareas.API.Services.Interfaces;

/// <summary>
/// Define el contrato de la capa de negocio para las tareas.
/// El servicio trabaja con DTOs (no con modelos internos),
/// manteniendo al controller completamente desacoplado de EF Core.
/// </summary>
public interface ITaskService
{
    Task<IEnumerable<TaskResponseDto>> GetAllAsync(string? status = null);
    Task<TaskResponseDto?> GetByIdAsync(int id);
    Task<TaskResponseDto> CreateAsync(CreateTaskDto dto);
    Task<TaskResponseDto?> UpdateAsync(int id, UpdateTaskDto dto);
    Task<bool> DeleteAsync(int id);
}