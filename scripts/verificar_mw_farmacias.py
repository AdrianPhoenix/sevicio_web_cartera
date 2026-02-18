#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para verificar que la tabla mw_farmacias fue corregida correctamente
"""

def extraer_create_table(archivo, nombre_tabla):
    """Extrae la definición CREATE TABLE de un archivo"""
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    # Buscar la línea que contiene CREATE TABLE para la tabla especificada
    lineas = contenido.split('\n')
    for i, linea in enumerate(lineas):
        if f'CREATE TABLE "{nombre_tabla}"' in linea or f'CREATE TABLE ""{nombre_tabla}""' in linea:
            return linea.strip()
    
    return None

def comparar_columnas(create_clickone, create_web):
    """Compara las columnas de dos definiciones CREATE TABLE"""
    # Extraer columnas de ClickOne
    inicio = create_clickone.find('(')
    fin = create_clickone.rfind(')')
    columnas_clickone = create_clickone[inicio+1:fin].split(', ')
    
    # Extraer columnas de Web
    inicio = create_web.find('(')
    fin = create_web.rfind(')')
    columnas_web = create_web[inicio+1:fin].split(', ')
    
    # Limpiar comillas dobles
    columnas_clickone = [col.replace('""', '"') for col in columnas_clickone]
    columnas_web = [col.replace('""', '"') for col in columnas_web]
    
    print(f"\n📊 Comparación de columnas:")
    print(f"   ClickOne: {len(columnas_clickone)} columnas")
    print(f"   Web Service: {len(columnas_web)} columnas")
    
    if len(columnas_clickone) == len(columnas_web):
        print("   ✅ Mismo número de columnas")
    else:
        print(f"   ❌ Diferente número de columnas (diferencia: {abs(len(columnas_clickone) - len(columnas_web))})")
    
    # Comparar columna por columna
    print("\n📋 Detalle de columnas:")
    max_len = max(len(columnas_clickone), len(columnas_web))
    
    diferencias = 0
    for i in range(max_len):
        col_click = columnas_clickone[i] if i < len(columnas_clickone) else "❌ FALTA"
        col_web = columnas_web[i] if i < len(columnas_web) else "❌ FALTA"
        
        if col_click == col_web:
            print(f"   {i+1}. ✅ {col_click}")
        else:
            print(f"   {i+1}. ❌ DIFERENTE:")
            print(f"      ClickOne:    {col_click}")
            print(f"      Web Service: {col_web}")
            diferencias += 1
    
    return diferencias == 0

def main():
    print("=" * 80)
    print("🔍 VERIFICACIÓN DE CORRECCIÓN: mw_farmacias")
    print("=" * 80)
    
    archivo_clickone = "../test_carteras/clickOne/Cartera_zona_343.txt"
    archivo_web = "../test_carteras/web_localhost/Cartera_zona_343_3.txt"
    tabla = "mw_farmacias"
    
    print(f"\n📁 Archivos a comparar:")
    print(f"   ClickOne: {archivo_clickone}")
    print(f"   Web Service: {archivo_web}")
    print(f"   Tabla: {tabla}")
    
    # Extraer definiciones
    create_clickone = extraer_create_table(archivo_clickone, tabla)
    create_web = extraer_create_table(archivo_web, tabla)
    
    if not create_clickone:
        print(f"\n❌ ERROR: No se encontró la tabla {tabla} en ClickOne")
        return
    
    if not create_web:
        print(f"\n❌ ERROR: No se encontró la tabla {tabla} en Web Service")
        print("   ⚠️  Necesitas generar una nueva cartera con el servicio web actualizado")
        return
    
    print("\n" + "=" * 80)
    print("📝 DEFINICIONES COMPLETAS:")
    print("=" * 80)
    print(f"\nClickOne:\n{create_clickone}")
    print(f"\nWeb Service:\n{create_web}")
    
    # Comparar
    print("\n" + "=" * 80)
    son_iguales = comparar_columnas(create_clickone, create_web)
    
    print("\n" + "=" * 80)
    print("🎯 RESULTADO FINAL:")
    print("=" * 80)
    
    if son_iguales:
        print("✅ ¡PERFECTO! La tabla mw_farmacias está 100% idéntica")
        print("   Todas las columnas coinciden en orden, nombre y tipo")
    else:
        print("❌ La tabla mw_farmacias aún tiene diferencias")
        print("   Revisa los detalles arriba para ver qué falta corregir")
    
    print("\n" + "=" * 80)

if __name__ == "__main__":
    main()
