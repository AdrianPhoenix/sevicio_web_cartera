# 📊 Resumen de Comparación de Datos - ClickOne vs Web Service

**Fecha:** 17 de Febrero, 2026  
**Estado:** ⚠️ REQUIERE ATENCIÓN

---

## 🎯 Resumen Ejecutivo

Se realizó una comparación completa de los datos (INSERT statements) entre ClickOne y Web Service (Cartera_zona_343_7.txt).

**Resultado General:**
- ✅ **75% de las tablas son idénticas** (15 de 20 tablas)
- ⚠️ **2 tablas tienen problemas con columna CICLO**
- ⚠️ **2 tablas históricas tienen más registros**
- ℹ️ **3 tablas nuevas en Web Service**

---

## ✅ Aspectos Positivos

### 1. Datos Idénticos (15 tablas)
Las siguientes tablas tienen exactamente el mismo número de registros:

| Tabla | Registros | Estado |
|-------|-----------|--------|
| ayuda_visual | 174 | ✅ |
| ciclos | 12 | ✅ |
| cierreciclo | 23 | ✅ |
| farmacias | 50 | ✅ |
| farmacias_detalles | 6 | ✅ |
| farmacias_detalles_productos | 20 | ✅ |
| farmacias_personal | 133 | ✅ |
| fichero | 160 | ✅ |
| fichero_horarios | 8 | ✅ |
| historialconceptodias | 1 | ✅ |
| hospital | 10 | ✅ |
| hospital_detalles | 3 | ✅ |
| medicos | 100 | ✅ |
| solicitudes | 5 | ✅ |
| visitas | 28 | ✅ |

**Total:** 679 registros coinciden perfectamente ✅

---

## ⚠️ Problemas Detectados

### 🔴 Prioridad Alta: Columna CICLO Faltante en INSERT (2 tablas)

#### 1. hoja_ruta
- **Problema:** ClickOne incluye CICLO en INSERT, Web Service NO
- **Impacto:** Los registros no tendrán valor de CICLO
- **Consecuencia:** La app Android no podrá filtrar por CICLO
- **Estado:** ⚠️ REQUIERE CORRECCIÓN

#### 2. hoja_ruta_propuesta
- **Problema:** ClickOne incluye CICLO en INSERT, Web Service NO
- **Impacto:** Los registros no tendrán valor de CICLO
- **Consecuencia:** La app Android no podrá filtrar por CICLO
- **Estado:** ⚠️ REQUIERE CORRECCIÓN

**Comparación:**

| Tabla | ClickOne | Web Service | Estado |
|-------|----------|-------------|--------|
| farmacias | ✅ Incluye CICLO | ✅ Incluye CICLO | ✅ OK |
| hospital | ✅ Incluye CICLO | ✅ Incluye CICLO | ✅ OK |
| hoja_ruta | ✅ Incluye CICLO | ❌ NO incluye CICLO | ⚠️ PROBLEMA |
| hoja_ruta_propuesta | ✅ Incluye CICLO | ❌ NO incluye CICLO | ⚠️ PROBLEMA |

**Nota:** Este es el mismo tipo de problema que se corrigió en:
- Issue #004: solicitudes (RESUELTO)
- Issue #006: visitas (RESUELTO)

---

### 🟡 Prioridad Media: Tablas Históricas con Más Registros (2 tablas)

#### 1. hfarmacias_detalles
- **ClickOne:** 38 registros
- **Web Service:** 78 registros
- **Diferencia:** +40 registros (+105%)
- **Análisis:** Posible duplicación o lógica diferente de generación

#### 2. hhospital_detalles
- **ClickOne:** 3 registros
- **Web Service:** 7 registros
- **Diferencia:** +4 registros (+133%)
- **Análisis:** Posible duplicación o lógica diferente de generación

**Patrón:** Ambas son tablas históricas (prefijo `h`) con más registros en Web Service.

---

### 🔵 Prioridad Baja: Tablas Nuevas en Web Service (3 tablas)

Estas tablas existen en Web Service pero NO en ClickOne:

| Tabla | Registros | Análisis |
|-------|-----------|----------|
| hfarmacias_detalles_productos | 10 | Tabla histórica nueva |
| hvisitas | 28 | Tabla histórica nueva |
| puntos | 10 | Sistema de puntos (mejora) |

**Análisis:** Probablemente son mejoras intencionales del Web Service.

---

## 🔧 Recomendaciones

### 1. Corregir Columna CICLO (URGENTE)
**Tablas afectadas:** hoja_ruta, hoja_ruta_propuesta

**Acción:**
1. Revisar `GeneradorService.cs` método que genera INSERT statements
2. Agregar estas tablas a la lista de tablas que permiten CICLO
3. Similar a la corrección de Issue #004 y #006

**Código a revisar:**
```csharp
// Buscar la lógica que filtra columnas en INSERT
// Agregar "hoja_ruta" y "hoja_ruta_propuesta" a la lista de tablas permitidas
```

### 2. Investigar Tablas Históricas (MEDIA PRIORIDAD)
**Tablas afectadas:** hfarmacias_detalles, hhospital_detalles

**Acción:**
1. Revisar lógica de generación de datos históricos
2. Verificar si hay duplicación de registros
3. Confirmar si los registros adicionales son correctos

### 3. Documentar Tablas Nuevas (BAJA PRIORIDAD)
**Tablas afectadas:** hfarmacias_detalles_productos, hvisitas, puntos

**Acción:**
1. Confirmar que son mejoras intencionales
2. Documentar su propósito
3. Verificar que la app Android las maneja correctamente

---

## 📈 Impacto

### Impacto de Problemas con CICLO

Si NO se corrige:
- ❌ Los registros de hoja_ruta no tendrán CICLO
- ❌ Los registros de hoja_ruta_propuesta no tendrán CICLO
- ❌ La app Android no podrá filtrar estas tablas por CICLO
- ❌ Funcionalidad de planificación de rutas afectada

Si se corrige:
- ✅ Los registros tendrán el valor correcto de CICLO
- ✅ La app Android podrá filtrar correctamente
- ✅ Funcionalidad completa de planificación de rutas

---

## 📊 Estadísticas Finales

| Métrica | Valor | Porcentaje |
|---------|-------|------------|
| Tablas con datos idénticos | 15 | 75% |
| Tablas con problemas de CICLO | 2 | 10% |
| Tablas históricas con más datos | 2 | 10% |
| Tablas nuevas en Web | 3 | 15% |
| **Total de tablas** | **20** | **100%** |

**Registros totales coincidentes:** 679 registros ✅

---

## 🔗 Archivos y Scripts

### Documentación
- `docs/ANALISIS_DATOS_COMPARACION.md` - Análisis detallado completo
- `docs/ISSUE_004_SOLUCION.md` - Referencia: Problema similar con solicitudes
- `docs/ISSUE_006_SOLUCION.md` - Referencia: Problema similar con visitas

### Scripts
- `scripts/comparar_datos_completo.py` - Comparación general de datos
- `scripts/analizar_ciclo_inserts.py` - Análisis específico de columna CICLO

### Carteras
- `test_carteras/clickOne/Cartera_zona_343.txt` - Referencia (ClickOne)
- `test_carteras/web_localhost/Cartera_zona_343_7.txt` - Web Service actual

---

## ✅ Próximos Pasos

1. **URGENTE:** Corregir columna CICLO en hoja_ruta y hoja_ruta_propuesta
2. **MEDIO:** Investigar duplicación en tablas históricas
3. **BAJO:** Documentar tablas nuevas

---

**Última Actualización:** 17 de Febrero, 2026  
**Responsable:** Equipo de Desarrollo  
**Estado:** ⚠️ REQUIERE CORRECCIÓN DE CICLO
