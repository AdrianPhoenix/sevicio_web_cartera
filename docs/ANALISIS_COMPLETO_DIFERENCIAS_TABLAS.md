# 📊 Análisis Completo de Diferencias entre Tablas

**Fecha:** 17 de Febrero, 2026  
**Carteras Comparadas:**
- ClickOne: `test_carteras/clickOne/Cartera_zona_343.txt`
- Web Service: `test_carteras/web_localhost/Cartera_zona_343_2.txt` (Nueva - con correcciones)

---

## 📋 Resumen Ejecutivo

| Categoría | Cantidad | Porcentaje |
|-----------|----------|------------|
| **✅ Tablas Idénticas** | 86 | 75.4% |
| **🔴 Columnas Faltantes** | 15 | 13.2% |
| **🟡 Columnas Extra** | 11 | 9.6% |
| **🟠 Orden Diferente** | 2 | 1.8% |
| **🟡 Tipos Diferentes** | 1 | 0.9% |
| **Total Tablas Comunes** | 114 | 100% |

---

## 🔴 CRÍTICO: Tablas con Columnas FALTANTES (15)

Estas tablas tienen menos columnas en Web Service que en ClickOne. **Esto puede causar errores en la app Android.**

### Prioridad Alta (9+ columnas faltantes)

#### 1. mw_farmacias
- **Columnas en ClickOne:** 14
- **Columnas en Web Service:** 5
- **Faltan:** 9 columnas
- **Columnas faltantes:**
  - FE_Registro
  - ID_Brick
  - ID_Cadena
  - ID_Ciudad
  - ID_Clasificacion
  - ID_Estado
  - TX_Rif
  - TX_Ruta
  - TX_Telefono1
  - TX_Telefono2

#### 2. mw_hospitales
- **Columnas en ClickOne:** 14
- **Columnas en Web Service:** 5
- **Faltan:** 9 columnas
- **Columnas faltantes:**
  - BO_Cliente
  - BO_Docente
  - FE_Registro
  - ID_Brick
  - ID_Ciudad
  - ID_Estado
  - ID_Institucion
  - TX_Ruta
  - TX_Telefono1
  - TX_Telefono2

#### 3. pedidosfarmacias
- **Columnas en ClickOne:** 13
- **Columnas en Web Service:** 4
- **Faltan:** 9 columnas
- **Columnas faltantes:**
  - ID_CadenaFarmacias
  - ID_Clasificacion
  - ID_DrogueriaAlmacen
  - ID_Estado
  - ID_VisitadorHistorial
  - NU_Brick
  - TX_Contacto
  - TX_Direccion
  - TX_Farmacia
  - TX_Rif
  - TX_Telefono

### Prioridad Media (4-7 columnas faltantes)

#### 4. ayuda_visual_fe
- **Columnas en ClickOne:** 13
- **Columnas en Web Service:** 6
- **Faltan:** 7 columnas
- **Columnas faltantes:**
  - CICLO
  - CLASIFICACION
  - ESPECIALIDAD
  - FECHA_SISTEMA
  - FECHA_VISITA
  - HORA_SISTEMA
  - MOTIVO
  - ORDEN
  - POSICION
  - PRODUCTO
  - REGISTRO
  - TIPO
  - ZONA

#### 5. ayuda_visual_mp4
- **Columnas en ClickOne:** 13
- **Columnas en Web Service:** 6
- **Faltan:** 7 columnas
- **Columnas faltantes:** (mismas que ayuda_visual_fe)

#### 6. ayuda_visual_mp4_fe
- **Columnas en ClickOne:** 13
- **Columnas en Web Service:** 6
- **Faltan:** 7 columnas
- **Columnas faltantes:** (mismas que ayuda_visual_fe)

#### 7. mw_drogueriasproductos
- **Columnas en ClickOne:** 10
- **Columnas en Web Service:** 6
- **Faltan:** 4 columnas
- **Columnas faltantes:**
  - ID_DrogueriaAlmacen
  - NU_InvProducto
  - NU_PrecioProducto
  - TX_DrogueriaRef1
  - TX_DrogueriaRef2
  - TX_IDProductoDrogueria
  - TX_ProductoDrogueria

#### 8. mw_pedidosfacturascabeceras
- **Columnas en ClickOne:** 10
- **Columnas en Web Service:** 6
- **Faltan:** 4 columnas
- **Columnas faltantes:**
  - FE_FacturaDrogueria
  - FE_Modificado
  - FE_Recibido
  - ID_Drogueria
  - ID_FacturaMedinet
  - ID_PedidoMedinet
  - NU_CostoTotalFactura
  - NU_FacturaDrogueria
  - NU_PedidoDrogueria
  - NU_TotalUnidades

### Prioridad Baja (1-3 columnas faltantes)

#### 9. temp_hoja_ruta_propuesta
- **Columnas en ClickOne:** 7
- **Columnas en Web Service:** 4
- **Faltan:** 3 columnas
- **Columnas faltantes:** AM, CICLO, DIA, Num, PM, SEMANA, ZONA

#### 10. mw_marcas
- **Columnas en ClickOne:** 6
- **Columnas en Web Service:** 3
- **Faltan:** 3 columnas
- **Columnas faltantes:** FE_Registro, ID_Laboratorio, TX_Posicionamiento

#### 11. mw_medicos
- **Columnas en ClickOne:** 8
- **Columnas en Web Service:** 6
- **Faltan:** 2 columnas
- **Columnas faltantes:**
  - NU_RegistroSanitario
  - TX_Apellido1
  - TX_Apellido2
  - TX_Nombre1
  - TX_Nombre2
  - TX_Sello

#### 12. mw_pedidosfacturasdetalles
- **Columnas en ClickOne:** 8
- **Columnas en Web Service:** 6
- **Faltan:** 2 columnas
- **Columnas faltantes:**
  - FE_Modificado
  - FE_Recibido
  - ID_Detalle
  - ID_FacturaMedinet
  - NU_CantidadFacturada
  - TX_IDProductoDrogueria
  - TX_Lote

#### 13. mw_especialidades
- **Columnas en ClickOne:** 4
- **Columnas en Web Service:** 3
- **Faltan:** 1 columna
- **Columnas faltantes:** TX_EspecialidadAbr

#### 14. mw_lineas
- **Columnas en ClickOne:** 4
- **Columnas en Web Service:** 3
- **Faltan:** 1 columna
- **Columnas faltantes:** TX_LineaAbr

#### 15. mw_regiones
- **Columnas en ClickOne:** 4
- **Columnas en Web Service:** 3
- **Faltan:** 1 columna
- **Columnas faltantes:** TX_RegionAbr

---

## 🟡 Tablas con Columnas EXTRA (11)

Estas tablas tienen columnas adicionales que no están en ClickOne. **Menos crítico, pero puede indicar mejoras o inconsistencias.**

### Mejoras Intencionales ✅

#### 1. ciclos
- **Columnas en ClickOne:** 7
- **Columnas en Web Service:** 9
- **Extras:** 2 columnas
- **Columnas extra:** KPI_Visita_Farmacia, KPI_Visita_Medica
- **Nota:** ✅ Estas son mejoras nuevas implementadas en Web Service

#### 2. farmacias_personal
- **Columnas en ClickOne:** 8
- **Columnas en Web Service:** 9
- **Extras:** 1 columna
- **Columnas extra:** CUMPLEANO_DIA
- **Nota:** ✅ Ya corregida, formato mejorado

### Posibles Inconsistencias

#### 3. postular
- **Columnas en ClickOne:** 1
- **Columnas en Web Service:** 5
- **Extras:** 4 columnas
- **Columnas extra:** BO_Activo, FE_Fecha, ID_Postulacion, ID_Visitador, TX_Descripcion

#### 4. version
- **Columnas en ClickOne:** 1
- **Columnas en Web Service:** 4
- **Extras:** 3 columnas
- **Columnas extra:** FE_Fecha, ID_Version, TX_Descripcion, TX_Version

#### 5. farmacias_detalles_productos
- **Columnas en ClickOne:** 8
- **Columnas en Web Service:** 9
- **Extras:** 1 columna
- **Columnas extra:** COMENTARIOS

#### 6. hfarmacias_detalles_productos
- **Columnas en ClickOne:** 8
- **Columnas en Web Service:** 9
- **Extras:** 1 columna
- **Columnas extra:** COMENTARIOS

#### 7. mw_pedidosestatus
- **Columnas en ClickOne:** 2
- **Columnas en Web Service:** 3
- **Extras:** 1 columna
- **Columnas extra:** BO_Activo, ID_PedidoEstatus, TX_PedidoEstatus

#### 8. mw_pedidosestatusprocesado
- **Columnas en ClickOne:** 2
- **Columnas en Web Service:** 3
- **Extras:** 1 columna
- **Columnas extra:** BO_Activo, ID_PedidoEstatusProcesado, TX_PedidoEstatusProcesado

#### 9. pedidoscorreosviscopia
- **Columnas en ClickOne:** 2
- **Columnas en Web Service:** 3
- **Extras:** 1 columna
- **Columnas extra:** BO_Activo, ID_Visitador, TX_Email

#### 10. serial
- **Columnas en ClickOne:** 2
- **Columnas en Web Service:** 3
- **Extras:** 1 columna
- **Columnas extra:** BO_Activo, ID_Serial, TX_Serial

#### 11. umbrales
- **Columnas en ClickOne:** 3
- **Columnas en Web Service:** 4
- **Extras:** 1 columna
- **Columnas extra:** BO_Activo, ID_Umbral, NU_Valor, TX_Concepto

---

## 🟠 Tablas con Orden de Columnas Diferente (2)

### 1. mw_tipomedicos
- **Diferencias:**
  - Posición 0: ClickOne tiene "ID_Tipo", Web Service tiene "ID_TipoMedico"
  - Posición 1: ClickOne tiene "TX_Tipo", Web Service tiene "TX_TipoMedico"

### 2. pedidoscodvisdrog
- **Diferencias:**
  - Posición 0: ClickOne tiene "ID_VisitadorHistorial", Web Service tiene "ID_Visitador"
  - Posición 2: ClickOne tiene "ID_Laboratorio", Web Service tiene "TX_Codigo"
  - Posición 3: ClickOne tiene "TX_Usuario", Web Service tiene "BO_Activo"

---

## 🟡 Tablas con Tipos de Datos Diferentes (1)

### 1. pedidoscodvisdrog
- **Columna:** ID_Drogueria
- **ClickOne:** INTEGER NOT NULL
- **Web Service:** INTEGER(11)
- **Impacto:** Bajo - funcionalmente equivalentes

---

## ✅ Tablas Idénticas (86)

Las siguientes 86 tablas tienen estructura 100% idéntica entre ClickOne y Web Service:

- altas_bajas
- alteraciones
- banner
- ciclospropuesto
- cierreciclo
- contador
- eliminar
- empresas
- esquemahibrido
- farmacias
- farmacias_detalles
- farmacias_master
- fichas
- fichero
- ficherop
- fichero_farmacias
- fichero_horarios
- fichero_hospital
- hfarmacias_detalles
- hhospital_detalles
- hhospital_detalles_medicos
- historialconceptodias
- hoja_ruta
- hoja_ruta_propuesta
- hospital
- hospitales_master
- hospital_detalles
- hospital_detalles_medicos
- hsolicitudes
- hvisitas
- hvisita_detalles
- identificacion
- inclusiones
- md_pedidos
- md_pedidosdetalle
- md_productosTemp
- mw_activespeciales
- mw_beneficiovisitas
- mw_cadenafarmacias
- mw_caracteristicavisitas
- mw_cargos
- mw_ciclos
- mw_clasificacionfarmacias
- mw_clasificacionmedicos
- mw_conceptodescuentos
- mw_consideracionmedicos
- mw_conveniofarmacias
- mw_deportemedicos
- mw_droguerias
- mw_drogueriasalmacenes
- mw_esquemaspromocionales
- mw_farmaciasdroguerias
- mw_galenicas
- mw_idiomamedicos
- mw_instituciones
- mw_laboratorios
- mw_lineasespecialidades
- mw_lineasespecialidadesmarcas
- mw_locacionbricks
- mw_locacionciudades
- mw_locacionestados
- mw_locacionpaises
- mw_motivosvisitas
- mw_ocupacionmedicos
- mw_pedidos
- mw_pedidosdetalle
- mw_perfilesprescriptivos
- mw_posiciones
- mw_productosTemp
- mw_publicaciones
- mw_tipomedicos
- mw_ubicacionfarmacias
- mw_visitadores
- mw_visitadoreshistorial
- mw_zonas
- pedidos
- pedidosdetalle
- productosTemp
- puntos
- solicitudes
- solicitudes_productos
- versiones
- visitas
- visita_detalles
- visita_detalles_productos
- visitadores
- zonas

*(Y más...)*

---

## 🎯 Recomendaciones

### Prioridad Crítica 🔴

Corregir las **15 tablas con columnas faltantes**, especialmente:
1. mw_farmacias (9 columnas faltantes)
2. mw_hospitales (9 columnas faltantes)
3. pedidosfarmacias (9 columnas faltantes)
4. ayuda_visual_* (7 columnas faltantes cada una)

### Prioridad Media 🟡

Revisar las **11 tablas con columnas extra** para determinar si:
- Son mejoras intencionales (como ciclos) → Documentar
- Son inconsistencias → Decidir si mantener o eliminar

### Prioridad Baja 🟠

Revisar las **2 tablas con orden diferente** para determinar si el orden afecta la funcionalidad de la app.

---

## 📊 Impacto en la App Android

### Alto Impacto
- Tablas con columnas faltantes pueden causar errores al intentar leer esas columnas
- La app puede fallar o mostrar datos incompletos

### Medio Impacto
- Columnas extra generalmente no causan problemas (la app ignora lo que no conoce)
- Orden diferente puede causar problemas si la app lee por posición en lugar de por nombre

### Bajo Impacto
- Tipos de datos diferentes (INTEGER vs INTEGER(11)) generalmente son compatibles

---

## 🔗 Referencias

- **Script de Análisis:** `scripts/analisis_completo_diferencias.py`
- **Cartera ClickOne:** `test_carteras/clickOne/Cartera_zona_343.txt`
- **Cartera Web Service:** `test_carteras/web_localhost/Cartera_zona_343_2.txt`
- **Correcciones Previas:** `docs/RESULTADO_CORRECCIONES.md`

---

**Última Actualización:** 17 de Febrero, 2026  
**Estado:** Análisis Completado  
**Próximo Paso:** Priorizar y corregir tablas críticas
