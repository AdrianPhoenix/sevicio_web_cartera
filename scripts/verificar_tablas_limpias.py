#!/usr/bin/env python3
# -*- coding: utf-8 -*-
"""
Script de VERIFICACIÓN EXHAUSTIVA
Asegura que el código limpio contiene TODAS las tablas correctas
y SOLO elimina las 26 tablas extras
"""

import re

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

def extraer_tablas_de_archivo(archivo):
    """Extrae nombres de tablas CREATE TABLE de un archivo"""
    tablas = []
    with open(archivo, 'r', encoding='utf-8') as f:
        contenido = f.read()
        patron = r'CREATE TABLE\s+"?(\w+)"?\s*\('
        matches = re.findall(patron, contenido, re.IGNORECASE)
        tablas = [m.lower() for m in matches]
    return sorted(set(tablas))

def verificar_integridad():
    """Verificación exhaustiva de integridad"""
    
    print("=" * 80)
    print("🔍 VERIFICACIÓN EXHAUSTIVA DE INTEGRIDAD")
    print("=" * 80)
    print()
    
    # 1. Extraer tablas de ClickOne (referencia correcta)
    print("📂 Paso 1: Extrayendo tablas de ClickOne (REFERENCIA CORRECTA)...")
    tablas_clickone = extraer_tablas_de_archivo('test_carteras/clickOne/Cartera_zona_343.txt')
    print(f"   ✅ {len(tablas_clickone)} tablas encontradas")
    print()
    
    # 2. Extraer tablas del Web Service actual
    print("📂 Paso 2: Extrayendo tablas del Web Service ACTUAL...")
    tablas_web_actual = extraer_tablas_de_archivo('test_carteras/web_localhost/Cartera_zona_343.txt')
    print(f"   ✅ {len(tablas_web_actual)} tablas encontradas")
    print()
    
    # 3. Extraer tablas del código LIMPIO generado
    print("📂 Paso 3: Extrayendo tablas del código LIMPIO generado...")
    tablas_limpio = extraer_tablas_de_archivo('GenerarEsquemaTablas_LIMPIO.cs')
    print(f"   ✅ {len(tablas_limpio)} tablas encontradas")
    print()
    
    # 4. Calcular diferencias
    print("=" * 80)
    print("📊 ANÁLISIS DE DIFERENCIAS")
    print("=" * 80)
    print()
    
    set_clickone = set(tablas_clickone)
    set_web_actual = set(tablas_web_actual)
    set_limpio = set(tablas_limpio)
    
    # Tablas extras en Web actual
    extras_en_web = set_web_actual - set_clickone
    
    # Tablas que se eliminarían
    tablas_eliminadas = set_web_actual - set_limpio
    
    # Tablas que se mantienen
    tablas_mantenidas = set_limpio
    
    print(f"📌 ClickOne (REFERENCIA):        {len(tablas_clickone)} tablas")
    print(f"📌 Web Service ACTUAL:           {len(tablas_web_actual)} tablas")
    print(f"📌 Código LIMPIO generado:       {len(tablas_limpio)} tablas")
    print()
    print(f"➕ Tablas extras en Web actual:  {len(extras_en_web)} tablas")
    print(f"➖ Tablas que se eliminarían:    {len(tablas_eliminadas)} tablas")
    print(f"✅ Tablas que se mantienen:      {len(tablas_mantenidas)} tablas")
    print()
    
    # 5. VERIFICACIÓN CRÍTICA 1: ¿El código limpio tiene TODAS las tablas de ClickOne?
    print("=" * 80)
    print("🔒 VERIFICACIÓN CRÍTICA #1")
    print("¿El código LIMPIO contiene TODAS las tablas de ClickOne?")
    print("=" * 80)
    print()
    
    faltantes_en_limpio = set_clickone - set_limpio
    
    if len(faltantes_en_limpio) == 0:
        print("✅ ¡PERFECTO! El código LIMPIO contiene TODAS las 114 tablas de ClickOne")
        print("   No se perderá ninguna tabla necesaria.")
    else:
        print(f"❌ ¡PELIGRO! Faltan {len(faltantes_en_limpio)} tablas de ClickOne en el código limpio:")
        for tabla in sorted(faltantes_en_limpio):
            print(f"   - {tabla}")
        return False
    
    print()
    
    # 6. VERIFICACIÓN CRÍTICA 2: ¿Solo se eliminan las 26 tablas extras?
    print("=" * 80)
    print("🔒 VERIFICACIÓN CRÍTICA #2")
    print("¿Solo se eliminan las 26 tablas extras y NADA MÁS?")
    print("=" * 80)
    print()
    
    # Verificar que las tablas eliminadas son exactamente las 26 extras
    set_tablas_a_eliminar = set(TABLAS_A_ELIMINAR)
    
    if tablas_eliminadas == set_tablas_a_eliminar:
        print(f"✅ ¡PERFECTO! Se eliminan EXACTAMENTE las {len(TABLAS_A_ELIMINAR)} tablas extras")
        print("   No se toca ninguna tabla necesaria.")
    else:
        print("❌ ¡ADVERTENCIA! Hay discrepancias:")
        
        # Tablas que se eliminan pero no deberían
        eliminadas_incorrectas = tablas_eliminadas - set_tablas_a_eliminar
        if eliminadas_incorrectas:
            print(f"\n   ❌ Tablas que se eliminarían INCORRECTAMENTE ({len(eliminadas_incorrectas)}):")
            for tabla in sorted(eliminadas_incorrectas):
                print(f"      - {tabla}")
        
        # Tablas que deberían eliminarse pero no se eliminan
        no_eliminadas = set_tablas_a_eliminar - tablas_eliminadas
        if no_eliminadas:
            print(f"\n   ⚠️  Tablas extras que NO se eliminarían ({len(no_eliminadas)}):")
            for tabla in sorted(no_eliminadas):
                print(f"      - {tabla}")
        
        return False
    
    print()
    
    # 7. VERIFICACIÓN CRÍTICA 3: Comparación exacta
    print("=" * 80)
    print("🔒 VERIFICACIÓN CRÍTICA #3")
    print("¿El código LIMPIO es IDÉNTICO a ClickOne?")
    print("=" * 80)
    print()
    
    if set_limpio == set_clickone:
        print("✅ ¡PERFECTO! El código LIMPIO es IDÉNTICO a ClickOne")
        print("   Paridad 100% garantizada.")
    else:
        diferencias = set_limpio.symmetric_difference(set_clickone)
        print(f"⚠️  Hay {len(diferencias)} diferencias:")
        for tabla in sorted(diferencias):
            if tabla in set_limpio:
                print(f"   + {tabla} (en LIMPIO, no en ClickOne)")
            else:
                print(f"   - {tabla} (en ClickOne, no en LIMPIO)")
        return False
    
    print()
    
    # 8. RESUMEN FINAL
    print("=" * 80)
    print("📋 RESUMEN FINAL DE VERIFICACIÓN")
    print("=" * 80)
    print()
    
    print("✅ VERIFICACIÓN #1: Código LIMPIO contiene todas las tablas de ClickOne")
    print("✅ VERIFICACIÓN #2: Solo se eliminan las 26 tablas extras")
    print("✅ VERIFICACIÓN #3: Código LIMPIO es idéntico a ClickOne")
    print()
    
    print("=" * 80)
    print("🎉 ¡VERIFICACIÓN EXITOSA AL 100%!")
    print("=" * 80)
    print()
    print("Es SEGURO aplicar el código limpio:")
    print()
    print(f"  • Se mantienen:  {len(tablas_mantenidas)} tablas (las correctas)")
    print(f"  • Se eliminan:   {len(tablas_eliminadas)} tablas (solo las extras)")
    print(f"  • Resultado:     Paridad 100% con ClickOne")
    print()
    print("✅ NO se perderá ninguna tabla necesaria")
    print("✅ SOLO se eliminarán las 26 tablas extras")
    print()
    
    # 9. Mostrar las 26 tablas que se eliminarán
    print("=" * 80)
    print("📝 LISTA DE LAS 26 TABLAS QUE SE ELIMINARÁN")
    print("=" * 80)
    print()
    
    for i, tabla in enumerate(sorted(tablas_eliminadas), 1):
        print(f"{i:2d}. {tabla}")
    
    print()
    print("=" * 80)
    
    return True

if __name__ == '__main__':
    exito = verificar_integridad()
    
    if exito:
        print("\n✅ Verificación completada exitosamente")
        print("   Es SEGURO proceder con la implementación")
    else:
        print("\n❌ Verificación FALLÓ")
        print("   NO proceder hasta resolver los problemas")
