using GestorTareas.API.Data;
using GestorTareas.API.Repositories;
using GestorTareas.API.Repositories.Interfaces;
using GestorTareas.API.Services;
using GestorTareas.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Base de datos ──────────────────────────────────────────────────────────
// En producción (Render) la connection string llega por variable de entorno.
// En desarrollo local se usa appsettings.json.
var connectionString =
    Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ── Inyección de dependencias ──────────────────────────────────────────────
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

// ── Controladores ──────────────────────────────────────────────────────────
builder.Services.AddControllers();

// ── Swagger ────────────────────────────────────────────────────────────────
// Activo en todos los entornos para que los jueces puedan explorar la API.
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
// FRONTEND_URL se configura como variable de entorno en Render una vez
// que el frontend esté desplegado. En desarrollo acepta Vite (5173).
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

// ── Migraciones automáticas al iniciar ────────────────────────────────────
// Garantiza que la base de datos en Supabase esté siempre sincronizada
// con el modelo sin intervención manual en cada deploy.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ── Middleware ─────────────────────────────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Gestor de Tareas v1"));

// Render maneja HTTPS en el proxy; la app interna corre en HTTP.
// UseHttpsRedirection causaría loops en ese contexto, se omite en producción.
if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors("Frontend");
app.UseAuthorization();
app.MapControllers();

app.Run();