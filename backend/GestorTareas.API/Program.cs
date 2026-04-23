using System.Net;
using System.Net.Sockets;
using GestorTareas.API.Data;
using GestorTareas.API.Repositories;
using GestorTareas.API.Repositories.Interfaces;
using GestorTareas.API.Services;
using GestorTareas.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

// ── 1. Configuración de Base de Datos ──────────────────────────────────────
var rawConnectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("No se encontró cadena de conexión.");

// Forzamos la resolución a IPv4 usando el Helper mejorado
var connectionString = ResolveToIPv4(rawConnectionString);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// ── 2. Inyección de dependencias ──────────────────────────────────────────
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<ITaskService, TaskService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ── 3. CORS (Configuración Robusta) ───────────────────────────────────────
var frontendUrl = (Environment.GetEnvironmentVariable("FRONTEND_URL") 
                  ?? "http://localhost:5173").TrimEnd('/'); // <--- IMPORTANTE

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins(frontendUrl)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowedToAllowWildcardSubdomains());
});

var app = builder.Build();

// ── 4. Migraciones con protección ante fallos ─────────────────────────────
// Si esto falla, la app sigue viva y el CORS puede informar el error al Front.
using (var scope = app.Services.CreateScope())
{
    try 
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
        Console.WriteLine("✅ Base de datos conectada y migrada.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error crítico de DB: {ex.Message}");
        // Dejamos que la app continúe para que no de error 139 (SegFault) inmediato
    }
}

// ── 5. Orden de Middleware (ESTRICTO) ─────────────────────────────────────
app.UseSwagger();
app.UseSwaggerUI();

// CORS DEBE IR ANTES QUE CUALQUIER COSA QUE RESPONDA AL CLIENTE
app.UseCors("Frontend");

if (app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();

// ── Helpers ───────────────────────────────────────────────────────────────

static string ResolveToIPv4(string connectionString)
{
    try
    {
        var cb = new NpgsqlConnectionStringBuilder(connectionString);
        string hostname = cb.Host;

        if (string.IsNullOrEmpty(hostname) || IPAddress.TryParse(hostname, out _)) 
            return connectionString;

        var addresses = Dns.GetHostAddresses(hostname);
        var ipv4 = Array.Find(addresses, a => a.AddressFamily == AddressFamily.InterNetwork);

        if (ipv4 != null)
        {
            cb.Host = ipv4.ToString();
            Console.WriteLine($"🌐 IPv4 Proxy: {hostname} -> {cb.Host}");
            return cb.ToString();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Error resolviendo DNS: {ex.Message}");
    }
    return connectionString;
}
