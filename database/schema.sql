-- =============================================================
-- schema.sql
-- Gestor de Tareas — Superior Tribunal de Justicia de La Pampa
-- Creación del esquema relacional
-- =============================================================

-- Crear la base de datos si no existe
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'GestorTareas')
BEGIN
    CREATE DATABASE GestorTareas;
END
GO

USE GestorTareas;
GO

-- =============================================================
-- Tabla: Users
-- Almacena los usuarios del sistema.
-- =============================================================
IF OBJECT_ID('dbo.Tasks', 'U') IS NOT NULL DROP TABLE dbo.Tasks;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
GO

CREATE TABLE dbo.Users (
    Id          INT             NOT NULL IDENTITY(1,1),
    Name        NVARCHAR(100)   NOT NULL,
    Email       NVARCHAR(150)   NOT NULL,
    CreatedAt   DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Users       PRIMARY KEY (Id),
    CONSTRAINT UQ_Users_Email UNIQUE      (Email)
);
GO

-- =============================================================
-- Tabla: Tasks
-- Almacena las tareas asociadas a cada usuario.
-- Estado posible: 'pending' | 'in_progress' | 'completed'
-- =============================================================
CREATE TABLE dbo.Tasks (
    Id          INT             NOT NULL IDENTITY(1,1),
    Title       NVARCHAR(200)   NOT NULL,
    Description NVARCHAR(1000)  NULL,
    Status      NVARCHAR(20)    NOT NULL DEFAULT 'pending',
    UserId      INT             NOT NULL,
    CreatedAt   DATETIME2       NOT NULL DEFAULT SYSUTCDATETIME(),

    CONSTRAINT PK_Tasks              PRIMARY KEY (Id),
    CONSTRAINT FK_Tasks_Users        FOREIGN KEY (UserId) REFERENCES dbo.Users(Id)
                                         ON DELETE CASCADE,
    CONSTRAINT CK_Tasks_Status       CHECK (Status IN ('pending', 'in_progress', 'completed'))
);
GO
