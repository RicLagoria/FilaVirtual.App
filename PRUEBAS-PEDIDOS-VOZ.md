# Pruebas de Pedidos por Voz con Gemini Pro

## Configuración Inicial

### 1. Configurar API Key
```powershell
# Ejecutar script de configuración
.\Configurar-Gemini.ps1

# O configurar manualmente
$env:GEMINI_API_KEY="tu_clave_gemini_aqui"
```

### 2. Compilar y Ejecutar
```bash
dotnet build
dotnet run
```

## Casos de Prueba

### **P1: Activación del Micrófono**
**Objetivo:** Verificar que el micrófono se active correctamente

**Pasos:**
1. Abrir la app
2. Ir a la página de Menú
3. Hacer clic en el botón "Activar Micrófono"

**Resultado Esperado:**
- ✅ Botón cambia a "Detener Micrófono"
- ✅ Estado muestra "Escuchando" en verde
- ✅ Texto muestra "🎤 Escuchando... di tu pedido"
- ✅ Se reproduce sonido de inicio

### **P2: Pedido Simple**
**Objetivo:** Verificar pedido básico con un producto

**Pasos:**
1. Activar micrófono
2. Decir: "quiero un café"
3. Esperar procesamiento

**Resultado Esperado:**
- ✅ Texto muestra "Escuché: 'quiero un café'"
- ✅ Se agrega 1x "Café con Leche" al carrito
- ✅ Texto muestra "✅ 1 producto(s) agregados al carrito"
- ✅ Se reproduce sonido de confirmación
- ✅ Aparece alert con el producto agregado

### **P3: Pedido Múltiple**
**Objetivo:** Verificar pedido con varios productos

**Pasos:**
1. Activar micrófono
2. Decir: "quiero dos cafés y una medialuna"
3. Esperar procesamiento

**Resultado Esperado:**
- ✅ Se agregan 2x "Café con Leche" y 1x "Medialuna"
- ✅ Texto muestra "✅ 3 producto(s) agregados al carrito"
- ✅ Se reproduce sonido de confirmación

### **P4: Pedido con Cantidades**
**Objetivo:** Verificar reconocimiento de cantidades

**Pasos:**
1. Activar micrófono
2. Decir: "tres medialunas y dos aguas"
3. Esperar procesamiento

**Resultado Esperado:**
- ✅ Se agregan 3x "Medialuna" y 2x "Agua 500 ml"
- ✅ Cantidades correctas en el carrito

### **P5: Producto No Encontrado**
**Objetivo:** Verificar manejo de productos inexistentes

**Pasos:**
1. Activar micrófono
2. Decir: "quiero una pizza"
3. Esperar procesamiento

**Resultado Esperado:**
- ✅ Texto muestra "❌ No encontré ese producto en el menú"
- ✅ Se reproduce sonido de error
- ✅ No se agrega nada al carrito

### **P6: Error de Conexión**
**Objetivo:** Verificar fallback cuando Gemini falla

**Pasos:**
1. Desconectar internet
2. Activar micrófono
3. Decir: "quiero un café"
4. Esperar procesamiento

**Resultado Esperado:**
- ✅ Usa sistema simple como fallback
- ✅ Funciona sin conexión a internet

### **P7: Desactivación del Micrófono**
**Objetivo:** Verificar que se pueda detener el micrófono

**Pasos:**
1. Activar micrófono
2. Hacer clic en "Detener Micrófono"

**Resultado Esperado:**
- ✅ Botón cambia a "Activar Micrófono"
- ✅ Estado muestra "Micrófono detenido" en gris
- ✅ Texto se limpia

## Productos Disponibles para Pruebas

### **Bebidas Calientes**
- "espresso" → Espresso
- "café" o "café con leche" → Café con Leche
- "capuccino" → Capuccino

### **Bebidas Frías**
- "agua" → Agua 500 ml
- "jugo" → Jugo de frutas

### **Comida**
- "medialuna" → Medialuna
- "tostada" → Tostada
- "jamón y queso" o "sandwich" → JyQ

## Comandos de Voz Sugeridos

### **Simples**
- "quiero un café"
- "dame una medialuna"
- "pido agua"

### **Con Cantidades**
- "dos cafés"
- "tres medialunas"
- "una agua y dos jugos"

### **Múltiples**
- "quiero un café y una medialuna"
- "dame dos aguas y un sandwich"
- "pido capuccino, medialuna y jugo"

## Troubleshooting

### **Error: "GEMINI_API_KEY no configurada"**
- Verificar que la variable de entorno esté configurada
- Reiniciar la terminal/IDE
- Ejecutar `echo $env:GEMINI_API_KEY` en PowerShell

### **Error: "No se pudieron parsear items"**
- Verificar conexión a internet
- Revisar logs en Debug Output
- El sistema usará fallback automáticamente

### **No se reproduce sonido**
- Verificar que el volumen esté activado
- Verificar permisos de audio en Windows
- Revisar logs para errores de audio

### **Micrófono no funciona**
- Verificar permisos de micrófono
- Verificar que no esté siendo usado por otra app
- Probar en modo VM si es necesario

## Logs de Debug

Para ver logs detallados:
1. Abrir Output en Visual Studio
2. Seleccionar "Debug" en el dropdown
3. Buscar mensajes que empiecen con:
   - `[Gemini]` - Procesamiento con IA
   - `[Audio]` - Reproducción de sonidos
   - `[MenuVM]` - Lógica del ViewModel
   - `[Speech]` - Reconocimiento de voz
