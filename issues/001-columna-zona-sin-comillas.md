# 🐛 Issue: Columna Zona sin comillas en tabla cierreciclo

## 📅 Fecha de Reporte
4 de Febrero, 2026

## 🔴 Prioridad
**BAJA** - No crítico, funcionalidad básica no afectada

## 📝 Descripción del Problema

La columna `Zona` en la tabla `cierreciclo` se está insertando sin comillas en los INSERT statements, cuando debería tener comillas por ser de tipo TEXT.

### **Comportamiento Actual:**
```sql
INSERT INTO cierreciclo (Zona, CICLO, Fecha, ...) 
VALUES (336, 12, '2024-12-13', ...);
```

### **Comportamiento Esperado (ClickOne):**
```sql
INSERT INTO cierreciclo (Zona, CICLO, Fecha, ...) 
VALUES ('336', 1, '2026-02-02', ...);
```

## 🔍 Análisis Técnico

### **Causa Raíz:**
1. La columna `ZONA` en SQL Server (tabla `MD_CierreCiclo`) es de tipo `VARCHAR/NVARCHAR`
2. El parámetro `@visitadorId` se pasa como `long` en la consulta
3. SQL Server hace conversión implícita de `ZONA` a numérico para la comparación
4. El `SqlDataReader` devuelve la columna como tipo numérico en lugar de string
5. El método `FormatearValor()` la trata como `Int64` y no le agrega comillas

### **Archivos Afectados:**
- `WebApplication1/Services/GeneradorService.cs`
  - Método: `GenerarContenidoCierreCiclosAsync()` (línea ~159)
  - Método: `ObtenerDatosTablaAsync()` (línea ~240)
  - Método: `FormatearValor()` (línea ~190)

## 🔧 Soluciones Intentadas

### **Intento 1: Modificar FormatearValor()**
```csharp
// Agregar verificación especial para columna Zona
if (columnName.Equals("Zona", StringComparison.Ordinal))
{
    return "'" + valor.ToString() + "'";
}
```
**Resultado**: ❌ No funcionó - El código nunca se ejecuta porque la columna ya viene como tipo numérico

### **Intento 2: Modificar consulta SQL**
```csharp
WHERE (ZONA = CAST(@visitadorId AS VARCHAR(10)) AND ...)
```
**Resultado**: ❌ No funcionó - El DataReader ya leyó la columna como numérico

### **Intento 3: Forzar conversión en ObtenerDatosTablaAsync()**
```csharp
if (col.Equals("Zona", StringComparison.Ordinal))
{
    valores.Add("'" + reader[col].ToString() + "'");
}
```
**Resultado**: ⏳ No probado aún

## ✅ Solución Propuesta (Pendiente de Implementar)

### **Opción A: Forzar conversión directa (Más simple)**
En el método `ObtenerDatosTablaAsync()`, antes de llamar a `FormatearValor()`:

```csharp
while (await reader.ReadAsync())
{
    var valores = new List<string>();
    foreach (var col in finalColumns)
    {
        int cicloParaFormateo = (nombreTablaSqlite.StartsWith("h") || nombreTablaSqlite == "cierreciclo") 
            ? Convert.ToInt32(reader["CICLO"]) : ciclo;
        
        // Forzar que la columna Zona se trate como string
        if (col.Equals("Zona", StringComparison.Ordinal))
        {
            valores.Add("'" + reader[col].ToString() + "'");
        }
        else
        {
            valores.Add(FormatearValor(reader[col], reader.GetFieldType(reader.GetOrdinal(col)), col, cicloParaFormateo));
        }
    }
    // ...
}
```

### **Opción B: Modificar la consulta para forzar tipo string**
```csharp
string query = @"SELECT 
                    CAST(ZONA AS VARCHAR(10)) AS Zona,
                    CICLO, Fecha, Hora, ... 
                 FROM [MD_CierreCiclo] 
                 WHERE ...";
```

## 🧪 Testing

### **Archivos de Prueba:**
- `test_carteras/clickOne/Cartera.txt` - Referencia correcta
- `test_carteras/web/Cartera_336_test6.txt` - Última versión con el problema

### **Comando de Verificación:**
```bash
grep -i "INSERT INTO.*cierreciclo" test_carteras/web/Cartera_336_test6.txt | head -1
```

### **Validación:**
Comparar con ClickOne:
```bash
grep -i "INSERT INTO.*cierreciclo" test_carteras/clickOne/Cartera.txt | head -1
```

## 📊 Impacto

### **Funcionalidad Afectada:**
- Tabla `cierreciclo` (historial de cierres de ciclo)

### **¿Afecta la app Android?**
- **Probablemente NO** - SQLite es flexible con tipos de datos
- La columna está definida como `TEXT(7)` en el CREATE TABLE
- SQLite puede aceptar valores numéricos en columnas TEXT
- **Requiere testing** para confirmar

### **Riesgo:**
- **BAJO** - No afecta funcionalidad básica
- **MEDIO** - Puede causar problemas en consultas que comparen Zona como string

## 📝 Notas Adicionales

- El ClickOne original usa comillas para Zona
- Todas las demás tablas formatean Zona correctamente
- Solo afecta a la tabla `cierreciclo`
- La columna CICLO se agregó correctamente (issue resuelto)

## ✅ Criterios de Aceptación

Para considerar este issue resuelto:
1. ✅ La columna Zona debe tener comillas: `'336'` no `336`
2. ✅ La columna CICLO debe estar presente
3. ✅ El formato debe ser idéntico al ClickOne
4. ✅ La app Android debe funcionar sin errores

## 🔗 Referencias

- Código ClickOne: `Generador_clickOne/MedinetGeneradorDB/Generador.cs` (líneas 220-350)
- Comparación de carteras: `test_carteras/`

---

**Estado**: 🟡 PENDIENTE  
**Asignado a**: Por definir  
**Última actualización**: 2026-02-04
