# Endpoint de Estado de Sincronización

**Fecha:** 20 de febrero de 2026  
**Versión:** 1.0.0  
**Estado:** ✅ Implementado

## 📋 Resumen

Endpoint de solo lectura que permite a la app Android verificar si los datos de una transmisión ya fueron procesados completamente en el servidor, antes de descargar la cartera actualizada.

## 🎯 Problemática que Resuelve

### Flujo Anterior (Con Problema):
1. APP → Transmite datos al servidor
2. SERVIDOR → Procesa datos (toma tiempo)
3. APP → Descarga Cartera INMEDIATAMENTE ❌
4. Resultado: Cartera puede estar desactualizada

### Flujo Nuevo (Con Verificación):
1. APP → Transmite datos al servidor
2. SERVIDOR → Procesa datos
3. APP → **Consulta estado de sincronización** ✅
4. APP → Espera hasta que `carteraActualizada = true`
5. APP → Descarga Cartera (datos actualizados)

## 🔌 Especificación del Endpoint

### URL
```
GET /api/visitador/{zona}/estado-sincronizacion
```

### Parámetros

| Parámetro | Tipo | Ubicación | Requerido | Descripción | Ejemplo |
|-----------|------|-----------|-----------|-------------|---------|
| `zona` | string | Path | Sí | Zona del visitador | `336` |
| `ano` | int | Query | No | Año del ciclo (default: año actual) | `2026` |
| `ciclo` | int | Query | No | Número de ciclo. Si no se envía, se auto-detecta del ciclo activo en MW_Ciclos | `2` |

### Ejemplo de Llamada
```http
GET /api/visitador/336/estado-sincronizacion?ano=2026&ciclo=2
```

## 📤 Respuestas

### Respuesta Exitosa (200 OK)

**Cuando la cartera está actualizada:**
```json
{
  "zona": "336",
  "ciclo": 2,
  "ano": 2026,
  "carteraActualizada": true,
  "estadoProcesamiento": "COMPLETADO",
  "ultimaTransmision": {
    "fecha": "2026-02-19",
    "hora": "08:18:58 P.M",
    "tipo": "CIERRE DIARIO",
    "timestamp": "2026-02-19T20:18:58"
  },
  "resumenProcesado": {
    "fecha": "2026-02-19",
    "hora": "08:18:57 P.M",
    "visitados": 71,
    "fichados": 144
  }
}
```

**Cuando la cartera está procesando:**
```json
{
  "zona": "336",
  "ciclo": 2,
  "ano": 2026,
  "carteraActualizada": false,
  "estadoProcesamiento": "PROCESANDO",
  "ultimaTransmision": {
    "fecha": "2026-02-19",
    "hora": "08:18:58 P.M",
    "tipo": "CIERRE DIARIO",
    "timestamp": "2026-02-19T20:18:58"
  },
  "resumenProcesado": null
}
```

### Respuesta Sin Datos (404 Not Found)
```json
{
  "zona": "336",
  "ciclo": 2,
  "ano": 2026,
  "mensaje": "No se encontraron transmisiones para esta zona, año y ciclo"
}
```

### Respuesta de Error (500 Internal Server Error)
```json
"Error obteniendo estado de sincronización: [mensaje de error]"
```

## 📊 Campos de Respuesta

### Objeto Principal

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `zona` | string | Zona del visitador consultada |
| `ciclo` | int | Número de ciclo consultado |
| `ano` | int | Año consultado |
| `carteraActualizada` | boolean | **true** = Cartera lista para descargar<br>**false** = Aún procesando |
| `estadoProcesamiento` | string | `"COMPLETADO"` o `"PROCESANDO"` |
| `ultimaTransmision` | object | Información de la última transmisión |
| `resumenProcesado` | object\|null | Datos del resumen procesado (null si aún no se procesó) |

### Objeto `ultimaTransmision`

| Campo | Tipo | Descripción | Ejemplo |
|-------|------|-------------|---------|
| `fecha` | string | Fecha de la transmisión | `"2026-02-19"` |
| `hora` | string | Hora de la transmisión | `"08:18:58 P.M"` |
| `tipo` | string | Tipo de transmisión | `"CIERRE DIARIO"` |
| `timestamp` | string | Timestamp ISO 8601 | `"2026-02-19T20:18:58"` |

### Objeto `resumenProcesado`

| Campo | Tipo | Descripción | Ejemplo |
|-------|------|-------------|---------|
| `fecha` | string | Fecha del resumen procesado | `"2026-02-19"` |
| `hora` | string | Hora del resumen procesado | `"08:18:57 P.M"` |
| `visitados` | int | Cantidad de médicos visitados | `71` |
| `fichados` | int | Cantidad de médicos fichados | `144` |

## 🔍 Lógica de Verificación

El endpoint consulta 3 tablas en SQL Server:

### 1. MD_Transmision (Metadata del Evento)
```sql
SELECT TOP 1 FECHA, HORA, TIPO 
FROM MD_Transmision 
WHERE ZONA = @zona AND CICLO = @ciclo AND NU_ANO = @ano 
ORDER BY FECHA DESC, HORA DESC
```

**Propósito:** Obtener la última transmisión registrada

### 2. MD_Resumen_Transmision (Estado Actual)
```sql
SELECT Fecha, Hora, Visitados, Fichados 
FROM MD_Resumen_Transmision 
WHERE Zona = @zona AND CICLO = @ciclo AND NU_ANO = @ano
```

**Propósito:** Verificar que el resumen actual fue actualizado  
**Comportamiento:** Se SOBRESCRIBE en cada transmisión (1 registro por zona/ciclo/año)

### 3. MD_Resumen_Transmision_Acumulada (Historial)
```sql
SELECT TOP 1 Fecha, Hora 
FROM MD_Resumen_Transmision_Acumulada 
WHERE Zona = @zona AND CICLO = @ciclo AND NU_ANO = @ano 
ORDER BY Fecha DESC, Hora DESC
```

**Propósito:** Verificar que el registro histórico fue guardado  
**Comportamiento:** Se INSERTA en cada transmisión (múltiples registros)

### Criterio de Verificación

```
carteraActualizada = true SI Y SOLO SI:
  - Existe registro en MD_Resumen_Transmision con fecha/hora coincidente (±3 segundos)
  Y
  - Existe registro en MD_Resumen_Transmision_Acumulada con fecha/hora coincidente (±3 segundos)
```

## ⚙️ Consideraciones Técnicas

### 1. Tolerancia de Tiempo
- Se acepta una diferencia de **±3 segundos** entre las horas de las tablas
- Razón: `MD_Transmision` puede tener 1-2 segundos de diferencia con las tablas de resumen

### 2. Normalización de Formato de Hora
- Formato inconsistente en BD: `"08:18:57 P.M"` vs `"08:18:57 P.M."`
- Solución: Se elimina el punto final antes de comparar

### 3. Conversión de Hora 12h a 24h
- Las horas se almacenan en formato 12 horas con AM/PM
- Se convierten a formato 24 horas para comparación precisa

### 4. Registros con Datos en Cero
- Son válidos (transmisiones sin visitas realizadas)
- No afectan la lógica de verificación

## 🚀 Uso Recomendado en la App Android

### Flujo de Polling
```kotlin
// Después de transmitir datos
suspend fun esperarCarteraActualizada(zona: String, ano: Int, ciclo: Int) {
    var intentos = 0
    val maxIntentos = 30 // 30 intentos = 5 minutos
    
    while (intentos < maxIntentos) {
        val estado = api.getEstadoSincronizacion(zona, ano, ciclo)
        
        if (estado.carteraActualizada) {
            // Cartera lista, proceder a descargar
            descargarCartera(zona, ano, ciclo)
            return
        }
        
        // Esperar 10 segundos antes de reintentar
        delay(10000)
        intentos++
    }
    
    // Timeout: mostrar mensaje al usuario
    mostrarError("La sincronización está tomando más tiempo de lo esperado")
}
```

### Manejo de Errores
```kotlin
try {
    val estado = api.getEstadoSincronizacion(zona, ano, ciclo)
    // Procesar respuesta
} catch (e: HttpException) {
    when (e.code()) {
        404 -> mostrarError("No hay transmisiones registradas")
        500 -> mostrarError("Error en el servidor")
        else -> mostrarError("Error desconocido")
    }
}
```

## 📝 Notas Importantes

### ✅ Lo que hace este endpoint:
- Consulta el estado de procesamiento de transmisiones
- Verifica que los datos fueron guardados en las tablas de resumen
- Proporciona información de la última transmisión

### ❌ Lo que NO hace este endpoint:
- NO modifica ninguna tabla
- NO genera carteras
- NO procesa transmisiones
- NO afecta el flujo de sincronización existente

## 🔗 Endpoints Relacionados

| Endpoint | Propósito |
|----------|-----------|
| `GET /api/visitador/{id}/cartera-txt` | Descarga la cartera en formato SQL |
| `GET /api/visitador/{id}/kpis` | Obtiene KPIs del visitador |
| `POST /api/visitador/{id}/sync` | Sincronización de datos (en desarrollo) |

## 📊 Ejemplo de Datos Reales

### Zona 336, Ciclo 2, Año 2026

**MD_Transmision:**
```
Fecha: 2026-02-19
Hora: 08:18:58 P.M
Tipo: CIERRE DIARIO
```

**MD_Resumen_Transmision:**
```
Fecha: 2026-02-19
Hora: 08:18:57 P.M
Visitados: 71
Fichados: 144
```

**MD_Resumen_Transmision_Acumulada:**
```
19 registros históricos
Último: 2026-02-19 08:18:57 P.M
```

**Resultado:** `carteraActualizada = true` ✅

## 🐛 Troubleshooting

### Problema: Siempre retorna `carteraActualizada = false`
**Posibles causas:**
- El proceso de sincronización aún no terminó
- Hay un error en el proceso de actualización de tablas
- Las horas tienen más de 3 segundos de diferencia

**Solución:** Verificar manualmente las 3 tablas en SQL Server

### Problema: Retorna 404 Not Found
**Causa:** No hay transmisiones registradas para esa zona/ciclo/año

**Solución:** Verificar que los parámetros sean correctos

### Problema: Timeout en la app
**Causa:** El proceso de sincronización está tomando mucho tiempo

**Solución:** Aumentar el tiempo de espera o revisar el proceso de sincronización en el servidor

## 📅 Historial de Cambios

| Versión | Fecha | Cambios |
|---------|-------|---------|
| 1.0.0 | 2026-02-20 | Implementación inicial del endpoint |

## 👥 Contacto

Para preguntas o problemas con este endpoint, contactar al equipo de desarrollo.
