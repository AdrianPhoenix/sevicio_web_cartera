#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para verificar PedidosFarmacias específicamente
"""

def main():
    print("="*80)
    print("🔍 VERIFICACIÓN DETALLADA: PedidosFarmacias")
    print("="*80)
    
    # Leer ClickOne
    with open("../test_carteras/clickOne/Cartera_zona_343.txt", 'r', encoding='utf-8') as f:
        lines_click = f.readlines()
    
    # Leer Web
    with open("../test_carteras/web_localhost/Cartera_zona_343_4.txt", 'r', encoding='utf-8') as f:
        lines_web = f.readlines()
    
    # Buscar PedidosFarmacias en ClickOne
    create_click = None
    for i, line in enumerate(lines_click):
        if 'CREATE TABLE "PedidosFarmacias"' in line:
            create_click = line.strip()
            print(f"\n📋 ClickOne (línea {i}):")
            print(create_click)
            break
    
    # Buscar PedidosFarmacias en Web
    create_web = None
    for i, line in enumerate(lines_web):
        if 'CREATE TABLE "PedidosFarmacias"' in line:
            create_web = line.strip()
            print(f"\n📋 Web Service (línea {i}):")
            print(create_web)
            break
    
    if not create_click or not create_web:
        print("\n❌ No se encontró la tabla en uno de los archivos")
        return
    
    # Extraer columnas
    inicio = create_click.find('(')
    fin = create_click.rfind(')')
    cols_click_str = create_click[inicio+1:fin]
    
    inicio = create_web.find('(')
    fin = create_web.rfind(')')
    cols_web_str = create_web[inicio+1:fin]
    
    # Dividir por comas, pero cuidado con PRIMARY KEY
    import re
    
    # Método más robusto: dividir por , " (coma-espacio-comilla) o ," (coma-comilla)
    cols_click = []
    cols_web = []
    
    # Para ClickOne (usa ," sin espacio)
    temp = cols_click_str.split(',"')
    for i, col in enumerate(temp):
        if i > 0:
            col = '"' + col  # Restaurar la comilla inicial
        cols_click.append(col.strip())
    
    # Para Web (usa , " con espacio)
    temp = cols_web_str.split(', "')
    for i, col in enumerate(temp):
        if i > 0:
            col = '"' + col  # Restaurar la comilla inicial
        cols_web.append(col.strip())
    
    print(f"\n📊 Número de columnas:")
    print(f"   ClickOne: {len(cols_click)}")
    print(f"   Web Service: {len(cols_web)}")
    
    if len(cols_click) != len(cols_web):
        print(f"   ❌ Diferente número de columnas")
    else:
        print(f"   ✅ Mismo número de columnas")
    
    print(f"\n📋 Comparación columna por columna:")
    max_len = max(len(cols_click), len(cols_web))
    
    diferencias = 0
    for i in range(max_len):
        col_c = cols_click[i] if i < len(cols_click) else "❌ FALTA"
        col_w = cols_web[i] if i < len(cols_web) else "❌ FALTA"
        
        # Normalizar espacios para comparar
        col_c_norm = ' '.join(col_c.split())
        col_w_norm = ' '.join(col_w.split())
        
        if col_c_norm == col_w_norm:
            print(f"   {i+1}. ✅ {col_w_norm}")
        else:
            print(f"   {i+1}. ❌ DIFERENTE:")
            print(f"      ClickOne:    {col_c_norm}")
            print(f"      Web Service: {col_w_norm}")
            diferencias += 1
    
    print("\n" + "="*80)
    if diferencias == 0 and len(cols_click) == len(cols_web):
        print("✅ ¡PERFECTO! PedidosFarmacias está 100% idéntica")
    else:
        print(f"❌ PedidosFarmacias tiene diferencias")
    print("="*80)

if __name__ == "__main__":
    main()
