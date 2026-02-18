# 📊 Análisis de Comparación de Datos

**Fecha:** 17 de Febrero, 2026  
**Archivos Comparados:**
- ClickOne: `test_carteras/clickOne/Cartera_zona_343.txt`
- Web Service: `test_carteras/web_localhost/Cartera_zona_343_7.txt`

---

## 📋 Resumen Ejecutivo

| Métrica | Valor |
|---------|-------|
| **Total de tablas con datos** | 20 |
| **Tablas idénticas (mismo # registros)** | 15 (75%) |
| **Tablas con diferente # registros** | 2 (10%) |
| **Tablas solo en ClickOne** | 0 (0%) |
| **Tablas solo en Web Service** | 3 (15%) |
| **Tablas con diferencias en columnas** | 6 |

---

## ✅ Tablas Idénticas (15 tablas - 75%)

Estas tablas tienen exactamente el mismo número de registros en ambas carteras:

| Tabla | Registros |
|-------|-----------|
| ayuda_visual | 174 |
| ciclos | 12 |
| cierreciclo | 23 |
| farmacias | 50 |
| farmacias_detalles | 6 |
| farmacias_detalles_productos | 20 |
| farmacias_personal | 133 |
| fichero | 160 |
| fichero_horarios | 8 |
| historialconceptodias | 1 |
| hospital | 10 |
| hospital_detalles | 3 |
| medicos | 100 |
| solicitudes | 5 |
| visitas | 28 |

**Total:** 679 registros en ClickOne = 679 registros en Web Service ✅

---

## ⚠️ Tablas con Diferente Número de Registros (2 tablas)

### 1. hfarmacias_detalles
- **ClickOne:** 38 registros
- **Web Service:** 78 registros
- **Diferencia:** +40 registros (105% más)
- **Análisis:** El Web Service tiene el doble de registros. Posible duplicación o datos adicionales.

### 2. hhospital_detalles
- **ClickOne:** 3 registros
- **Web Service:** 7 registros
- **Diferencia:** +4 registros (133% más)
- **Análisis:** El Web Service tiene más del doble de registros. Posible duplicación o datos adicionales.

**Patrón detectado:** Ambas tablas son tablas históricas (prefijo `h`) y ambas tienen más registros en Web Service.

---

## 🟡 Tablas Solo en Web Service (3 tablas)

Estas tablas existen en el Web Service pero NO en ClickOne:

| Tabla | Registros | Análisis |
|-------|-----------|----------|
| hfarmacias_detalles_productos | 10 | Tabla histórica de productos de farmacias |
| hvisitas | 28 | Tabla histórica de visitas |
| puntos | 10 | Sistema de puntos (posible mejora) |

**Análisis:**
- 2 de 3 son tablas históricas (prefijo `h`)
- La tabla `puntos` parece ser una funcionalidad nueva del Web Service
- Estas pueden ser mejoras intencionales del Web Service

---

## 🔍 Análisis de Estructura de Columnas

### Tablas con Diferencias en Columnas (6 tablas)

#### 1. ayuda_visual
**Diferencia:** Orden y nombres de columnas diferentes
- **ClickOne:** Incluye REGISTRO, ZONA al inicio
- **Web Service:** No incluye REGISTRO, ZONA en el INSERT
- **Impacto:** Posible diferencia en cómo se insertan los datos

#### 2. ciclos ⭐
**Diferencia:** Web Service tiene columnas adicionales
- **ClickOne:** 6 columnas básicas
- **Web Service:** 8 columnas (incluye ANO, KPI_VISITA_MEDICA, KPI_VISITA_FARMACIA)
- **Análisis:** ✅ Mejora intencional documentada (Issue #002)
- **Estado:** ✅ Correcto - Mejora del Web Service

#### 3. farmacias
**Diferencia:** Columna CICLO
- **ClickOne:** Incluye CICLO en INSERT
- **Web Service:** NO incluye CICLO en INSERT
- **Impacto:** Posible problema si la app Android espera CICLO

#### 4. hoja_ruta
**Diferencia:** Columna CICLO
- **ClickOne:** Incluye CICLO en INSERT
- **Web Service:** NO incluye CICLO en INSERT
- **Impacto:** Posible problema si la app Android espera CICLO

#### 5. hoja_ruta_propuesta
**Diferencia:** Columna CICLO
- **ClickOne:** Incluye CICLO en INSERT
- **Web Service:** NO incluye CICLO en INSERT
- **Impacto:** Posible problema si la app Android espera CICLO

#### 6. hospital
**Diferencia:** Columna CICLO
- **ClickOne:** Incluye CICLO en INSERT
- **Web Service:** NO incluye CICLO en INSERT
- **Impacto:** Posible problema si la app Android espera CICLO

---

## 🎯 Hallazgos Importantes

### 1. Patrón: Columna CICLO Faltante en INSERT
**Tablas afectadas:** farmacias, hoja_ruta, hoja_ruta_propuesta, hospital

**Problema:**
- ClickOne incluye la columna CICLO en los INSERT statements
- Web Service NO incluye CICLO en los INSERT statements
- Esto puede causar que los datos no tengan el valor de CICLO

**Impacto:**
- Si la app Android filtra por CICLO, estos registros no aparecerán
- Similar al Issue #004 (solicitudes) y Issue #006 (visitas) que ya fueron corregidos

**Recomendación:** ⚠️ ALTA PRIORIDAD - Verificar si estas 4 tablas necesitan incluir CICLO en INSERT

### 2. Tablas Históricas con Más Datos
**Tablas afectadas:** hfarmacias_detalles, hhospital_detalles

**Problema:**
- Web Service tiene significativamente más registros que ClickOne
- Posible duplicación de datos o lógica diferente de generación

**Recomendación:** 🔍 MEDIA PRIORIDAD - Investigar por qué hay más registros

### 3. Tablas Nuevas en Web Service
**Tablas:** hfarmacias_detalles_productos, hvisitas, puntos

**Análisis:**
- Pueden ser mejoras intencionales del Web Service
- La tabla `puntos` parece ser funcionalidad nueva

**Recomendación:** ℹ️ BAJA PRIORIDAD - Documentar si son mejoras intencionales

---

## 📊 Resumen de Prioridades

### 🔴 Prioridad Alta
1. **Verificar columna CICLO en INSERT statements** (4 tablas)
   - farmacias
   - hoja_ruta
   - hoja_ruta_propuesta
   - hospital
   - **Acción:** Revisar si necesitan incluir CICLO en los INSERT

### 🟡 Prioridad Media
2. **Investigar duplicación en tablas históricas** (2 tablas)
   - hfarmacias_detalles (+40 registros)
   - hhospital_detalles (+4 registros)
   - **Acción:** Verificar lógica de generación de datos históricos

### 🔵 Prioridad Baja
3. **Documentar tablas nuevas** (3 tablas)
   - hfarmacias_detalles_productos
   - hvisitas
   - puntos
   - **Acción:** Confirmar si son mejoras intencionales

---

## 💡 Conclusión

**Estado General:** ⚠️ REQUIERE ATENCIÓN

**Aspectos Positivos:**
- ✅ 75% de las tablas tienen datos idénticos (15 de 20)
- ✅ 679 registros coinciden perfectamente
- ✅ No hay tablas faltantes en Web Service

**Aspectos a Revisar:**
- ⚠️ 4 tablas no incluyen CICLO en INSERT (posible problema crítico)
- ⚠️ 2 tablas históricas tienen más registros (posible duplicación)
- ℹ️ 3 tablas nuevas en Web Service (posibles mejoras)

**Recomendación Principal:**
Revisar urgentemente las 4 tablas que no incluyen CICLO en INSERT, ya que esto puede causar problemas similares a los Issues #004 y #006 que ya fueron corregidos.

---

## 🔗 Referencias

### Documentos Relacionados
- `docs/CORRECCIONES_FINALIZADAS.md` - Correcciones de estructura
- `docs/ISSUE_004_SOLUCION.md` - Problema similar con CICLO en solicitudes
- `docs/ISSUE_006_SOLUCION.md` - Problema similar con CICLO en visitas

### Scripts
- `scripts/comparar_datos_completo.py` - Script de comparación de datos

### Carteras
- `test_carteras/clickOne/Cartera_zona_343.txt` - Referencia (ClickOne)
- `test_carteras/web_localhost/Cartera_zona_343_7.txt` - Web Service actual

---

**Última Actualización:** 17 de Febrero, 2026  
**Responsable:** Equipo de Desarrollo  
**Estado:** ⚠️ REQUIERE REVISIÓN
