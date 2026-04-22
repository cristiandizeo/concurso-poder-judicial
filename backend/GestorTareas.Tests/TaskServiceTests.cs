// TaskServiceTests.cs
// ─────────────────────────────────────────────────────────────────────────────
// Tests unitarios para TaskService.
//
// Estrategia: mockear ITaskRepository con Moq para aislar el servicio
// de la base de datos. Así los tests son rápidos, deterministas y no
// requieren infraestructura externa.
//
// Cada test sigue el patrón AAA (Arrange / Act / Assert), que hace
// explícita la intención de cada sección del test.
// ─────────────────────────────────────────────────────────────────────────────

using GestorTareas.API.DTOs;
using GestorTareas.API.Models;
using GestorTareas.API.Repositories.Interfaces;
using GestorTareas.API.Services;
using Moq;
using Xunit;

namespace GestorTareas.Tests;

public class TaskServiceTests
{
    // Mock compartido entre todos los tests de esta clase
    private readonly Mock<ITaskRepository> _repoMock = new();
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        // El servicio recibe el mock como dependencia (inyección de dependencias)
        _service = new TaskService(_repoMock.Object);
    }

    // ── GetAllAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task GetAllAsync_SinFiltro_RetornaTodosLosDtos()
    {
        // Arrange
        var fakeTasks = new List<TaskItem>
        {
            new() { Id = 1, Title = "Tarea A", Status = "pending",     UserId = 1, User = new User { Name = "Ana" } },
            new() { Id = 2, Title = "Tarea B", Status = "completed",   UserId = 2, User = new User { Name = "Carlos" } },
        };
        _repoMock.Setup(r => r.GetAllAsync(null)).ReturnsAsync(fakeTasks);

        // Act
        var result = (await _service.GetAllAsync()).ToList();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Tarea A", result[0].Title);
        Assert.Equal("Ana",     result[0].UserName);
    }

    [Fact]
    public async Task GetAllAsync_ConFiltroEstado_PasaElFiltroAlRepositorio()
    {
        // Arrange
        var fakeTasks = new List<TaskItem>
        {
            new() { Id = 3, Title = "Tarea C", Status = "in_progress", UserId = 1, User = new User { Name = "Ana" } },
        };
        _repoMock.Setup(r => r.GetAllAsync("in_progress")).ReturnsAsync(fakeTasks);

        // Act
        var result = (await _service.GetAllAsync("in_progress")).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal("in_progress", result[0].Status);
        // Verificar que el repositorio fue llamado con el filtro correcto
        _repoMock.Verify(r => r.GetAllAsync("in_progress"), Times.Once);
    }

    // ── GetByIdAsync ──────────────────────────────────────────────────────────

    [Fact]
    public async Task GetByIdAsync_IdExistente_RetornaDto()
    {
        // Arrange
        var fakeTask = new TaskItem { Id = 1, Title = "Tarea X", Status = "pending", UserId = 1, User = new User { Name = "Ana" } };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(fakeTask);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1,         result.Id);
        Assert.Equal("Tarea X", result.Title);
    }

    [Fact]
    public async Task GetByIdAsync_IdInexistente_RetornaNull()
    {
        // Arrange
        _repoMock.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((TaskItem?)null);

        // Act
        var result = await _service.GetByIdAsync(99);

        // Assert
        Assert.Null(result);
    }

    // ── CreateAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task CreateAsync_DatosValidos_RetornaDtoCreado()
    {
        // Arrange
        var dto = new CreateTaskDto { Title = "Nueva tarea", Status = "pending", UserId = 1 };
        var savedTask = new TaskItem { Id = 5, Title = "Nueva tarea", Status = "pending", UserId = 1, User = new User { Name = "Ana" } };

        // Simulamos que el repositorio persiste y devuelve la tarea con ID asignado
        _repoMock
            .Setup(r => r.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(savedTask);

        // Act
        var result = await _service.CreateAsync(dto);

        // Assert
        Assert.Equal(5,             result.Id);
        Assert.Equal("Nueva tarea", result.Title);
        Assert.Equal("pending",     result.Status);
    }

    // ── DeleteAsync ───────────────────────────────────────────────────────────

    [Fact]
    public async Task DeleteAsync_IdExistente_RetornaTrue()
    {
        // Arrange
        _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteAsync_IdInexistente_RetornaFalse()
    {
        // Arrange
        _repoMock.Setup(r => r.DeleteAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _service.DeleteAsync(99);

        // Assert
        Assert.False(result);
    }
}