# Script de Compilación y Empaquetado - Fila Virtual MVP
# Genera el ejecutable en modo Release y crea el archivo ZIP para distribución

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Compilación Fila Virtual MVP" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Paso 1: Limpiar compilaciones anteriores
Write-Host "[1/4] Limpiando compilaciones anteriores..." -ForegroundColor Yellow
dotnet clean -c Release > $null 2>&1

# Paso 2: Compilar en modo Release
Write-Host "[2/4] Compilando proyecto en modo Release..." -ForegroundColor Yellow
dotnet build -c Release

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "❌ Error en la compilación. Revisa los errores arriba." -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✅ Compilación exitosa!" -ForegroundColor Green

# Paso 3: Publicar con optimizaciones
Write-Host "[3/4] Publicando con optimizaciones..." -ForegroundColor Yellow
dotnet publish -c Release -p:PublishReadyToRun=true > $null 2>&1

# Paso 4: Copiar archivo LEEME a la carpeta de distribución
Write-Host "[4/5] Copiando archivo LEEME..." -ForegroundColor Yellow

$sourcePath = "bin\x64\Release\net8.0-windows10.0.19041.0\win10-x64"

if (Test-Path "LEEME-DISTRIBUCION.txt") {
    Copy-Item "LEEME-DISTRIBUCION.txt" -Destination "$sourcePath\" -Force
}

# Paso 5: Crear archivo ZIP
Write-Host "[5/5] Creando archivo ZIP para distribución..." -ForegroundColor Yellow

$zipName = "FilaVirtual-MVP-v1.0.zip"

# Eliminar ZIP anterior si existe
if (Test-Path $zipName) {
    Remove-Item $zipName -Force
}

# Crear nuevo ZIP
Add-Type -A 'System.IO.Compression.FileSystem'
[IO.Compression.ZipFile]::CreateFromDirectory($sourcePath, $zipName)

# Obtener información del archivo
$zipFile = Get-Item $zipName
$sizeMB = [math]::Round($zipFile.Length / 1MB, 2)

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "  ✅ COMPILACIÓN COMPLETADA" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "📦 Archivo distribuible:" -ForegroundColor Cyan
Write-Host "   Nombre: $zipName" -ForegroundColor White
Write-Host "   Tamaño: $sizeMB MB" -ForegroundColor White
Write-Host "   Ruta completa: $($zipFile.FullName)" -ForegroundColor White
Write-Host ""
Write-Host "📁 Ejecutable directo en:" -ForegroundColor Cyan
Write-Host "   $sourcePath\FilaVirtual.App.exe" -ForegroundColor White
Write-Host ""
Write-Host "🎉 ¡Listo para distribuir!" -ForegroundColor Green
Write-Host ""

