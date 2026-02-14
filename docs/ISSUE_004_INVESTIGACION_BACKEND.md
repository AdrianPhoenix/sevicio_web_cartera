# 🔍 Issue #004 - Investigación Backend: Comentarios Vacíos

**Fecha de Investigación:** 14 de Febrero, 2026  
**Issue Original:** #004 - Comentarios Vacíos desde Backend  
**Investigador:** Backend Team  
**Estado:** 🔍 En Investigación

---

## 📋 Resumen del Problema

La app Android no recibe comentarios en la tabla `Solicitudes` porque el archivo `Cartera.txt` no contiene datos de comentarios, a pesar de que el backend está configurado para exportarlos.

---

## ✅ Hallazgos del Código Backend

### 1. El Backend SÍ Intenta Exportar Comentarios

**Archivo:** `WebApplication1/Services/GeneradorService.cs`

**Línea 56:** La tabla `MD_Solicitudes` está en la lista de tablas a copiar:
```csharp
tablasACopiar.AddRange(new[]
{
    "MD_Eliminar", "MD_Farmacias_Detalles", "MD_Farmacias_Detalles_Productos", 
    "MD_HistorialConceptoDias", "MD_Hospital_Detalles", "MD_Hospital_Detalles_Medicos", 
    "MD_Inclusiones", "MD_Solicitudes",  // ← AQUÍ ESTÁ
    "MD_Visita_Detalles", "MD_Visitas", "MD_Ayuda_Visual", "MD_Ayuda_Visual_FE",
    "MD_Ayuda_Visual_MP4", "MD_Ayuda_Visual_MP4_FE"
});
```

**Línea 71:** También se copian datos históricos:
```csharp
var tablasHistoricas = new[]
{
    "MD_Visita_Detalles", "MD_Visitas", "MD_Solicitudes",  // ← AQUÍ TAMBIÉN
    "MD_Hospital_Detalles", "MD_Hospital_Detalles_Medicos",
    "MD_Farmacias_Detalles", "MD_Farmacias_Detalles_Productos"
};
```

### 2. Query Utilizada para Exportar

**Método:** `GetDefaultQuery()` - Línea 299

**Query generada:**
```sql
SELECT * FROM MD_Solicitudes 
WHERE NU_ANO = @ano 
  AND CICLO = @ciclo 
  AND ZONA = @visitadorId
```

**Parámetros de ejemplo:**
- `@ano` = 2026
- `@ciclo` = 2
- `@visitadorId` = 336

### 3. Esquema de Tabla en Cartera.txt

**Línea 449:** Definición de tabla `solicitudes` en SQLite:
```sql
CREATE TABLE "solicitudes" (
    "REGISTRO" TEXT(5), 
    "DOCTOR" TEXT(40), 
    "ZONA" TEXT(7), 
    "CICLO" INTEGER(11), 
    "COMENTARIOS_PERSONALES" TEXT(255), 
    "COMETARIO_PUBLICOS" TEXT(255)  -- Nota: typo en nombre
);
```

**Línea 375:** Definición de tabla histórica `hsolicitudes`:
```sql
CREATE TABLE "hsolicitudes" (
    "REGISTRO" TEXT(5), 
    "DOCTOR" TEXT(40), 
    "ZONA" TEXT(7), 
    "CICLO" INTEGER(11), 
    "COMENTARIOS_PERSONALES" TEXT(255), 
    "COMETARIO_PUBLICOS" TEXT(255)
);
```

---

## 🐛 Posibles Causas del Problema

### Causa #1: Tabla MD_Solicitudes Vacía en SQL Server ⚠️ MÁS PROBABLE

**Hipótesis:** La tabla `MD_Solicitudes` en SQL Server no tiene datos.

**Razones posibles:**
1. Ningún visitador ha creado comentarios aún
2. Los comentarios se guardan en otra tabla
3. El proceso de sincronización desde la app no está funcionando
4. Los comentarios se borran periódicamente

**Cómo verificar:**
```sql
-- En SQL Server
SELECT COUNT(*) FROM MD_Solicitudes;

SELECT TOP 10 * FROM MD_Solicitudes 
WHERE ZONA = '336' AND CICLO = 2 AND NU_ANO = 2026;
```

### Causa #2: Nombres de Columnas Incorrectos ⚠️ POSIBLE

**Hipótesis:** La tabla `MD_Solicitudes` tiene nombres de columnas diferentes a los esperados.

**Query esperado:**
```sql
SELECT * FROM MD_Solicitudes 
WHERE NU_ANO = @ano AND CICLO = @ciclo AND ZONA = @visitadorId
```

**Problema:** Si la tabla tiene columnas `ANO` en lugar de `NU_ANO`, el query fallará.

**Cómo verificar:**
```sql
-- En SQL Server
SELECT COLUMN_NAME, DATA_TYPE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'MD_Solicitudes'
ORDER BY ORDINAL_POSITION;
```

**Columnas esperadas:**
- `NU_ANO` (o `ANO`)
- `CICLO`
- `ZONA`
- `REGISTRO`
- `DOCTOR`
- `COMENTARIOS_PERSONALES`
- `COMETARIO_PUBLICOS` (con typo)

### Causa #3: Tabla No Existe en SQL Server ⚠️ MENOS PROBABLE

**Hipótesis:** La tabla `MD_Solicitudes` no existe en la base de datos.

**Cómo verificar:**
```sql
-- En SQL Server
SELECT * FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME = 'MD_Solicitudes';
```

### Causa #4: Permisos de Lectura ⚠️ POCO PROBABLE

**Hipótesis:** El usuario de la conexión no tiene permisos para leer `MD_Solicitudes`.

**Cómo verificar:**
```sql
-- En SQL Server
SELECT * FROM fn_my_permissions('MD_Solicitudes', 'OBJECT');
```

---

## 🔧 Plan de Verificación

### Paso 1: Verificar Existencia de Tabla

```sql
USE [TU_BASE_DE_DATOS];
GO

-- Verificar si la tabla existe
SELECT TABLE_NAME, TABLE_TYPE 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME LIKE '%Solicitudes%';
```

**Resultado esperado:**
```
TABLE_NAME          TABLE_TYPE
MD_Solicitudes      BASE TABLE
```

### Paso 2: Verificar Estructura de Tabla

```sql
-- Ver columnas de la tabla
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    CHARACTER_MAXIMUM_LENGTH,
    IS_NULLABLE
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'MD_Solicitudes'
ORDER BY ORDINAL_POSITION;
```

**Resultado esperado:**
```
COLUMN_NAME                 DATA_TYPE    MAX_LENGTH    IS_NULLABLE
NU_ANO (o ANO)             int          NULL          NO/YES
CICLO                       int          NULL          NO/YES
ZONA                        varchar      7             NO/YES
REGISTRO                    varchar      5             NO/YES
DOCTOR                      varchar      40            YES
COMENTARIOS_PERSONALES      varchar      255           YES
COMETARIO_PUBLICOS          varchar      255           YES
```

### Paso 3: Verificar Datos en Tabla

```sql
-- Contar registros totales
SELECT COUNT(*) AS TotalComentarios FROM MD_Solicitudes;

-- Ver datos recientes
SELECT TOP 10 * FROM MD_Solicitudes 
ORDER BY NU_ANO DESC, CICLO DESC;

-- Buscar datos específicos del issue
SELECT * FROM MD_Solicitudes 
WHERE ZONA = '336' 
  AND CICLO = 2 
  AND NU_ANO = 2026;
```

**Resultado esperado:**
- Si hay comentarios: Debería retornar registros
- Si no hay comentarios: Tabla vacía (0 registros)

### Paso 4: Verificar Datos Históricos

```sql
-- Ver todos los ciclos con comentarios
SELECT 
    NU_ANO, 
    CICLO, 
    ZONA, 
    COUNT(*) AS CantidadComentarios
FROM MD_Solicitudes
GROUP BY NU_ANO, CICLO, ZONA
ORDER BY NU_ANO DESC, CICLO DESC;
```

### Paso 5: Probar Query del Backend

```sql
-- Simular el query exacto que usa el backend
DECLARE @ano INT = 2026;
DECLARE @ciclo INT = 2;
DECLARE @visitadorId VARCHAR(7) = '336';

SELECT * FROM MD_Solicitudes 
WHERE NU_ANO = @ano 
  AND CICLO = @ciclo 
  AND ZONA = @visitadorId;
```

**Resultado esperado:**
- Si retorna datos: Backend debería exportarlos
- Si no retorna datos: Confirma que tabla está vacía

---

## 🔍 Verificación en Cartera.txt Generado

### Buscar Sección de Solicitudes

Después de generar un `Cartera.txt`, buscar:

```bash
# En el archivo Cartera.txt
grep -A 10 "INSERT INTO solicitudes" Cartera.txt
```

**Resultado esperado si hay datos:**
```sql
INSERT INTO solicitudes (REGISTRO, DOCTOR, ZONA, CICLO, COMENTARIOS_PERSONALES, COMETARIO_PUBLICOS) 
VALUES ('7', 'Dr. Juan Pérez', '336', 2, 'Comentario personal', 'Comentario público');
```

**Resultado si NO hay datos:**
- No aparece ningún `INSERT INTO solicitudes`
- Solo aparece el `CREATE TABLE solicitudes`

---

## 📊 Diagnóstico Basado en Resultados

### Escenario A: Tabla Vacía
**Síntomas:**
- `SELECT COUNT(*)` retorna 0
- No hay `INSERT INTO solicitudes` en Cartera.txt

**Causa:** Ningún visitador ha creado comentarios

**Solución:**
1. Crear comentarios de prueba en la app
2. Sincronizar con el servidor
3. Verificar que se guardan en `MD_Solicitudes`
4. Generar nueva cartera

### Escenario B: Columnas Incorrectas
**Síntomas:**
- Tabla tiene datos pero query falla
- Error en logs del backend

**Causa:** Nombres de columnas no coinciden

**Solución:**
1. Actualizar query en `GetDefaultQuery()` para usar nombres correctos
2. O renombrar columnas en SQL Server

### Escenario C: Tabla No Existe
**Síntomas:**
- `INFORMATION_SCHEMA.TABLES` no muestra la tabla
- Error "Invalid object name" en logs

**Causa:** Tabla no fue creada en SQL Server

**Solución:**
1. Crear tabla `MD_Solicitudes` en SQL Server
2. Definir estructura correcta
3. Configurar permisos

---

## 🛠️ Soluciones Propuestas

### Solución 1: Si Tabla Está Vacía (Más Probable)

**Acción:** Crear datos de prueba

```sql
-- Insertar comentarios de prueba
INSERT INTO MD_Solicitudes (NU_ANO, CICLO, ZONA, REGISTRO, DOCTOR, COMENTARIOS_PERSONALES, COMETARIO_PUBLICOS)
VALUES 
    (2026, 2, '336', '7', 'Dr. Juan Pérez', 'Comentario de prueba 1', 'Comentario público 1'),
    (2026, 2, '336', '58', 'Dra. María López', 'Comentario de prueba 2', 'Comentario público 2');
```

**Luego:**
1. Generar nueva cartera: `http://localhost:5130/api/visitador/336/cartera?ano=2026&ciclo=2`
2. Verificar que aparezcan los `INSERT INTO solicitudes` en el archivo
3. Probar en la app Android

### Solución 2: Si Nombres de Columnas Son Diferentes

**Opción A:** Actualizar el código backend

```csharp
// En GetDefaultQuery(), agregar caso especial para MD_Solicitudes
if (nombreTabla == "MD_Solicitudes")
{
    return $"SELECT * FROM {nombreTabla} WHERE ANO=@ano AND CICLO=@ciclo AND ZONA=@visitadorId";
    // Nota: Usar ANO en lugar de NU_ANO si ese es el nombre real
}
```

**Opción B:** Renombrar columnas en SQL Server

```sql
EXEC sp_rename 'MD_Solicitudes.ANO', 'NU_ANO', 'COLUMN';
```

### Solución 3: Agregar Logging para Debugging

**Modificar `ObtenerDatosTablaAsync()`:**

```csharp
// Agregar después de ejecutar el query
if (nombreTabla == "MD_Solicitudes")
{
    Console.WriteLine($"[DEBUG] MD_Solicitudes query: {query}");
    Console.WriteLine($"[DEBUG] Parámetros: ano={ano}, ciclo={ciclo}, visitadorId={visitadorId}");
    Console.WriteLine($"[DEBUG] Registros encontrados: {(reader.HasRows ? "Sí" : "No")}");
}
```

---

## 📝 Checklist de Verificación

### Backend (SQL Server)
- [ ] Verificar que tabla `MD_Solicitudes` existe
- [ ] Verificar estructura de columnas (nombres y tipos)
- [ ] Verificar que hay datos en la tabla
- [ ] Probar query manualmente con parámetros reales
- [ ] Verificar permisos del usuario de conexión

### Generador (WebApplication1)
- [ ] Verificar que `MD_Solicitudes` está en lista de tablas
- [ ] Verificar query en `GetDefaultQuery()`
- [ ] Generar Cartera.txt de prueba
- [ ] Buscar `INSERT INTO solicitudes` en archivo generado
- [ ] Verificar logs del generador (si existen)

### App Android
- [ ] Verificar que tabla `Solicitudes` se crea correctamente
- [ ] Verificar que `crearBD()` ejecuta los INSERTs
- [ ] Verificar query de consulta en `ComentariosSearch.java`
- [ ] Probar crear comentario local y verificar que se guarda

---

## 🎯 Próximos Pasos Inmediatos

1. **Ejecutar queries de verificación** en SQL Server (Paso 1-5)
2. **Documentar resultados** de cada query
3. **Generar Cartera.txt de prueba** y verificar contenido
4. **Determinar causa raíz** basado en resultados
5. **Aplicar solución correspondiente**
6. **Probar con app Android**

---

## 📞 Información de Contacto

**Para ejecutar verificaciones:**
- Acceso a SQL Server requerido
- Connection string en `appsettings.json`
- Permisos de lectura en tabla `MD_Solicitudes`

**Para probar generación:**
```bash
# Generar cartera de prueba
GET http://localhost:5130/api/visitador/336/cartera?ano=2026&ciclo=2

# O en producción
GET http://mdnconsultores.com:8080/api/visitador/336/cartera?ano=2026&ciclo=2
```

---

**Última Actualización:** 14 de Febrero, 2026  
**Estado:** 🔍 Pendiente Ejecución de Verificaciones  
**Prioridad:** Media  
**Tiempo Estimado:** 30-60 minutos para verificación completa
