#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para verificar las 3 tablas de prioridad alta corregidas
"""

def extraer_create_table(archivo, nombre_tabla):
    """Extrae la definición CREATE TABLE de un archivo"""
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    # Buscar la línea que contiene CREATE TABLE para la tabla especificada
    lineas = contenido.split('\n')
    for i, linea in enumerate(lineas):
        # Buscar tanto en la misma línea como en líneas separadas
        if f'CREATE TABLE "{nombre_tabla}"' in linea or f'CREATE TABLE ""{nombre_tabla}""' in linea:
            return linea.strip()
        # Si encontramos DROP TABLE, revisar la siguiente línea
        if (f'DROP TABLE IF EXISTS "{nombre_tabla}"' in linea or 
            f'DROP TABLE IF EXISTS ""{nombre_tabla}""' in linea):
            # Revisar si CREATE TABLE está en la misma línea
            if 'CREATE TABLE' in linea:
                # Extraer solo la parte del CREATE TABLE
                create_start = linea.find('CREATE TABLE')
                return linea[create_start:].strip()
            # Si no, revisar la siguiente línea
            elif i + 1 < len(lineas):
                siguiente = lineas[i + 1].strip()
                if 'CREATE TABLE' in siguiente:
                    return siguiente
    
    return None

def comparar_tabla(nombre_tabla, create_clickone, create_web):
    """Compara una tabla y retorna si son iguales"""
    print(f"\n{'='*80}")
    print(f"📋 Verificando: {nombre_tabla}")
    print('='*80)
    
    if not create_clickone:
        print(f"❌ ERROR: No se encontró {nombre_tabla} en ClickOne")
        return False
    
    if not create_web:
        print(f"❌ ERROR: No se encontró {nombre_tabla} en Web Service")
        return False
    
    # Extraer columnas
    inicio = create_clickone.find('(')
    fin = create_clickone.rfind(')')
    columnas_clickone = create_clickone[inicio+1:fin].split(', ')
    
    inicio = create_web.find('(')
    fin = create_web.rfind(')')
    columnas_web = create_web[inicio+1:fin].split(', ')
    
    # Limpiar comillas dobles
    columnas_clickone = [col.replace('""', '"') for col in columnas_clickone]
    columnas_web = [col.replace('""', '"') for col in columnas_web]
    
    print(f"\n📊 Columnas:")
    print(f"   ClickOne: {len(columnas_clickone)}")
    print(f"   Web Service: {len(columnas_web)}")
    
    if len(columnas_clickone) != len(columnas_web):
        print(f"   ❌ Diferente número de columnas")
        return False
    
    # Comparar columna por columna
    diferencias = 0
    for i in range(len(columnas_clickone)):
        if columnas_clickone[i] != columnas_web[i]:
            if diferencias == 0:
                print(f"\n❌ Diferencias encontradas:")
            print(f"   Columna {i+1}:")
            print(f"      ClickOne:    {columnas_clickone[i]}")
            print(f"      Web Service: {columnas_web[i]}")
            diferencias += 1
    
    if diferencias == 0:
        print(f"\n✅ ¡PERFECTO! {nombre_tabla} está 100% idéntica")
        return True
    else:
        print(f"\n❌ {nombre_tabla} tiene {diferencias} diferencia(s)")
        return False

def main():
    print("="*80)
    print("🔍 VERIFICACIÓN DE 3 TABLAS DE PRIORIDAD ALTA")
    print("="*80)
    
    archivo_clickone = "../test_carteras/clickOne/Cartera_zona_343.txt"
    archivo_web = "../test_carteras/web_localhost/Cartera_zona_343_4.txt"
    
    tablas = [
        "mw_farmacias",
        "mw_hospitales",
        "PedidosFarmacias"
    ]
    
    print(f"\n📁 Archivos:")
    print(f"   ClickOne: {archivo_clickone}")
    print(f"   Web Service: {archivo_web}")
    
    resultados = {}
    
    for tabla in tablas:
        create_clickone = extraer_create_table(archivo_clickone, tabla)
        create_web = extraer_create_table(archivo_web, tabla)
        resultados[tabla] = comparar_tabla(tabla, create_clickone, create_web)
    
    # Resumen final
    print("\n" + "="*80)
    print("🎯 RESUMEN FINAL")
    print("="*80)
    
    correctas = sum(1 for v in resultados.values() if v)
    total = len(resultados)
    
    for tabla, resultado in resultados.items():
        estado = "✅ CORRECTA" if resultado else "❌ CON ERRORES"
        print(f"   {tabla}: {estado}")
    
    print(f"\n📊 Total: {correctas}/{total} tablas correctas ({correctas*100//total}%)")
    
    if correctas == total:
        print("\n🎉 ¡EXCELENTE! Todas las tablas están correctamente corregidas")
    else:
        print(f"\n⚠️  Faltan {total - correctas} tabla(s) por corregir")
    
    print("\n" + "="*80)

if __name__ == "__main__":
    main()
