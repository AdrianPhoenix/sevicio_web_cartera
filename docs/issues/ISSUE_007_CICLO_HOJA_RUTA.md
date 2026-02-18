# Issue #007: Columna CICLO Faltante en hoja_ruta y hoja_ruta_propuesta

**Fecha de Detección:** 17 de Febrero, 2026  
**Fecha de Resolución:** 17 de Febrero, 2026  
**Estado:** ✅ RESUELTO  
**Prioridad:** 🔴 ALTA

---

## 📋 Descripción del Problema

Durante la comparación de datos entre ClickOne y Web Service, se detectó que las tablas `hoja_ruta` y `hoja_ruta_propuesta` NO incluían la columna CICLO en sus INSERT statements, mientras que en ClickOne SÍ la incluyen.

---

## 🔍 Análisis

### Comportamiento Observado

**ClickOne:**
```sql
INSERT INTO hoja_ruta (CICLO, ZONA, SEMANA, DIA, AM, PM) VALUES (...);
INSERT INTO hoja_ruta_propuesta (CICLO, ZONA, SEMANA, DIA, AM, PM) VALUES (...);
```

**Web Service (ANTES de la corrección):**
```sql
INSERT INTO hoja_ruta (ZONA, SEMANA, DIA, AM, PM) VALUES (...);
INSERT INTO hoja_ruta_propuesta (ZONA, SEMANA, DIA, AM, PM) VALUES (...);
```

### Impacto

- ❌ Los registros insertados NO tenían valor de CICLO
- ❌ La app Android no podía filtrar hojas de ruta por ciclo
- ❌ Funcionalidad de planificación de rutas afectada
- ❌ Inconsistencia con ClickOne

### Causa Raíz

El código en `GeneradorService.cs` excluía la columna CICLO de los INSERT statements para todas las tablas excepto:
- cierreciclo
- solicitudes
- hsolicitudes
- visitas
- hvisitas

Las tablas `hoja_ruta` y `hoja_ruta_propuesta` NO estaban en esta lista, por lo que CICLO era excluido.

---

## 🔧 Solución Implementada

### Cambio en el Código

**Archivo:** `WebApplication1/Services/GeneradorService.cs`  
**Líneas:** ~277-283

**ANTES:**
```csharp
// Para cierreciclo, solicitudes, hsolicitudes, visitas y hvisitas, INCLUIR la columna CICLO
bool esCiclo = col.Equals("CICLO", StringComparison.OrdinalIgnoreCase);
bool tablaPermiteCiclo = nombreTablaSqlite == "cierreciclo" || 
                        nombreTablaSqlite == "solicitudes" || 
                        nombreTablaSqlite == "hsolicitudes" ||
                        nombreTablaSqlite == "visitas" ||
                        nombreTablaSqlite == "hvisitas";
```

**DESPUÉS:**
```csharp
// Para cierreciclo, solicitudes, hsolicitudes, visitas, hvisitas, hoja_ruta y hoja_ruta_propuesta, INCLUIR la columna CICLO
bool esCiclo = col.Equals("CICLO", StringComparison.OrdinalIgnoreCase);
bool tablaPermiteCiclo = nombreTablaSqlite == "cierreciclo" || 
                        nombreTablaSqlite == "solicitudes" || 
                        nombreTablaSqlite == "hsolicitudes" ||
                        nombreTablaSqlite == "visitas" ||
                        nombreTablaSqlite == "hvisitas" ||
                        nombreTablaSqlite == "hoja_ruta" ||
                        nombreTablaSqlite == "hoja_ruta_propuesta";
```

### Resultado Esperado

Después de la corrección, los INSERT statements incluirán CICLO:

```sql
INSERT INTO hoja_ruta (CICLO, ZONA, SEMANA, DIA, AM, PM) VALUES (...);
INSERT INTO hoja_ruta_propuesta (CICLO, ZONA, SEMANA, DIA, AM, PM) VALUES (...);
```

---

## ✅ Verificación

### Script de Verificación

`scripts/analizar_ciclo_inserts.py`

**Resultado ANTES de la corrección:**
```
Tabla                                 ClickOne     Web Service               Estado
--------------------------------------------------------------------------------------------------
hoja_ruta                                 ✅ SÍ            ❌ NO          ⚠️ PROBLEMA
hoja_ruta_propuesta                       ✅ SÍ            ❌ NO          ⚠️ PROBLEMA
```

**Resultado ESPERADO después de la corrección:**
```
Tabla                                 ClickOne     Web Service               Estado
--------------------------------------------------------------------------------------------------
hoja_ruta                                 ✅ SÍ            ✅ SÍ                 ✅ OK
hoja_ruta_propuesta                       ✅ SÍ            ✅ SÍ                 ✅ OK
```

### Pasos de Verificación

1. ✅ Compilar el proyecto: `dotnet build`
2. ⏳ Generar nueva cartera: Cartera_zona_343_8.txt
3. ⏳ Ejecutar script de verificación: `python scripts/analizar_ciclo_inserts.py`
4. ⏳ Confirmar que ambas tablas ahora incluyen CICLO

---

## 📊 Impacto de la Corrección

### Antes
- ❌ Registros sin valor de CICLO
- ❌ Imposible filtrar por ciclo en la app Android
- ❌ Funcionalidad de planificación limitada

### Después
- ✅ Registros con valor correcto de CICLO
- ✅ Filtrado por ciclo funcional
- ✅ Funcionalidad completa de planificación de rutas
- ✅ Consistencia 100% con ClickOne

---

## 🔗 Issues Relacionados

Este problema es similar a issues anteriores ya resueltos:

- **Issue #004:** Columna CICLO faltante en solicitudes/hsolicitudes ✅ RESUELTO
- **Issue #006:** Columna CICLO faltante en visitas/hvisitas ✅ RESUELTO
- **Issue #007:** Columna CICLO faltante en hoja_ruta/hoja_ruta_propuesta ✅ RESUELTO

**Patrón:** Todas las tablas que necesitan filtrado por ciclo deben estar en la lista `tablaPermiteCiclo`.

---

## 📝 Lecciones Aprendidas

1. **Verificar datos, no solo estructura:** Las correcciones anteriores se enfocaron en estructura (CREATE TABLE), pero también es importante verificar los datos (INSERT statements).

2. **Patrón consistente:** Todas las tablas que la app Android filtra por CICLO deben incluir esta columna en los INSERT.

3. **Documentación de lista:** Mantener documentada la lista de tablas que permiten CICLO para futuras referencias.

---

## 🔗 Referencias

### Documentación
- `docs/ANALISIS_DATOS_COMPARACION.md` - Análisis que detectó el problema
- `docs/RESUMEN_COMPARACION_DATOS.md` - Resumen de comparación de datos
- `docs/ISSUE_004_SOLUCION.md` - Problema similar con solicitudes
- `docs/ISSUE_006_SOLUCION.md` - Problema similar con visitas

### Scripts
- `scripts/comparar_datos_completo.py` - Comparación general de datos
- `scripts/analizar_ciclo_inserts.py` - Análisis específico de CICLO

### Código
- `WebApplication1/Services/GeneradorService.cs` (líneas ~277-283)

---

## ✅ Estado Final

**Estado:** ✅ RESUELTO  
**Compilación:** ✅ EXITOSA  
**Próximo Paso:** Generar Cartera_zona_343_8.txt y verificar

---

**Última Actualización:** 17 de Febrero, 2026  
**Responsable:** Equipo de Desarrollo  
**Cartera Generada:** ✅ Cartera_zona_343_8.txt

---

## ✅ Verificación Final

### Script Ejecutado
`scripts/analizar_ciclo_inserts.py`

**Resultado:**
```
Tabla                                 ClickOne     Web Service               Estado
--------------------------------------------------------------------------------------------------
farmacias                                 ✅ SÍ            ✅ SÍ                 ✅ OK
hoja_ruta                                 ✅ SÍ            ✅ SÍ                 ✅ OK
hoja_ruta_propuesta                       ✅ SÍ            ✅ SÍ                 ✅ OK
hospital                                  ✅ SÍ            ✅ SÍ                 ✅ OK

💡 CONCLUSIÓN: ✅ ¡PERFECTO! Todas las tablas incluyen CICLO correctamente
```

### Comparación de INSERT Statements

**hoja_ruta:**
- ClickOne:    `CICLO, ZONA, SEMANA, DIA, AM, PM`
- Web Service: `CICLO, ZONA, SEMANA, DIA, AM, PM` ✅

**hoja_ruta_propuesta:**
- ClickOne:    `CICLO, ZONA, SEMANA, DIA, AM, PM`
- Web Service: `CICLO, ZONA, SEMANA, DIA, AM, PM` ✅

---

**Estado:** ✅ VERIFICADO Y FUNCIONANDO
