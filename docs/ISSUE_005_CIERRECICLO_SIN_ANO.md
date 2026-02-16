# Issue #005 - Tabla cierreciclo sin columna ANO

**Fecha de Detección:** 2026-02-14  
**Versión:** v4.2.9  
**Severidad:** 🟢 Baja (No crítico)  
**Estado:** 📋 Documentado - Pendiente Implementación  
**Tipo:** Mejora / Estructura de Datos

---

## 📋 Descripción

La tabla `cierreciclo` no tiene la columna `ANO` en su esquema, solo tiene la columna `CICLO` (número de ciclo 1-12). Esto podría causar problemas si la aplicación Android necesita filtrar datos históricos de cierre de ciclo por año.

---

## 🔍 Análisis Actual

### Esquema de la Tabla

**Tabla `cierreciclo` (actual):**
```sql
CREATE TABLE "cierreciclo" (
    "Zona" TEXT(7), 
    "Ciclo" INTEGER(11),        -- ⚠️ Solo CICLO, no ANO
    "Fecha" TEXT(40),           -- Fecha como texto
    "Hora" TEXT(15), 
    "Fichados" INTEGER(11), 
    "Visitados" INTEGER(11), 
    "PorcentajeFichadoVisitados" REAL(8), 
    "Revisitas" INTEGER(11), 
    "Revisitados" INTEGER(11), 
    "PorcentajeRevisitasRevisitados" REAL(8), 
    "DiasDeCiclos" INTEGER(11), 
    "DiasTranscurridos" INTEGER(11), 
    "DiasTrabajados" REAL(8), 
    "DiasDescontados" REAL(8), 
    "PDE" REAL(8), 
    "PDR" REAL(8), 
    "PDP" REAL(8), 
    "PR" REAL(8), 
    "PP" REAL(8), 
    "Mix" REAL(8), 
    "FichadosPtos" INTEGER(11), 
    "VisitadosPtos" INTEGER(11), 
    "PorcentajeFichadoVisitadosPtos" REAL(8), 
    "RevisitasPtos" INTEGER(11), 
    "RevisitadosPtos" INTEGER(11), 
    "PorcentajeRevisitasRevisitadosPtos" REAL(8), 
    "PDEPtos" REAL(8), 
    "PDRPtos" REAL(8), 
    "PDPPtos" REAL(8), 
    "PRPtos" REAL(8), 
    "PPPtos" REAL(8), 
    "MixPtos" REAL(8), 
    "DescontadoS" REAL(8), 
    "DescontadoN" REAL(8), 
    "Ficha" REAL(8), 
    "AltasPostulados" INTEGER(11), 
    "AltasAprobados" INTEGER(11), 
    "BajasPostulados" INTEGER(11), 
    "BajasAprobados" INTEGER(11)
);
```

### INSERT Actual

**Ejemplo de INSERT:**
```sql
INSERT INTO cierreciclo (Zona, CICLO, Fecha, Hora, Fichados, Visitados, ...) 
VALUES (336, 2, '2025-02-28', '06:28:12 P.M', 160, 118, ...);
```

**Datos de ejemplo:**
- Zona: 336
- CICLO: 2 (solo número de ciclo)
- Fecha: '2025-02-28' (fecha como texto, incluye año pero no es filtrable fácilmente)

---

## ⚠️ Problema Potencial

### Escenario Problemático

Si la app Android necesita consultar datos históricos de cierre de ciclo:

```sql
-- ❌ Esta consulta NO funcionará correctamente
SELECT * FROM cierreciclo 
WHERE Zona = '336' AND CICLO = 2 AND ANO = 2026;
-- Error: columna ANO no existe
```

Sin la columna `ANO`, la app tendría que:
1. Parsear la columna `Fecha` (texto) para extraer el año
2. O asumir que todos los registros son del año actual
3. O no poder distinguir entre Ciclo 2 de 2025 vs Ciclo 2 de 2026

### Comparación con Tabla `ciclos`

La tabla `ciclos` **SÍ tiene** la columna `ANO`:

```sql
CREATE TABLE "ciclos" (
    "FECHAI_CICLO" TEXT(8), 
    "FECHAF_CICLO" TEXT(8), 
    "NRO_CICLO" INTEGER(11), 
    "CICLO_CERRADO" TEXT(255), 
    "DIAS_HABILES" INTEGER(11), 
    "ESTATUS" TEXT(255), 
    "ANO" integer(11),              -- ✅ Tiene ANO
    "KPI_Visita_Medica" INTEGER(11), 
    "KPI_Visita_Farmacia" INTEGER(11)
);
```

---

## 🔧 Solución Propuesta (Futura)

### Opción 1: Agregar columna ANO al esquema

**Modificar esquema:**
```sql
CREATE TABLE "cierreciclo" (
    "Zona" TEXT(7), 
    "Ciclo" INTEGER(11),
    "ANO" INTEGER(11),          -- ← NUEVO
    "Fecha" TEXT(40), 
    ...
);
```

**Modificar INSERT:**
```sql
INSERT INTO cierreciclo (Zona, CICLO, ANO, Fecha, Hora, ...) 
VALUES (336, 2, 2026, '2025-02-28', '06:28:12 P.M', ...);
```

### Opción 2: Usar columna Fecha existente

Si la app puede parsear la fecha, no es necesario agregar ANO. Pero esto requiere:
- Lógica adicional en la app para extraer el año
- Conversión de texto a fecha en cada consulta

---

## 📊 Impacto Actual

### Para la Aplicación Android
- 🟢 **Bajo**: Si la app no filtra por año en cierreciclo, no hay problema
- 🟡 **Medio**: Si necesita filtrar por año, tendrá que parsear la columna Fecha

### Para el Sistema
- 🟢 **Bajo**: No es crítico actualmente
- 🟡 **Medio**: Podría ser necesario en el futuro para datos históricos multi-año

---

## 🔍 Investigación Necesaria

Antes de implementar, verificar:

1. **¿La app Android consulta cierreciclo filtrando por año?**
   - Revisar código Android para ver si usa ANO en queries
   - Verificar si parsea la columna Fecha

2. **¿Cuántos años de datos históricos se mantienen?**
   - Si solo se mantiene 1 año, no es necesario ANO
   - Si se mantienen múltiples años, ANO es importante

3. **¿El generador ClickOne tiene ANO en cierreciclo?**
   - Verificar si el sistema original tiene esta columna
   - Mantener compatibilidad con ClickOne

---

## 🎯 Criterios de Aceptación (Cuando se implemente)

- [ ] Columna `ANO` agregada al esquema de `cierreciclo`
- [ ] INSERT statements incluyen valor de ANO
- [ ] Valor de ANO se lee desde la BD SQL Server (tabla MD_CierreCiclo)
- [ ] Cartera generada y verificada con ANO
- [ ] App Android actualizada para usar ANO (si es necesario)
- [ ] Documentación actualizada

---

## 📝 Archivos Involucrados (Futura Implementación)

- `WebApplication1/Services/GeneradorService.cs` (método `GenerarContenidoCierreCiclosAsync`)
- Esquema de tabla en `GenerarEsquemaTablas()`
- App Android (si necesita cambios en queries)

---

## 🔗 Issues Relacionados

- **Issue #002**: Campo ANO en 0 en tabla Ciclos (✅ Resuelto)
- **Issue #003**: Campo CICLO en 0 (✅ Resuelto)
- **Issue #004**: Columna CICLO faltante en Solicitudes (✅ Resuelto)

**Patrón:** Múltiples tablas tenían problemas con columnas de año/ciclo.

---

## 💡 Recomendación

**Prioridad:** 🟢 Baja - No implementar ahora

**Razones:**
1. No es crítico para la funcionalidad actual
2. La columna `Fecha` contiene el año (aunque como texto)
3. Primero verificar si la app realmente necesita filtrar por año
4. Implementar solo si se confirma la necesidad

**Próximos pasos:**
1. Monitorear si surgen problemas relacionados con filtrado por año
2. Consultar con el equipo Android si necesitan esta columna
3. Si se confirma la necesidad, implementar en una versión futura

---

**Última Actualización:** 2026-02-14  
**Responsable:** Backend  
**Estado:** 📋 Documentado - No Crítico  
**Prioridad:** Baja
