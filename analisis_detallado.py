#!/usr/bin/env python3
import re

def extraer_tablas_con_esquemas(archivo):
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    # Buscar CREATE TABLE con sus esquemas completos
    patron = r'CREATE TABLE\s+"?(\w+)"?\s*\((.*?)\);'
    matches = re.findall(patron, contenido, re.IGNORECASE | re.DOTALL)
    
    tablas = {}
    for nombre, esquema in matches:
        # Limpiar esquema
        esquema_limpio = ' '.join(esquema.split())
        tablas[nombre.lower()] = esquema_limpio
    
    return tablas

def extraer_inserts(archivo):
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    # Contar INSERT por tabla
    patron = r'INSERT INTO\s+"?(\w+)"?'
    inserts = re.findall(patron, contenido, re.IGNORECASE)
    
    conteo = {}
    for tabla in inserts:
        tabla_lower = tabla.lower()
        conteo[tabla_lower] = conteo.get(tabla_lower, 0) + 1
    
    return conteo

print("Extrayendo información de ambos archivos...")
clickone_tablas = extraer_tablas_con_esquemas('test_carteras/clickOne/Cartera.txt')
web_tablas = extraer_tablas_con_esquemas('test_carteras/web/Cartera.txt')

clickone_inserts = extraer_inserts('test_carteras/clickOne/Cartera.txt')
web_inserts = extraer_inserts('test_carteras/web/Cartera.txt')

faltantes = set(clickone_tablas.keys()) - set(web_tablas.keys())
extras = set(web_tablas.keys()) - set(clickone_tablas.keys())
comunes = set(clickone_tablas.keys()) & set(web_tablas.keys())

print("\n" + "="*80)
print("ANÁLISIS DETALLADO - ClickOne vs Web Service")
print("="*80)

print(f"\n📊 RESUMEN:")
print(f"   Tablas en ClickOne:  {len(clickone_tablas)}")
print(f"   Tablas en Web:       {len(web_tablas)}")
print(f"   Tablas comunes:      {len(comunes)}")
print(f"   Tablas faltantes:    {len(faltantes)}")
print(f"   Tablas extras:       {len(extras)}")

# Analizar tablas faltantes
print(f"\n{'='*80}")
print(f"❌ TABLAS FALTANTES EN WEB SERVICE ({len(faltantes)}):")
print(f"{'='*80}")

for tabla in sorted(faltantes):
    inserts_count = clickone_inserts.get(tabla, 0)
    esquema = clickone_tablas[tabla][:100] + "..." if len(clickone_tablas[tabla]) > 100 else clickone_tablas[tabla]
    
    print(f"\n📋 {tabla}")
    print(f"   INSERTs en ClickOne: {inserts_count}")
    print(f"   Esquema: {esquema}")

# Analizar tablas extras (posibles errores)
print(f"\n{'='*80}")
print(f"⚠️  TABLAS EXTRAS EN WEB SERVICE ({len(extras)}) - POSIBLES ERRORES:")
print(f"{'='*80}")

for tabla in sorted(extras):
    inserts_count = web_inserts.get(tabla, 0)
    esquema = web_tablas[tabla][:100] + "..." if len(web_tablas[tabla]) > 100 else web_tablas[tabla]
    
    print(f"\n📋 {tabla}")
    print(f"   INSERTs en Web: {inserts_count}")
    print(f"   Esquema: {esquema}")

# Comparar esquemas de tablas comunes
print(f"\n{'='*80}")
print(f"🔍 VERIFICACIÓN DE ESQUEMAS EN TABLAS COMUNES:")
print(f"{'='*80}")

esquemas_diferentes = []
for tabla in sorted(comunes):
    if clickone_tablas[tabla] != web_tablas[tabla]:
        esquemas_diferentes.append(tabla)

if esquemas_diferentes:
    print(f"\n⚠️  {len(esquemas_diferentes)} tablas con esquemas DIFERENTES:")
    for tabla in esquemas_diferentes[:10]:  # Mostrar solo las primeras 10
        print(f"\n   📋 {tabla}:")
        print(f"      ClickOne: {clickone_tablas[tabla][:80]}...")
        print(f"      Web:      {web_tablas[tabla][:80]}...")
else:
    print("\n✅ Todas las tablas comunes tienen esquemas IDÉNTICOS")

# Comparar cantidad de INSERTs
print(f"\n{'='*80}")
print(f"📊 COMPARACIÓN DE DATOS (INSERTs):")
print(f"{'='*80}")

diferencias_datos = []
for tabla in sorted(comunes):
    click_count = clickone_inserts.get(tabla, 0)
    web_count = web_inserts.get(tabla, 0)
    
    if click_count != web_count:
        diferencias_datos.append((tabla, click_count, web_count))

if diferencias_datos:
    print(f"\n⚠️  {len(diferencias_datos)} tablas con diferente cantidad de datos:")
    for tabla, click_count, web_count in diferencias_datos[:15]:
        diff = web_count - click_count
        simbolo = "+" if diff > 0 else ""
        print(f"   {tabla:40} ClickOne: {click_count:5} | Web: {web_count:5} | Diff: {simbolo}{diff}")
else:
    print("\n✅ Todas las tablas tienen la misma cantidad de registros")

print("\n" + "="*80)
print("CONCLUSIÓN:")
print("="*80)
print(f"✅ Tablas correctas: {len(comunes) - len(esquemas_diferentes)}")
print(f"⚠️  Tablas con esquemas diferentes: {len(esquemas_diferentes)}")
print(f"❌ Tablas faltantes que DEBEN agregarse: {len(faltantes)}")
print(f"🗑️  Tablas extras que NO deberían estar: {len(extras)}")
print("="*80)
