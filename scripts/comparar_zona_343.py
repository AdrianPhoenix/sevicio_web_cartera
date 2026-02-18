#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para comparar carteras de la zona 343
ClickOne vs Web Service
"""

import re
from collections import defaultdict

def extraer_tablas(archivo):
    """Extrae nombres de tablas CREATE TABLE de un archivo Cartera.txt"""
    tablas = []
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
        # Buscar todas las sentencias CREATE TABLE
        patron = r'CREATE TABLE\s+"?(\w+)"?\s*\('
        matches = re.findall(patron, contenido, re.IGNORECASE)
        tablas = [m.lower() for m in matches]
    return sorted(set(tablas))

def comparar_carteras(clickone_file, web_file):
    """Compara las tablas entre dos carteras"""
    print("=" * 80)
    print("COMPARACIÓN DE CARTERAS - ZONA 343")
    print("=" * 80)
    print()
    
    # Extraer tablas
    print("📂 Extrayendo tablas de ClickOne...")
    tablas_clickone = extraer_tablas(clickone_file)
    print(f"   ✅ {len(tablas_clickone)} tablas encontradas")
    
    print("📂 Extrayendo tablas de Web Service...")
    tablas_web = extraer_tablas(web_file)
    print(f"   ✅ {len(tablas_web)} tablas encontradas")
    print()
    
    # Convertir a sets para comparación
    set_clickone = set(tablas_clickone)
    set_web = set(tablas_web)
    
    # Calcular diferencias
    solo_clickone = set_clickone - set_web
    solo_web = set_web - set_clickone
    comunes = set_clickone & set_web
    
    # Mostrar resultados
    print("=" * 80)
    print("📊 RESUMEN")
    print("=" * 80)
    print(f"Tablas en ClickOne:     {len(tablas_clickone)}")
    print(f"Tablas en Web Service:  {len(tablas_web)}")
    print(f"Tablas comunes:         {len(comunes)}")
    print(f"Solo en ClickOne:       {len(solo_clickone)}")
    print(f"Solo en Web Service:    {len(solo_web)} ⚠️")
    print()
    
    # Tablas faltantes en Web
    if solo_clickone:
        print("=" * 80)
        print("❌ TABLAS FALTANTES EN WEB SERVICE (Solo en ClickOne)")
        print("=" * 80)
        for tabla in sorted(solo_clickone):
            print(f"  - {tabla}")
        print()
    
    # Tablas extras en Web
    if solo_web:
        print("=" * 80)
        print("⚠️  TABLAS EXTRAS EN WEB SERVICE (No están en ClickOne)")
        print("=" * 80)
        
        # Categorizar tablas extras
        categorias = defaultdict(list)
        
        for tabla in sorted(solo_web):
            if tabla.startswith('mw_ayuda_visual'):
                categorias['Ayuda Visual'].append(tabla)
            elif tabla.startswith('mw_') and any(x in tabla for x in ['configuracion', 'inclusiones', 'logs']):
                categorias['Configuración y Logs'].append(tabla)
            elif tabla.startswith('mw_') and any(x in tabla for x in ['empresas', 'especialidades', 'estados', 'motivos', 'productos', 'tipo', 'visitadores', 'zonas']):
                categorias['Catálogos Maestros (MW_)'].append(tabla)
            elif 'farmacia' in tabla and any(x in tabla for x in ['detalles', 'personal']):
                categorias['Detalles de Farmacias'].append(tabla)
            elif 'hospital' in tabla and any(x in tabla for x in ['detalles', 'personal']):
                categorias['Detalles de Hospitales'].append(tabla)
            elif any(x in tabla for x in ['solicitud', 'visita']) and not tabla.startswith('h'):
                categorias['Gestión de Visitas'].append(tabla)
            elif '_productos' in tabla:
                categorias['Productos en Solicitudes/Visitas'].append(tabla)
            else:
                categorias['Otras'].append(tabla)
        
        # Mostrar por categorías
        for categoria, tablas in sorted(categorias.items()):
            print(f"\n📁 {categoria} ({len(tablas)} tablas):")
            for tabla in tablas:
                print(f"   - {tabla}")
        
        print()
        print(f"Total de tablas extras: {len(solo_web)}")
        print()
    
    # Verificación de compatibilidad
    print("=" * 80)
    print("✅ VERIFICACIÓN DE COMPATIBILIDAD")
    print("=" * 80)
    
    if len(solo_clickone) == 0:
        print("✅ EXCELENTE: Web Service tiene TODAS las tablas de ClickOne")
        print("   No hay tablas faltantes.")
    else:
        print(f"❌ PROBLEMA: Faltan {len(solo_clickone)} tablas en Web Service")
        print("   Esto puede causar errores en la app Android.")
    
    print()
    
    if len(solo_web) > 0:
        print(f"⚠️  ADVERTENCIA: Web Service tiene {len(solo_web)} tablas extras")
        print("   Estas tablas no están en ClickOne.")
        print("   Pueden ser:")
        print("   - Residuos de migración")
        print("   - Funcionalidades nuevas")
        print("   - Tablas de prueba")
    else:
        print("✅ PERFECTO: Paridad exacta con ClickOne")
    
    print()
    print("=" * 80)
    
    return {
        'clickone': len(tablas_clickone),
        'web': len(tablas_web),
        'comunes': len(comunes),
        'solo_clickone': len(solo_clickone),
        'solo_web': len(solo_web),
        'tablas_extras': sorted(solo_web)
    }

if __name__ == '__main__':
    clickone = 'test_carteras/clickOne/Cartera_zona_343.txt'
    web = 'test_carteras/web_localhost/Cartera_zona_343.txt'
    
    resultado = comparar_carteras(clickone, web)
    
    # Guardar resultado en archivo
    with open('analisis_tablas_zona_343.txt', 'w', encoding='utf-8') as f:
        f.write("TABLAS EXTRAS EN WEB SERVICE\n")
        f.write("=" * 80 + "\n\n")
        for tabla in resultado['tablas_extras']:
            f.write(f"{tabla}\n")
    
    print(f"📝 Resultado guardado en: analisis_tablas_zona_343.txt")
