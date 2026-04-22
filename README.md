# Gestor de Tareas — Fullstack

Aplicación web de gestión de tareas desarrollada como prueba técnica para el
Superior Tribunal de Justicia de la Provincia de La Pampa.

Permite crear, listar, editar, eliminar y filtrar tareas por estado, con una
API REST en ASP.NET Core y una interfaz en React consumiendo dicha API.

---

## Stack tecnológico

| Capa              | Tecnología                         |
|-------------------|------------------------------------|
| Frontend          | React 18 + Vite                    |
| Backend           | ASP.NET Core 8 Web API             |
| Base de datos     | SQL Server Express / LocalDB       |
| ORM               | Entity Framework Core 8            |
| Documentación API | Swagger / Swashbuckle              |
| Testing .NET      | xUnit + Moq                        |
| Testing React     | Vitest + React Testing Library     |
| Control de versiones | Git + GitHub                    |

---

## Estructura del repositorio

```
/
├── backend/
│   ├── GestorTareas.API/          → API REST en ASP.NET Core
│   │   ├── Controllers/           → Endpoints HTTP
│   │   ├── Data/                  → DbContext (EF Core)
│   │   ├── DTOs/                  → Modelos de entrada y salida
│   │   ├── Models/                → Entidades de dominio
│   │   ├── Repositories/          → Acceso a datos
│   │   ├── Services/              → Lógica de negocio
│   │   └── Program.cs             → Configuración e inyección de dependencias
│   └── GestorTareas.Tests/        → Tests unitarios xUnit
├── frontend/
│   └── gestor-tareas/
│       └── src/
│           ├── api/               → Capa de acceso a la API REST
│           ├── components/        → Componentes React
│           ├── hooks/             → Hook personalizado useTasks
│           └── App.jsx            → Componente raíz
├── database/
│   ├── schema.sql                 → Creación de tablas
│   └── seed.sql                   → Datos de prueba
└── README.md
```

---

## Cómo ejecutar el proyecto localmente

### Requisitos previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- SQL Server Express o LocalDB

---

### 1. Base de datos

Ejecutar los scripts en orden desde SQL Server Management Studio o `sqlcmd`:

```bash
sqlcmd -S localhost -i database/schema.sql
sqlcmd -S localhost -i database/seed.sql
```

Esto crea la base de datos `GestorTareas` con 3 usuarios y 6 tareas de prueba.

---

### 2. Backend

```bash
cd backend/GestorTareas.API
dotnet restore
dotnet run
```

La API queda disponible en:
- API: `https://localhost:xxxx/api/tasks`
- Swagger: `https://localhost:xxxx/swagger`

La cadena de conexión se configura en `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=GestorTareas;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

---

### 3. Frontend

```bash
cd frontend/gestor-tareas
cp .env.example .env.local   # configurar la URL de la API
npm install
npm run dev
```

La app queda disponible en `http://localhost:5173`.

El archivo `.env.local` debe contener:

```
VITE_API_URL=https://localhost:xxxx/api
```

---

### 4. Tests

**Backend (xUnit):**
```bash
cd backend/GestorTareas.Tests
dotnet test
```

**Frontend (Vitest):**
```bash
cd frontend/gestor-tareas
npx vitest run
```

---

## Endpoints de la API

| Método | Endpoint                      | Descripción                     |
|--------|-------------------------------|---------------------------------|
| GET    | `/api/tasks`                  | Listar todas las tareas         |
| GET    | `/api/tasks?status={estado}`  | Filtrar tareas por estado       |
| GET    | `/api/tasks/{id}`             | Obtener una tarea por ID        |
| POST   | `/api/tasks`                  | Crear una nueva tarea           |
| PUT    | `/api/tasks/{id}`             | Actualizar una tarea existente  |
| DELETE | `/api/tasks/{id}`             | Eliminar una tarea              |

Estados válidos: `pending` · `in_progress` · `completed`

La documentación interactiva completa está disponible en `/swagger`.

---

## Decisiones de diseño

**Arquitectura en capas (Controller → Service → Repository)**
Separar responsabilidades permite que cada capa tenga una única razón para
cambiar. El controller no sabe nada de EF Core; el repositorio no sabe nada
de HTTP. Esto también hace que los tests unitarios sean simples: se mockea
la capa inmediatamente inferior.

**DTOs de entrada y salida**
Los modelos de dominio (`TaskItem`, `User`) nunca se exponen directamente.
Los DTOs permiten validar datos en el límite de la API, controlar qué campos
se devuelven al cliente y evolucionar el modelo interno sin romper el contrato
de la API.

**Hook personalizado `useTasks`**
Encapsula toda la lógica de fetching y estado del frontend. Los componentes
(`TaskList`, `TaskForm`, `TaskFilter`) son puramente presentacionales: reciben
datos y callbacks, no saben cómo se obtienen ni se persisten.

**`useCallback` en `fetchTasks`**
Sin `useCallback`, la función se recrearía en cada render y el `useEffect`
que depende de ella entraría en loop infinito. Es una decisión consciente,
no boilerplate.

**Actualización parcial en PUT**
`UpdateTaskDto` tiene todos los campos opcionales. El servicio solo sobreescribe
los campos que vienen con valor, lo que da más flexibilidad al cliente sin
necesidad de un endpoint PATCH separado.

**Variables de entorno**
La cadena de conexión (backend) y la URL de la API (frontend) nunca están
hardcodeadas. Se leen desde `appsettings.json` y `.env.local` respectivamente,
lo que facilita desplegar en distintos entornos sin tocar el código.

**`TaskItem` en lugar de `Task`**
El modelo de dominio se llama `TaskItem` para evitar el conflicto de nombres
con `System.Threading.Tasks.Task` de C#. Decisión explícita documentada en
el propio modelo.

---

## Herramientas de IA utilizadas

Se utilizó **Claude (Anthropic), Google Gemini y ChatGPT** como asistentes durante el desarrollo para:

- Estructuración inicial del proyecto y diagrama de fases
- Definición de la arquitectura en capas y patrones utilizados (Repository, Service, DTOs)
- Generación de boilerplate y revisión de convenciones de ASP.NET Core 8
- Revisión del patrón `useCallback` + `useEffect` en el hook personalizado
- Redacción de mensajes de commit bajo el estándar Conventional Commits

Todas las decisiones técnicas fueron evaluadas, comprendidas y adaptadas al
contexto del proyecto. El código refleja criterio propio sobre cada elección
realizada y puede ser explicado en detalle durante la instancia de devolución.

---
