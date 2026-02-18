#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para comparar datos completos entre carteras ClickOne y Web Service
Compara tabla por tabla los INSERT statements con análisis detallado
"""

import re
from collections import defaultdict

def extraer_inserts_por_tabla(archivo):
    """Extrae todos los INSERT statements agrupados por tabla"""
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    # Patrón para capturar INSERT INTO tabla (...) VALUES (...);
    # Maneja tanto comillas simples como dobles en nombres de tabla
    patron = r'INSERT INTO\s+["\']?([^\s("\']+)["\']?\s*\(([^)]+)\)\s*VALUES\s*\(([^;]+)\);'
    
    inserts_por_tabla = defaultdict(list)
    
    for match in re.finditer(patron, contenido, re.IGNORECASE):
        tabla = match.group(1).strip().strip('"').strip("'").lower()
        columnas = match.group(2).strip()
        valores = match.group(3).strip()
        
        inserts_por_tabla[tabla].append({
            'columnas': columnas,
            'valores': valores,
            'completo': match.group(0)
        })
    
    return inserts_por_tabla

def comparar_tablas(clickone_file, web_file):
    """Compara las tablas entre ambas carteras"""
    print("=" * 100)
    print("🔍 COMPARACIÓN DE DATOS - CARTERAS ZONA 343")
    print("=" * 100)
    print()
    
    print(f"📁 Archivos:")
    print(f"   ClickOne:    {clickone_file}")
    print(f"   Web Service: {web_file}")
    print()
    
    print("⏳ Extrayendo datos de ClickOne...")
    clickone_data = extraer_inserts_por_tabla(clickone_file)
    
    print("⏳ Extrayendo datos de Web Service...")
    web_data = extraer_inserts_por_tabla(web_file)
    
    # Obtener todas las tablas
    todas_tablas = set(clickone_data.keys()) | set(web_data.keys())
    tablas_ordenadas = sorted(todas_tablas)
    
    print(f"\n📊 Total de tablas con datos: {len(tablas_ordenadas)}")
    print()
    
    # Resumen general
    print("=" * 100)
    print("📋 RESUMEN GENERAL")
    print("=" * 100)
    
    tablas_ok = []
    tablas_diferentes = []
    tablas_solo_clickone = []
    tablas_solo_web = []
    
    for tabla in tablas_ordenadas:
        clickone_count = len(clickone_data.get(tabla, []))
        web_count = len(web_data.get(tabla, []))
        
        if clickone_count == 0 and web_count > 0:
            tablas_solo_web.append((tabla, web_count))
        elif web_count == 0 and clickone_count > 0:
            tablas_solo_clickone.append((tabla, clickone_count))
        elif clickone_count == web_count:
            tablas_ok.append((tabla, clickone_count))
        else:
            tablas_diferentes.append((tabla, clickone_count, web_count))
    
    print(f"\n✅ Tablas con mismo número de registros: {len(tablas_ok)}")
    print(f"⚠️  Tablas con diferente número de registros: {len(tablas_diferentes)}")
    print(f"🔵 Tablas solo en ClickOne: {len(tablas_solo_clickone)}")
    print(f"🟡 Tablas solo en Web: {len(tablas_solo_web)}")
    
    # Detalles de tablas con diferencias
    if tablas_diferentes:
        print()
        print("=" * 100)
        print("⚠️  TABLAS CON DIFERENTE NÚMERO DE REGISTROS")
        print("=" * 100)
        print()
        print(f"{'Tabla':<40} {'ClickOne':>12} {'Web':>12} {'Diferencia':>15}")
        print("-" * 100)
        
        for tabla, click_count, web_count in sorted(tablas_diferentes, key=lambda x: abs(x[2]-x[1]), reverse=True):
            diff = web_count - click_count
            simbolo = "+" if diff > 0 else ""
            print(f"{tabla:<40} {click_count:>12} {web_count:>12} {simbolo}{diff:>14}")
    
    # Tablas solo en ClickOne
    if tablas_solo_clickone:
        print()
        print("=" * 100)
        print("🔵 TABLAS SOLO EN CLICKONE")
        print("=" * 100)
        print()
        for tabla, count in sorted(tablas_solo_clickone):
            print(f"   {tabla:<50} {count:>5} registros")
    
    # Tablas solo en Web
    if tablas_solo_web:
        print()
        print("=" * 100)
        print("🟡 TABLAS SOLO EN WEB SERVICE")
        print("=" * 100)
        print()
        for tabla, count in sorted(tablas_solo_web):
            print(f"   {tabla:<50} {count:>5} registros")
    
    # Tablas OK (resumen compacto)
    if tablas_ok:
        print()
        print("=" * 100)
        print(f"✅ TABLAS CON MISMO NÚMERO DE REGISTROS ({len(tablas_ok)} tablas)")
        print("=" * 100)
        print()
        
        # Mostrar en columnas
        total_registros_click = sum(count for _, count in tablas_ok)
        total_registros_web = sum(count for _, count in tablas_ok)
        
        print(f"Total de registros en estas tablas:")
        print(f"   ClickOne:    {total_registros_click:,} registros")
        print(f"   Web Service: {total_registros_web:,} registros")
        print()
        
        # Mostrar primeras 10 tablas como ejemplo
        print("Ejemplos (primeras 10 tablas):")
        for tabla, count in sorted(tablas_ok)[:10]:
            print(f"   {tabla:<50} {count:>5} registros")
        
        if len(tablas_ok) > 10:
            print(f"   ... y {len(tablas_ok) - 10} tablas más")
    
    # Análisis de columnas
    print()
    print("=" * 100)
    print("🔍 ANÁLISIS DE ESTRUCTURA DE COLUMNAS")
    print("=" * 100)
    
    tablas_columnas_diferentes = []
    
    for tabla in tablas_ordenadas:
        click_inserts = clickone_data.get(tabla, [])
        web_inserts = web_data.get(tabla, [])
        
        if click_inserts and web_inserts:
            click_cols = click_inserts[0]['columnas']
            web_cols = web_inserts[0]['columnas']
            
            if click_cols != web_cols:
                tablas_columnas_diferentes.append((tabla, click_cols, web_cols))
    
    if tablas_columnas_diferentes:
        print(f"\n⚠️  {len(tablas_columnas_diferentes)} tablas tienen diferencias en columnas:")
        print()
        for tabla, click_cols, web_cols in tablas_columnas_diferentes[:10]:
            print(f"\n📊 {tabla}")
            print(f"   ClickOne: {click_cols[:80]}...")
            print(f"   Web:      {web_cols[:80]}...")
        
        if len(tablas_columnas_diferentes) > 10:
            print(f"\n   ... y {len(tablas_columnas_diferentes) - 10} tablas más con diferencias")
    else:
        print("\n✅ Todas las tablas tienen la misma estructura de columnas")
    
    # Conclusión
    print()
    print("=" * 100)
    print("💡 CONCLUSIÓN")
    print("=" * 100)
    print()
    
    if len(tablas_diferentes) == 0 and len(tablas_solo_clickone) == 0 and len(tablas_solo_web) == 0:
        print("✅ ¡PERFECTO! Los datos son idénticos entre ClickOne y Web Service")
        print("   - Mismo número de tablas")
        print("   - Mismo número de registros en cada tabla")
    else:
        print("⚠️  Hay diferencias en los datos:")
        if tablas_diferentes:
            print(f"   - {len(tablas_diferentes)} tablas con diferente número de registros")
        if tablas_solo_clickone:
            print(f"   - {len(tablas_solo_clickone)} tablas solo en ClickOne")
        if tablas_solo_web:
            print(f"   - {len(tablas_solo_web)} tablas solo en Web Service")
        
        print()
        print("💡 Recomendaciones:")
        if tablas_diferentes:
            print("   1. Revisar por qué hay diferencias en el número de registros")
            print("   2. Verificar si los datos adicionales/faltantes son intencionales")
        if tablas_solo_web:
            print("   3. Las tablas solo en Web pueden ser mejoras intencionales")
        if tablas_solo_clickone:
            print("   4. Las tablas solo en ClickOne pueden necesitar ser agregadas al Web Service")
    
    print()
    print("=" * 100)
    
    return {
        'tablas_ok': tablas_ok,
        'tablas_diferentes': tablas_diferentes,
        'tablas_solo_clickone': tablas_solo_clickone,
        'tablas_solo_web': tablas_solo_web,
        'tablas_columnas_diferentes': tablas_columnas_diferentes,
        'clickone_data': clickone_data,
        'web_data': web_data
    }

if __name__ == '__main__':
    clickone_file = '../test_carteras/clickOne/Cartera_zona_343.txt'
    web_file = '../test_carteras/web_localhost/Cartera_zona_343_8.txt'
    
    resultado = comparar_tablas(clickone_file, web_file)

