#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para analizar el patrón de mayúsculas/minúsculas en nombres de tablas
"""

import re

def extraer_nombres_tablas(archivo):
    """Extrae todos los nombres de tablas de un archivo"""
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    # Buscar todos los CREATE TABLE
    patron = r'CREATE TABLE "([^"]+)"'
    tablas = re.findall(patron, contenido)
    
    return tablas

def analizar_patron(tablas):
    """Analiza el patrón de mayúsculas/minúsculas"""
    
    # Categorizar tablas
    todas_minusculas = []
    todas_mayusculas = []
    mixtas = []
    con_mw = []
    con_MW = []
    
    for tabla in tablas:
        # Verificar si tiene prefijo mw/MW
        if tabla.startswith('mw_'):
            con_mw.append(tabla)
        elif tabla.startswith('MW_'):
            con_MW.append(tabla)
        
        # Verificar patrón de mayúsculas
        if tabla.islower():
            todas_minusculas.append(tabla)
        elif tabla.isupper():
            todas_mayusculas.append(tabla)
        else:
            # Tiene mezcla
            if tabla[0].isupper():
                mixtas.append(tabla)
            else:
                todas_minusculas.append(tabla)
    
    return {
        'todas_minusculas': todas_minusculas,
        'todas_mayusculas': todas_mayusculas,
        'mixtas': mixtas,
        'con_mw': con_mw,
        'con_MW': con_MW
    }

def main():
    print("="*80)
    print("🔍 ANÁLISIS DE MAYÚSCULAS/MINÚSCULAS EN NOMBRES DE TABLAS")
    print("="*80)
    
    archivo_clickone = "../test_carteras/clickOne/Cartera_zona_343.txt"
    archivo_web = "../test_carteras/web_localhost/Cartera_zona_343_6.txt"
    
    print(f"\n📁 Archivos:")
    print(f"   ClickOne: {archivo_clickone}")
    print(f"   Web Service: {archivo_web}")
    
    # Extraer tablas
    tablas_click = extraer_nombres_tablas(archivo_clickone)
    tablas_web = extraer_nombres_tablas(archivo_web)
    
    print(f"\n📊 Total de tablas:")
    print(f"   ClickOne: {len(tablas_click)} tablas")
    print(f"   Web Service: {len(tablas_web)} tablas")
    
    # Analizar ClickOne
    print("\n" + "="*80)
    print("📋 CLICKONE - Patrón de Nombres")
    print("="*80)
    
    patron_click = analizar_patron(tablas_click)
    
    print(f"\n🔤 Tablas con prefijo 'mw_' (minúsculas): {len(patron_click['con_mw'])}")
    if len(patron_click['con_mw']) <= 20:
        for tabla in sorted(patron_click['con_mw']):
            print(f"   - {tabla}")
    else:
        print(f"   (Mostrando primeras 20)")
        for tabla in sorted(patron_click['con_mw'])[:20]:
            print(f"   - {tabla}")
        print(f"   ... y {len(patron_click['con_mw']) - 20} más")
    
    print(f"\n🔤 Tablas con prefijo 'MW_' (mayúsculas): {len(patron_click['con_MW'])}")
    if len(patron_click['con_MW']) <= 20:
        for tabla in sorted(patron_click['con_MW']):
            print(f"   - {tabla}")
    else:
        print(f"   (Mostrando primeras 20)")
        for tabla in sorted(patron_click['con_MW'])[:20]:
            print(f"   - {tabla}")
        print(f"   ... y {len(patron_click['con_MW']) - 20} más")
    
    # Analizar Web Service
    print("\n" + "="*80)
    print("📋 WEB SERVICE - Patrón de Nombres")
    print("="*80)
    
    patron_web = analizar_patron(tablas_web)
    
    print(f"\n🔤 Tablas con prefijo 'mw_' (minúsculas): {len(patron_web['con_mw'])}")
    if len(patron_web['con_mw']) <= 20:
        for tabla in sorted(patron_web['con_mw']):
            print(f"   - {tabla}")
    else:
        print(f"   (Mostrando primeras 20)")
        for tabla in sorted(patron_web['con_mw'])[:20]:
            print(f"   - {tabla}")
        print(f"   ... y {len(patron_web['con_mw']) - 20} más")
    
    print(f"\n🔤 Tablas con prefijo 'MW_' (mayúsculas): {len(patron_web['con_MW'])}")
    if len(patron_web['con_MW']) <= 20:
        for tabla in sorted(patron_web['con_MW']):
            print(f"   - {tabla}")
    else:
        print(f"   (Mostrando primeras 20)")
        for tabla in sorted(patron_web['con_MW'])[:20]:
            print(f"   - {tabla}")
        print(f"   ... y {len(patron_web['con_MW']) - 20} más")
    
    # Comparar diferencias
    print("\n" + "="*80)
    print("🔍 DIFERENCIAS ENTRE CLICKONE Y WEB SERVICE")
    print("="*80)
    
    # Tablas que están en minúsculas en ClickOne pero en mayúsculas en Web
    tablas_click_mw = set(patron_click['con_mw'])
    tablas_web_MW = set(patron_web['con_MW'])
    
    # Buscar tablas que cambiaron de mw_ a MW_
    cambios = []
    for tabla_click in tablas_click_mw:
        # Convertir a mayúsculas para buscar
        tabla_mayus = tabla_click.replace('mw_', 'MW_')
        if tabla_mayus in tablas_web_MW:
            cambios.append((tabla_click, tabla_mayus))
    
    if cambios:
        print(f"\n⚠️  Tablas que cambiaron de 'mw_' a 'MW_': {len(cambios)}")
        for click, web in sorted(cambios):
            print(f"   ClickOne: {click:40} → Web: {web}")
    else:
        print("\n✅ No hay cambios de mw_ a MW_")
    
    # Buscar tablas que están en mayúsculas en ambos
    tablas_click_MW = set(patron_click['con_MW'])
    comunes_MW = tablas_click_MW & tablas_web_MW
    
    if comunes_MW:
        print(f"\n✅ Tablas con 'MW_' en ambos: {len(comunes_MW)}")
        for tabla in sorted(comunes_MW)[:10]:
            print(f"   - {tabla}")
        if len(comunes_MW) > 10:
            print(f"   ... y {len(comunes_MW) - 10} más")
    
    print("\n" + "="*80)
    print("💡 CONCLUSIÓN")
    print("="*80)
    
    if cambios:
        print(f"\n⚠️  Se detectaron {len(cambios)} tablas con cambio de nomenclatura:")
        print("   - En ClickOne usan 'mw_' (minúsculas)")
        print("   - En Web Service usan 'MW_' (MAYÚSCULAS)")
        print("\n   Esto NO afecta la funcionalidad en SQLite (case-insensitive)")
        print("   pero puede causar confusión en el código.")
        print("\n   Recomendación: Mantener consistencia con ClickOne (mw_ en minúsculas)")
    else:
        print("\n✅ La nomenclatura es consistente entre ClickOne y Web Service")
    
    print("\n" + "="*80)

if __name__ == "__main__":
    main()
