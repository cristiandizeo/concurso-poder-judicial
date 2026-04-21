-- =============================================================
-- seed.sql
-- Gestor de Tareas — Superior Tribunal de Justicia de La Pampa
-- Datos de prueba: 3 usuarios y 6 tareas
-- =============================================================

USE GestorTareas;
GO

-- Limpiar datos previos respetando el orden de FK
DELETE FROM dbo.Tasks;
DELETE FROM dbo.Users;

-- Reiniciar identidades
DBCC CHECKIDENT ('dbo.Tasks', RESEED, 0);
DBCC CHECKIDENT ('dbo.Users', RESEED, 0);
GO

-- =============================================================
-- Usuarios
-- =============================================================
INSERT INTO dbo.Users (Name, Email) VALUES
    ('Ana García',    'ana.garcia@example.com'),
    ('Carlos López',  'carlos.lopez@example.com'),
    ('María Fernández','maria.fernandez@example.com');
GO

-- =============================================================
-- Tareas (distribuidas entre los 3 usuarios, los 3 estados)
-- =============================================================
INSERT INTO dbo.Tasks (Title, Description, Status, UserId) VALUES
    ('Revisar expediente 142/2026',
     'Leer y resumir el expediente antes de la audiencia del jueves.',
     'pending',      1),

    ('Preparar informe mensual',
     'Consolidar métricas del sistema de gestión documental.',
     'in_progress',  1),

    ('Actualizar manual de procedimientos',
     'Incorporar los cambios aprobados en la reunión del 15/04.',
     'completed',    2),

    ('Configurar entorno de testing',
     'Instalar xUnit y crear proyecto de pruebas para la API.',
     'in_progress',  2),

    ('Redactar acta de reunión',
     'Documentar los acuerdos alcanzados en la sesión del 20/04.',
     'pending',      3),

    ('Migrar base de datos a nueva versión',
     'Ejecutar scripts de migración y validar integridad referencial.',
     'completed',    3);
GO
