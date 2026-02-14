# 📦 Publicaciones para Producción

Esta carpeta contiene las publicaciones compiladas del Servicio Web Generador listas para desplegar en producción.

---

## 📁 Estructura de Carpetas

Cada publicación se organiza por fecha en formato `dd_MM_yyyy`:

```
publicaciones/
├── 4_2_2026/          # Publicación del 4 de Febrero, 2026
├── 14_2_2026/         # Publicación del 14 de Febrero, 2026 (con endpoint KPIs)
└── README.md          # Este archivo
```

---

## 🚀 Cómo Generar una Nueva Publicación

### Opción 1: Usar el Script Automatizado (Recomendado)

Desde la raíz del proyecto:

```powershell
.\publicar.ps1
```

El script:
- ✅ Crea carpeta con fecha actual
- ✅ Compila en modo Release
- ✅ Verifica archivos esenciales
- ✅ Muestra tamaño de publicación
- ✅ Lista próximos pasos

### Opción 2: Comando Manual

```bash
cd WebApplication1
dotnet publish -c Release -o ../publicaciones/[fecha] --no-self-contained
```

---

## 📋 Archivos Esenciales en Cada Publicación

### Archivos Principales
- `WebApplication1.dll` - Aplicación compilada
- `WebApplication1.exe` - Ejecutable de Windows
- `appsettings.json` - Configuración de producción
- `web.config` - Configuración de IIS

### Archivos de Configuración
- `appsettings.Development.json` - Configuración de desarrollo (opcional)
- `WebApplication1.deps.json` - Dependencias
- `WebApplication1.runtimeconfig.json` - Configuración de runtime

### Dependencias
- `System.Data.SqlClient.dll` - Conexión a SQL Server
- `System.Data.SQLite.dll` - Manejo de SQLite
- `Swashbuckle.*.dll` - Swagger/OpenAPI
- Carpeta `runtimes/` - Librerías nativas por plataforma

---

## 🔧 Despliegue en IIS (Windows Server)

### Paso 1: Preparar el Servidor

1. Instalar IIS
2. Instalar ASP.NET Core Runtime (versión 10.0 o superior)
3. Instalar módulo ASP.NET Core para IIS

### Paso 2: Configurar Connection String

Editar `appsettings.json` en la publicación:

```json
{
  "ConnectionStrings": {
    "MedinetWeb": "Server=TU_SERVIDOR;Database=TU_BD;User Id=TU_USUARIO;Password=TU_PASSWORD;"
  }
}
```

### Paso 3: Copiar Archivos

Copiar toda la carpeta de publicación al servidor:
```
C:\inetpub\wwwroot\generador_servicio_web\
```

### Paso 4: Crear Aplicación en IIS

1. Abrir IIS Manager
2. Crear nuevo Application Pool:
   - Nombre: `GeneradorServicePool`
   - .NET CLR Version: `No Managed Code`
   - Managed Pipeline Mode: `Integrated`
3. Crear nueva aplicación:
   - Alias: `generador` (o el que prefieras)
   - Application Pool: `GeneradorServicePool`
   - Physical Path: Ruta donde copiaste los archivos

### Paso 5: Configurar Permisos

Dar permisos de lectura/ejecución al usuario de IIS:
```
IIS_IUSRS
IIS APPPOOL\GeneradorServicePool
```

### Paso 6: Probar

Acceder a:
```
http://tu-servidor/generador/api/visitador
```

---

## 🧪 Verificación Post-Despliegue

### Endpoints a Probar

1. **Health Check**
   ```
   GET http://servidor/generador/api/visitador/annios
   ```
   Debe retornar lista de años

2. **Lista de Visitadores**
   ```
   GET http://servidor/generador/api/visitador
   ```
   Debe retornar lista de visitadores

3. **KPIs (Nuevo en 14/Feb/2026)**
   ```
   GET http://servidor/generador/api/visitador/336/kpis?ano=2026&ciclo=1
   ```
   Debe retornar KPIs del visitador

4. **Generar Cartera**
   ```
   GET http://servidor/generador/api/visitador/336/cartera?ano=2026&ciclo=1
   ```
   Debe descargar archivo Cartera.txt

---

## 📝 Historial de Publicaciones

### 14/02/2026 - v1.1
- ✅ Nuevo endpoint: `/api/visitador/{id}/kpis`
- ✅ Obtención de KPIs desde tabla MW_Ciclos
- ✅ Documentación técnica completa
- ✅ Compilación exitosa sin errores

### 04/02/2026 - v1.0
- ✅ Corrección de esquemas de tablas
- ✅ 114 tablas idénticas a ClickOne
- ✅ 25 tablas agregadas
- ✅ 26 tablas erróneas eliminadas
- ✅ Compatibilidad 100% con apps Android

---

## ⚠️ Notas Importantes

### Antes de Desplegar

1. **Backup**: Hacer backup de la versión actual en producción
2. **Connection String**: Verificar que apunte a la BD correcta
3. **Permisos**: Verificar permisos de archivos y carpetas
4. **Testing**: Probar en ambiente de staging primero

### Durante el Despliegue

1. **Detener IIS**: Detener el Application Pool antes de copiar archivos
2. **Copiar Archivos**: Reemplazar todos los archivos
3. **Iniciar IIS**: Iniciar el Application Pool
4. **Verificar Logs**: Revisar logs de IIS y de la aplicación

### Después del Despliegue

1. **Probar Endpoints**: Verificar que todos los endpoints funcionen
2. **Monitorear**: Revisar logs por errores
3. **Notificar**: Informar al equipo del despliegue exitoso

---

## 🔍 Troubleshooting

### Error: "HTTP Error 500.31 - Failed to load ASP.NET Core runtime"
**Solución**: Instalar ASP.NET Core Runtime en el servidor

### Error: "Cannot connect to SQL Server"
**Solución**: Verificar connection string en appsettings.json

### Error: "Access Denied"
**Solución**: Verificar permisos del Application Pool en la carpeta

### Error: "DLL not found"
**Solución**: Verificar que todas las DLLs se copiaron correctamente

---

## 📞 Contacto

Para problemas o preguntas sobre el despliegue:
1. Revisar documentación en `docs/`
2. Verificar logs de IIS
3. Consultar `docs/ENDPOINTS.md` para API

---

**Última Actualización**: 14 de Febrero, 2026
