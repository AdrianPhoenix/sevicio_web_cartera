# Issue #008: Nombres de columnas incorrectos en tabla mw_tipomedicos

**Fecha de Detección:** 24 de Marzo, 2026  
**Fecha de Resolución:** 24 de Marzo, 2026  
**Estado:** ✅ RESUELTO  
**Prioridad:** 🔴 ALTA

---

## 📋 Descripción del Problema

La tabla `mw_tipomedicos` en el esquema generado por el servicio web tenía nombres de columnas diferentes a los del ClickOnce original, causando incompatibilidad con la app Android.

---

## 🔍 Análisis

### Comportamiento Observado

**ClickOnce (correcto):**
```sql
CREATE TABLE "mw_tipomedicos" ("ID_Tipo" INTEGER(11), "TX_Tipo" TEXT(255), "BO_Activo" INTEGER(11));
```

**Servicio Web (ANTES de la corrección):**
```sql
CREATE TABLE "mw_tipomedicos" ("ID_TipoMedico" INTEGER(11), "TX_TipoMedico" TEXT(255), "BO_Activo" INTEGER(11));
```

### Columnas afectadas

| Columna ClickOnce | Columna Servicio Web (incorrecta) |
|---|---|
| `ID_Tipo` | `ID_TipoMedico` |
| `TX_Tipo` | `TX_TipoMedico` |

### Tablas verificadas en la misma revisión

Se verificaron también `mw_deportemedicos` y `mw_idiomamedicos`, ambas estaban correctas.

---

## ✅ Solución Aplicada

Corregidos los nombres de columnas en el esquema embebido de `GeneradorService.cs`:

- `"ID_TipoMedico"` → `"ID_Tipo"`
- `"TX_TipoMedico"` → `"TX_Tipo"`

### Archivo modificado
- `WebApplication1/Services/GeneradorService.cs` — Definición de esquema de `mw_tipomedicos`

### Verificación
- Cartera generada: `test_carteras/web_localhost/Cartera_334_24032026.txt`
- Esquema verificado coincide con ClickOnce original

---

## 📊 Impacto

- ✅ Esquema de `mw_tipomedicos` consistente con ClickOnce
- ✅ App Android puede consultar correctamente `ID_Tipo` y `TX_Tipo`
