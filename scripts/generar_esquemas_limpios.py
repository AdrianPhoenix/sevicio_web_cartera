#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script para generar esquemas de tablas sin las 26 tablas extras
"""

# Las 26 tablas que deben eliminarse
TABLAS_A_ELIMINAR = [
    'mw_ayuda_visual',
    'mw_ayuda_visual_fe',
    'mw_ayuda_visual_mp4',
    'mw_ayuda_visual_mp4_fe',
    'mw_configuracion',
    'mw_empresas',
    'mw_especialidadesmedicas',
    'mw_estados',
    'mw_farmacias_detalles_productos',
    'mw_farmacias_personal',
    'mw_hospital_detalles_medicos',
    'mw_hospital_personal',
    'mw_inclusiones',
    'mw_logs',
    'mw_motivos',
    'mw_motivossolicitudes',
    'mw_productoslineas',
    'mw_solicitudes',
    'mw_tipodescuentos',
    'mw_visita_detalles',
    'mw_visitadores',
    'mw_visitadoreshistorial',
    'mw_visitas',
    'mw_zonas',
    'solicitudes_productos',
    'visita_detalles_productos'
]

def extraer_definiciones_tablas(archivo):
    """Extrae las definiciones de tablas de un archivo Cartera.txt"""
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
    
    # Dividir por DROP TABLE
    bloques = contenido.split('DROP TABLE IF EXISTS')
    
    definiciones = []
    for bloque in bloques[1:]:  # Saltar el primer bloque vacío
        # Extraer nombre de tabla
        lineas = bloque.strip().split('\n')
        primera_linea = lineas[0]
        nombre_tabla = primera_linea.split('"')[1].lower()
        
        # Reconstruir definición completa
        definicion = 'DROP TABLE IF EXISTS' + bloque
        
        definiciones.append({
            'nombre': nombre_tabla,
            'definicion': definicion
        })
    
    return definiciones

def generar_codigo_csharp(definiciones):
    """Genera el código C# para el método GenerarEsquemaTablas"""
    
    # Filtrar tablas a eliminar
    definiciones_limpias = [
        d for d in definiciones 
        if d['nombre'] not in TABLAS_A_ELIMINAR
    ]
    
    print(f"Total de tablas originales: {len(definiciones)}")
    print(f"Tablas a eliminar: {len(TABLAS_A_ELIMINAR)}")
    print(f"Tablas resultantes: {len(definiciones_limpias)}")
    print()
    
    # Generar string C#
    codigo = 'private void GenerarEsquemaTablas(StringBuilder contenido)\n'
    codigo += '{\n'
    codigo += '    // 114 definiciones de tablas (sin las 26 tablas extras)\n'
    codigo += '    var todasLasDefiniciones = @"'
    
    # Agregar definiciones
    for i, def_tabla in enumerate(definiciones_limpias):
        codigo += def_tabla['definicion']
        if i < len(definiciones_limpias) - 1:
            codigo += '\n'
    
    codigo += '";\n\n'
    codigo += '    contenido.AppendLine(todasLasDefiniciones);\n'
    codigo += '}\n'
    
    return codigo, definiciones_limpias

if __name__ == '__main__':
    # Extraer de ClickOne (referencia limpia)
    print("Extrayendo definiciones de ClickOne...")
    definiciones = extraer_definiciones_tablas('test_carteras/clickOne/Cartera_zona_343.txt')
    
    # Generar código C#
    print("\nGenerando código C#...")
    codigo, tablas_limpias = generar_codigo_csharp(definiciones)
    
    # Guardar en archivo
    with open('GenerarEsquemaTablas_LIMPIO.cs', 'w', encoding='utf-8') as f:
        f.write(codigo)
    
    print(f"\n✅ Código generado en: GenerarEsquemaTablas_LIMPIO.cs")
    print(f"   Total de tablas: {len(tablas_limpias)}")
    print()
    
    # Listar tablas incluidas
    print("Tablas incluidas:")
    for tabla in sorted([t['nombre'] for t in tablas_limpias]):
        print(f"  - {tabla}")
