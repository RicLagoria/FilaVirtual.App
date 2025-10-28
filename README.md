# FilaVirtual.App

Fila Virtual — Cantina Universitaria (MVP .NET MAUI Windows)

MVP de una fila virtual para cantina universitaria, orientado a escritorio (Windows/WinUI 3) con .NET MAUI.
Permite armar pedido, confirmar (QR demo), pago simulado, cola con prioridad (ACC > EMB > DOC > STD), vista de operador y notificación cuando el pedido está Listo.
Idiomas/formatos: es-AR (precios y cultura).

🧭 Índice

Características (SRS resumido)

Arquitectura y stack

Estructura del repo

Requisitos

Primer arranque (Windows)

Configuración de cultura es-AR

Datos de ejemplo (seed)

Flujo demo (criterios de aceptación)

Pruebas mínimas (P/N)

Accesibilidad (PoC)

Comandos útiles

Solución de problemas

Roadmap corto

Trabajo con Cursor AI

Licencia

✅ Características (SRS resumido)

Catálogo local (SQLite) con categorías: Café, Bebidas, Pastelería, Sándwiches.

Carrito: agregar/quitar ítems, ver total en es-AR.

Confirmar pedido: genera orderId, sound (opcional) y muestra QR (payload: {orderId,total,timestamp}).

Pago simulado (demo): botón “Marcar como pagado” → pasa a En preparación.

Cola priorizada: ACC > EMB > DOC > STD; dentro de cada prioridad FIFO por createdAt.

Estados: En cola / En preparación / Listo.

Operador (vista oculta): listar por estado, acciones Preparar y Listo.

Notificación al quedar Listo (toast + sonido opcional).

Accesibilidad base: foco visible, AutomationProperties, SemanticScreenReader.Announce.

🏗 Arquitectura y stack

Plataforma: .NET MAUI (WinUI 3, solo Windows).

Patrón: MVVM (CommunityToolkit.Mvvm).

Persistencia: SQLite (sqlite-net-pcl), seed inicial.

Servicios: IMenuService, IOrderService, IQueueService, INotificationService, IAccessibilityService, IStorage.

UI: XAML + Shell.

Notificaciones: CommunityToolkit.Maui (toast).

QR: QRCoder.

Sonido (opcional): Plugin.Maui.Audio.

📁 Estructura del repo
FilaVirtual.App/
  FilaVirtual.App.sln
  FilaVirtual.App/                   # proyecto MAUI
    App.xaml / App.xaml.cs
    AppShell.xaml
    Models/
      MenuItem.cs
      Order.cs
      OrderItem.cs
      Enums.cs                       # Estados {EnCola, EnPrep, Listo}; Prioridad {ACC, EMB, DOC, STD}
    ViewModels/
      HomeVM.cs
      MenuVM.cs
      CartVM.cs
      OrderStatusVM.cs
      OperatorVM.cs
    Views/
      HomePage.xaml
      MenuPage.xaml
      CartPage.xaml
      OrderStatusPage.xaml
      OperatorPage.xaml
      SettingsPage.xaml
    Services/
      IMenuService.cs / LocalMenuService.cs
      IOrderService.cs / LocalOrderService.cs
      IQueueService.cs / LocalQueueService.cs
      INotificationService.cs / LocalNotificationService.cs
      IAccessibilityService.cs / AccessibilityService.cs
      IStorage.cs / SQLiteStorage.cs
    Data/
      seed.json
    Resources/
      Styles/Theme.xaml
    Platforms/
      Windows/ ...                   # WinUI 3

🧩 Requisitos

Windows 11

Visual Studio 2022 (17.9 o superior) con workload .NET Multi-platform App UI

.NET 8 SDK

Paquetes NuGet:

CommunityToolkit.Mvvm

sqlite-net-pcl

CommunityToolkit.Maui

QRCoder

(opcional) Plugin.Maui.Audio

TargetFramework del proyecto: net8.0-windows10.0.19041.0 (solo Windows).

🚀 Primer arranque (Windows)

Clonar el repositorio

git clone https://github.com/<usuario>/<repo>.git
cd <repo>/FilaVirtual.App


(Solo si hace falta) Instalar workload

dotnet workload install maui


Asegurar target Windows en .csproj

<TargetFramework>net8.0-windows10.0.19041.0</TargetFramework>


Restaurar y compilar

Desde Visual Studio: seleccionar Windows Machine → Build → Start.

Por CLI:

dotnet build
dotnet build -t:Run -f net8.0-windows10.0.19041.0

🌎 Configuración de cultura es-AR

En App.xaml.cs:

using System.Globalization;

var culture = new CultureInfo("es-AR");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;


Para mostrar precios: precio.ToString("C").

🍽 Datos de ejemplo (seed)

Data/seed.json (muestra):

{
  "menu": [
    {"id": 1, "categoria": "Café", "nombre": "Espresso", "precio": 900},
    {"id": 2, "categoria": "Café", "nombre": "Café con Leche", "precio": 1200},
    {"id": 3, "categoria": "Café", "nombre": "Capuccino", "precio": 1300},
    {"id": 4, "categoria": "Bebidas", "nombre": "Agua 500 ml", "precio": 700},
    {"id": 5, "categoria": "Bebidas", "nombre": "Jugo de frutas", "precio": 1100},
    {"id": 6, "categoria": "Pastelería", "nombre": "Medialuna", "precio": 500},
    {"id": 7, "categoria": "Pastelería", "nombre": "Tostada", "precio": 900},
    {"id": 8, "categoria": "Sándwiches", "nombre": "JyQ", "precio": 2200}
  ]
}


La app carga el seed al primer arranque si la DB está vacía.

🎬 Flujo demo (criterios de aceptación)

Menú → agregar 1+ ítems → Carrito (ver total).

Confirmar → genera orderId, muestra QR, dispara toast/sonido.

Marcar como pagado (demo) → el pedido pasa a En preparación.

Operador (vista oculta/switch) → ver listas En cola/En preparación, marcar Listo.

Cliente: en Mi pedido, estado Listo + toast/sonido.

🧪 Pruebas mínimas (P/N)

Positivas

P1: Confirmar con carrito válido crea Order + OrderItems.

P2: “Marcar pagado (demo)” cambia a En preparación.

P3: “Listo” en Operador → toast (y sonido si está habilitado) + estado Listo en cliente.

Negativas

N1: Carrito vacío → bloquear Confirmar + mensaje accesible.

N2: MenuItem inexistente en carrito → ignorar y notificar.

N3: orderId duplicado (muy improbable) → reintentar generación + log.

♿ Accesibilidad (PoC)

Navegación Tab/Shift+Tab/Enter/Esc (WinUI).

Foco visible en todos los controles (estilos).

AutomationProperties.Name/HelpText en botones clave (Confirmar, Marcar pagado, Preparar, Listo).

SemanticScreenReader.Announce(...) en Confirmar y al pasar a Listo.

Tamaños mínimos 44×44 pt y contraste AA.

🛠 Comandos útiles
# Compilar y ejecutar en Windows
dotnet build -t:Run -f net8.0-windows10.0.19041.0

# Compilar en modo Release
dotnet build -c Release

# Generar ejecutable para distribución
.\Compilar-Release.ps1

# Agregar paquetes
dotnet add package CommunityToolkit.Mvvm
dotnet add package sqlite-net-pcl
dotnet add package CommunityToolkit.Maui
dotnet add package QRCoder
dotnet add package Plugin.Maui.Audio

# Ver workloads instalados
dotnet workload list

📦 Distribución y Exportación

Para crear un ejecutable distribuible del MVP:

1. **Método Rápido** - Usar el script automatizado:
   ```powershell
   .\Compilar-Release.ps1
   ```
   Esto generará `FilaVirtual-MVP-v1.0.zip` (~35 MB) listo para distribuir.

2. **Método Manual**:
   ```powershell
   # Compilar en Release
   dotnet build -c Release
   
   # Crear ZIP
   powershell -Command "Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('bin\x64\Release\net8.0-windows10.0.19041.0\win10-x64', 'FilaVirtual-MVP-v1.0.zip')"
   ```

3. **Archivos generados**:
   - 📦 `FilaVirtual-MVP-v1.0.zip` - Paquete completo para distribución
   - 📄 `DISTRIBUCION.md` - Guía completa de distribución
   - 📄 `LEEME-DISTRIBUCION.txt` - Instrucciones para el usuario final (incluido en el ZIP)

**Contenido del paquete distribuible:**
- `FilaVirtual.App.exe` - Ejecutable principal
- Todas las dependencias (DLLs)
- Recursos (fuentes, imágenes, iconos)
- `seed.json` - Datos iniciales
- `LEEME-DISTRIBUCION.txt` - Instrucciones de uso

**Requisitos del sistema destino:**
- Windows 10 versión 19041 o superior
- Arquitectura x64
- ~100 MB espacio en disco
- No requiere instalación ni permisos de administrador

Ver `DISTRIBUCION.md` para más detalles.

🧯 Solución de problemas

NETSDK1202 / EOL de iOS/MacCatalyst

Causa: el proyecto estaba multi-target (net8.0-ios, net8.0-maccatalyst).

Fix: dejar solo Windows en el .csproj:

<TargetFramework>net8.0-windows10.0.19041.0</TargetFramework>


Luego:

dotnet clean
dotnet build -f net8.0-windows10.0.19041.0


No suena el audio

Asegurar Plugin.Maui.Audio instalado y un ding.mp3 en recursos de app.

Verificar que el archivo se copie al paquete (Build Action / MauiAsset).

No se ve el QR

Confirmar QRCoder agregado y que se use ImageSource.FromStream(() => new MemoryStream(bytes)).

🗺 Roadmap corto

 Búsqueda por teclado de productos en el menú (filtrado en tiempo real).

 Settings: alternar Texto grande / Alto contraste.

 Persistencia de preferencia (rol Operador/Cliente).

 Logs mínimos (archivos) para auditoría de estados.

 Refactor para separar UI/servicios y facilitar portar a Android luego.

 Tests de servicios (cola/prioridad) con proyectos de prueba.


 🤖 Trabajo con Cursor AI

Archivo .cursorrules sugerido (breve):
    Eres el agente del proyecto "Fila Virtual Cantina" (MVP .NET MAUI Windows).
Objetivos: Menú/Carrito/Confirmar (QR), Pago demo, Cola priorizada (ACC>EMB>DOC>STD), Estados EnCola/EnPrep/Listo, Operador oculto, Accesibilidad mínima, Notificación (toast/sonido).
Arquitectura: MVVM + Services + SQLite (sqlite-net-pcl). Idioma: es-AR. Código y comentarios en español.
Mantener estructura de carpetas. No agregar dependencias innecesarias.
Definition of Done: compila, navega, cumple aceptación y commit con mensaje claro.


Prompts de sprints (copiar en Agent):

Sprint 1 — Seed + menú

Implementar SQLite + seed.json + IMenuService/LocalMenuService + MenuPage/MenuVM (listar categorías e ítems). Cultura es-AR. Compila y corre.


Sprint 2 — Carrito + Confirmar (QR)

Agregar CartVM/CartPage con total. Confirmar → orderId + payload QR y render con QRCoder (preview). No persistir aún.


Sprint 3 — Persistencia de Order + Pago demo

Persistir Order/OrderItems en SQLite al confirmar. Botón “Marcar pagado (demo)” → En preparación. OrderStatusPage muestra estado.


Sprint 4 — Cola + Operador + Notificación

IQueueService/LocalQueueService con prioridad ACC>EMB>DOC>STD + FIFO. OperatorPage con Preparar/Listo. En cliente, toast + sonido al quedar Listo.
