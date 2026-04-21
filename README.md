# Gestor de Tareas — Fullstack

Aplicación web de gestión de tareas desarrollada como prueba técnica para el
Superior Tribunal de Justicia de la Provincia de La Pampa.

## Stack tecnológico

| Capa        | Tecnología                        |
|-------------|-----------------------------------|
| Frontend    | React 18 + Vite                   |
| Backend     | ASP.NET Core 8 Web API            |
| Base de datos | SQL Server Express / LocalDB    |
| Testing     | xUnit (.NET) · Jest / RTL (React) |
| Docs API    | Swagger / Swashbuckle             |

## Estructura del proyecto

```
/
├── backend/      → API REST en ASP.NET Core
├── frontend/     → Interfaz en React + Vite
├── database/     → Scripts SQL (schema + seed data)
└── README.md
```

## Cómo ejecutar el proyecto

> Las instrucciones detalladas se completan al finalizar el desarrollo.

### Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- SQL Server Express o LocalDB

### Base de datos

```bash
# Ejecutar el script en SQL Server Management Studio o sqlcmd
sqlcmd -S localhost -i database/schema.sql
sqlcmd -S localhost -i database/seed.sql
```

### Backend

```bash
cd backend
dotnet restore
dotnet run
# API disponible en https://localhost:5001
# Swagger en https://localhost:5001/swagger
```

### Frontend

```bash
cd frontend
npm install
npm run dev
# App disponible en http://localhost:5173
```

## Herramientas de IA utilizadas

Se utilizó **Claude (Anthropic)** como asistente durante el desarrollo para:
- Estructuración inicial del proyecto y decisiones de arquitectura
- Revisión de patrones de diseño (Repository, Service Layer, DTOs)
- Generación de boilerplate y corrección de errores puntuales

Todas las decisiones técnicas fueron evaluadas, comprendidas y adaptadas
al contexto del proyecto. El código refleja criterio propio sobre cada
elección realizada.

## Decisiones de diseño

- **Arquitectura en capas** (Controller → Service → Repository): permite
  separar responsabilidades, facilita el testing unitario y hace el código
  más mantenible.
- **DTOs de entrada y salida**: evita exponer directamente el modelo de base
  de datos y permite validar los datos en el límite de la API.
- **Hook personalizado `useTasks`**: encapsula toda la lógica de estado y
  fetching en el frontend, manteniendo los componentes simples y enfocados
  en la presentación.
- **Variables de entorno**: la cadena de conexión y la URL de la API nunca
  están hardcodeadas; se leen desde configuración externa.
