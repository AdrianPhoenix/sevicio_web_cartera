# Guía de Pruebas - Endpoint Estado de Sincronización

**Fecha:** 20 de febrero de 2026  
**Endpoint:** `GET /api/visitador/{zona}/estado-sincronizacion`

## 🧪 Casos de Prueba

### Caso 1: Zona con Transmisión Completa (336)
**Descripción:** Zona que tiene transmisión procesada completamente

**Request:**
```http
GET /api/visitador/336/estado-sincronizacion?ano=2026&ciclo=2
```

**Respuesta Esperada:** `200 OK`
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

**Validaciones:**
- ✅ `carteraActualizada` debe ser `true`
- ✅ `estadoProcesamiento` debe ser `"COMPLETADO"`
- ✅ `ultimaTransmision` debe tener todos los campos
- ✅ `resumenProcesado` NO debe ser `null`

---

### Caso 2: Zona sin Año Especificado (usa año actual)
**Descripción:** Prueba con parámetro año omitido

**Request:**
```http
GET /api/visitador/336/estado-sincronizacion?ciclo=2
```

**Respuesta Esperada:** `200 OK` (usa año actual = 2026)

**Validaciones:**
- ✅ Debe usar año 2026 por defecto
- ✅ Debe retornar los mismos datos que el Caso 1

---

### Caso 3: Zona sin Ciclo Especificado (usa ciclo 1)
**Descripción:** Prueba con parámetro ciclo omitido

**Request:**
```http
GET /api/visitador/336/estado-sincronizacion?ano=2026
```

**Respuesta Esperada:** `200 OK` o `404 Not Found` (depende si hay datos para ciclo 1)

**Validaciones:**
- ✅ Debe usar ciclo 1 por defecto

---

### Caso 4: Zona sin Transmisiones
**Descripción:** Zona que no tiene transmisiones registradas

**Request:**
```http
GET /api/visitador/999/estado-sincronizacion?ano=2026&ciclo=2
```

**Respuesta Esperada:** `404 Not Found`
```json
{
  "zona": "999",
  "ciclo": 2,
  "ano": 2026,
  "mensaje": "No se encontraron transmisiones para esta zona, año y ciclo"
}
```

**Validaciones:**
- ✅ Status code debe ser `404`
- ✅ Debe incluir mensaje descriptivo

---

### Caso 5: Zona con Transmisión en Proceso
**Descripción:** Zona que transmitió pero aún no se procesó completamente

**Request:**
```http
GET /api/visitador/[zona_en_proceso]/estado-sincronizacion?ano=2026&ciclo=2
```

**Respuesta Esperada:** `200 OK`
```json
{
  "zona": "[zona]",
  "ciclo": 2,
  "ano": 2026,
  "carteraActualizada": false,
  "estadoProcesamiento": "PROCESANDO",
  "ultimaTransmision": {
    "fecha": "2026-02-20",
    "hora": "10:30:00 A.M",
    "tipo": "CIERRE DIARIO",
    "timestamp": "2026-02-20T10:30:00"
  },
  "resumenProcesado": null
}
```

**Validaciones:**
- ✅ `carteraActualizada` debe ser `false`
- ✅ `estadoProcesamiento` debe ser `"PROCESANDO"`
- ✅ `resumenProcesado` debe ser `null`

---

## 🔧 Herramientas de Prueba

### 1. Usando cURL
```bash
# Caso 1: Zona 336 completa
curl -X GET "http://localhost:5000/api/visitador/336/estado-sincronizacion?ano=2026&ciclo=2"

# Caso 2: Sin año (usa actual)
curl -X GET "http://localhost:5000/api/visitador/336/estado-sincronizacion?ciclo=2"

# Caso 4: Zona inexistente
curl -X GET "http://localhost:5000/api/visitador/999/estado-sincronizacion?ano=2026&ciclo=2"
```

### 2. Usando PowerShell
```powershell
# Caso 1: Zona 336 completa
Invoke-RestMethod -Uri "http://localhost:5000/api/visitador/336/estado-sincronizacion?ano=2026&ciclo=2" -Method Get

# Caso 4: Zona inexistente (capturar error)
try {
    Invoke-RestMethod -Uri "http://localhost:5000/api/visitador/999/estado-sincronizacion?ano=2026&ciclo=2" -Method Get
} catch {
    Write-Host "Error esperado: $_"
}
```

### 3. Usando Postman
1. Crear nueva request GET
2. URL: `http://localhost:5000/api/visitador/336/estado-sincronizacion`
3. Params:
   - `ano`: `2026`
   - `ciclo`: `2`
4. Send

### 4. Usando el navegador
```
http://localhost:5000/api/visitador/336/estado-sincronizacion?ano=2026&ciclo=2
```

---

## 📊 Verificación en Base de Datos

### Consulta 1: Verificar MD_Transmision
```sql
SELECT TOP 1 * 
FROM MD_Transmision 
WHERE ZONA = 336 AND CICLO = 2 AND NU_ANO = 2026 
ORDER BY FECHA DESC, HORA DESC;
```

**Resultado Esperado:**
- Debe retornar 1 registro con la última transmisión

### Consulta 2: Verificar MD_Resumen_Transmision
```sql
SELECT * 
FROM MD_Resumen_Transmision 
WHERE Zona = 336 AND CICLO = 2 AND NU_ANO = 2026;
```

**Resultado Esperado:**
- Debe retornar 1 registro (se sobrescribe)
- Fecha y Hora deben coincidir con MD_Transmision (±3 segundos)

### Consulta 3: Verificar MD_Resumen_Transmision_Acumulada
```sql
SELECT * 
FROM MD_Resumen_Transmision_Acumulada 
WHERE Zona = 336 AND CICLO = 2 AND NU_ANO = 2026 
ORDER BY Fecha DESC, Hora DESC;
```

**Resultado Esperado:**
- Debe retornar múltiples registros (historial)
- El último registro debe coincidir con MD_Transmision (±3 segundos)

---

## ✅ Checklist de Validación

### Funcionalidad Básica
- [ ] El endpoint responde en `http://localhost:5000/api/visitador/{zona}/estado-sincronizacion`
- [ ] Acepta parámetros `ano` y `ciclo` en query string
- [ ] Retorna JSON válido
- [ ] Status codes correctos (200, 404, 500)

### Lógica de Negocio
- [ ] `carteraActualizada = true` cuando existe en ambas tablas de resumen
- [ ] `carteraActualizada = false` cuando falta en alguna tabla
- [ ] Tolerancia de ±3 segundos funciona correctamente
- [ ] Maneja formato de hora inconsistente (con/sin punto final)

### Casos Edge
- [ ] Zona inexistente retorna 404
- [ ] Año/ciclo sin datos retorna 404
- [ ] Parámetros omitidos usan valores por defecto
- [ ] Errores de BD retornan 500 con mensaje descriptivo

### Datos de Respuesta
- [ ] Todos los campos requeridos están presentes
- [ ] Tipos de datos correctos (string, int, boolean)
- [ ] Timestamp en formato ISO 8601
- [ ] `resumenProcesado` es null cuando no hay datos

---

## 🐛 Problemas Comunes y Soluciones

### Problema 1: Error 500 al llamar el endpoint
**Causa posible:** Error de conexión a base de datos

**Solución:**
1. Verificar connection string en `appsettings.json`
2. Verificar que SQL Server esté corriendo
3. Verificar permisos de acceso a las tablas

### Problema 2: Siempre retorna `carteraActualizada = false`
**Causa posible:** Diferencia de tiempo mayor a 3 segundos

**Solución:**
1. Ejecutar las consultas SQL manualmente
2. Verificar las horas en las 3 tablas
3. Ajustar tolerancia si es necesario

### Problema 3: 404 para zona que sí tiene datos
**Causa posible:** Parámetros incorrectos (año/ciclo)

**Solución:**
1. Verificar año y ciclo en la BD
2. Asegurar que los parámetros coincidan exactamente

### Problema 4: Formato de hora no se parsea
**Causa posible:** Formato de hora inesperado en BD

**Solución:**
1. Verificar formato exacto en BD
2. Ajustar método `ParsearHora` si es necesario

---

## 📈 Métricas de Éxito

### Performance
- ⏱️ Tiempo de respuesta < 500ms
- 🔄 Maneja múltiples requests concurrentes
- 💾 No consume memoria excesiva

### Confiabilidad
- ✅ 100% de requests válidos retornan respuesta
- ✅ Errores de BD se manejan correctamente
- ✅ No hay memory leaks

### Precisión
- ✅ Lógica de verificación es correcta
- ✅ Tolerancia de tiempo funciona
- ✅ Datos retornados son precisos

---

## 📝 Reporte de Pruebas

### Plantilla de Reporte

```
Fecha: [fecha]
Tester: [nombre]
Versión: 1.0.0

CASOS PROBADOS:
[ ] Caso 1: Zona con transmisión completa
[ ] Caso 2: Zona sin año especificado
[ ] Caso 3: Zona sin ciclo especificado
[ ] Caso 4: Zona sin transmisiones
[ ] Caso 5: Zona con transmisión en proceso

RESULTADOS:
- Casos exitosos: X/5
- Casos fallidos: X/5
- Bugs encontrados: X

BUGS:
1. [Descripción del bug]
   - Severidad: Alta/Media/Baja
   - Pasos para reproducir: [pasos]
   - Resultado esperado: [esperado]
   - Resultado actual: [actual]

OBSERVACIONES:
[Comentarios adicionales]

CONCLUSIÓN:
[ ] Aprobado para producción
[ ] Requiere correcciones
```

---

## 🚀 Siguiente Paso

Una vez completadas las pruebas exitosamente:
1. Documentar resultados
2. Integrar con la app Android
3. Realizar pruebas de integración
4. Desplegar a producción
