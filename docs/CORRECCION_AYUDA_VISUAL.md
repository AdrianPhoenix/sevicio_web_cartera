# Corrección de Tablas de Ayuda Visual

**Fecha:** 20 de febrero de 2026  
**Estado:** ✅ Completado

## 📋 Resumen

Se corrigieron las 4 tablas de ayuda visual para que coincidan exactamente con el esquema de ClickOne, eliminando el prefijo `mw_` incorrecto y actualizando la estructura de columnas.

## 🔧 Cambios Realizados

### 1. Corrección de Nomenclatura y Esquema

**ANTES (Incorrecto):**
```sql
-- Tablas con prefijo mw_ y esquema tipo catálogo
DROP TABLE IF EXISTS "mw_ayuda_visual";
CREATE TABLE "mw_ayuda_visual" (
    "ID_AyudaVisual" INTEGER(11), 
    "TX_AyudaVisual" TEXT(255), 
    "TX_Ruta" TEXT(255), 
    "ID_Linea" INTEGER(11), 
    "ID_Marca" INTEGER(11), 
    "BO_Activo" INTEGER(11)
);
-- (Similar para mw_ayuda_visual_fe, mw_ayuda_visual_mp4, mw_ayuda_visual_mp4_fe)
```

**DESPUÉS (Correcto - ClickOne):**
```sql
-- Tablas SIN prefijo mw_ y esquema de datos de visitas
DROP TABLE IF EXISTS "ayuda_visual";
CREATE TABLE "ayuda_visual" (
    "REGISTRO" TEXT(5), 
    "ZONA" TEXT(7), 
    "FECHA_VISITA" TEXT(10), 
    "CICLO" INTEGER(11), 
    "TIPO" TEXT(16), 
    "MOTIVO" TEXT(20), 
    "ESPECIALIDAD" TEXT(25), 
    "CLASIFICACION" TEXT(2), 
    "FECHA_SISTEMA" TEXT(10), 
    "HORA_SISTEMA" TEXT(8), 
    "PRODUCTO" TEXT(30), 
    "POSICION" TEXT(2), 
    "ORDEN" INTEGER(11)
);
-- (Mismo esquema para ayuda_visual_FE, ayuda_visual_MP4, ayuda_visual_MP4_FE)
```

### 2. Actualización de Lógica de Exclusión de Columnas

Se actualizó la lógica en `GeneradorService.cs` para excluir correctamente las columnas `REGISTRO` y `ZONA` en todas las variantes de ayuda visual:

```csharp
// ANTES: Solo para ayuda_visual
if (nombreTablaSqlite == "ayuda_visual" && ...)

// DESPUÉS: Para todas las variantes
if ((nombreTablaSqlite == "ayuda_visual" || 
     nombreTablaSqlite == "ayuda_visual_fe" || 
     nombreTablaSqlite == "ayuda_visual_mp4" || 
     nombreTablaSqlite == "ayuda_visual_mp4_fe") && ...)
```

## 📊 Tablas Corregidas

| Tabla Original (Incorrecta) | Tabla Corregida (ClickOne) | Datos en ClickOne | Estado |
|------------------------------|----------------------------|-------------------|--------|
| `mw_ayuda_visual` | `ayuda_visual` | ✅ 174 registros | ✅ Corregida |
| `mw_ayuda_visual_fe` | `ayuda_visual_FE` | ❌ Sin datos | ✅ Corregida |
| `mw_ayuda_visual_mp4` | `ayuda_visual_MP4` | ❌ Sin datos | ✅ Corregida |
| `mw_ayuda_visual_mp4_fe` | `ayuda_visual_MP4_FE` | ❌ Sin datos | ✅ Corregida |

### 📈 Datos Encontrados en ClickOne

La tabla `ayuda_visual` contiene **174 registros** de ayudas visuales mostradas durante visitas médicas en la zona 343, ciclo 2 (febrero 2026).

**Ejemplo de datos reales:**
```sql
INSERT INTO ayuda_visual (REGISTRO, ZONA, FECHA_VISITA, CICLO, TIPO, MOTIVO, ESPECIALIDAD, 
    CLASIFICACION, FECHA_SISTEMA, HORA_SISTEMA, PRODUCTO, POSICION, ORDEN) 
VALUES ('147', '343', '06/02/2026', 2, 'EFECTIVA', 'PRESENCIAL CON A/V', 'PEDIATRA', 
    'B2', '06/02/2026', '06:22:12', 'RONIPRAZOL', 'P', 1);
```

**Interpretación de los datos:**
- **REGISTRO**: '147' - Código del médico visitado
- **ZONA**: '343' - Zona del visitador
- **FECHA_VISITA**: '06/02/2026' - Fecha de la visita
- **PRODUCTO**: 'RONIPRAZOL' - Producto presentado
- **POSICION**: 'P' - Posición en el esquema promocional (P=Primaria, S=Secundaria, T=Terciaria, R1/R2/R3=Refuerzo)
- **ORDEN**: 1 - Orden de presentación en la visita

Las otras 3 variantes (`ayuda_visual_FE`, `ayuda_visual_MP4`, `ayuda_visual_MP4_FE`) no tienen datos en la cartera analizada, pero mantienen la misma estructura para cuando se utilicen.

## 🎯 Propósito de las Tablas

Estas tablas almacenan información sobre las **ayudas visuales** (presentaciones, videos, materiales) mostrados durante las visitas médicas:

- **`ayuda_visual`**: Ayudas visuales estándar
- **`ayuda_visual_FE`**: Ayudas visuales para Fuerza Externa
- **`ayuda_visual_MP4`**: Ayudas visuales en formato video (MP4)
- **`ayuda_visual_MP4_FE`**: Videos para Fuerza Externa

### Campos Principales:
- `REGISTRO`: Código del médico visitado
- `ZONA`: Zona del visitador
- `FECHA_VISITA`: Fecha de la visita
- `PRODUCTO`: Producto presentado
- `POSICION`: Posición en la presentación
- `ORDEN`: Orden de visualización

## 📁 Archivos Modificados

1. **`WebApplication1/Services/GeneradorService.cs`**
   - Líneas 475-482: Definiciones de tablas corregidas
   - Líneas 273-280: Lógica de exclusión actualizada

## ✅ Verificación

- [x] Nomenclatura corregida (sin prefijo `mw_`)
- [x] Esquema actualizado a ClickOne
- [x] Lógica de exclusión de columnas actualizada
- [x] Referencias en código verificadas
- [x] Documentación actualizada

## 🔄 Próximos Pasos

1. Probar la generación de carteras con las tablas corregidas
2. Verificar que los datos de ayudas visuales se copien correctamente
3. Validar con una zona de prueba

## 📝 Notas

- Las tablas MD_ (`MD_Ayuda_Visual`, etc.) ya estaban correctamente nombradas
- No se requieren cambios en la base de datos de producción hasta validar
- Los archivos de documentación antiguos mantienen las referencias históricas
