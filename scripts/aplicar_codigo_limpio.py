#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para aplicar el código limpio al servicio GeneradorService.cs
"""

import re

# Leer el contenido del método limpio
with open('metodo_contenido.txt', 'r', encoding='utf-8') as f:
    metodo_limpio = f.read()

# Leer el archivo del servicio
with open('WebApplication1/Services/GeneradorService.cs', 'r', encoding='utf-8') as f:
    servicio = f.read()

# Buscar el método GenerarEsquemaTablas y reemplazarlo
# Patrón: desde "private void GenerarEsquemaTablas" hasta el cierre del método
patron = r'(private void GenerarEsquemaTablas\(StringBuilder contenido\)\s*\{)(.*?)(\n\s*\})'

def reemplazar_metodo(match):
    # Mantener la firma del método y el cierre
    firma = match.group(1)
    cierre = match.group(3)
    
    # Construir el nuevo método con el contenido limpio
    nuevo_metodo = f"{firma}\n            // 114 definiciones de tablas (sin las 26 tablas extras)\n            {metodo_limpio}\n        {cierre}"
    
    return nuevo_metodo

# Reemplazar el método
servicio_nuevo = re.sub(patron, reemplazar_metodo, servicio, flags=re.DOTALL)

# Verificar que se hizo el reemplazo
if servicio_nuevo == servicio:
    print("❌ No se pudo reemplazar el método")
    exit(1)

# Guardar el archivo modificado
with open('WebApplication1/Services/GeneradorService.cs', 'w', encoding='utf-8') as f:
    f.write(servicio_nuevo)

print("✅ Método GenerarEsquemaTablas reemplazado exitosamente")
print("   El servicio ahora genera solo las 114 tablas de ClickOne")
print("   Se eliminaron las 26 tablas extras")
