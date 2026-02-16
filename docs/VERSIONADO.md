# Guía de Versionado del Proyecto

## 📋 Estrategia de Versionado

Este proyecto utiliza **Versionado Semántico (SemVer)** y una estrategia de ramas simplificada basada en Git Flow.

### Formato de Versiones

```
vMAJOR.MINOR.PATCH
```

- **MAJOR (v4.x.x):** Cambios incompatibles con versiones anteriores
- **MINOR (v4.1.x):** Nueva funcionalidad compatible con versiones anteriores  
- **PATCH (v4.0.1):** Correcciones de bugs compatibles

**Ejemplo:** `v4.0.1`

---

## 🌳 Estrategia de Ramas

### Ramas Principales

#### `main`
- **Propósito:** Código en producción estable
- **Protección:** Solo recibe merges desde ramas `release/vX.X.X`
- **Tags:** Cada merge a main debe tener un tag de versión

#### `development`
- **Propósito:** Rama de desarrollo activo
- **Uso:** Desarrollo diario, integración de features y fixes
- **Origen:** Base para crear ramas `release/vX.X.X`

### Ramas Temporales

#### `release/vX.X.X`
- **Propósito:** Preparación de una nueva versión
- **Creación:** Desde `development` cuando se completan features/fixes
- **Contenido:** 
  - Actualización de CHANGELOG.md
  - Ajustes finales de documentación
  - Testing de pre-producción
- **Destino:** Merge a `main` y tag de versión
- **Ejemplo:** `release/v4.0.1`

#### `hotfix/nombre-descriptivo` (opcional)
- **Propósito:** Correcciones urgentes en producción
- **Creación:** Desde `main`
- **Destino:** Merge a `main` y `development`

#### `feature/nombre-descriptivo` (opcional)
- **Propósito:** Desarrollo de nuevas funcionalidades
- **Creación:** Desde `development`
- **Destino:** Merge a `development`

---

## 🏷️ Tags de Versión

Cada versión en producción debe tener un tag anotado:

```powershell
git tag -a v4.0.1 -m "Release v4.0.1: Fixes columnas CICLO/ANO + Endpoint KPIs"
git push origin v4.0.1
```

### Convención de Mensajes de Tag

```
Release vX.X.X: Descripción breve de cambios principales
```

**Ejemplos:**
- `Release v4.0.1: Fixes columnas CICLO/ANO + Endpoint KPIs`
- `Release v4.1.0: Nueva funcionalidad de reportes`
- `Release v4.0.2: Hotfix error en generación de cartera`

---

## 🔄 Flujo de Trabajo para Releases

### 1. Desarrollo en `development`

```powershell
# Trabajar en development
git checkout development
# ... hacer cambios, commits ...
git add .
git commit -m "feat: descripción del cambio"
```

### 2. Crear Rama Release

```powershell
# Crear rama release desde development
git checkout -b release/v4.0.1 development

# Actualizar CHANGELOG.md con los cambios
# Actualizar documentación si es necesario

git add CHANGELOG.md docs/
git commit -m "docs: preparar release v4.0.1"
```

### 3. Merge a Main y Tag

```powershell
# Cambiar a main
git checkout main

# Merge de la rama release
git merge --no-ff release/v4.0.1 -m "Release v4.0.1"

# Crear tag anotado
git tag -a v4.0.1 -m "Release v4.0.1: Fixes columnas CICLO/ANO + Endpoint KPIs"

# Push a remoto
git push origin main
git push origin v4.0.1
```

### 4. Merge de vuelta a Development

```powershell
# Sincronizar cambios de release con development
git checkout development
git merge --no-ff release/v4.0.1 -m "Merge release v4.0.1 back to development"
git push origin development
```

### 5. Limpiar Rama Release (opcional)

```powershell
# Eliminar rama release local
git branch -d release/v4.0.1

# Eliminar rama release remota (si existe)
git push origin --delete release/v4.0.1
```

---

## 📝 Convención de Commits

Usamos commits semánticos para facilitar la generación de CHANGELOG:

### Tipos de Commits

- `feat:` Nueva funcionalidad
- `fix:` Corrección de bug
- `docs:` Cambios en documentación
- `refactor:` Refactorización de código
- `test:` Agregar o modificar tests
- `chore:` Tareas de mantenimiento
- `perf:` Mejoras de rendimiento

### Formato

```
tipo: descripción breve

Descripción detallada (opcional)

Refs: #issue-number (opcional)
```

### Ejemplos

```bash
feat: agregar endpoint GET /api/visitador/{id}/kpis

Endpoint que retorna métricas KPI del visitador para un ciclo específico.
Incluye kpiVisitaMedica, kpiVisitaFarmacia, fechas y estatus del ciclo.

Refs: #001
```

```bash
fix: agregar columna CICLO en INSERT de tabla visitas

La columna CICLO no se incluía en los INSERT statements de las tablas
visitas y hvisitas, causando que los filtros por ciclo fallaran.

Refs: #006
```

---

## 🚀 Despliegue a Producción

### Proceso de Despliegue

1. **Compilar el proyecto:**
   ```powershell
   cd WebApplication1
   dotnet build -c Release
   ```

2. **Publicar:**
   ```powershell
   dotnet publish -c Release -o ../publicaciones/v4_0_1
   ```

3. **Copiar a servidor:**
   ```powershell
   # Copiar archivos publicados al servidor de producción
   # http://mdnconsultores.com:8080
   ```

4. **Verificar en producción:**
   ```powershell
   # Probar endpoints críticos
   curl http://mdnconsultores.com:8080/api/visitador/336/kpis?ano=2026&ciclo=2
   ```

5. **Documentar despliegue:**
   - Actualizar `docs/DESPLIEGUE_[FECHA].md`
   - Registrar en CHANGELOG.md

---

## 📊 Historial de Versiones

| Versión | Fecha | Descripción | Tag |
|---------|-------|-------------|-----|
| v4.0.1 | 2026-02-16 | Fixes CICLO/ANO + Endpoint KPIs | ✅ |
| v4.0.0 | 2026-02-01 | Versión base | ✅ |

---

## 🔗 Referencias

- [Semantic Versioning](https://semver.org/lang/es/)
- [Keep a Changelog](https://keepachangelog.com/es-ES/1.0.0/)
- [Git Flow](https://nvie.com/posts/a-successful-git-branching-model/)
- [Conventional Commits](https://www.conventionalcommits.org/es/)
