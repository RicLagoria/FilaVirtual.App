# FilaVirtual.App

**Fila Virtual — Cantina Universitaria (MVP .NET MAUI Windows)**

Aplicación MVP para gestión de pedidos de cantina universitaria con soporte para pedidos por voz. Permite armar pedido, confirmar con QR, pago simulado, cola priorizada y notificaciones.

---

## 🎯 Características Principales

- **Catálogo de Menú**: SQLite con categorías (Café, Bebidas, Pastelería, Sándwiches)
- **Pedidos por Voz**: Reconocimiento de voz con Windows Speech Recognition API
- **Carrito**: Agregar/quitar ítems, ver total en es-AR
- **Confirmación con QR**: Genera orderId y muestra QR con payload
- **Pago Simulado**: Botón demo para marcar pedido como pagado
- **Cola Priorizada**: ACC > EMB > DOC > STD (dentro de cada nivel FIFO)
- **Estados**: En cola → En preparación → Listo
- **Vista Operador**: Gestión de pedidos por estado
- **Notificaciones**: Toast + sonido cuando el pedido está Listo
- **Accesibilidad**: Foco visible, AutomationProperties, soporte para lectores de pantalla

---

## 🏗️ Tecnologías

- **Plataforma**: .NET MAUI 8.0 (WinUI 3, solo Windows)
- **Patrón**: MVVM con CommunityToolkit.Mvvm
- **Persistencia**: SQLite (sqlite-net-pcl)
- **Reconocimiento de Voz**: System.Speech.Recognition
- **QR**: QRCoder
- **Notificaciones**: CommunityToolkit.Maui

---

## 📋 Requisitos

- **Windows 10/11** (versión 19041 o superior)
- **.NET 8 SDK**
- **Visual Studio 2022** (17.9+) con workload MAUI
- **Arquitectura**: x64

---

## 🚀 Instalación y Ejecución

### Clonar el Repositorio
```bash
git clone <url-del-repo>
cd FilaVirtual.App
```

### Instalar Workload (si es necesario)
```bash
dotnet workload install maui-windows
```

### Compilar y Ejecutar
```bash
# Compilar
dotnet build

# Ejecutar
dotnet run
```

### Desde Visual Studio
1. Abrir `FilaVirtual.App.sln`
2. Configurar plataforma a `x64`
3. Seleccionar perfil `Windows Machine`
4. Presionar `F5`

---

## 📁 Estructura del Proyecto

```
FilaVirtual.App/
├── Models/          # MenuItem, Order, OrderItem, Enums
├── ViewModels/      # MVVM (Menu, Cart, OrderStatus, Operator)
├── Views/           # XAML (MenuPage, CartPage, etc.)
├── Services/        # IStorage, IMenuService, IOrderService, 
│                    # IQueueService, ISpeechRecognitionService, etc.
├── Data/            # seed.json (datos iniciales)
├── Resources/       # Estilos, fuentes, imágenes
└── Platforms/       # Windows-specific config
```

---

## 🎤 Pedidos por Voz

La aplicación incluye reconocimiento de voz integrado:

- **Activar/desactivar** micrófono con un botón
- **Detecta automáticamente** el idioma disponible (es-AR, es-ES, es-MX, en-US)
- **Reconoce productos** mencionados en voz y los agrega al carrito
- **Feedback visual** del estado (Inactivo, Escuchando, Error)

### Ejemplo de Uso
1. Ir a la página de Menú
2. Hacer clic en "Activar Micrófono"
3. Decir: "quiero un café y una medialuna"
4. Los productos se agregan automáticamente al carrito

Ver `SPRINT-5-PEDIDOS-VOZ.md` para más detalles.

---

## 📦 Distribución

### Generar Ejecutable
```powershell
# Script automatizado
.\Compilar-Release.ps1

# O manualmente
dotnet build -c Release
```

### Ubicación del Ejecutable
```
bin\x64\Release\net8.0-windows10.0.19041.0\win10-x64\FilaVirtual.App.exe
```

### Paquete Distribuible
El archivo `FilaVirtual-MVP-v1.0.zip` contiene todo lo necesario para ejecutar la aplicación.

Ver `DISTRIBUCION.md` para más detalles.

---

## 🌎 Configuración (es-AR)

La aplicación usa cultura `es-AR` por defecto para:
- Formato de precios: `$ 1.200` (con punto como separador de miles)
- Formato de fechas y números
- Mensajes de interfaz

---

## 🧪 Pruebas Manuales

### Positivas
- **P1**: Confirmar pedido con carrito válido crea Order + OrderItems
- **P2**: "Marcar pagado (demo)" cambia estado a En preparación
- **P3**: "Listo" en vista Operador → notificación + estado Listo

### Negativas
- **N1**: Carrito vacío → Confirmar bloqueado + mensaje
- **N2**: Producto inexistente en carrito → ignorar y notificar
- **N3**: Error en reconocimiento de voz → feedback visual de error

---

## 🛠️ Comandos Útiles

```bash
# Limpiar
dotnet clean

# Restaurar paquetes
dotnet restore

# Compilar Release
dotnet build -c Release

# Ver workloads instalados
dotnet workload list
```

---

## 🐛 Solución de Problemas

### Error: "No recognizer found"
**Solución**: Asegurar que Windows tenga instalado un paquete de reconocimiento de voz (al menos en-US está disponible por defecto)

### Error: "Plataforma no válida"
**Solución**: Cambiar plataforma a `x64` en Visual Studio (Build > Configuration Manager)

### La aplicación no inicia
**Solución**: Limpiar y reconstruir
```bash
dotnet clean
dotnet build
```

---

## 🔐 Seguridad

⚠️ **IMPORTANTE:** Esta aplicación usa la API de Google Gemini Pro para reconocimiento de voz avanzado. La clave API debe ser protegida.

### Configuración Segura

```bash
# Opción 1: User Secrets (recomendado para desarrollo)
dotnet user-secrets set "GeminiApiKey" "TU_CLAVE_AQUI"

# Opción 2: Variable de entorno
$env:GEMINI_API_KEY="TU_CLAVE_AQUI"
```

**⚠️ NUNCA hagas commit de claves API en el código.**

Ver `SEGURIDAD.md` para más detalles sobre gestión de secretos.

---

## 📚 Documentación Adicional

- `SEGURIDAD.md` - **⭐ Guía de gestión segura de claves API**
- `SPRINT-5-PEDIDOS-VOZ.md` - Documentación del Sprint de Pedidos por Voz
- `DISTRIBUCION.md` - Guía de distribución y empaquetado
- `VS2022-SETUP.md` - Configuración para Visual Studio 2022
- `CONFIGURACION-GEMINI.md` - Configuración de Gemini Pro para pedidos por voz

---

## 📝 Licencia

Este es un proyecto MVP para fines educativos.
