# 📋 Plan de Corrección de Tablas - Estructura

**Fecha:** 17 de Febrero, 2026  
**Objetivo:** Corregir las diferencias de estructura entre ClickOne y Web Service

---

## 🎯 Tablas a Corregir (4 tablas prioritarias)

### 1. farmacias_personal

**Problema:** Columnas en diferente orden y tipos diferentes

**ClickOne (CORRECTO):**
```sql
CREATE TABLE "farmacias_personal" (
    "ID" INTEGER(11),
    "NUMERO" INTEGER(11),
    "ZONA" TEXT(7),
    "NOMBRE" TEXT(40),
    "CARGO" TEXT(40),
    "TELEFONO" TEXT(40),
    "CORREO" TEXT(255),
    "CUMPLEANO_MES" TEXT(25)
);
```

**Web Service (INCORRECTO):**
```sql
CREATE TABLE "farmacias_personal" (
    "NUMERO" INTEGER(11),
    "ZONA" TEXT(7),
    "FARMACIA" TEXT(100),
    "PERSONAL" TEXT(100),
    "CARGO" TEXT(50),
    "TELEFONO" TEXT(30),
    "CUMPLEANO" TEXT(10),
    "OBSERVACION" TEXT(100)
);
```

**Acción:** Reemplazar completamente la definición en GeneradorService.cs

---

### 2. mw_productos

**Problema:** Columnas en orden diferente

**ClickOne (CORRECTO):**
```sql
CREATE TABLE "mw_productos" (
    "ID_Producto" INTEGER NOT NULL,
    "ID_Marca" INTEGER NOT NULL,
    "TX_Producto" TEXT(150) NOT NULL,
    "TX_IDProductoCliente" TEXT(15) NOT NULL,
    "TX_ProductoDesc" TEXT(250) NOT NULL,
    "BO_Activo" INTEGER NOT NULL
);
```

**Web Service (INCORRECTO):**
```sql
CREATE TABLE "mw_productos" (
    "ID_Producto" INTEGER NOT NULL,
    "TX_Producto" TEXT(50) NOT NULL,
    "TX_ProductoDesc" TEXT(100),
    "ID_Linea" INTEGER NOT NULL,
    "ID_Marca" INTEGER NOT NULL,
    "BO_Activo" INTEGER NOT NULL
);
```

**Acción:** Reemplazar completamente la definición en GeneradorService.cs

---

### 3. visita_detalles

**Problema:** Tipo de dato diferente en columna PRODUCTO

**ClickOne (CORRECTO):**
- Columna 6: "PRODUCTO" TEXT(30)

**Web Service (INCORRECTO):**
- Columna 6: "PRODUCTO" TEXT(255)

**Acción:** Cambiar TEXT(255) a TEXT(30) en la definición

---

### 4. visitas

**Problema:** Tipos de datos diferentes en FECHA_SISTEMA y HORA_SISTEMA

**ClickOne (CORRECTO):**
- Columna 9: "FECHA_SISTEMA" TEXT(255)
- Columna 10: "HORA_SISTEMA" TEXT(8)

**Web Service (INCORRECTO):**
- Columna 9: "FECHA_SISTEMA" TEXT(20)
- Columna 10: "HORA_SISTEMA" TEXT(20)

**Acción:** Cambiar TEXT(20) a TEXT(255) para FECHA_SISTEMA y TEXT(20) a TEXT(8) para HORA_SISTEMA

---

## 📝 Procedimiento de Corrección

### Paso 1: Localizar las definiciones en GeneradorService.cs
- Buscar cada tabla en el método `GenerarEsquemaTablas`
- Identificar la línea exacta de la definición CREATE TABLE

### Paso 2: Extraer definición correcta de ClickOne
- Copiar la definición exacta desde la cartera de ClickOne
- Verificar que sea idéntica

### Paso 3: Reemplazar en GeneradorService.cs
- Usar strReplace para cambiar la definición completa
- Verificar que no haya errores de sintaxis

### Paso 4: Compilar y probar
- Compilar el servicio
- Generar una cartera de prueba
- Comparar con ClickOne usando el script de comparación

---

## ✅ Checklist de Verificación

Después de cada corrección:
- [ ] Definición copiada exactamente de ClickOne
- [ ] Reemplazo aplicado en GeneradorService.cs
- [ ] Compilación exitosa
- [ ] Cartera generada sin errores
- [ ] Comparación con ClickOne muestra tabla idéntica

---

## 🚨 Notas Importantes

1. **NO tocar otras tablas** - Solo corregir las 4 identificadas
2. **Copiar exactamente** - No modificar nombres o tipos
3. **Verificar después de cada cambio** - No hacer todos a la vez
4. **Mantener backup** - Git ya tiene el backup automático

---

**Estado:** Pendiente de ejecución
