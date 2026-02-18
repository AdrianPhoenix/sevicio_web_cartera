#!/usr/bin/env python3
import re

def extraer_esquemas_clickone(archivo):
    """Extrae TODOS los esquemas CREATE TABLE del ClickOne"""
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    # Buscar todos los bloques DROP + CREATE TABLE
    patron = r'(DROP TABLE IF EXISTS\s+"?(\w+)"?;\s*CREATE TABLE\s+"?\2"?\s*\([^;]+\);)'
    matches = re.findall(patron, contenido, re.IGNORECASE | re.DOTALL)
    
    esquemas = []
    for bloque_completo, nombre_tabla in matches:
        esquemas.append(bloque_completo.strip())
    
    return esquemas

print("Extrayendo esquemas del ClickOne...")
esquemas = extraer_esquemas_clickone('test_carteras/clickOne/Cartera.txt')

print(f"✅ Extraídos {len(esquemas)} esquemas de tablas")

# Generar el código C# con los esquemas
codigo_csharp = 'var todasLasDefiniciones = @"'

for esquema in esquemas:
    codigo_csharp += esquema + '\n'

codigo_csharp += '";'

# Guardar en archivo
with open('esquemas_clickone_extraidos.txt', 'w', encoding='utf-8') as f:
    f.write(codigo_csharp)

print(f"✅ Código C# generado en: esquemas_clickone_extraidos.txt")
print(f"\n📊 ESTADÍSTICAS:")
print(f"   Total de tablas: {len(esquemas)}")
print(f"   Tamaño del código: {len(codigo_csharp)} caracteres")
