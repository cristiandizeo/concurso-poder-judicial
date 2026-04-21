using GestorTareas.API.Data;
using GestorTareas.API.Models;
using GestorTareas.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GestorTareas.API.Repositories;

/// <summary>
/// Implementación concreta del repositorio usando Entity Framework Core.
/// Toda la lógica de acceso a datos queda encapsulada aquí,
/// aislada del resto de la aplicación.
/// </summary>
public class TaskRepository(AppDbContext context) : ITaskRepository
{
    private readonly AppDbContext _context = context;

    /// <inheritdoc/>
    public async Task<IEnumerable<TaskItem>> GetAllAsync(string? status = null)
    {
        var query = _context.Tasks.Include(t => t.User).AsQueryable();

        // Filtro opcional por estado. Si no se pasa, devuelve todas.
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(t => t.Status == status);

        return await query.OrderByDescending(t => t.CreatedAt).ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<TaskItem?> GetByIdAsync(int id) =>
        await _context.Tasks.Include(t => t.User).FirstOrDefaultAsync(t => t.Id == id);

    /// <inheritdoc/>
    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        // Cargar la navegación para devolver el usuario en la respuesta
        await _context.Entry(task).Reference(t => t.User).LoadAsync();
        return task;
    }

    /// <inheritdoc/>
    public async Task<TaskItem?> UpdateAsync(int id, TaskItem updatedTask)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task is null) return null;

        // Actualización parcial: solo se sobreescriben los campos que vienen con valor
        if (!string.IsNullOrWhiteSpace(updatedTask.Title))
            task.Title = updatedTask.Title;

        if (updatedTask.Description is not null)
            task.Description = updatedTask.Description;

        if (!string.IsNullOrWhiteSpace(updatedTask.Status))
            task.Status = updatedTask.Status;

        await _context.SaveChangesAsync();
        await _context.Entry(task).Reference(t => t.User).LoadAsync();
        return task;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task is null) return false;

        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        return true;
    }
}