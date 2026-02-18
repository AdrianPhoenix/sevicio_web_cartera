#!/usr/bin/env python3
# -*- coding: utf-8 -*-

# Leer ClickOne
with open("../test_carteras/clickOne/Cartera_zona_343.txt", 'r', encoding='utf-8') as f:
    lines_click = f.readlines()

# Leer Web
with open("../test_carteras/web_localhost/Cartera_zona_343_5.txt", 'r', encoding='utf-8') as f:
    lines_web = f.readlines()

# Buscar en ClickOne
for i, line in enumerate(lines_click):
    if 'CREATE TABLE "MW_PedidosFacturasCabeceras"' in line:
        print("ClickOne:")
        print(line.rstrip())
        
        inicio = line.find('(')
        fin = line.rfind(')')
        cols_str = line[inicio+1:fin]
        
        cols = []
        temp = cols_str.split(',"')
        for j, col in enumerate(temp):
            if j > 0:
                col = '"' + col
            cols.append(' '.join(col.split()))
        
        print(f"\nColumnas ({len(cols)}):")
        for j, col in enumerate(cols, 1):
            print(f"{j}. {col}")
        break

print("\n" + "="*80 + "\n")

# Buscar en Web
for i, line in enumerate(lines_web):
    if 'CREATE TABLE "MW_PedidosFacturasCabeceras"' in line:
        print("Web Service:")
        print(line.rstrip())
        
        inicio = line.find('(')
        fin = line.rfind(')')
        cols_str = line[inicio+1:fin]
        
        cols = []
        temp = cols_str.split(', "')
        for j, col in enumerate(temp):
            if j > 0:
                col = '"' + col
            cols.append(' '.join(col.split()))
        
        print(f"\nColumnas ({len(cols)}):")
        for j, col in enumerate(cols, 1):
            print(f"{j}. {col}")
        break
