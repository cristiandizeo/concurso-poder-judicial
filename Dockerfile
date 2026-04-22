# ── Etapa 1: build ────────────────────────────────────────────────────────────
# Usamos la imagen oficial del SDK de .NET 8 para compilar el proyecto.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar el .csproj y restaurar dependencias primero.
# Docker cachea esta capa si el .csproj no cambia, acelerando builds futuros.
COPY backend/GestorTareas.API/GestorTareas.API.csproj ./backend/GestorTareas.API/
RUN dotnet restore ./backend/GestorTareas.API/GestorTareas.API.csproj

# Copiar el resto del código y compilar en modo Release
COPY backend/GestorTareas.API/ ./backend/GestorTareas.API/
RUN dotnet publish ./backend/GestorTareas.API/GestorTareas.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

# ── Etapa 2: runtime ──────────────────────────────────────────────────────────
# Imagen más liviana que solo tiene el runtime, no el SDK completo.
# Esto reduce el tamaño final de la imagen considerablemente.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

# Render inyecta el puerto en la variable PORT; ASP.NET Core lo lee aquí.
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

ENTRYPOINT ["dotnet", "GestorTareas.API.dll"]