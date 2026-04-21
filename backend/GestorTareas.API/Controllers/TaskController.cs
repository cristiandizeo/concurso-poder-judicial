using GestorTareas.API.DTOs;
using GestorTareas.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GestorTareas.API.Controllers;

/// <summary>
/// Controller de tareas. Expone los endpoints REST de la API.
/// Responsabilidades: recibir requests HTTP, delegar al servicio,
/// y retornar la respuesta con el código HTTP correcto.
/// NO contiene lógica de negocio ni acceso a datos.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class TasksController(ITaskService service) : ControllerBase
{
    private readonly ITaskService _service = service;

    /// <summary>
    /// Lista todas las tareas. Acepta filtro opcional por estado.
    /// </summary>
    /// <param name="status">Filtro por estado: pending | in_progress | completed</param>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? status = null)
    {
        var tasks = await _service.GetAllAsync(status);
        return Ok(tasks);
    }

    /// <summary>
    /// Obtiene una tarea por su ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _service.GetByIdAsync(id);
        return task is null ? NotFound(new { message = $"Tarea con id {id} no encontrada." }) : Ok(task);
    }

    /// <summary>
    /// Crea una nueva tarea.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        // ModelState es validado automáticamente por [ApiController]
        // Si el DTO tiene errores de validación, retorna 400 antes de llegar aquí
        var created = await _service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Actualiza una tarea existente (actualización parcial).
    /// Solo se modifican los campos incluidos en el body.
    /// </summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTaskDto dto)
    {
        var updated = await _service.UpdateAsync(id, dto);
        return updated is null ? NotFound(new { message = $"Tarea con id {id} no encontrada." }) : Ok(updated);
    }

    /// <summary>
    /// Elimina una tarea por su ID.
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound(new { message = $"Tarea con id {id} no encontrada." });
    }
}