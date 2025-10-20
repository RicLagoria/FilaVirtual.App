# Configuración para Visual Studio 2022

## 📋 Requisitos Previos

### Software Necesario
1. **Visual Studio 2022** (versión 17.0 o superior)
2. **Carga de trabajo requerida**: 
   - `.NET Multi-platform App UI development` (MAUI)
   - Incluye automáticamente:
     - .NET 8.0 SDK
     - Windows App SDK
     - WinUI 3

### Verificar Instalación
```powershell
# Verificar .NET 8.0
dotnet --version
# Debe mostrar: 8.0.x o superior

# Verificar workloads de MAUI
dotnet workload list
# Debe incluir: maui-windows
```

## 🚀 Abrir el Proyecto en VS2022

### Opción 1: Abrir Solución
1. Doble clic en `FilaVirtual.App.sln`
2. Visual Studio 2022 se abrirá automáticamente

### Opción 2: Desde Visual Studio
1. Abrir Visual Studio 2022
2. `File` > `Open` > `Project/Solution`
3. Seleccionar `FilaVirtual.App.sln`

## ⚙️ Configuración del Proyecto

### Plataforma y Configuración
- **Plataforma**: `x64` (obligatorio)
- **Configuración**: `Debug` o `Release`
- **Framework**: `net8.0-windows10.0.19041.0`

### Configurar la Plataforma
1. En la barra de herramientas superior, buscar el selector de plataforma
2. Cambiar de `Any CPU` a `x64`
3. Si no aparece `x64`:
   - `Build` > `Configuration Manager`
   - En `Active solution platform`, seleccionar `x64`

## 🔧 Compilar el Proyecto

### Desde Visual Studio
1. **Compilar**: `Build` > `Build Solution` (Ctrl+Shift+B)
2. **Limpiar**: `Build` > `Clean Solution`
3. **Reconstruir**: `Build` > `Rebuild Solution`

### Desde Terminal Integrada
```powershell
# Limpiar
dotnet clean

# Compilar
dotnet build

# Ejecutar
dotnet run
```

## ▶️ Ejecutar el Proyecto

### Modo Debug (Recomendado)
1. Seleccionar perfil: `Windows (Unpackaged)` o `Windows Machine`
2. Presionar `F5` o clic en el botón verde `▶ Windows Machine`

### Modo sin Debug
1. Presionar `Ctrl+F5`
2. O ejecutar: `dotnet run`

### Ejecutable Directo
```powershell
# Ubicación del ejecutable
.\bin\x64\Debug\net8.0-windows10.0.19041.0\win10-x64\FilaVirtual.App.exe
```

## 🐛 Debugging

### Breakpoints
- Funcionan normalmente en código C#
- No funcionan en XAML (usar Live Visual Tree)

### Hot Reload
- **XAML Hot Reload**: Habilitado por defecto
  - Cambios en XAML se reflejan automáticamente
- **C# Hot Reload**: Disponible en .NET 8
  - Funciona para cambios menores en código

### Herramientas de Debug
- **Live Visual Tree**: `Debug` > `Windows` > `Live Visual Tree`
- **Live Property Explorer**: Ver propiedades en tiempo real
- **Output Window**: Ver logs de Debug.WriteLine

## 📦 Dependencias NuGet

El proyecto usa las siguientes dependencias:
```xml
- Microsoft.Maui.Controls
- Microsoft.Maui.Controls.Compatibility
- Microsoft.Extensions.Logging.Debug
- CommunityToolkit.Mvvm (8.2.2)
- sqlite-net-pcl (1.9.172)
- SQLitePCLRaw.bundle_green (2.1.8)
- QRCoder (1.4.3)
```

### Restaurar Paquetes
```powershell
dotnet restore
```

O desde VS2022:
- `Tools` > `NuGet Package Manager` > `Manage NuGet Packages for Solution`
- Clic en `Restore`

## ⚠️ Problemas Comunes

### Error: "No se puede encontrar el proyecto"
**Solución**: Verificar que la plataforma sea `x64`, no `Any CPU`

### Error: "WindowsAppSDK not found"
**Solución**: 
1. Instalar workload de MAUI:
```powershell
dotnet workload install maui-windows
```

### Error: "SQLite initialization failed"
**Solución**: Ya está resuelto con `SQLitePCL.Batteries_V2.Init()` en `MauiProgram.cs`

### Error de compilación: "Plataforma no válida"
**Solución**:
1. `Build` > `Configuration Manager`
2. Verificar que el proyecto esté configurado para `x64`

### La aplicación no inicia
**Solución**:
1. Limpiar solución: `Build` > `Clean Solution`
2. Eliminar carpetas `bin` y `obj` manualmente
3. Reconstruir: `Build` > `Rebuild Solution`

## 🔍 Estructura del Proyecto

```
FilaVirtual.App/
├── Converters/          # Convertidores XAML
├── Data/                # Datos de seed (JSON)
├── Models/              # Modelos de datos
├── Platforms/           
│   └── Windows/         # Configuración específica de Windows
├── Properties/          # Configuración de lanzamiento
├── Resources/           # Recursos (fuentes, imágenes, estilos)
├── Services/            # Servicios (Storage, Menu, Notificaciones)
├── ViewModels/          # ViewModels (MVVM)
└── Views/               # Vistas XAML
```

## 📝 Notas Importantes

1. **Solo Windows**: Este MVP está configurado solo para Windows
2. **Modo Unpackaged**: La app se ejecuta sin empaquetado MSIX
3. **x64 Obligatorio**: No funciona con Any CPU o x86
4. **SQLite**: Base de datos local para persistencia
5. **MVVM**: Patrón con CommunityToolkit.Mvvm

## 🆘 Soporte

Si encuentras problemas:
1. Verificar que VS2022 esté actualizado
2. Verificar que el workload de MAUI esté instalado
3. Limpiar y reconstruir la solución
4. Verificar que la plataforma sea `x64`

## 📚 Referencias

- [.NET MAUI Documentation](https://learn.microsoft.com/es-es/dotnet/maui/)
- [Visual Studio 2022 MAUI](https://learn.microsoft.com/es-es/dotnet/maui/get-started/installation)
- [Windows App SDK](https://learn.microsoft.com/es-es/windows/apps/windows-app-sdk/)

