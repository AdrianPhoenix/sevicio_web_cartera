#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para verificar que las 4 tablas están correctamente en minúsculas
"""

import re

def extraer_tablas_especificas(archivo, tablas_buscar):
    """Extrae las tablas específicas del archivo"""
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    resultados = {}
    for tabla in tablas_buscar:
        # Buscar tanto en mayúsculas como minúsculas
        patron_min = rf'CREATE TABLE "{tabla.lower()}"'
        patron_may = rf'CREATE TABLE "{tabla.upper()}"'
        
        if re.search(patron_min, contenido):
            resultados[tabla] = tabla.lower()
        elif re.search(patron_may, contenido):
            resultados[tabla] = tabla.upper()
        else:
            resultados[tabla] = "NO ENCONTRADA"
    
    return resultados

# Tablas a verificar
tablas_verificar = ['mw_lineas', 'mw_marcas', 'mw_regiones', 'mw_tipomedicos']

print("="*80)
print("🔍 VERIFICACIÓN DE CORRECCIÓN DE MAYÚSCULAS/MINÚSCULAS")
print("="*80)

archivo_clickone = "../test_carteras/clickOne/Cartera_zona_343.txt"
archivo_web_anterior = "../test_carteras/web_localhost/Cartera_zona_343_6.txt"
archivo_web_nuevo = "../test_carteras/web_localhost/Cartera_zona_343_7.txt"

print(f"\n📁 Archivos:")
print(f"   ClickOne (referencia): {archivo_clickone}")
print(f"   Web Service (anterior): {archivo_web_anterior}")
print(f"   Web Service (nuevo):    {archivo_web_nuevo}")

# Extraer tablas de cada archivo
print("\n" + "="*80)
print("📊 RESULTADOS")
print("="*80)

tablas_click = extraer_tablas_especificas(archivo_clickone, tablas_verificar)
tablas_web_ant = extraer_tablas_especificas(archivo_web_anterior, tablas_verificar)
tablas_web_new = extraer_tablas_especificas(archivo_web_nuevo, tablas_verificar)

print(f"\n{'Tabla':<25} {'ClickOne':<20} {'Web (anterior)':<20} {'Web (nuevo)':<20} {'Estado'}")
print("-"*110)

todo_correcto = True
for tabla in tablas_verificar:
    click = tablas_click[tabla]
    web_ant = tablas_web_ant[tabla]
    web_new = tablas_web_new[tabla]
    
    # Verificar si está correcto
    if click == web_new:
        estado = "✅ CORRECTO"
    else:
        estado = "❌ INCORRECTO"
        todo_correcto = False
    
    print(f"{tabla:<25} {click:<20} {web_ant:<20} {web_new:<20} {estado}")

print("\n" + "="*80)
print("💡 CONCLUSIÓN")
print("="*80)

if todo_correcto:
    print("\n✅ ¡TODAS LAS TABLAS ESTÁN CORRECTAS!")
    print("   Las 4 tablas ahora coinciden con ClickOne (minúsculas)")
    print("\n📋 Tablas corregidas:")
    for tabla in tablas_verificar:
        print(f"   - {tabla}")
    print("\n🎉 La inconsistencia de mayúsculas/minúsculas ha sido resuelta.")
else:
    print("\n⚠️  Algunas tablas aún tienen inconsistencias")
    print("   Revisar el código en GeneradorService.cs")

print("\n" + "="*80)
