# 🔧 Scripts de Utilidad

Scripts para análisis, comparación y extracción de datos del proyecto.

## 📋 Scripts Disponibles

### 🐍 Scripts Python

#### `analisis_detallado.py`
Análisis detallado de diferencias entre carteras.

**Uso:**
```bash
python analisis_detallado.py
```

#### `comparar_tablas.py`
Compara tablas entre diferentes carteras generadas.

**Uso:**
```bash
python comparar_tablas.py
```

#### `extraer_esquemas.py`
Extrae esquemas de tablas desde archivos Cartera.txt.

**Uso:**
```bash
python extraer_esquemas.py
```

**Output:** Genera archivos con esquemas extraídos.

#### `extraer_esquemas_csharp.py`
Extrae esquemas desde código C# del generador ClickOnce.

**Uso:**
```bash
python extraer_esquemas_csharp.py
```

#### `generar_correcciones.py`
Genera correcciones automáticas para esquemas de tablas.

**Uso:**
```bash
python generar_correcciones.py
```

---

### 💻 Scripts PowerShell

#### `comparar_carteras.ps1`
Script completo de comparación entre carteras ClickOne y Web.

**Uso:**
```powershell
powershell -ExecutionPolicy Bypass -File scripts/comparar_carteras.ps1
```

**Funcionalidad:**
- Compara tablas entre carteras
- Identifica diferencias
- Genera reporte detallado

---

## 🎯 Casos de Uso Comunes

### Comparar Carteras
```bash
# Opción 1: PowerShell (recomendado)
powershell -ExecutionPolicy Bypass -File scripts/comparar_carteras.ps1

# Opción 2: Python
python scripts/comparar_tablas.py
```

### Extraer Esquemas
```bash
# Desde archivos Cartera.txt
python scripts/extraer_esquemas.py

# Desde código C#
python scripts/extraer_esquemas_csharp.py
```

### Análisis Detallado
```bash
python scripts/analisis_detallado.py
```

---

## 📝 Notas

- Los scripts Python requieren Python 3.x
- Los scripts PowerShell pueden requerir permisos de ejecución
- Algunos scripts generan archivos de output en la raíz del proyecto

---

**Última actualización:** Febrero 2026
