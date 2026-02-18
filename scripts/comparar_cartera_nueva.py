#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para comparar la nueva cartera generada con ClickOne
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

# Tablas que corregimos
TABLAS_CORREGIDAS = ['farmacias_personal', 'mw_productos', 'visita_detalles', 'visitas']

import os

# Ajustar rutas según desde dónde se ejecute
if os.path.exists('test_carteras'):
    base_path = ''
else:
    base_path = '../'

clickone_file = f'{base_path}test_carteras/clickOne/Cartera_zona_343.txt'
web_file = f'{base_path}test_carteras/web_localhost/Cartera_zona_343_2.txt'

print("=" * 100)
print("VERIFICACIÓN DE CORRECCIONES - CARTERA NUEVA")
print("=" * 100)
print()

print("📖 Extrayendo estructura de ClickOne...")
clickone_tablas = extraer_estructura_tablas(clickone_file)

print("📖 Extrayendo estructura de Web Service (NUEVA)...")
web_tablas = extraer_estructura_tablas(web_file)

print()
print("=" * 100)
print("VERIFICACIÓN DE LAS 4 TABLAS CORREGIDAS")
print("=" * 100)

for tabla in TABLAS_CORREGIDAS:
    print()
    print(f"📋 Tabla: {tabla}")
    print("-" * 100)
    
    if tabla not in clickone_tablas:
        print(f"   ⚠️  Tabla NO existe en ClickOne")
        continue
    
    if tabla not in web_tablas:
        print(f"   ⚠️  Tabla NO existe en Web Service")
        continue
    
    click_cols = clickone_tablas[tabla]['columnas']
    web_cols = web_tablas[tabla]['columnas']
    
    # Comparar número de columnas
    if len(click_cols) != len(web_cols):
        print(f"   ❌ Diferente número de columnas:")
        print(f"      ClickOne:    {len(click_cols)} columnas")
        print(f"      Web Service: {len(web_cols)} columnas")
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
        print(f"   ❌ Diferencias encontradas:")
        for diff in diferencias:
            if diff['tipo'] == 'nombre':
                print(f"      Posición {diff['posicion']}:")
                print(f"         ClickOne:    \"{diff['clickone']}\"")
                print(f"         Web Service: \"{diff['web']}\"")
            elif diff['tipo'] == 'tipo_dato':
                print(f"      Columna \"{diff['columna']}\":")
                print(f"         ClickOne:    {diff['clickone']}")
                print(f"         Web Service: {diff['web']}")
    else:
        print(f"   ✅ IDÉNTICA - Estructura 100% igual a ClickOne")

print()
print("=" * 100)
print("RESUMEN")
print("=" * 100)

tablas_ok = []
tablas_con_diferencias = []

for tabla in TABLAS_CORREGIDAS:
    if tabla not in clickone_tablas or tabla not in web_tablas:
        continue
    
    click_cols = clickone_tablas[tabla]['columnas']
    web_cols = web_tablas[tabla]['columnas']
    
    if len(click_cols) != len(web_cols):
        tablas_con_diferencias.append(tabla)
        continue
    
    diferencias = []
    for click_col, web_col in zip(click_cols, web_cols):
        if click_col['nombre'] != web_col['nombre'] or click_col['tipo'] != web_col['tipo']:
            diferencias.append(True)
    
    if diferencias:
        tablas_con_diferencias.append(tabla)
    else:
        tablas_ok.append(tabla)

print()
print(f"✅ Tablas corregidas exitosamente: {len(tablas_ok)}/{len(TABLAS_CORREGIDAS)}")
if tablas_ok:
    for tabla in tablas_ok:
        print(f"   - {tabla}")

if tablas_con_diferencias:
    print()
    print(f"❌ Tablas con diferencias pendientes: {len(tablas_con_diferencias)}/{len(TABLAS_CORREGIDAS)}")
    for tabla in tablas_con_diferencias:
        print(f"   - {tabla}")

print()
print("=" * 100)
