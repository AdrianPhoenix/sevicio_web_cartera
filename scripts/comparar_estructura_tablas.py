#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para comparar la estructura de tablas (columnas y tipos) entre carteras
ClickOne vs Web Service
"""

import re
from collections import defaultdict

def extraer_estructura_tablas(archivo):
    """Extrae las definiciones CREATE TABLE de un archivo"""
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    # Dividir por CREATE TABLE
    bloques = contenido.split('CREATE TABLE')
    
    tablas = {}
    
    for bloque in bloques[1:]:  # Saltar el primer bloque vacío
        # Extraer nombre de tabla
        match_nombre = re.search(r'"([^"]+)"', bloque)
        if not match_nombre:
            continue
        
        nombre_tabla = match_nombre.group(1).lower()
        
        # Extraer definición de columnas (entre paréntesis)
        match_columnas = re.search(r'\((.+?)\);', bloque, re.DOTALL)
        if not match_columnas:
            continue
        
        definicion_columnas = match_columnas.group(1)
        
        # Parsear columnas
        columnas = []
        # Dividir por comas, pero respetando paréntesis
        partes = re.split(r',\s*(?![^()]*\))', definicion_columnas)
        
        for parte in partes:
            parte = parte.strip()
            if not parte:
                continue
            
            # Extraer nombre de columna y tipo
            match_col = re.match(r'"([^"]+)"\s+(.+)', parte)
            if match_col:
                col_nombre = match_col.group(1)
                col_tipo = match_col.group(2).strip()
                columnas.append({
                    'nombre': col_nombre,
                    'tipo': col_tipo,
                    'definicion': parte
                })
        
        tablas[nombre_tabla] = {
            'columnas': columnas,
            'num_columnas': len(columnas)
        }
    
    return tablas

def comparar_estructuras(clickone_file, web_file):
    """Compara las estructuras de tablas entre ambas carteras"""
    print("=" * 100)
    print("COMPARACIÓN DE ESTRUCTURA DE TABLAS - ZONA 343")
    print("=" * 100)
    print()
    
    print("📖 Extrayendo estructura de ClickOne...")
    clickone_tablas = extraer_estructura_tablas(clickone_file)
    print(f"   Encontradas: {len(clickone_tablas)} tablas")
    
    print("📖 Extrayendo estructura de Web Service...")
    web_tablas = extraer_estructura_tablas(web_file)
    print(f"   Encontradas: {len(web_tablas)} tablas")
    
    # Obtener todas las tablas
    todas_tablas = set(clickone_tablas.keys()) | set(web_tablas.keys())
    tablas_comunes = set(clickone_tablas.keys()) & set(web_tablas.keys())
    
    print()
    print("=" * 100)
    print("RESUMEN")
    print("=" * 100)
    print(f"Tablas en ClickOne:     {len(clickone_tablas)}")
    print(f"Tablas en Web Service:  {len(web_tablas)}")
    print(f"Tablas comunes:         {len(tablas_comunes)}")
    print(f"Solo en ClickOne:       {len(clickone_tablas.keys() - web_tablas.keys())}")
    print(f"Solo en Web Service:    {len(web_tablas.keys() - clickone_tablas.keys())}")
    
    # Comparar tablas comunes
    tablas_identicas = []
    tablas_diferentes = []
    
    for tabla in sorted(tablas_comunes):
        click_cols = clickone_tablas[tabla]['columnas']
        web_cols = web_tablas[tabla]['columnas']
        
        # Comparar número de columnas
        if len(click_cols) != len(web_cols):
            tablas_diferentes.append({
                'tabla': tabla,
                'tipo': 'num_columnas',
                'clickone': len(click_cols),
                'web': len(web_cols)
            })
            continue
        
        # Comparar columnas una por una
        diferencias = []
        for i, (click_col, web_col) in enumerate(zip(click_cols, web_cols)):
            if click_col['nombre'] != web_col['nombre']:
                diferencias.append({
                    'posicion': i,
                    'tipo': 'nombre',
                    'clickone': click_col['nombre'],
                    'web': web_col['nombre']
                })
            elif click_col['tipo'] != web_col['tipo']:
                diferencias.append({
                    'posicion': i,
                    'columna': click_col['nombre'],
                    'tipo': 'tipo_dato',
                    'clickone': click_col['tipo'],
                    'web': web_col['tipo']
                })
        
        if diferencias:
            tablas_diferentes.append({
                'tabla': tabla,
                'tipo': 'columnas',
                'diferencias': diferencias,
                'clickone_cols': click_cols,
                'web_cols': web_cols
            })
        else:
            tablas_identicas.append(tabla)
    
    print()
    print("=" * 100)
    print("RESULTADO DE COMPARACIÓN")
    print("=" * 100)
    print(f"✅ Tablas idénticas:    {len(tablas_identicas)}")
    print(f"⚠️  Tablas diferentes:   {len(tablas_diferentes)}")
    
    # Mostrar tablas diferentes
    if tablas_diferentes:
        print()
        print("=" * 100)
        print("⚠️  TABLAS CON DIFERENCIAS EN ESTRUCTURA")
        print("=" * 100)
        
        for item in tablas_diferentes:
            tabla = item['tabla']
            print()
            print(f"📋 Tabla: {tabla}")
            print("-" * 100)
            
            if item['tipo'] == 'num_columnas':
                print(f"   ❌ Diferente número de columnas:")
                print(f"      ClickOne:    {item['clickone']} columnas")
                print(f"      Web Service: {item['web']} columnas")
            
            elif item['tipo'] == 'columnas':
                print(f"   ❌ Diferencias en columnas:")
                for diff in item['diferencias']:
                    if diff['tipo'] == 'nombre':
                        print(f"      Posición {diff['posicion']}:")
                        print(f"         ClickOne:    \"{diff['clickone']}\"")
                        print(f"         Web Service: \"{diff['web']}\"")
                    elif diff['tipo'] == 'tipo_dato':
                        print(f"      Columna \"{diff['columna']}\":")
                        print(f"         ClickOne:    {diff['clickone']}")
                        print(f"         Web Service: {diff['web']}")
    
    # Tablas solo en ClickOne
    solo_clickone = sorted(clickone_tablas.keys() - web_tablas.keys())
    if solo_clickone:
        print()
        print("=" * 100)
        print("🔵 TABLAS SOLO EN CLICKONE")
        print("=" * 100)
        for tabla in solo_clickone:
            num_cols = clickone_tablas[tabla]['num_columnas']
            print(f"   {tabla} ({num_cols} columnas)")
    
    # Tablas solo en Web
    solo_web = sorted(web_tablas.keys() - clickone_tablas.keys())
    if solo_web:
        print()
        print("=" * 100)
        print("🟡 TABLAS SOLO EN WEB SERVICE")
        print("=" * 100)
        for tabla in solo_web:
            num_cols = web_tablas[tabla]['num_columnas']
            print(f"   {tabla} ({num_cols} columnas)")
    
    print()
    print("=" * 100)
    
    return {
        'tablas_identicas': tablas_identicas,
        'tablas_diferentes': tablas_diferentes,
        'solo_clickone': solo_clickone,
        'solo_web': solo_web,
        'clickone_tablas': clickone_tablas,
        'web_tablas': web_tablas
    }

def mostrar_detalle_tabla(tabla, clickone_tablas, web_tablas):
    """Muestra el detalle completo de una tabla específica"""
    print()
    print("=" * 100)
    print(f"DETALLE DE TABLA: {tabla}")
    print("=" * 100)
    
    if tabla in clickone_tablas:
        print()
        print("📘 CLICKONE:")
        print("-" * 100)
        for i, col in enumerate(clickone_tablas[tabla]['columnas']):
            print(f"   {i+1:3d}. \"{col['nombre']}\" {col['tipo']}")
    else:
        print("\n⚠️  Tabla NO existe en ClickOne")
    
    if tabla in web_tablas:
        print()
        print("📗 WEB SERVICE:")
        print("-" * 100)
        for i, col in enumerate(web_tablas[tabla]['columnas']):
            print(f"   {i+1:3d}. \"{col['nombre']}\" {col['tipo']}")
    else:
        print("\n⚠️  Tabla NO existe en Web Service")
    
    print()

if __name__ == '__main__':
    import sys
    import os
    
    # Ajustar rutas según desde dónde se ejecute
    if os.path.exists('test_carteras'):
        base_path = ''
    else:
        base_path = '../'
    
    clickone_file = f'{base_path}test_carteras/clickOne/Cartera_zona_343.txt'
    web_file = f'{base_path}test_carteras/web_localhost/Cartera_zona_343.txt'
    
    resultado = comparar_estructuras(clickone_file, web_file)
    
    # Si se pasa un nombre de tabla como argumento, mostrar detalle
    if len(sys.argv) > 1:
        tabla = sys.argv[1].lower()
        mostrar_detalle_tabla(
            tabla, 
            resultado['clickone_tablas'], 
            resultado['web_tablas']
        )
    else:
        print()
        print("💡 Para ver el detalle de una tabla específica:")
        print("   python scripts/comparar_estructura_tablas.py <nombre_tabla>")
        print()
        print("Ejemplo:")
        print("   python scripts/comparar_estructura_tablas.py fichero")
