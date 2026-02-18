#!/usr/bin/env python3
import re

def extraer_tablas(archivo):
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    # Buscar todas las sentencias CREATE TABLE
    patron = r'CREATE TABLE\s+"?(\w+)"?'
    tablas = re.findall(patron, contenido, re.IGNORECASE)
    return set(t.lower() for t in tablas)

# Extraer tablas de ambos archivos
clickone = extraer_tablas('test_carteras/clickOne/Cartera.txt')
web = extraer_tablas('test_carteras/web/Cartera.txt')

# Comparar
faltantes_en_web = clickone - web
extras_en_web = web - clickone

print("=" * 60)
print("ANÁLISIS DE TABLAS - ClickOne vs Web Service")
print("=" * 60)
print(f"\n📊 ESTADÍSTICAS:")
print(f"   Tablas en ClickOne: {len(clickone)}")
print(f"   Tablas en Web:      {len(web)}")
print(f"   Tablas faltantes:   {len(faltantes_en_web)}")
print(f"   Tablas extras:      {len(extras_en_web)}")

if faltantes_en_web:
    print(f"\n❌ TABLAS FALTANTES EN WEB SERVICE ({len(faltantes_en_web)}):")
    for tabla in sorted(faltantes_en_web):
        print(f"   - {tabla}")

if extras_en_web:
    print(f"\n➕ TABLAS EXTRAS EN WEB SERVICE ({len(extras_en_web)}):")
    for tabla in sorted(extras_en_web):
        print(f"   - {tabla}")

if not faltantes_en_web and not extras_en_web:
    print("\n✅ PERFECTO: Ambos archivos tienen las mismas tablas")

print("\n" + "=" * 60)
