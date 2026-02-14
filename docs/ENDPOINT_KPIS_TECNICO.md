# 📊 Endpoint de KPIs - Documentación Técnica

## **Información General**

**Endpoint**: `GET /api/visitador/{id}/kpis`  
**Versión**: 1.0  
**Fecha de Implementación**: 14 de Febrero, 2026  
**Estado**: Producción ✅

---

## 🎯 **Propósito**

Este endpoint proporciona acceso a los KPIs (Key Performance Indicators) configurados para cada visitador en un ciclo específico. Los KPIs son métricas de desempeño que establecen las metas diarias de visitas que debe cumplir un visitador médico.

### **KPIs Implementados**:
1. **KPI_Visita_Medica**: Meta de médicos a visitar por día (default: 8)
2. **KPI_Visita_Farmacia**: Meta de farmacias a visitar por día (default: 4)

---

## 🏗️ **Arquitectura de Implementación**

### **Componentes Involucrados**

```
┌─────────────────────────────────────────────────────────────┐
│                    Cliente (App/Browser)                     │
└────────────────────────────┬────────────────────────────────┘
                             │ HTTP GET
                             ▼
┌─────────────────────────────────────────────────────────────┐
│              VisitadorController.GetKpis()                   │
│  - Validación de parámetros                                  │
│  - Manejo de errores                                         │
│  - Serialización JSON                                        │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│            DataService.ObtenerKpisAsync()                    │
│  - Conexión a SQL Server                                     │
│  - Ejecución de query                                        │
│  - Mapeo de datos                                            │
└────────────────────────────┬────────────────────────────────┘
                             │
                             ▼
┌─────────────────────────────────────────────────────────────┐
│                  SQL Server - MW_Ciclos                      │
│  - Tabla maestra de ciclos                                   │
│  - Configuración de KPIs                                     │
└─────────────────────────────────────────────────────────────┘
```

---

## 📁 **Archivos Modificados/Creados**

### **1. Modelo de Datos**
**Archivo**: `WebApplication1/Models/KpiResponse.cs`

```csharp
namespace WebApplication1.Models
{
    public class KpiResponse
    {
        public long VisitadorId { get; set; }
        public int Ano { get; set; }
        public int Ciclo { get; set; }
        public int KpiVisitaMedica { get; set; }
        public int KpiVisitaFarmacia { get; set; }
        public string FechaInicio { get; set; } = string.Empty;
        public string FechaFin { get; set; } = string.Empty;
        public int DiasHabiles { get; set; }
        public string Estatus { get; set; } = string.Empty;
    }
}
```

### **2. Servicio de Datos**
**Archivo**: `WebApplication1/Services/DataService.cs`

**Método agregado**: `ObtenerKpisAsync(long visitadorId, int ano, int ciclo)`

**Query SQL**:
```sql
SELECT 
    ID_Ciclo,
    NU_Ano,
    NU_Ciclo,
    FE_CicloIni,
    FE_CicloFin,
    NU_DiasHabiles,
    NU_Estatus,
    KPI_Visita_Medica,
    KPI_Visita_Farmacia
FROM MW_Ciclos 
WHERE NU_Ano = @ano AND NU_Ciclo = @ciclo
```

**Lógica de Estatus**:
```csharp
int estatusNum = reader.GetInt16(6);
string estatus = estatusNum switch
{
    1 => "A", // Activo
    2 => "C", // Cerrado
    _ => "P"  // Pendiente
};
```

### **3. Controlador**
**Archivo**: `WebApplication1/Controllers/VisitadorController.cs`

**Método agregado**: `GetKpis(long id, int ano, int ciclo)`

**Características**:
- Validación de parámetros
- Año por defecto: año actual
- Ciclo por defecto: 1
- Manejo de errores 404 y 500
- Respuesta JSON automática

---

## 🗄️ **Estructura de la Tabla MW_Ciclos**

### **Esquema de la Tabla**

| Columna | Tipo | Descripción |
|---------|------|-------------|
| `ID_Ciclo` | INTEGER(11) | ID único del ciclo |
| `NU_Ano` | INTEGER(11) | Año del ciclo |
| `NU_Ciclo` | INTEGER(11) | Número del ciclo (1-12) |
| `FE_CicloIni` | TEXT(8) | Fecha de inicio (formato: YYYYMMDD) |
| `FE_CicloFin` | TEXT(8) | Fecha de fin (formato: YYYYMMDD) |
| `FE_CicloProrroga` | TEXT(8) | Fecha de prórroga |
| `NU_DiasCancelarVehiculo` | INTEGER(11) | Días para cancelar vehículo |
| `NU_DiasHabiles` | INTEGER(11) | Días hábiles del ciclo |
| `FE_CargaInv` | TEXT(8) | Fecha de carga de inventario |
| `FE_IniDistribucion` | TEXT(8) | Fecha inicio distribución |
| `FE_FinDistribucion` | TEXT(8) | Fecha fin distribución |
| `NU_Estatus` | INTEGER(11) | Estatus del ciclo (1=Activo, 2=Cerrado) |
| `BO_Activo` | INTEGER(11) | Indicador de activo |
| **`KPI_Visita_Medica`** | **INTEGER(11)** | **Meta de visitas médicas/día** |
| **`KPI_Visita_Farmacia`** | **INTEGER(11)** | **Meta de visitas farmacias/día** |

### **Índices de Columnas en el Reader**

```csharp
reader.GetInt64(0)   // ID_Ciclo
reader.GetInt16(1)   // NU_Ano
reader.GetInt16(2)   // NU_Ciclo
reader.GetDateTime(3) // FE_CicloIni
reader.GetDateTime(4) // FE_CicloFin
reader.GetInt16(5)   // NU_DiasHabiles
reader.GetInt16(6)   // NU_Estatus
reader.GetInt32(7)   // KPI_Visita_Medica ⭐
reader.GetInt32(8)   // KPI_Visita_Farmacia ⭐
```

---

## 🔄 **Flujo de Ejecución**

### **Caso Exitoso (200 OK)**

```
1. Cliente → GET /api/visitador/336/kpis?ano=2026&ciclo=1
2. Controller valida parámetros
3. Controller llama a DataService.ObtenerKpisAsync(336, 2026, 1)
4. DataService abre conexión a SQL Server
5. DataService ejecuta query en MW_Ciclos
6. DataService lee resultado y mapea a KpiResponse
7. DataService retorna KpiResponse
8. Controller serializa a JSON
9. Cliente recibe respuesta 200 OK con JSON
```

### **Caso de Error - Ciclo No Encontrado (404)**

```
1. Cliente → GET /api/visitador/336/kpis?ano=2099&ciclo=99
2. Controller valida parámetros
3. Controller llama a DataService.ObtenerKpisAsync(336, 2099, 99)
4. DataService ejecuta query
5. Query no retorna resultados (ciclo no existe)
6. DataService retorna null
7. Controller detecta null
8. Controller retorna 404 Not Found con mensaje
9. Cliente recibe error 404
```

### **Caso de Error - Excepción SQL (500)**

```
1. Cliente → GET /api/visitador/336/kpis
2. Controller llama a DataService
3. DataService intenta conectar a SQL Server
4. Conexión falla (timeout, credenciales, etc.)
5. Se lanza SqlException
6. Controller captura excepción en catch
7. Controller retorna 500 Internal Server Error
8. Cliente recibe error 500 con mensaje
```

---

## 🧪 **Testing**

### **Casos de Prueba Recomendados**

#### **1. Prueba Básica - Ciclo Actual**
```bash
GET http://localhost:5130/api/visitador/336/kpis
```
**Resultado Esperado**: 200 OK con KPIs del ciclo actual

#### **2. Prueba con Parámetros Específicos**
```bash
GET http://localhost:5130/api/visitador/336/kpis?ano=2026&ciclo=1
```
**Resultado Esperado**: 200 OK con KPIs del ciclo 1 de 2026

#### **3. Prueba de Ciclo Inexistente**
```bash
GET http://localhost:5130/api/visitador/336/kpis?ano=2099&ciclo=99
```
**Resultado Esperado**: 404 Not Found

#### **4. Prueba de Visitador Diferente**
```bash
GET http://localhost:5130/api/visitador/334/kpis?ano=2026&ciclo=1
```
**Resultado Esperado**: 200 OK (mismo ciclo, diferente visitador)

#### **5. Prueba de Múltiples Ciclos**
```bash
# Ciclo 1
GET http://localhost:5130/api/visitador/336/kpis?ano=2026&ciclo=1

# Ciclo 2
GET http://localhost:5130/api/visitador/336/kpis?ano=2026&ciclo=2

# Ciclo 12
GET http://localhost:5130/api/visitador/336/kpis?ano=2026&ciclo=12
```
**Resultado Esperado**: 200 OK para cada ciclo existente

---

## 🔒 **Seguridad**

### **Consideraciones de Seguridad Implementadas**

1. **SQL Injection Prevention**: ✅
   - Uso de parámetros SQL (`@ano`, `@ciclo`)
   - No concatenación de strings en queries

2. **Validación de Entrada**: ✅
   - Validación de tipos (long, int)
   - Valores por defecto seguros

3. **Manejo de Errores**: ✅
   - No expone detalles internos en producción
   - Mensajes de error genéricos

### **Consideraciones Pendientes**

- ⚠️ **Autenticación**: No implementada (considerar JWT/OAuth)
- ⚠️ **Autorización**: No valida si el usuario puede ver KPIs de ese visitador
- ⚠️ **Rate Limiting**: No implementado
- ⚠️ **CORS**: Configurar según necesidades del cliente

---

## 📈 **Performance**

### **Optimizaciones Implementadas**

1. **Query Eficiente**:
   - Filtro directo por año y ciclo (índices en BD)
   - Solo selecciona columnas necesarias
   - Sin JOINs innecesarios

2. **Conexión Asíncrona**:
   - Uso de `async/await`
   - No bloquea threads del servidor

3. **Disposición de Recursos**:
   - `using` statements para conexiones y readers
   - Liberación automática de recursos

### **Métricas Esperadas**

- **Tiempo de Respuesta**: < 100ms (red local)
- **Tiempo de Query**: < 10ms
- **Memoria**: Mínima (un solo registro)

---

## 🔧 **Mantenimiento**

### **Modificar Valores por Defecto de KPIs**

Los KPIs se configuran en la base de datos, tabla `MW_Ciclos`:

```sql
UPDATE MW_Ciclos 
SET KPI_Visita_Medica = 10,
    KPI_Visita_Farmacia = 5
WHERE NU_Ano = 2026 AND NU_Ciclo = 1;
```

### **Agregar Nuevos KPIs**

1. Agregar columna a tabla `MW_Ciclos`
2. Actualizar query en `DataService.ObtenerKpisAsync()`
3. Agregar propiedad a `KpiResponse`
4. Actualizar documentación

### **Logs y Debugging**

Para habilitar logs detallados, agregar en `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "WebApplication1.Services.DataService": "Debug"
    }
  }
}
```

---

## 📚 **Referencias**

- **Tabla de Origen**: `MW_Ciclos` (SQL Server)
- **Endpoint Similar**: `/api/visitador/{id}/cartera` (generación de carteras)
- **Documentación de API**: `docs/ENDPOINTS.md`
- **Estado del Proyecto**: `docs/ESTADO_ACTUAL.md`

---

## 📝 **Changelog**

### **v1.0 - 14/Feb/2026**
- ✅ Implementación inicial del endpoint
- ✅ Modelo `KpiResponse` creado
- ✅ Método `ObtenerKpisAsync()` en DataService
- ✅ Endpoint `GetKpis()` en VisitadorController
- ✅ Documentación completa
- ✅ Testing exitoso en desarrollo

---

**Autor**: Kiro AI Assistant  
**Fecha de Creación**: 14 de Febrero, 2026  
**Última Actualización**: 14 de Febrero, 2026  
**Estado**: Producción ✅
