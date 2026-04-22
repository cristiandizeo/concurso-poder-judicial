using System.Net;
using System.Net.Sockets;
using GestorTareas.API.Data;
using GestorTareas.API.Repositories;
using GestorTareas.API.Repositories.Interfaces;
using GestorTareas.API.Services;
using GestorTareas.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ── Base de datos ──────────────────────────────────────────────────────────
// Supabase resuelve a IPv6 por defecto; Render no tiene conectividad IPv6.
// Solución: resolver el hostname a su IP IPv4 antes de construir la cadena
// de conexión, para que Npgsql nunca intente conectarse por IPv6.
var connectionString =
    Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontró cadena de conexión.");

connectionString = ResolveToIPv4(connectionString);

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
/// Resuelve el hostname en la connection string a su dirección IPv4.
/// Necesario porque Render no tiene conectividad IPv6 pero Supabase
/// devuelve registros AAAA (IPv6) primero en el DNS.
/// Si la resolución falla o ya es una IP, devuelve la cadena sin cambios.
/// </summary>
static string ResolveToIPv4(string connectionString)
{
    try
    {
        // Extraer el valor del parámetro Host=...
        var hostKey = "Host=";
        var hostStart = connectionString.IndexOf(hostKey, StringComparison.OrdinalIgnoreCase);
        if (hostStart < 0) return connectionString;

        hostStart += hostKey.Length;
        var hostEnd = connectionString.IndexOf(';', hostStart);
        var hostname = hostEnd < 0
            ? connectionString[hostStart..]
            : connectionString[hostStart..hostEnd];

        // Si ya es una IP no hace falta resolver
        if (IPAddress.TryParse(hostname, out _)) return connectionString;

        // Resolver y tomar la primera dirección IPv4
        var addresses = Dns.GetHostAddresses(hostname);
        var ipv4 = Array.Find(addresses, a => a.AddressFamily == AddressFamily.InterNetwork);
        if (ipv4 is null) return connectionString;

        // Reemplazar el hostname por la IP en la cadena de conexión
        return connectionString.Replace($"{hostKey}{hostname}", $"{hostKey}{ipv4}");
    }
    catch
    {
        // En caso de error de DNS, intentar con la cadena original
        return connectionString;
    }
}