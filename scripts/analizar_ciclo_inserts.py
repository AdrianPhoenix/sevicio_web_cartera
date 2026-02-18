#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para analizar si las tablas incluyen la columna CICLO en sus INSERT statements
"""

import re

def analizar_inserts_ciclo(archivo, nombre_archivo):
    """Analiza si los INSERT statements incluyen la columna CICLO"""
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    # Tablas a verificar
    tablas_verificar = ['farmacias', 'hoja_ruta', 'hoja_ruta_propuesta', 'hospital']
    
    resultados = {}
    
    for tabla in tablas_verificar:
        # Buscar INSERT INTO para esta tabla
        patron = rf'INSERT INTO\s+["\']?{tabla}["\']?\s*\(([^)]+)\)\s*VALUES'
        match = re.search(patron, contenido, re.IGNORECASE)
        
        if match:
            columnas = match.group(1).strip()
            tiene_ciclo = 'CICLO' in columnas.upper()
            resultados[tabla] = {
                'encontrado': True,
                'tiene_ciclo': tiene_ciclo,
                'columnas': columnas
            }
        else:
            resultados[tabla] = {
                'encontrado': False,
                'tiene_ciclo': False,
                'columnas': None
            }
    
    return resultados

def main():
    print("=" * 100)
    print("🔍 ANÁLISIS DE COLUMNA CICLO EN INSERT STATEMENTS")
    print("=" * 100)
    print()
    
    archivo_clickone = "../test_carteras/clickOne/Cartera_zona_343.txt"
    archivo_web = "../test_carteras/web_localhost/Cartera_zona_343_8.txt"
    
    print("📁 Archivos:")
    print(f"   ClickOne:    {archivo_clickone}")
    print(f"   Web Service: {archivo_web}")
    print()
    
    print("⏳ Analizando ClickOne...")
    resultados_click = analizar_inserts_ciclo(archivo_clickone, "ClickOne")
    
    print("⏳ Analizando Web Service...")
    resultados_web = analizar_inserts_ciclo(archivo_web, "Web Service")
    
    print()
    print("=" * 100)
    print("📊 RESULTADOS")
    print("=" * 100)
    print()
    
    print(f"{'Tabla':<30} {'ClickOne':>15} {'Web Service':>15} {'Estado':>20}")
    print("-" * 100)
    
    problemas = []
    
    for tabla in ['farmacias', 'hoja_ruta', 'hoja_ruta_propuesta', 'hospital']:
        click_ciclo = "✅ SÍ" if resultados_click[tabla]['tiene_ciclo'] else "❌ NO"
        web_ciclo = "✅ SÍ" if resultados_web[tabla]['tiene_ciclo'] else "❌ NO"
        
        if resultados_click[tabla]['tiene_ciclo'] and not resultados_web[tabla]['tiene_ciclo']:
            estado = "⚠️ PROBLEMA"
            problemas.append(tabla)
        elif resultados_click[tabla]['tiene_ciclo'] == resultados_web[tabla]['tiene_ciclo']:
            estado = "✅ OK"
        else:
            estado = "ℹ️ DIFERENTE"
        
        print(f"{tabla:<30} {click_ciclo:>15} {web_ciclo:>15} {estado:>20}")
    
    # Detalles de las columnas
    print()
    print("=" * 100)
    print("📋 DETALLES DE COLUMNAS EN INSERT")
    print("=" * 100)
    
    for tabla in ['farmacias', 'hoja_ruta', 'hoja_ruta_propuesta', 'hospital']:
        print(f"\n📊 {tabla.upper()}")
        print("-" * 100)
        
        if resultados_click[tabla]['encontrado']:
            print(f"ClickOne:    {resultados_click[tabla]['columnas'][:90]}...")
        else:
            print("ClickOne:    ❌ No se encontró INSERT")
        
        if resultados_web[tabla]['encontrado']:
            print(f"Web Service: {resultados_web[tabla]['columnas'][:90]}...")
        else:
            print("Web Service: ❌ No se encontró INSERT")
    
    # Conclusión
    print()
    print("=" * 100)
    print("💡 CONCLUSIÓN")
    print("=" * 100)
    print()
    
    if problemas:
        print(f"⚠️  SE DETECTARON {len(problemas)} TABLAS CON PROBLEMAS:")
        print()
        for tabla in problemas:
            print(f"   ❌ {tabla}")
            print(f"      - ClickOne incluye CICLO en INSERT")
            print(f"      - Web Service NO incluye CICLO en INSERT")
            print(f"      - Esto puede causar que los registros no tengan valor de CICLO")
            print()
        
        print("🔧 RECOMENDACIÓN:")
        print("   Revisar el código en GeneradorService.cs para incluir CICLO en los INSERT")
        print("   de estas tablas, similar a como se hizo con 'solicitudes' y 'visitas'")
        print()
        print("📝 REFERENCIAS:")
        print("   - Issue #004: Problema similar con solicitudes (RESUELTO)")
        print("   - Issue #006: Problema similar con visitas (RESUELTO)")
    else:
        print("✅ ¡PERFECTO! Todas las tablas incluyen CICLO correctamente")
    
    print()
    print("=" * 100)

if __name__ == '__main__':
    main()
