# Documentación de Endpoints de la API - Servicio Web Generador

## ✅ **PROYECTO COMPLETADO - API TOTALMENTE FUNCIONAL**

Este documento describe los endpoints del servicio web `WebApplication1` que **reemplaza exitosamente** la aplicación ClickOnce original. **Las aplicaciones Android funcionan perfectamente** con las carteras generadas por esta API.

---

## 🎉 **Estado del Proyecto: ÉXITO COMPLETO**

- ✅ **Migración exitosa** de ClickOnce a Web Service
- ✅ **Compatibilidad total** con aplicaciones Android existentes
- ✅ **Paridad funcional** completa con el sistema original
- ✅ **Testing exitoso** - Apps Android llegan al dashboard sin errores

---

## 🚀 Endpoints Implementados y Funcionando

### 1. Obtener Años Disponibles ✅

*   **Descripción**: Lista de años con datos de ciclos disponibles. Equivale a la funcionalidad de la aplicación ClickOnce original.
*   **Ruta**: `GET /api/visitador/annios`
*   **Parámetros**: Ninguno.
*   **Estado**: **FUNCIONANDO PERFECTAMENTE**
*   **Respuesta Exitosa (200 OK)**:
    ```json
    ["2025", "2024", "2023"]
    ```
*   **Ejemplo de URL**: `https://localhost:5000/api/visitador/annios`

### 2. Obtener Lista de Visitadores Activos ✅

*   **Descripción**: Lista completa de visitadores activos. Permite identificar el `id` necesario para generar carteras.
*   **Ruta**: `GET /api/visitador`
*   **Parámetros**: Ninguno.
*   **Estado**: **FUNCIONANDO PERFECTAMENTE**
*   **Respuesta Exitosa (200 OK)**:
    ```json
    [
      {
        "id_VisitadoresHistorial": 334,
        "tx_Nombre": "Juan Perez",
        "tx_Apellido": "Perez",
        "tx_Usuario": "",
        "tx_Password": "",
        "id_Empresa": 10,
        "id_Linea": 1,
        "bo_Activo": true
      }
    ]
    ```
*   **Ejemplo de URL**: `https://localhost:5000/api/visitador`

### 3. Obtener Google Registration ID ✅

*   **Descripción**: ID de registro de Google para un visitador específico.
*   **Ruta**: `GET /api/visitador/{id}/google-registration`
*   **Parámetros**: `id` (ID del visitador)
*   **Estado**: **FUNCIONANDO PERFECTAMENTE**
*   **Respuesta Exitosa (200 OK)**:
    ```
    "ABC123XYZ"
    ```
*   **Ejemplo de URL**: `https://localhost:5000/api/visitador/334/google-registration`

### 4. **🏆 Generar Cartera (Endpoint Principal) ✅**

*   **Descripción**: **ENDPOINT CRÍTICO** - Genera archivos Cartera.txt que son **100% compatibles** con aplicaciones Android existentes. **Reemplaza completamente** la funcionalidad ClickOnce.
*   **Ruta**: `GET /api/visitador/{id}/cartera`
*   **Parámetros**:
    *   `id` (obligatorio): ID del visitador
    *   `ano` (opcional): Año, por defecto actual
    *   `ciclo` (opcional): Ciclo. Si no se envía, se auto-detecta de `MW_Ciclos` (NU_Estatus=1). Si se envía explícitamente, se respeta.
    *   `limpia` (opcional): Limpieza de datos, por defecto false
    *   `cicloAbierto` (opcional): Marcar ciclo como abierto, por defecto false
*   **Estado**: **✅ FUNCIONANDO PERFECTAMENTE - PROBADO CON ANDROID**
*   **Respuesta**: Archivo `Cartera.txt` para descarga
*   **Tipo de Contenido**: `text/plain`
*   **Compatibilidad Android**: **100% VERIFICADA** ✅

**Ejemplo de URL exitosa**:
```
https://localhost:5000/api/visitador/334/cartera?ano=2026&ciclo=1&limpia=false&cicloAbierto=false
http://mdnconsultores.com:8080/api/visitador/334/cartera-txt?ano=2026&ciclo=1&cicloAbierto=true
```

### 5. **📊 Obtener KPIs de Visitador ✅**

*   **Descripción**: Obtiene los KPIs (Key Performance Indicators) de un visitador para un ciclo y año específico. Consulta la tabla `MW_Ciclos` para obtener las metas de visitas médicas y farmacéuticas configuradas, junto con información del ciclo.
*   **Ruta**: `GET /api/visitador/{id}/kpis`
*   **Parámetros**:
    *   `id` (obligatorio): ID del visitador (ej: 336 para "Visitador Caracas Zona 3")
    *   `ano` (opcional): Año del ciclo. Si no se especifica, usa el año actual
    *   `ciclo` (opcional): Número de ciclo (1-12). Por defecto: 1
*   **Estado**: **✅ FUNCIONANDO PERFECTAMENTE - PROBADO**
*   **Tipo de Contenido**: `application/json`

**Respuesta Exitosa (200 OK)**:
```json
{
  "visitadorId": 336,
  "ano": 2026,
  "ciclo": 1,
  "kpiVisitaMedica": 8,
  "kpiVisitaFarmacia": 4,
  "fechaInicio": "01/01/2026",
  "fechaFin": "31/01/2026",
  "diasHabiles": 22,
  "estatus": "A"
}
```

**Respuesta de Error (404 Not Found)**:
```json
"No se encontraron KPIs para el visitador 336, año 2026, ciclo 1"
```

**Respuesta de Error (500 Internal Server Error)**:
```json
"Error obteniendo KPIs: [mensaje de error]"
```

**Ejemplos de Uso**:
```bash
# Obtener KPIs del ciclo actual
GET http://localhost:5130/api/visitador/336/kpis

# Obtener KPIs de un ciclo específico
GET http://localhost:5130/api/visitador/336/kpis?ano=2026&ciclo=1

# Obtener KPIs de otro visitador
GET http://localhost:5130/api/visitador/334/kpis?ano=2025&ciclo=12

# PRODUCCIÓN - URL Real
GET http://mdnconsultores.com:8080/api/visitador/336/kpis?ano=2026&ciclo=2
```

**Descripción de Campos de Respuesta**:

| Campo | Tipo | Descripción |
|-------|------|-------------|
| `visitadorId` | number | ID del visitador consultado |
| `ano` | number | Año del ciclo |
| `ciclo` | number | Número del ciclo (1-12) |
| `kpiVisitaMedica` | number | Meta de médicos a visitar por día (típicamente 8) |
| `kpiVisitaFarmacia` | number | Meta de farmacias a visitar por día (típicamente 4) |
| `fechaInicio` | string | Fecha de inicio del ciclo (formato: dd/MM/yyyy) |
| `fechaFin` | string | Fecha de fin del ciclo (formato: dd/MM/yyyy) |
| `diasHabiles` | number | Cantidad de días hábiles en el ciclo |
| `estatus` | string | Estado del ciclo: "A" (Activo), "C" (Cerrado), "P" (Pendiente) |

**Notas Importantes**:
*   Los KPIs son configurables por ciclo en la tabla `MW_Ciclos` de la base de datos
*   Los valores por defecto son: 8 médicos/día y 4 farmacias/día
*   El `visitadorId` se incluye en la respuesta para facilitar el tracking
*   El estatus del ciclo se determina según el campo `NU_Estatus` en la base de datos:
    *   `1` = Activo (A)
    *   `2` = Cerrado (C)
    *   Otros = Pendiente (P)
*   Si el ciclo no existe, se devuelve un error 404

**Casos de Uso**:
*   Aplicaciones móviles que necesitan mostrar las metas del visitador
*   Dashboards de seguimiento de desempeño
*   Reportes de cumplimiento de KPIs
*   Planificación de rutas basada en metas diarias

---

## 📊 **Métricas de Éxito Verificadas**

| Funcionalidad | ClickOnce | Web Service | Estado |
|---------------|-----------|-------------|---------|
| Generación de carteras | ✅ | ✅ | **IDÉNTICO** |
| Compatibilidad Android | ✅ | ✅ | **VERIFICADO** |
| Esquemas de tablas | 114 | 114 | **COMPLETO** |
| Datos históricos | ✅ | ✅ | **FUNCIONAL** |
| Login en app | ✅ | ✅ | **EXITOSO** |
| Dashboard accesible | ✅ | ✅ | **CONFIRMADO** |

---

## 🔧 **Detalles Técnicos de la Migración**

### **Problemas Resueltos Exitosamente:**

1. **Esquemas de Tablas** ✅
   - 114 tablas con esquemas exactos
   - Tipos de datos correctos (TEXT, INTEGER, REAL)
   - Nombres de columnas idénticos

2. **Tablas Críticas Corregidas** ✅
   - `ayuda_visual` - Esquema de visitas corregido
   - `versiones` - Columna PVM agregada
   - `mw_umbrales` - Tabla faltante implementada
   - `resumen_transmision*` - Columnas Fecha/Hora agregadas

3. **Lógica de Datos** ✅
   - Copia de últimos 2 ciclos
   - Exclusión correcta de columnas
   - Transformaciones de datos
   - Manejo de valores NULL

---

## 🚀 **Listo para Producción**

### **Recomendaciones de Deployment:**

1. **Configuración IIS** - Windows Server
2. **SSL/HTTPS** - Certificados de seguridad
3. **Monitoreo** - Logs y health checks
4. **Backup** - Estrategia de respaldos

### **URLs de Producción Sugeridas:**
```
https://api.medinet.com/api/visitador/annios
https://api.medinet.com/api/visitador
https://api.medinet.com/api/visitador/{id}/cartera
```

### **URLs de Producción Actuales (Funcionando):**
```
http://mdnconsultores.com:8080/api/visitador/annios
http://mdnconsultores.com:8080/api/visitador
http://mdnconsultores.com:8080/api/visitador/{id}/cartera
http://mdnconsultores.com:8080/api/visitador/{id}/kpis ⭐ NUEVO (14/Feb/2026)
```

---

## 📱 **Integración con Apps Android**

### **Estado Actual: FUNCIONANDO PERFECTAMENTE**

Las aplicaciones Android existentes pueden:
- ✅ Descargar archivos Cartera.txt
- ✅ Procesar la base de datos sin errores
- ✅ Ejecutar todas las consultas SQL
- ✅ Acceder al dashboard principal
- ✅ Funcionar sin modificaciones de código

### **Flujo de Integración:**
1. App Android llama al endpoint `/cartera`
2. Descarga archivo Cartera.txt
3. Ejecuta `crearBD()` sin errores
4. Usuario accede normalmente a la aplicación

---

## 🎉 **CONCLUSIÓN: MIGRACIÓN EXITOSA**

**El servicio web ha reemplazado exitosamente la aplicación ClickOnce. Las aplicaciones Android funcionan perfectamente con las carteras generadas por la nueva API.**

**Fecha de Finalización**: 21 de Enero, 2026  
**Estado Final**: **ÉXITO COMPLETO** ✅

---

## Endpoints Futuros (Opcional)

### Sincronización Bidireccional
*   **Ruta**: `POST /api/visitador/{id}/sync`
*   **Estado**: Placeholder para futuras mejoras
*   **Prioridad**: Baja (funcionalidad básica completa)
