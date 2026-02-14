# Script de Publicacion para Produccion
# Servicio Web Generador - Medinet

$ErrorActionPreference = "Stop"

# Configuracion
$fecha = Get-Date -Format "dd_MM_yyyy"
$carpetaPublicacion = "publicaciones\$fecha"
$proyecto = "WebApplication1"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  PUBLICACION PARA PRODUCCION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Fecha: $fecha" -ForegroundColor Yellow
Write-Host "Carpeta destino: $carpetaPublicacion" -ForegroundColor Yellow
Write-Host ""

# Verificar si la carpeta ya existe
if (Test-Path $carpetaPublicacion) {
    Write-Host "ADVERTENCIA: La carpeta $carpetaPublicacion ya existe" -ForegroundColor Red
    $respuesta = Read-Host "Desea sobrescribirla? (S/N)"
    if ($respuesta -ne "S" -and $respuesta -ne "s") {
        Write-Host "Publicacion cancelada" -ForegroundColor Yellow
        exit
    }
    Remove-Item -Path $carpetaPublicacion -Recurse -Force
    Write-Host "Carpeta anterior eliminada" -ForegroundColor Green
}

# Crear carpeta de publicacion
Write-Host ""
Write-Host "Creando carpeta de publicacion..." -ForegroundColor Yellow
New-Item -ItemType Directory -Path $carpetaPublicacion -Force | Out-Null

# Cambiar al directorio del proyecto
Write-Host "Cambiando al directorio del proyecto..." -ForegroundColor Yellow
Set-Location $proyecto

# Limpiar compilaciones anteriores
Write-Host "Limpiando compilaciones anteriores..." -ForegroundColor Yellow
dotnet clean -c Release | Out-Null

# Publicar en modo Release
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  COMPILANDO Y PUBLICANDO..." -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$outputPath = "..\$carpetaPublicacion"
dotnet publish -c Release -o $outputPath --no-self-contained

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "ERROR: La publicacion fallo" -ForegroundColor Red
    Set-Location ..
    exit 1
}

# Volver al directorio raiz
Set-Location ..

# Verificar archivos generados
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  VERIFICANDO PUBLICACION" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$archivosEsenciales = @(
    "WebApplication1.dll",
    "WebApplication1.exe",
    "appsettings.json",
    "web.config"
)

$todosPresentes = $true
foreach ($archivo in $archivosEsenciales) {
    $rutaArchivo = Join-Path $carpetaPublicacion $archivo
    if (Test-Path $rutaArchivo) {
        Write-Host "OK - $archivo" -ForegroundColor Green
    } else {
        Write-Host "FALTA - $archivo" -ForegroundColor Red
        $todosPresentes = $false
    }
}

Write-Host ""

if ($todosPresentes) {
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  PUBLICACION EXITOSA" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host ""
    Write-Host "Archivos generados en: $carpetaPublicacion" -ForegroundColor Green
    Write-Host ""
    Write-Host "PROXIMOS PASOS:" -ForegroundColor Cyan
    Write-Host "1. Revisar appsettings.json (connection strings)" -ForegroundColor Yellow
    Write-Host "2. Copiar carpeta a servidor de produccion" -ForegroundColor Yellow
    Write-Host "3. Configurar en IIS" -ForegroundColor Yellow
    Write-Host "4. Probar endpoints" -ForegroundColor Yellow
    Write-Host ""
} else {
    Write-Host "========================================" -ForegroundColor Red
    Write-Host "  ADVERTENCIA: FALTAN ARCHIVOS" -ForegroundColor Red
    Write-Host "========================================" -ForegroundColor Red
    Write-Host ""
}

# Mostrar tamano de la publicacion
$tamano = (Get-ChildItem -Path $carpetaPublicacion -Recurse | Measure-Object -Property Length -Sum).Sum / 1MB
Write-Host "Tamano total: $([math]::Round($tamano, 2)) MB" -ForegroundColor Cyan
Write-Host ""
