using GestorTareas.API.Data;
using GestorTareas.API.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestorTareas.API.Controllers;

/// <summary>
/// Controller de usuarios.
/// Por ahora expone solo el listado para poblar selectores en el frontend.
/// La gestión completa de usuarios (CRUD) queda fuera del alcance de esta prueba.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class UsersController(AppDbContext context) : ControllerBase
{
    private readonly AppDbContext _context = context;

    /// <summary>
    /// Lista todos los usuarios disponibles.
    /// Usado por el frontend para poblar el selector al crear una tarea.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _context.Users
            .OrderBy(u => u.Name)
            .Select(u => new UserResponseDto
            {
                Id    = u.Id,
                Name  = u.Name,
                Email = u.Email,
            })
            .ToListAsync();

        return Ok(users);
    }
}