using GestorTareas.API.Data;
using GestorTareas.API.Repositories;
using GestorTareas.API.Repositories.Interfaces;
using GestorTareas.API.Services;
using GestorTareas.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Base de datos ──────────────────────────────────────────────────────────
// La cadena de conexión se lee desde appsettings.json (nunca hardcodeada).
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Inyección de dependencias ──────────────────────────────────────────────
// Registrar interfaces con sus implementaciones concretas.
// Scoped: una instancia por request HTTP (correcto para repositorios con DbContext).
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

// ── Controladores ──────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger / OpenAPI ──────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title   = "Gestor de Tareas API",
        Version = "v1",
        Description = "API REST para gestión de tareas — Prueba técnica STJ La Pampa"
    });

    // Incluir comentarios XML en Swagger (los /// del controller)
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// ── CORS ───────────────────────────────────────────────────────────────────
// Permite que el frontend (Vite en localhost:5173) consuma la API en desarrollo.
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendDev", policy =>
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// ── Middleware pipeline ────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestor de Tareas v1"));
}

app.UseHttpsRedirection();
app.UseCors("FrontendDev");
app.UseAuthorization();
app.MapControllers();

app.Run();