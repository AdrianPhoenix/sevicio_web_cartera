#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para extraer el contenido del método GenerarEsquemaTablas
"""

with open('GenerarEsquemaTablas_LIMPIO.cs', 'r', encoding='utf-8') as f:
    contenido = f.read()

# Extraer solo el contenido dentro del método (sin la firma)
# Buscar desde "var todasLasDefiniciones" hasta el final
inicio = contenido.find('var todasLasDefiniciones')
fin = contenido.rfind('contenido.AppendLine(todasLasDefiniciones);')

if inicio != -1 and fin != -1:
    # Extraer el contenido
    metodo_contenido = contenido[inicio:fin + len('contenido.AppendLine(todasLasDefiniciones);')]
    
    # Guardar en archivo temporal
    with open('metodo_contenido.txt', 'w', encoding='utf-8') as out:
        out.write(metodo_contenido)
    
    print("✅ Contenido del método extraído exitosamente")
    print(f"   Tamaño: {len(metodo_contenido)} caracteres")
else:
    print("❌ No se pudo extraer el contenido del método")
