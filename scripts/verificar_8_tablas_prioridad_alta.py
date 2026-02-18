#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para verificar las 8 tablas de prioridad alta corregidas
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
    
    if not create_clickone:
        print(f"   ❌ No se encontró en ClickOne")
        return False
    
    if not create_web:
        print(f"   ❌ No se encontró en Web Service")
        return False
    
    # Extraer columnas
    inicio = create_clickone.find('(')
    fin = create_clickone.rfind(')')
    cols_click_str = create_clickone[inicio+1:fin]
    
    inicio = create_web.find('(')
    fin = create_web.rfind(')')
    cols_web_str = create_web[inicio+1:fin]
    
    # Dividir columnas (ClickOne usa ," y Web usa , ")
    # Pero también manejar espacios dobles en ClickOne
    cols_click = []
    # Intentar primero con ," (sin espacio)
    if ',"' in cols_click_str:
        temp = cols_click_str.split(',"')
        for i, col in enumerate(temp):
            if i > 0:
                col = '"' + col
            cols_click.append(col.strip())
    # Si no, intentar con , " (con espacio)
    elif ', "' in cols_click_str:
        temp = cols_click_str.split(', "')
        for i, col in enumerate(temp):
            if i > 0:
                col = '"' + col
            cols_click.append(col.strip())
    else:
        # Fallback: solo una columna
        cols_click = [cols_click_str.strip()]
    
    cols_web = []
    # Web siempre usa , " (con espacio)
    if ', "' in cols_web_str:
        temp = cols_web_str.split(', "')
        for i, col in enumerate(temp):
            if i > 0:
                col = '"' + col
            cols_web.append(col.strip())
    elif ',"' in cols_web_str:
        temp = cols_web_str.split(',"')
        for i, col in enumerate(temp):
            if i > 0:
                col = '"' + col
            cols_web.append(col.strip())
    else:
        # Fallback: solo una columna
        cols_web = [cols_web_str.strip()]
    
    # Comparar número de columnas
    if len(cols_click) != len(cols_web):
        print(f"   ❌ ClickOne: {len(cols_click)} cols, Web: {len(cols_web)} cols")
        return False
    
    # Comparar columna por columna (normalizando espacios)
    diferencias = 0
    for i in range(len(cols_click)):
        col_c_norm = ' '.join(cols_click[i].split())
        col_w_norm = ' '.join(cols_web[i].split())
        
        if col_c_norm != col_w_norm:
            diferencias += 1
    
    if diferencias == 0:
        print(f"   ✅ {len(cols_click)} columnas - 100% idéntica")
        return True
    else:
        print(f"   ❌ {diferencias} diferencia(s) en columnas")
        return False

def main():
    print("="*80)
    print("🔍 VERIFICACIÓN DE 8 TABLAS DE PRIORIDAD ALTA")
    print("="*80)
    
    archivo_clickone = "../test_carteras/clickOne/Cartera_zona_343.txt"
    archivo_web = "../test_carteras/web_localhost/Cartera_zona_343_5.txt"
    
    # Tablas a verificar (nombre en ClickOne)
    tablas = [
        ("mw_farmacias", "mw_farmacias"),
        ("mw_hospitales", "mw_hospitales"),
        ("PedidosFarmacias", "PedidosFarmacias"),
        ("ayuda_visual_FE", "ayuda_visual_FE"),
        ("ayuda_visual_MP4", "ayuda_visual_MP4"),
        ("ayuda_visual_MP4_FE", "ayuda_visual_MP4_FE"),
        ("MW_DrogueriasProductos", "MW_DrogueriasProductos"),
        ("MW_PedidosFacturasCabeceras", "MW_PedidosFacturasCabeceras"),
    ]
    
    print(f"\n📁 Archivos:")
    print(f"   ClickOne: {archivo_clickone}")
    print(f"   Web Service: {archivo_web}")
    print(f"\n{'='*80}")
    
    resultados = {}
    
    for i, (tabla_click, tabla_web) in enumerate(tablas, 1):
        print(f"\n{i}. {tabla_click}")
        create_clickone = extraer_create_table(archivo_clickone, tabla_click)
        create_web = extraer_create_table(archivo_web, tabla_web)
        resultados[tabla_click] = comparar_tabla(tabla_click, create_clickone, create_web)
    
    # Resumen final
    print("\n" + "="*80)
    print("🎯 RESUMEN FINAL")
    print("="*80)
    
    correctas = sum(1 for v in resultados.values() if v)
    total = len(resultados)
    
    for tabla, resultado in resultados.items():
        estado = "✅" if resultado else "❌"
        print(f"   {estado} {tabla}")
    
    print(f"\n📊 Total: {correctas}/{total} tablas correctas ({correctas*100//total}%)")
    
    if correctas == total:
        print("\n🎉 ¡EXCELENTE! Todas las tablas de prioridad alta están correctamente corregidas")
        print("   Puedes continuar con las tablas de prioridad media (1-3 columnas faltantes)")
    else:
        print(f"\n⚠️  Faltan {total - correctas} tabla(s) por corregir")
    
    print("\n" + "="*80)

if __name__ == "__main__":
    main()
