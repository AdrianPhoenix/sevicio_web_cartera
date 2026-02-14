#!/usr/bin/env python3
import re

def extraer_tablas_con_esquemas(archivo):
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    patron = r'CREATE TABLE\s+"?(\w+)"?\s*\((.*?)\);'
    matches = re.findall(patron, contenido, re.IGNORECASE | re.DOTALL)
    
    tablas = {}
    for nombre, esquema in matches:
        esquema_limpio = ' '.join(esquema.split())
        tablas[nombre.lower()] = (nombre, esquema_limpio)
    
    return tablas

print("Analizando archivos...")
clickone = extraer_tablas_con_esquemas('test_carteras/clickOne/Cartera.txt')
web = extraer_tablas_con_esquemas('test_carteras/web/Cartera.txt')

faltantes = set(clickone.keys()) - set(web.keys())
extras = set(web.keys()) - set(clickone.keys())

print("\n" + "="*80)
print("REPORTE PARA CORRECCIÓN DEL GENERADORSERVICE.CS")
print("="*80)

print(f"\n🎯 ACCIÓN REQUERIDA:")
print(f"   1. AGREGAR {len(faltantes)} tablas faltantes")
print(f"   2. ELIMINAR {len(extras)} tablas extras (errores)")

# Generar código para agregar
if faltantes:
    print(f"\n{'='*80}")
    print(f"📝 CÓDIGO PARA AGREGAR AL GeneradorService.cs")
    print(f"{'='*80}")
    print("\nAgregar estas líneas en la variable 'todasLasDefiniciones':\n")
    
    for tabla_lower in sorted(faltantes):
        nombre_original, esquema = clickone[tabla_lower]
        print(f'DROP TABLE IF EXISTS ""{nombre_original}"";')
        print(f'CREATE TABLE ""{nombre_original}"" ({esquema});')

# Identificar tablas extras a eliminar
if extras:
    print(f"\n{'='*80}")
    print(f"🗑️  TABLAS A ELIMINAR DEL GeneradorService.cs")
    print(f"{'='*80}")
    print("\nEliminar estas líneas de la variable 'todasLasDefiniciones':\n")
    
    for tabla_lower in sorted(extras):
        nombre_original, _ = web[tabla_lower]
        print(f'   - DROP TABLE IF EXISTS ""{nombre_original}"";')
        print(f'   - CREATE TABLE ""{nombre_original}"" (...);')

# Verificar esquemas diferentes
print(f"\n{'='*80}")
print(f"⚠️  VERIFICACIÓN DE ESQUEMAS")
print(f"{'='*80}")

comunes = set(clickone.keys()) & set(web.keys())
esquemas_diferentes = []

for tabla_lower in sorted(comunes):
    click_nombre, click_esquema = clickone[tabla_lower]
    web_nombre, web_esquema = web[tabla_lower]
    
    if click_esquema != web_esquema:
        esquemas_diferentes.append((tabla_lower, click_nombre, click_esquema, web_esquema))

if esquemas_diferentes:
    print(f"\n⚠️  {len(esquemas_diferentes)} tablas con esquemas DIFERENTES que deben corregirse:")
    for tabla_lower, nombre, click_esq, web_esq in esquemas_diferentes[:5]:
        print(f"\n   📋 {nombre}:")
        print(f"      ClickOne: {click_esq[:100]}...")
        print(f"      Web:      {web_esq[:100]}...")
else:
    print("\n✅ Todos los esquemas de tablas comunes son correctos")

print(f"\n{'='*80}")
print("RESUMEN FINAL:")
print(f"{'='*80}")
print(f"✅ Tablas correctas: {len(comunes) - len(esquemas_diferentes)}")
print(f"⚠️  Tablas con esquemas incorrectos: {len(esquemas_diferentes)}")
print(f"❌ Tablas faltantes: {len(faltantes)}")
print(f"🗑️  Tablas extras (eliminar): {len(extras)}")
print(f"\n🎯 TOTAL DE CAMBIOS NECESARIOS: {len(faltantes) + len(extras) + len(esquemas_diferentes)}")
print("="*80)
