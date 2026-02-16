# Changelog

Todos los cambios notables en este proyecto serán documentados en este archivo.

El formato está basado en [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/),
y este proyecto adhiere a [Versionado Semántico](https://semver.org/lang/es/).

## [4.0.1] - 2026-02-16

### 🐛 Corregido

#### Issue #002: Columna ANO faltante en tabla Ciclos
- **Problema:** La app Android leía ANO=0 en lugar del año actual (2026)
- **Solución:** Agregada columna ANO en INSERT de tabla `Ciclos`
- **Archivo:** `WebApplication1/Services/GeneradorService.cs` (líneas 145-160)
- **Documentación:** `docs/ISSUE_002_SOLUCION.md`

#### Issue #004: Columna CICLO faltante en tablas solicitudes/hsolicitudes
- **Problema:** Query `WHERE CICLO = '2'` retornaba 0 resultados en Android
- **Causa raíz:** Columna CICLO excluida de INSERT statements
- **Solución:** Modificada lógica de filtrado de columnas para incluir CICLO en solicitudes/hsolicitudes
- **Archivo:** `WebApplication1/Services/GeneradorService.cs` (líneas 265-290)
- **Documentación:** `docs/ISSUE_004_SOLUCION.md`

#### Issue #006: Columna CICLO faltante en tablas visitas/hvisitas
- **Problema:** Tablas visitas y hvisitas no incluían columna CICLO en INSERT statements
- **Solución:** Agregadas tablas `visitas` y `hvisitas` a la lista de tablas que permiten CICLO
- **Archivo:** `WebApplication1/Services/GeneradorService.cs` (líneas 276-285)
- **Impacto:** Permite filtrar visitas por ciclo correctamente en la app Android

### ✨ Agregado

#### Endpoint KPIs de Visitador
- **Nuevo endpoint:** `GET /api/visitador/{id}/kpis?ano={ano}&ciclo={ciclo}`
- **Funcionalidad:** Retorna métricas KPI del visitador para un ciclo específico
- **Respuesta incluye:**
  - `kpiVisitaMedica`: Meta de visitas médicas (default: 8)
  - `kpiVisitaFarmacia`: Meta de visitas a farmacias (default: 4)
  - `fechaInicioCiclo`: Fecha de inicio del ciclo
  - `fechaFinCiclo`: Fecha de fin del ciclo
  - `estatusCiclo`: Estado del ciclo (Abierto/Cerrado)
- **Archivos:**
  - `WebApplication1/Models/KpiResponse.cs`
  - `WebApplication1/Services/DataService.cs`
  - `WebApplication1/Controllers/VisitadorController.cs`
- **Documentación:** `docs/ENDPOINT_KPIS_TECNICO.md`
- **Producción:** `http://mdnconsultores.com:8080/api/visitador/{id}/kpis`

### 📝 Documentación

#### Issue #005: Tabla cierreciclo sin columna ANO
- **Documentado:** Tabla `cierreciclo` no tiene columna ANO (solo CICLO)
- **Estado:** Baja prioridad, no crítico
- **Archivo:** `docs/ISSUE_005_CIERRECICLO_SIN_ANO.md`

### 🚀 Despliegue

- **Fecha:** 14 de febrero de 2026
- **Servidor:** http://mdnconsultores.com:8080
- **Documentación:** `docs/DESPLIEGUE_14_FEB_2026.md`

---

## [4.0.0] - 2026-02-01

### Versión Base
- Sistema generador de carteras ClickOne funcional
- Compatibilidad con aplicación Android
- 114 tablas en cartera base

---

## Formato de Versiones

- **MAJOR.MINOR.PATCH** (ej: 4.0.1)
- **MAJOR:** Cambios incompatibles con versiones anteriores
- **MINOR:** Nueva funcionalidad compatible con versiones anteriores
- **PATCH:** Correcciones de bugs compatibles

## Enlaces

- [Documentación Técnica](docs/README.md)
- [Estado Actual del Proyecto](docs/ESTADO_ACTUAL.md)
- [Endpoints API](docs/ENDPOINTS.md)
