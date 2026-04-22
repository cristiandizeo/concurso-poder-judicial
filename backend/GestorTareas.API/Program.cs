using GestorTareas.API.Data;
using GestorTareas.API.Repositories;
using GestorTareas.API.Repositories.Interfaces;
using GestorTareas.API.Services;
using GestorTareas.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Base de datos ──────────────────────────────────────────────────────────
// Supabase provee DATABASE_URL en formato URI: postgres://user:pass@host:port/db
// Npgsql espera formato key-value, por lo que convertimos si es necesario.
var rawConnectionString =
    Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontró cadena de conexión.");

var connectionString = ConvertToNpgsqlFormat(rawConnectionString);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ── Inyección de dependencias ──────────────────────────────────────────────
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

// ── Controladores ──────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger ────────────────────────────────────────────────────────────────
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title       = "Gestor de Tareas API",
        Version     = "v1",
        Description = "API REST para gestión de tareas — Prueba técnica STJ La Pampa"
    });

    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
        options.IncludeXmlComments(xmlPath);
});

// ── CORS ───────────────────────────────────────────────────────────────────
var frontendUrl = Environment.GetEnvironmentVariable("FRONTEND_URL")
                  ?? "http://localhost:5173";

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

// ── Migraciones automáticas ────────────────────────────────────────────────
// Solo aplica migraciones pendientes; si las tablas ya existen en Supabase
// y la migración inicial fue marcada como aplicada, esto no hace nada.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ── Middleware ─────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestor de Tareas v1"));

if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();

app.Run();

// ── Helpers ───────────────────────────────────────────────────────────────

/// <summary>
/// Convierte una URI de PostgreSQL (postgres://user:pass@host:port/db)
/// al formato key-value que espera Npgsql.
/// Si ya está en formato key-value, la devuelve sin cambios.
/// </summary>
static string ConvertToNpgsqlFormat(string connectionString)
{
    if (!connectionString.StartsWith("postgres://") &&
        !connectionString.StartsWith("postgresql://"))
        return connectionString;

    var uri      = new Uri(connectionString);
    var userInfo = uri.UserInfo.Split(':');
    var user     = userInfo[0];
    var password = userInfo.Length > 1 ? userInfo[1] : "";
    var host     = uri.Host;
    var port     = uri.Port > 0 ? uri.Port : 5432;
    var database = uri.AbsolutePath.TrimStart('/');

    return $"Host={host};Port={port};Database={database};Username={user};Password={password};SSL Mode=Require;Trust Server Certificate=true";
}