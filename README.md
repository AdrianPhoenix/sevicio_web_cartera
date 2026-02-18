# 🏥 Servicio Web Generador - Medinet

Sistema de generación de carteras para visitadores médicos. Migración exitosa de aplicación ClickOnce a servicio web moderno con ASP.NET Core.

## 📊 Estado del Proyecto

**Versión:** 4.0.1  
**Estado:** ✅ Producción  
**Última actualización:** 16 de Febrero, 2026

- ✅ API REST funcional al 100%
- ✅ Compatible con apps Android existentes
- ✅ 5 endpoints implementados
- ✅ Desplegado en producción: http://mdnconsultores.com:8080

## 🚀 Inicio Rápido

### Requisitos
- .NET 10.0 SDK
- SQL Server (conexión a Medinet_PR)
- Windows (desarrollo) o Linux (producción con IIS)

### Compilar y Ejecutar

```bash
# Restaurar dependencias
cd WebApplication1
dotnet restore

# Compilar
dotnet build

# Ejecutar en desarrollo
dotnet run
```

La API estará disponible en `https://localhost:5001`

## 📚 Documentación

- **[Estado Actual](docs/ESTADO_ACTUAL.md)** - Estado del proyecto y comparación con ClickOne
- **[Endpoints API](docs/ENDPOINTS.md)** - Documentación completa de la API REST
- **[Versionado](docs/VERSIONADO.md)** - Guía de versionado semántico
- **[Issues Resueltos](docs/issues/)** - Historial de bugs y soluciones
- **[Despliegues](docs/deployment/)** - Guía de deployment y historial

## 🔧 Scripts Disponibles

Ver [scripts/README.md](scripts/README.md) para documentación completa de scripts de utilidad.

## 🏗️ Arquitectura

```
Apps Android ↔ API REST ↔ GeneradorService ↔ SQL Server
                    ↓
              Cartera.txt (SQLite)
```

### Tecnologías
- **Backend:** ASP.NET Core 10.0
- **Base de Datos:** SQL Server (Medinet_PR)
- **Output:** Archivos SQLite (Cartera.txt)
- **Deployment:** IIS en Windows Server

## 📦 Estructura del Proyecto

```
servicio_web_generador/
├── WebApplication1/          # Código fuente del servicio web
├── docs/                     # Documentación completa
├── scripts/                  # Scripts de utilidad (Python/PowerShell)
├── test_carteras/           # Carteras de prueba
├── publicaciones/           # Builds de producción
└── Generador_clickOne/      # Sistema legacy (referencia)
```

## 🔗 Enlaces Útiles

- **Producción:** http://mdnconsultores.com:8080
- **Swagger:** http://mdnconsultores.com:8080/swagger
- **CHANGELOG:** [CHANGELOG.md](CHANGELOG.md)

## 👥 Contribuir

1. Crear rama desde `development`
2. Hacer cambios y commits
3. Crear Pull Request a `development`
4. Después de testing, merge a `main`
5. Crear release tag para producción

## 📝 Licencia

Uso interno - Medinet

---

**Última actualización:** Febrero 2026  
**Mantenido por:** Equipo de Desarrollo Medinet
