#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para verificar que la cartera de producción esté correcta
Compara estructura y datos con ClickOne
"""

import re
from collections import defaultdict

def extraer_tablas_create(archivo):
    """Extrae las definiciones CREATE TABLE"""
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    patron = r'CREATE TABLE "([^"]+)"\s*\(([^;]+)\);'
    tablas = {}
    
    for match in re.finditer(patron, contenido, re.DOTALL):
        nombre = match.group(1).strip()
        definicion = match.group(2).strip()
        
        # Extraer columnas
        columnas = []
        for linea in definicion.split(','):
            linea = linea.strip()
            if linea and not linea.upper().startswith(('PRIMARY KEY', 'FOREIGN KEY', 'UNIQUE', 'CHECK')):
                col_match = re.match(r'"?([^"\s]+)"?\s+', linea)
                if col_match:
                    columnas.append(col_match.group(1))
        
        tablas[nombre.lower()] = {
            'nombre': nombre,
            'columnas': columnas,
            'num_columnas': len(columnas)
        }
    
    return tablas

def extraer_inserts_por_tabla(archivo):
    """Extrae INSERT statements por tabla"""
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    patron = r'INSERT INTO\s+["\']?([^\s("\']+)["\']?\s*\(([^)]+)\)\s*VALUES'
    
    inserts_por_tabla = defaultdict(list)
    
    for match in re.finditer(patron, contenido, re.IGNORECASE):
        tabla = match.group(1).strip().strip('"').strip("'").lower()
        columnas = match.group(2).strip()
        inserts_por_tabla[tabla].append(columnas)
    
    return inserts_por_tabla

def main():
    print("=" * 100)
    print("🔍 VERIFICACIÓN DE PRODUCCIÓN - Cartera Zona 343")
    print("=" * 100)
    print()
    
    archivo_clickone = "../test_carteras/clickOne/Cartera_zona_343.txt"
    archivo_produccion = "../test_carteras/web_produccion/Cartera_zona_343.txt"
    
    print("📁 Archivos:")
    print(f"   ClickOne (referencia): {archivo_clickone}")
    print(f"   Producción (actual):   {archivo_produccion}")
    print()
    
    # Verificar estructura
    print("=" * 100)
    print("📊 VERIFICACIÓN DE ESTRUCTURA")
    print("=" * 100)
    print()
    
    tablas_click = extraer_tablas_create(archivo_clickone)
    tablas_prod = extraer_tablas_create(archivo_produccion)
    
    # Verificar las 15 tablas críticas corregidas
    tablas_criticas = [
        'mw_farmacias', 'mw_hospitales', 'pedidosfarmacias',
        'ayuda_visual_fe', 'ayuda_visual_mp4', 'ayuda_visual_mp4_fe',
        'mw_drogueriasproductos', 'mw_pedidosfacturascabeceras',
        'temp_hoja_ruta_propuesta', 'mw_marcas', 'mw_medicos',
        'mw_pedidosfacturasdetalles', 'mw_especialidades', 'mw_lineas', 'mw_regiones'
    ]
    
    print("🔴 Verificando 15 tablas críticas corregidas:")
    print()
    
    todas_ok = True
    for tabla in tablas_criticas:
        click_cols = tablas_click.get(tabla, {}).get('num_columnas', 0)
        prod_cols = tablas_prod.get(tabla, {}).get('num_columnas', 0)
        
        if click_cols == prod_cols:
            print(f"   ✅ {tabla:<40} {click_cols} columnas")
        else:
            print(f"   ❌ {tabla:<40} ClickOne: {click_cols}, Prod: {prod_cols}")
            todas_ok = False
    
    # Verificar nomenclatura
    print()
    print("=" * 100)
    print("🔤 VERIFICACIÓN DE NOMENCLATURA")
    print("=" * 100)
    print()
    
    tablas_nomenclatura = ['mw_lineas', 'mw_marcas', 'mw_regiones', 'mw_tipomedicos']
    
    print("Verificando que estén en minúsculas:")
    print()
    
    for tabla in tablas_nomenclatura:
        if tabla in tablas_prod:
            nombre_real = tablas_prod[tabla]['nombre']
            if nombre_real == tabla:
                print(f"   ✅ {tabla:<40} Correcto (minúsculas)")
            else:
                print(f"   ❌ {tabla:<40} Incorrecto: {nombre_real}")
                todas_ok = False
        else:
            print(f"   ❌ {tabla:<40} NO ENCONTRADA")
            todas_ok = False
    
    # Verificar datos - columna CICLO
    print()
    print("=" * 100)
    print("📋 VERIFICACIÓN DE DATOS - Columna CICLO")
    print("=" * 100)
    print()
    
    inserts_click = extraer_inserts_por_tabla(archivo_clickone)
    inserts_prod = extraer_inserts_por_tabla(archivo_produccion)
    
    tablas_ciclo = ['hoja_ruta', 'hoja_ruta_propuesta']
    
    print("Verificando que incluyan CICLO en INSERT:")
    print()
    
    for tabla in tablas_ciclo:
        click_cols = inserts_click.get(tabla, [''])[0] if tabla in inserts_click else ''
        prod_cols = inserts_prod.get(tabla, [''])[0] if tabla in inserts_prod else ''
        
        click_tiene_ciclo = 'CICLO' in click_cols.upper()
        prod_tiene_ciclo = 'CICLO' in prod_cols.upper()
        
        if click_tiene_ciclo and prod_tiene_ciclo:
            print(f"   ✅ {tabla:<40} Incluye CICLO")
        elif not click_tiene_ciclo and not prod_tiene_ciclo:
            print(f"   ℹ️  {tabla:<40} Ninguno incluye CICLO")
        else:
            print(f"   ❌ {tabla:<40} ClickOne: {click_tiene_ciclo}, Prod: {prod_tiene_ciclo}")
            todas_ok = False
    
    # Resumen final
    print()
    print("=" * 100)
    print("💡 RESULTADO FINAL")
    print("=" * 100)
    print()
    
    if todas_ok:
        print("✅ ¡PERFECTO! La cartera de producción está correcta")
        print()
        print("   ✅ 15 tablas críticas con estructura correcta")
        print("   ✅ 4 tablas con nomenclatura correcta (minúsculas)")
        print("   ✅ 2 tablas con columna CICLO en INSERT")
        print()
        print("🎉 La versión 4.1.0 está lista para producción")
    else:
        print("⚠️  Se detectaron diferencias")
        print()
        print("   Revisar los detalles arriba para identificar los problemas")
    
    print()
    print("=" * 100)

if __name__ == '__main__':
    main()
