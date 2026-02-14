# 📚 Documentación - Servicio Web Generador

Documentación completa del proyecto de migración de ClickOnce a Web Service para generación de carteras de visitadores médicos.

---

## 📋 Índice de Documentos

### **Documentos Activos** ✅

1. **[ESTADO_ACTUAL.md](ESTADO_ACTUAL.md)** - **LEER PRIMERO**
   - Estado actual del proyecto (actualizado: 14/Feb/2026)
   - Comparación de carteras ClickOne vs Web Producción
   - Análisis de 26 tablas extras
   - Recomendaciones y próximos pasos

2. **[ENDPOINTS.md](ENDPOINTS.md)**
   - Documentación completa de la API REST
   - Endpoints implementados y funcionando
   - Ejemplos de uso y respuestas
   - Estado: Producción ✅

3. **[ENDPOINT_KPIS_TECNICO.md](ENDPOINT_KPIS_TECNICO.md)** - **NUEVO** ⭐
   - Documentación técnica del endpoint de KPIs
   - Arquitectura de implementación
   - Estructura de base de datos
   - Casos de prueba y ejemplos
   - Fecha: 14/Feb/2026

4. **[CAMBIOS_APLICADOS.md](CAMBIOS_APLICADOS.md)**
   - Historial de cambios del 4 de Febrero, 2026
   - Tablas agregadas, eliminadas y corregidas
   - Proceso de migración de esquemas
   - Referencia histórica

### **Documentos Obsoletos** ⚠️

4. **[ELEMENTOS_FALTANTES.md](ELEMENTOS_FALTANTES.md)** - **OBSOLETO**
   - Análisis original de tablas faltantes (INCORRECTO)
   - Reemplazado por: `ESTADO_ACTUAL.md`
   - Mantener solo como referencia histórica

---

## 🚀 Inicio Rápido

### Para entender el proyecto:
1. Lee **ESTADO_ACTUAL.md** para conocer el estado actual
2. Revisa **ENDPOINTS.md** para ver la API disponible
3. Consulta **CAMBIOS_APLICADOS.md** para entender el historial

### Para desarrolladores nuevos:
```bash
# 1. Clonar el repositorio
git clone <repo-url>
cd servicio_web_generador

# 2. Revisar la rama actual
git branch --show-current

# 3. Leer documentación
cd docs
# Leer ESTADO_ACTUAL.md primero
```

---

## 📊 Estado del Proyecto

| Aspecto | Estado | Notas |
|---------|--------|-------|
| **API en Producción** | ✅ Funcionando | 100% operativa |
| **Apps Android** | ✅ Compatible | Sin errores |
| **Carteras ClickOne** | ✅ Compatible | Todas las tablas presentes |
| **Tablas Extras** | ⚠️ 26 extras | No críticas |
| **Documentación** | ✅ Actualizada | 14/Feb/2026 |
| **Endpoint KPIs** | ✅ Nuevo | Implementado 14/Feb/2026 |

---

## 🚀 Endpoints Disponibles

### Endpoints Principales

1. **GET /api/visitador** - Lista de visitadores activos
2. **GET /api/visitador/annios** - Años disponibles
3. **GET /api/visitador/{id}/google-registration** - Google Registration ID
4. **GET /api/visitador/{id}/cartera** - Generar archivo Cartera.txt
5. **GET /api/visitador/{id}/kpis** - Obtener KPIs del visitador ⭐ NUEVO

Ver documentación completa en [ENDPOINTS.md](ENDPOINTS.md)

---

## 🔧 Herramientas de Análisis

### Scripts Disponibles

1. **comparar_carteras.ps1** (raíz del proyecto)
   - Compara tablas entre carteras
   - Identifica diferencias
   - Genera reporte detallado

```powershell
# Ejecutar comparación
powershell -ExecutionPolicy Bypass -File comparar_carteras.ps1
```

2. **extraer_esquemas.py** (raíz del proyecto)
   - Extrae esquemas de archivos Cartera.txt
   - Genera archivos de análisis

```bash
python3 extraer_esquemas.py
```

---

## 📁 Estructura del Proyecto

```
servicio_web_generador/
├── docs/                          # Documentación (estás aquí)
│   ├── README.md                  # Este archivo
│   ├── ESTADO_ACTUAL.md          # Estado actual del proyecto ⭐
│   ├── ENDPOINTS.md              # Documentación de API
│   ├── CAMBIOS_APLICADOS.md      # Historial de cambios
│   └── ELEMENTOS_FALTANTES.md    # OBSOLETO
├── test_carteras/                # Carteras de prueba
│   ├── clickOne/                 # Cartera ClickOne (referencia)
│   └── web_produccion/           # Cartera Web Producción
├── WebApplication1/              # Código fuente del servicio web
│   └── Services/
│       └── GeneradorService.cs   # Servicio principal
├── comparar_carteras.ps1         # Script de comparación
└── extraer_esquemas.py           # Script de extracción
```

---

## 🎯 Próximos Pasos

### Recomendaciones Inmediatas
1. ✅ Mantener sistema en producción (funciona perfectamente)
2. 🔄 Monitorear logs para verificar uso de tablas extras
3. 📝 Documentar nuevas funcionalidades según se agreguen

### Mejoras Futuras (Opcional)
1. Eliminar 26 tablas extras para paridad exacta con ClickOne
2. Optimizar esquemas según uso real
3. Implementar sincronización bidireccional

---

## 📞 Contacto y Soporte

Para preguntas o problemas:
1. Revisar documentación en `docs/`
2. Consultar logs de producción
3. Ejecutar scripts de análisis

---

**Última Actualización**: 14 de Febrero, 2026  
**Versión de Documentación**: 1.0  
**Estado**: Producción ✅
