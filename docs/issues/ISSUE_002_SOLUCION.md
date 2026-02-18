# Issue #002 - Solución: Campo ANO en 0 en Tabla Ciclos

**Fecha de Resolución:** 2026-02-14  
**Versión:** v4.2.9  
**Severidad:** 🟡 Media  
**Estado:** ✅ Resuelto  
**Tipo:** Bug Fix / Datos

---

## 📋 Descripción del Problema

La aplicación Android leía el campo `ANO` de la tabla `Ciclos` y obtenía el valor `0` en lugar del año actual (2026). Esto causaba problemas en la lógica de la app que dependía de este valor para filtrar y mostrar datos correctamente.

---

## 🔍 Investigación

### Análisis del Esquema

La tabla `ciclos` **SÍ tenía** la columna `ANO` definida en el esquema:

```sql
CREATE TABLE "ciclos" (
    "FECHAI_CICLO" TEXT(8), 
    "FECHAF_CICLO" TEXT(8), 
    "NRO_CICLO" INTEGER(11), 
    "CICLO_CERRADO" TEXT(255), 
    "DIAS_HABILES" INTEGER(11), 
    "ESTATUS" TEXT(255), 
    "ANO" integer(11),           -- ✅ Columna definida
    "KPI_Visita_Medica" INTEGER(11), 
    "KPI_Visita_Farmacia" INTEGER(11)
);
```

### Análisis de los INSERT

Sin embargo, los INSERT **NO incluían** la columna `ANO`:

**ANTES (Incorrecto):**
```sql
INSERT INTO Ciclos (FECHAI_CICLO,FECHAF_CICLO,NRO_CICLO,CICLO_CERRADO,DIAS_HABILES,ESTATUS,KPI_Visita_Medica,KPI_Visita_Farmacia) 
VALUES ('13/01/2026','30/01/2026',1,'C',14,'C',8,4);
```

Resultado: La columna `ANO` quedaba en `NULL` o `0` (valor por defecto).

### Comparación con ClickOne

Curiosamente, el generador ClickOne (referencia original) **también tenía este problema**:
- ✅ Esquema: Columna `ANO` definida
- ❌ INSERT: No incluía la columna `ANO`

Esto sugiere que es un bug heredado del sistema original.

---

## 🔧 Solución Implementada

### Cambios en el Código

**Archivo:** `WebApplication1/Services/GeneradorService.cs`  
**Método:** `GenerarOtrosPasosAsync()`  
**Líneas:** 145-160

Se modificó el código para:
1. Leer el valor de `NU_Ano` desde la base de datos SQL Server
2. Incluir la columna `ANO` en el INSERT
3. Asignar el valor correcto del año

**Código Modificado:**

```csharp
if (cicloAbierto)
{
    contenido.AppendLine("-- Poblando tabla de Ciclos");
    await using var sqlCommand = new SqlCommand("SELECT * FROM MW_Ciclos WHERE NU_Ano=" + ano, sqlConnection);
    await using var reader = await sqlCommand.ExecuteReaderAsync();
    while(await reader.ReadAsync())
    {
        string estatus = (reader.GetInt16(2) < ciclo) ? "C" : (reader.GetInt16(2) == ciclo) ? "A" : "P";
        int anoValue = reader.GetInt16(1); // NU_Ano ← NUEVO: Leer el año
        int kpiVisitaMedica = reader.GetInt32(13);
        int kpiVisitaFarmacia = reader.GetInt32(14);
        
        // NUEVO: Incluir ANO en el INSERT
        contenido.AppendLine($"INSERT INTO Ciclos (FECHAI_CICLO,FECHAF_CICLO,NRO_CICLO,CICLO_CERRADO,DIAS_HABILES,ESTATUS,ANO,KPI_Visita_Medica,KPI_Visita_Farmacia) VALUES ('{reader.GetDateTime(3):dd/MM/yyyy}','{reader.GetDateTime(4):dd/MM/yyyy}',{reader.GetInt16(2)},'{estatus}',{reader.GetInt16(7)},'{estatus}',{anoValue},{kpiVisitaMedica},{kpiVisitaFarmacia});");
    }
}
```

### Resultado

**DESPUÉS (Correcto):**
```sql
INSERT INTO Ciclos (FECHAI_CICLO,FECHAF_CICLO,NRO_CICLO,CICLO_CERRADO,DIAS_HABILES,ESTATUS,ANO,KPI_Visita_Medica,KPI_Visita_Farmacia) 
VALUES ('13/01/2026','30/01/2026',1,'C',14,'C',2026,8,4);
```

Ahora la columna `ANO` tiene el valor correcto: `2026`

---

## ✅ Verificación

### Comparación de Carteras Generadas

| Cartera | Columna ANO en INSERT | Valor ANO | Estado |
|---------|----------------------|-----------|---------|
| ClickOne (referencia) | ❌ No | NULL/0 | Bug original |
| Web Cartera2.txt | ❌ No | NULL/0 | Antes del fix |
| Web Cartera3.txt | ✅ Sí | 2026 | ✅ Corregido |

### Ejemplo de INSERT Generado

**Cartera3.txt (líneas 6063-6074):**
```sql
-- Poblando tabla de Ciclos
INSERT INTO Ciclos (FECHAI_CICLO,FECHAF_CICLO,NRO_CICLO,CICLO_CERRADO,DIAS_HABILES,ESTATUS,ANO,KPI_Visita_Medica,KPI_Visita_Farmacia) VALUES ('13/01/2026','30/01/2026',1,'C',14,'C',2026,8,4);
INSERT INTO Ciclos (FECHAI_CICLO,FECHAF_CICLO,NRO_CICLO,CICLO_CERRADO,DIAS_HABILES,ESTATUS,ANO,KPI_Visita_Medica,KPI_Visita_Farmacia) VALUES ('02/02/2026','27/02/2026',2,'A',18,'A',2026,8,4);
INSERT INTO Ciclos (FECHAI_CICLO,FECHAF_CICLO,NRO_CICLO,CICLO_CERRADO,DIAS_HABILES,ESTATUS,ANO,KPI_Visita_Medica,KPI_Visita_Farmacia) VALUES ('02/03/2026','27/03/2026',3,'P',20,'P',2026,8,4);
...
```

✅ Todos los registros tienen `ANO=2026`

---

## 📊 Impacto

### Para la Aplicación Android
- ✅ La app ahora puede leer correctamente el año desde la tabla `Ciclos`
- ✅ Los filtros por año funcionarán correctamente
- ✅ La lógica de negocio que depende del año tendrá datos válidos

### Para el Sistema
- ✅ Mejora la integridad de los datos
- ✅ Elimina un bug heredado del sistema ClickOne original
- ✅ Previene problemas futuros relacionados con filtrado por año

---

## 🔗 Issues Relacionados

- **Issue #003**: Campo CICLO en 0 (resuelto previamente)
- **Issue #004**: Columna CICLO faltante en Solicitudes (resuelto previamente)

**Patrón identificado:** Múltiples campos no se estaban incluyendo en los INSERT statements, a pesar de estar definidos en los esquemas de las tablas.

---

## 📝 Archivos Modificados

- `WebApplication1/Services/GeneradorService.cs` (líneas 145-160)

---

## 📎 Archivos de Prueba

- `test_carteras/web/Cartera2.txt` - Antes del fix (sin ANO)
- `test_carteras/web/Cartera3.txt` - Después del fix (con ANO=2026)
- `test_carteras/clickOne/Cartera.txt` - Referencia original (también sin ANO)

---

## 🎯 Criterios de Aceptación

- ✅ Columna `ANO` incluida en INSERT statements
- ✅ Valor correcto del año (2026) en todos los registros
- ✅ Cartera generada y verificada (Cartera3.txt)
- ✅ Código compilado sin errores
- ✅ Documentación completa

---

**Última Actualización:** 2026-02-14  
**Responsable:** Backend  
**Estado:** ✅ Resuelto y Verificado
