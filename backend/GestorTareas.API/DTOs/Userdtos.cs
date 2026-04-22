namespace GestorTareas.API.DTOs;

/// <summary>
/// DTO de salida para usuarios.
/// Solo expone los campos necesarios para poblar el selector del frontend.
/// </summary>
public class UserResponseDto
{
    public int    Id    { get; set; }
    public string Name  { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}