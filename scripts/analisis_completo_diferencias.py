#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Análisis completo de TODAS las diferencias entre ClickOne y Web Service
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

import os

# Ajustar rutas según desde dónde se ejecute
if os.path.exists('test_carteras'):
    base_path = ''
else:
    base_path = '../'

clickone_file = f'{base_path}test_carteras/clickOne/Cartera_zona_343.txt'
web_file = f'{base_path}test_carteras/web_localhost/Cartera_zona_343_2.txt'

print("=" * 120)
print("ANÁLISIS COMPLETO DE DIFERENCIAS - TODAS LAS TABLAS")
print("=" * 120)
print()

print("📖 Extrayendo estructura de ClickOne...")
clickone_tablas = extraer_estructura_tablas(clickone_file)
print(f"   Encontradas: {len(clickone_tablas)} tablas")

print("📖 Extrayendo estructura de Web Service (NUEVA)...")
web_tablas = extraer_estructura_tablas(web_file)
print(f"   Encontradas: {len(web_tablas)} tablas")

# Obtener tablas comunes
tablas_comunes = set(clickone_tablas.keys()) & set(web_tablas.keys())

print()
print("=" * 120)
print("ANÁLISIS DETALLADO")
print("=" * 120)

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
            'web': len(web_cols),
            'clickone_cols': click_cols,
            'web_cols': web_cols
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
print(f"✅ Tablas idénticas: {len(tablas_identicas)}")
print(f"⚠️  Tablas con diferencias: {len(tablas_diferentes)}")

if tablas_diferentes:
    print()
    print("=" * 120)
    print("⚠️  TABLAS CON DIFERENCIAS")
    print("=" * 120)
    
    # Categorizar diferencias
    falta_columnas = []
    sobran_columnas = []
    orden_diferente = []
    tipo_diferente = []
    
    for item in tablas_diferentes:
        tabla = item['tabla']
        
        if item['tipo'] == 'num_columnas':
            if item['web'] < item['clickone']:
                falta_columnas.append(item)
            else:
                sobran_columnas.append(item)
        elif item['tipo'] == 'columnas':
            tiene_nombres_diferentes = any(d['tipo'] == 'nombre' for d in item['diferencias'])
            tiene_tipos_diferentes = any(d['tipo'] == 'tipo_dato' for d in item['diferencias'])
            
            if tiene_nombres_diferentes:
                orden_diferente.append(item)
            if tiene_tipos_diferentes:
                tipo_diferente.append(item)
    
    # Mostrar tablas con columnas faltantes
    if falta_columnas:
        print()
        print("🔴 CRÍTICO: Tablas con COLUMNAS FALTANTES en Web Service")
        print("-" * 120)
        for item in falta_columnas:
            print(f"\n📋 {item['tabla']}")
            print(f"   ClickOne: {item['clickone']} columnas")
            print(f"   Web:      {item['web']} columnas")
            print(f"   Faltan:   {item['clickone'] - item['web']} columnas")
            
            # Identificar columnas faltantes
            click_nombres = {col['nombre'] for col in item['clickone_cols']}
            web_nombres = {col['nombre'] for col in item['web_cols']}
            faltantes = click_nombres - web_nombres
            
            if faltantes:
                print(f"   Columnas faltantes: {', '.join(sorted(faltantes))}")
    
    # Mostrar tablas con columnas extra
    if sobran_columnas:
        print()
        print("🟡 ADVERTENCIA: Tablas con COLUMNAS EXTRA en Web Service")
        print("-" * 120)
        for item in sobran_columnas:
            print(f"\n📋 {item['tabla']}")
            print(f"   ClickOne: {item['clickone']} columnas")
            print(f"   Web:      {item['web']} columnas")
            print(f"   Extras:   {item['web'] - item['clickone']} columnas")
            
            # Identificar columnas extra
            click_nombres = {col['nombre'] for col in item['clickone_cols']}
            web_nombres = {col['nombre'] for col in item['web_cols']}
            extras = web_nombres - click_nombres
            
            if extras:
                print(f"   Columnas extra: {', '.join(sorted(extras))}")
    
    # Mostrar tablas con orden diferente
    if orden_diferente:
        print()
        print("🟠 ADVERTENCIA: Tablas con ORDEN DE COLUMNAS diferente")
        print("-" * 120)
        for item in orden_diferente:
            print(f"\n📋 {item['tabla']}")
            for diff in item['diferencias']:
                if diff['tipo'] == 'nombre':
                    print(f"   Posición {diff['posicion']}:")
                    print(f"      ClickOne:    \"{diff['clickone']}\"")
                    print(f"      Web Service: \"{diff['web']}\"")
    
    # Mostrar tablas con tipos diferentes
    if tipo_diferente:
        print()
        print("🟡 ADVERTENCIA: Tablas con TIPOS DE DATOS diferentes")
        print("-" * 120)
        for item in tipo_diferente:
            print(f"\n📋 {item['tabla']}")
            for diff in item['diferencias']:
                if diff['tipo'] == 'tipo_dato':
                    print(f"   Columna \"{diff['columna']}\":")
                    print(f"      ClickOne:    {diff['clickone']}")
                    print(f"      Web Service: {diff['web']}")

print()
print("=" * 120)
print("RESUMEN POR CATEGORÍA")
print("=" * 120)
print()
print(f"🔴 Tablas con columnas FALTANTES:        {len(falta_columnas)}")
print(f"🟡 Tablas con columnas EXTRA:            {len(sobran_columnas)}")
print(f"🟠 Tablas con ORDEN diferente:           {len(orden_diferente)}")
print(f"🟡 Tablas con TIPOS DE DATOS diferentes: {len(tipo_diferente)}")
print(f"✅ Tablas IDÉNTICAS:                     {len(tablas_identicas)}")
print()
print("=" * 120)
