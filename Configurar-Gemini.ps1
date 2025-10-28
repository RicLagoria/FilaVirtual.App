# Script para configurar Gemini Pro en Fila Virtual App
# Ejecutar como administrador si es necesario

Write-Host "=== Configuración de Gemini Pro para Fila Virtual ===" -ForegroundColor Green
Write-Host "🎤 Funcionalidad: Pedidos por Voz con IA" -ForegroundColor Cyan
Write-Host "🤖 Motor: Google Gemini Pro" -ForegroundColor Cyan
Write-Host "💰 Costo: ~$0.025-0.045 por 1000 pedidos" -ForegroundColor Cyan

# Verificar si ya está configurado
$currentKey = $env:GEMINI_API_KEY
if ($currentKey) {
    Write-Host "✅ GEMINI_API_KEY ya está configurada" -ForegroundColor Green
    Write-Host "Clave actual: $($currentKey.Substring(0, 10))..." -ForegroundColor Yellow
    $response = Read-Host "¿Quieres configurar una nueva clave? (s/n)"
    if ($response -ne "s") {
        Write-Host "Configuración actual mantenida." -ForegroundColor Yellow
        exit
    }
}

# Solicitar API key
Write-Host "`n📋 Para obtener tu API key:" -ForegroundColor Cyan
Write-Host "1. Ve a https://aistudio.google.com/" -ForegroundColor White
Write-Host "2. Haz clic en 'Get API Key'" -ForegroundColor White
Write-Host "3. Crea una nueva clave" -ForegroundColor White
Write-Host "4. Copia la clave (formato: AIza...)" -ForegroundColor White

$apiKey = Read-Host "`n🔑 Ingresa tu API key de Gemini Pro"

# Validar formato de la clave
if (-not $apiKey -or -not $apiKey.StartsWith("AIza")) {
    Write-Host "❌ Formato de API key inválido. Debe empezar con 'AIza'" -ForegroundColor Red
    exit 1
}

# Configurar variable de entorno para la sesión actual
$env:GEMINI_API_KEY = $apiKey
Write-Host "✅ Variable configurada para esta sesión" -ForegroundColor Green

# Configurar variable de entorno permanente
try {
    [Environment]::SetEnvironmentVariable("GEMINI_API_KEY", $apiKey, "User")
    Write-Host "✅ Variable configurada permanentemente" -ForegroundColor Green
} catch {
    Write-Host "⚠️  No se pudo configurar permanentemente. Usa: setx GEMINI_API_KEY `"$apiKey`"" -ForegroundColor Yellow
}

# Mostrar información de costos
Write-Host "`n💰 Información de Costos:" -ForegroundColor Cyan
Write-Host "• Por pedido: ~$0.000025-0.000045" -ForegroundColor White
Write-Host "• 1000 pedidos/mes: ~$0.025-0.045" -ForegroundColor White
Write-Host "• Free tier: 15 requests/minuto, 1M tokens/día" -ForegroundColor White

Write-Host "`n🚀 ¡Configuración completada!" -ForegroundColor Green
Write-Host "Ahora puedes ejecutar la app y usar pedidos por voz con IA" -ForegroundColor White

# Pausa para que el usuario lea
Read-Host "`nPresiona Enter para continuar"

