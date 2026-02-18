#!/usr/bin/env python3
# -*- coding: utf-8 -*-

# Leer Web
with open("../test_carteras/web_localhost/Cartera_zona_343_4.txt", 'r', encoding='utf-8') as f:
    lines_web = f.readlines()

# Buscar PedidosFarmacias en Web
for i, line in enumerate(lines_web):
    if 'CREATE TABLE "PedidosFarmacias"' in line:
        print(f"Línea {i}: {repr(line)}")
        
        # Extraer columnas
        inicio = line.find('(')
        fin = line.rfind(')')
        cols_str = line[inicio+1:fin]
        
        print(f"\nColumnas string: {repr(cols_str)}")
        print(f"\nLongitud: {len(cols_str)}")
        
        # Dividir
        cols = cols_str.split(',"')
        print(f"\nNúmero de partes al dividir por ',\"': {len(cols)}")
        
        for j, col in enumerate(cols):
            print(f"{j+1}. {repr(col[:50])}")
        
        break
