# Issue #009 - Columna COMENTARIOS sobrante en farmacias_detalles_productos

## Fecha
2026-03-24

## Descripción
Las tablas `farmacias_detalles_productos`, `hfarmacias_detalles_productos` y `mw_farmacias_detalles_productos` en el esquema DDL generado por el servicio web incluían una columna `COMENTARIOS TEXT(255)` que no existe en la cartera generada por el ClickOnce original.

## Impacto
Diferencia de esquema entre la cartera del ClickOnce (8 columnas) y la del servicio web (9 columnas).

## Solución
Se eliminó la columna `COMENTARIOS` de las 3 tablas en `GeneradorService.cs` para que el esquema coincida con el ClickOnce (8 columnas).

## Archivos modificados
- `WebApplication1/Services/GeneradorService.cs`

## Versión
v4.1.6
