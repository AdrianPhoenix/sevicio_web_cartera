# 🚀 Guía de Deployment

Documentación de despliegues y procedimientos de publicación.

## 📋 Historial de Despliegues

- **[17 de Febrero, 2026](DESPLIEGUE_17_FEB_2026.md)** - v4.1.0: Correcciones de estructura (15 tablas, 76 columnas), nomenclatura (4 tablas) y datos (CICLO en 2 tablas) ✅ ACTUAL
- **[14 de Febrero, 2026](DESPLIEGUE_14_FEB_2026.md)** - v4.0.1: Endpoint KPIs + Fixes Issues #002, #004, #006

## 🎯 Proceso de Deployment

### 1. Preparación

```bash
# Asegurarse de estar en la rama correcta
git checkout main

# Verificar que todo compila
cd WebApplication1
dotnet build -c Release
```

### 2. Publicación

```bash
# Generar publicación
dotnet publish -c Release -o ../publicaciones/v{version}

# Ejemplo:
dotnet publish -c Release -o ../publicaciones/v4_0_1
```

### 3. Deployment en Servidor

**Servidor:** mdnconsultores.com:8080

1. Detener Application Pool en IIS
2. Reemplazar archivos en servidor
3. Iniciar Application Pool
4. Verificar funcionamiento

### 4. Verificación Post-Deployment

```bash
# Verificar endpoints básicos
curl http://mdnconsultores.com:8080/api/visitador
curl http://mdnconsultores.com:8080/api/visitador/annios

# Verificar endpoint crítico
curl "http://mdnconsultores.com:8080/api/visitador/336/cartera?ano=2026&ciclo=2"
```

## 🔄 Rollback Plan

En caso de problemas:

1. Detener IIS Application Pool
2. Restaurar versión anterior desde `publicaciones/v{version_anterior}/`
3. Reiniciar Application Pool
4. Verificar funcionamiento

## 📝 Checklist de Deployment

- [ ] Código compilado sin errores
- [ ] Tests pasando (si aplica)
- [ ] CHANGELOG actualizado
- [ ] Versión incrementada
- [ ] Tag de git creado
- [ ] Publicación generada
- [ ] Backup de versión anterior
- [ ] Deployment ejecutado
- [ ] Verificación post-deployment
- [ ] Documentación actualizada

## 🔗 Enlaces Útiles

- **Producción:** http://mdnconsultores.com:8080
- **Swagger:** http://mdnconsultores.com:8080/swagger
- **Script de publicación:** `publicar.ps1` (raíz del proyecto)

---

**Última actualización:** Febrero 2026
