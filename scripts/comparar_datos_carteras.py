#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para comparar datos entre carteras ClickOne y Web Service
Compara tabla por tabla los INSERT statements
"""

import re
from collections import defaultdict

def extraer_inserts_por_tabla(archivo):
    """Extrae todos los INSERT statements agrupados por tabla"""
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    # Patrón para capturar INSERT INTO tabla (...) VALUES (...);
    patron = r'INSERT INTO\s+([^\s(]+)\s*\(([^)]+)\)\s*VALUES\s*\(([^;]+)\);'
    
    inserts_por_tabla = defaultdict(list)
    
    for match in re.finditer(patron, contenido, re.IGNORECASE):
        tabla = match.group(1).strip().lower()
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
    print("=" * 80)
    print("COMPARACIÓN DE DATOS - CARTERAS ZONA 343")
    print("=" * 80)
    print()
    
    print("Extrayendo datos de ClickOne...")
    clickone_data = extraer_inserts_por_tabla(clickone_file)
    
    print("Extrayendo datos de Web Service...")
    web_data = extraer_inserts_por_tabla(web_file)
    
    # Obtener todas las tablas
    todas_tablas = set(clickone_data.keys()) | set(web_data.keys())
    tablas_ordenadas = sorted(todas_tablas)
    
    print(f"\nTotal de tablas con datos: {len(tablas_ordenadas)}")
    print()
    
    # Resumen general
    print("=" * 80)
    print("RESUMEN GENERAL")
    print("=" * 80)
    
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
        print("=" * 80)
        print("⚠️  TABLAS CON DIFERENTE NÚMERO DE REGISTROS")
        print("=" * 80)
        for tabla, click_count, web_count in tablas_diferentes:
            diff = web_count - click_count
            simbolo = "+" if diff > 0 else ""
            print(f"\n📊 {tabla}")
            print(f"   ClickOne: {click_count} registros")
            print(f"   Web:      {web_count} registros")
            print(f"   Diferencia: {simbolo}{diff}")
    
    # Tablas solo en ClickOne
    if tablas_solo_clickone:
        print()
        print("=" * 80)
        print("🔵 TABLAS SOLO EN CLICKONE")
        print("=" * 80)
        for tabla, count in tablas_solo_clickone:
            print(f"   {tabla}: {count} registros")
    
    # Tablas solo en Web
    if tablas_solo_web:
        print()
        print("=" * 80)
        print("🟡 TABLAS SOLO EN WEB SERVICE")
        print("=" * 80)
        for tabla, count in tablas_solo_web:
            print(f"   {tabla}: {count} registros")
    
    # Tablas OK (solo resumen)
    if tablas_ok:
        print()
        print("=" * 80)
        print(f"✅ TABLAS OK ({len(tablas_ok)} tablas)")
        print("=" * 80)
        print("Estas tablas tienen el mismo número de registros en ambas carteras.")
        print("(Use --verbose para ver la lista completa)")
    
    return {
        'tablas_ok': tablas_ok,
        'tablas_diferentes': tablas_diferentes,
        'tablas_solo_clickone': tablas_solo_clickone,
        'tablas_solo_web': tablas_solo_web,
        'clickone_data': clickone_data,
        'web_data': web_data
    }

def comparar_tabla_detallada(tabla, clickone_data, web_data):
    """Compara en detalle una tabla específica"""
    print()
    print("=" * 80)
    print(f"COMPARACIÓN DETALLADA: {tabla}")
    print("=" * 80)
    
    click_inserts = clickone_data.get(tabla, [])
    web_inserts = web_data.get(tabla, [])
    
    print(f"\nClickOne: {len(click_inserts)} registros")
    print(f"Web:      {len(web_inserts)} registros")
    
    if len(click_inserts) == 0 and len(web_inserts) == 0:
        print("\n⚠️  Ambas carteras no tienen datos para esta tabla")
        return
    
    # Mostrar primeros registros de cada uno
    print("\n--- PRIMEROS 3 REGISTROS EN CLICKONE ---")
    for i, insert in enumerate(click_inserts[:3]):
        print(f"\n{i+1}. {insert['completo'][:200]}...")
    
    print("\n--- PRIMEROS 3 REGISTROS EN WEB ---")
    for i, insert in enumerate(web_inserts[:3]):
        print(f"\n{i+1}. {insert['completo'][:200]}...")
    
    # Comparar estructura de columnas
    if click_inserts and web_inserts:
        click_cols = click_inserts[0]['columnas']
        web_cols = web_inserts[0]['columnas']
        
        if click_cols != web_cols:
            print("\n⚠️  DIFERENCIA EN COLUMNAS:")
            print(f"ClickOne: {click_cols}")
            print(f"Web:      {web_cols}")
        else:
            print("\n✅ Las columnas son idénticas")

if __name__ == '__main__':
    clickone_file = 'test_carteras/clickOne/Cartera_zona_343.txt'
    web_file = 'test_carteras/web_localhost/Cartera_zona_343.txt'
    
    resultado = comparar_tablas(clickone_file, web_file)
    
    # Preguntar si quiere ver detalles de alguna tabla
    print()
    print("=" * 80)
    print("ANÁLISIS INTERACTIVO")
    print("=" * 80)
    print()
    print("Para ver detalles de una tabla específica, ejecute:")
    print("  python scripts/comparar_datos_carteras.py <nombre_tabla>")
    print()
    print("Ejemplo:")
    print("  python scripts/comparar_datos_carteras.py fichero")
    print()
    
    # Si hay tablas con diferencias, sugerir cuáles revisar
    if resultado['tablas_diferentes']:
        print("Tablas sugeridas para revisar (tienen diferencias):")
        for tabla, _, _ in resultado['tablas_diferentes'][:5]:
            print(f"  - {tabla}")
