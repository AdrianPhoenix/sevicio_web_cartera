# Migración a Servicio Web - Plan del Proyecto

## Objetivo Principal
Modernizar el sistema actual de generación y distribución de bases de datos para visitadores médicos, migrando de un proceso manual con archivos estáticos a una arquitectura de servicios web moderna.

## Problema Actual
- **Distribución manual** del archivo `Cartera.txt`
- **Sin control de versiones** de los datos
- **Proceso rudimentario** que genera problemas logísticos
- **Dificultades** cuando hay cambios en la app Android
- **Falta de sincronización** en tiempo real

## Solución Propuesta

### Arquitectura Objetivo
```
Apps Android ↔ API REST ↔ Lógica del Generador ↔ SQL Server
```

### Beneficios Esperados
- **Eliminación** de distribución manual
- **Sincronización automática** de datos
- **Trabajo offline** mantenido
- **Escalabilidad** para múltiples visitadores
- **Competitividad** en el mercado

## Fases de Implementación

### Fase 1: API Básica (1-2 semanas)
- Crear API REST con ASP.NET Core
- Migrar lógica de `Generador.cs` y `Data.cs`
- Endpoints básicos para obtener datos de visitadores

### Fase 2: Sincronización (1 semana)
- Implementar endpoint de sincronización bidireccional
- Mantener funcionalidad offline de las apps
- Sistema de versionado de datos

### Fase 3: Deployment y Optimización (3-5 días)
- Configurar en Windows Server 2025
- Optimización de rendimiento
- Testing y documentación

## Tecnologías Seleccionadas
- **Backend**: ASP.NET Core 8+ (reutiliza código C# existente)
- **Base de datos**: SQL Server (actual)
- **Servidor**: Windows Server 2025 con IIS
- **Arquitectura**: RESTful API

## Flujo de Trabajo Futuro

### Para Visitadores:
1. Abrir app Android
2. Presionar "Transmitir" cuando tengan internet
3. App sincroniza automáticamente:
   - Sube cambios locales al servidor
   - Descarga actualizaciones del servidor
   - Actualiza base de datos local (SQLite)

### Para Administradores:
- Sin intervención manual
- Monitoreo a través de logs de la API
- Actualizaciones automáticas para todos los visitadores

## Métricas de Éxito
- **Reducción 80%** en tiempo de distribución
- **Eliminación** de errores manuales
- **Mejora** en satisfacción de usuarios
- **Preparación** para funcionalidades futuras (analytics, reportes en tiempo real)

## Consideraciones Técnicas
- Mantener compatibilidad con apps Android existentes
- Migración gradual sin interrumpir operaciones
- Backup y rollback plan
- Documentación de API para desarrolladores
