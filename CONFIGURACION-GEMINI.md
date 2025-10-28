# Configuración de Gemini Pro para Pedidos por Voz

## Pasos para Configurar Gemini Pro

### 1. Obtener API Key
1. Ir a [Google AI Studio](https://aistudio.google.com/)
2. Hacer clic en "Get API Key"
3. Crear nueva clave
4. Copiar la clave (formato: `AIza...`)

### 2. Configurar Variable de Entorno

#### Opción A: Variable de Sistema (Recomendado)
```bash
# En PowerShell (Windows)
$env:GEMINI_API_KEY="AIzaSyDwDPIC-v27c8urAgEhFuTe_MODVE2NSzA"

# En CMD
set GEMINI_API_KEY=tu_clave_aqui
```

#### Opción B: En el Código (Solo para Testing)
```csharp
// En MauiProgram.cs (temporal)
Environment.SetEnvironmentVariable("GEMINI_API_KEY", "AIzaSyDwDPIC-v27c8urAgEhFuTe_MODVE2NSzA");
```

### 3. Verificar Configuración
La app detectará automáticamente si tienes la API key configurada:
- **Con API key:** Usa Gemini Pro (IA avanzada)
- **Sin API key:** Usa sistema simple (pattern matching)

## Costos de Gemini Pro

### Precios (Diciembre 2024)
- **Input:** $0.50 por 1M tokens
- **Output:** $1.50 por 1M tokens

### Estimación para tu App
- **Por pedido:** ~$0.000025-0.000045
- **1000 pedidos/mes:** ~$0.025-0.045
- **10,000 pedidos/mes:** ~$0.25-0.45

## Límites
- **Free tier:** 15 requests/minuto, 1M tokens/día
- **Pay-as-you-go:** Sin límite de requests

## Testing
1. Configurar API key
2. Ejecutar la app
3. Ir a Menú
4. Hacer clic en el micrófono
5. Decir: "quiero dos cafés y una medialuna"
6. Verificar que se agreguen al carrito

## Troubleshooting
- **Error "GEMINI_API_KEY no configurada":** Configurar variable de entorno
- **Error de conexión:** Verificar internet
- **No funciona:** La app usará el sistema simple como fallback

