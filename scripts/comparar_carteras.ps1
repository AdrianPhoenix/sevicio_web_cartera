# Script para comparar tablas entre carteras ClickOne y Web Produccion

$clickOne = "test_carteras/clickOne/Cartera.txt"
$webProd = "test_carteras/web_produccion/Cartera.txt"

Write-Host "=== COMPARACION DE CARTERAS ===" -ForegroundColor Cyan
Write-Host ""

# Extraer nombres de tablas de ClickOne
Write-Host "Extrayendo tablas de ClickOne..." -ForegroundColor Yellow
$tablasClickOne = Select-String -Path $clickOne -Pattern 'CREATE TABLE "(\w+)"' | ForEach-Object {
    if ($_ -match 'CREATE TABLE "(\w+)"') {
        $matches[1]
    }
} | Sort-Object -Unique

# Extraer nombres de tablas de Web Produccion
Write-Host "Extrayendo tablas de Web Produccion..." -ForegroundColor Yellow
$tablasWebProd = Select-String -Path $webProd -Pattern 'CREATE TABLE "(\w+)"' | ForEach-Object {
    if ($_ -match 'CREATE TABLE "(\w+)"') {
        $matches[1]
    }
} | Sort-Object -Unique

Write-Host ""
Write-Host "=== RESULTADOS ===" -ForegroundColor Cyan
Write-Host "Total tablas ClickOne: $($tablasClickOne.Count)" -ForegroundColor Green
Write-Host "Total tablas Web Produccion: $($tablasWebProd.Count)" -ForegroundColor Green
Write-Host ""

# Tablas que estan en ClickOne pero NO en Web Produccion
$faltantesEnWeb = $tablasClickOne | Where-Object { $_ -notin $tablasWebProd }
if ($faltantesEnWeb.Count -gt 0) {
    Write-Host "TABLAS FALTANTES EN WEB PRODUCCION ($($faltantesEnWeb.Count)):" -ForegroundColor Red
    $faltantesEnWeb | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    Write-Host ""
} else {
    Write-Host "OK - No hay tablas faltantes en Web Produccion" -ForegroundColor Green
    Write-Host ""
}

# Tablas que estan en Web Produccion pero NO en ClickOne
$extrasEnWeb = $tablasWebProd | Where-Object { $_ -notin $tablasClickOne }
if ($extrasEnWeb.Count -gt 0) {
    Write-Host "TABLAS EXTRAS EN WEB PRODUCCION ($($extrasEnWeb.Count)):" -ForegroundColor Yellow
    $extrasEnWeb | ForEach-Object { Write-Host "  - $_" -ForegroundColor Yellow }
    Write-Host ""
} else {
    Write-Host "OK - No hay tablas extras en Web Produccion" -ForegroundColor Green
    Write-Host ""
}

# Resumen final
Write-Host "=== RESUMEN ===" -ForegroundColor Cyan
if ($faltantesEnWeb.Count -eq 0 -and $extrasEnWeb.Count -eq 0) {
    Write-Host "PERFECTO: Ambas carteras tienen exactamente las mismas tablas" -ForegroundColor Green
} else {
    Write-Host "Hay diferencias entre las carteras" -ForegroundColor Yellow
}
