#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para verificar qué tablas MW_ deberían estar en minúsculas
"""

import re

def extraer_tablas_MW(archivo):
    """Extrae tablas que empiezan con MW_ o mw_"""
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    # Buscar CREATE TABLE con MW_ o mw_
    patron = r'CREATE TABLE "([Mm][Ww]_[^"]+)"'
    tablas = re.findall(patron, contenido)
    
    return tablas

# Leer ambos archivos
tablas_click = extraer_tablas_MW("../test_carteras/clickOne/Cartera_zona_343.txt")
tablas_web = extraer_tablas_MW("../test_carteras/web_localhost/Cartera_zona_343_6.txt")

print("="*80)
print("TABLAS MW_ EN CLICKONE:")
print("="*80)
for tabla in sorted(set(tablas_click)):
    print(f"  {tabla}")

print(f"\nTotal: {len(set(tablas_click))} tablas")

print("\n" + "="*80)
print("TABLAS MW_ EN WEB SERVICE:")
print("="*80)
for tabla in sorted(set(tablas_web)):
    print(f"  {tabla}")

print(f"\nTotal: {len(set(tablas_web))} tablas")

# Comparar
print("\n" + "="*80)
print("DIFERENCIAS:")
print("="*80)

tablas_click_set = set(t.lower() for t in tablas_click)
tablas_web_dict = {t.lower(): t for t in tablas_web}

print("\nTablas que están en MAYÚSCULAS en Web pero en minúsculas en ClickOne:")
for tabla_click in sorted(set(tablas_click)):
    tabla_web = tablas_web_dict.get(tabla_click.lower())
    if tabla_web and tabla_click != tabla_web:
        print(f"  ClickOne: {tabla_click:40} → Web: {tabla_web}")
