# Guía de Distribución - Fila Virtual Cantina MVP

## Ejecutable Generado

El ejecutable de la aplicación se encuentra en:
```
bin\x64\Release\net8.0-windows10.0.19041.0\win10-x64\FilaVirtual.App.exe
```

## Cómo Distribuir

### Método Recomendado: Usar el archivo ZIP

El archivo **FilaVirtual-MVP-v1.0.zip** ya está listo para distribuir. Simplemente:

1. Comparte el archivo `FilaVirtual-MVP-v1.0.zip` 
2. El destinatario descomprime el archivo
3. Ejecuta `FilaVirtual.App.exe` desde la carpeta descomprimida

### Método Alternativo: Copiar la carpeta completa

También puedes copiar manualmente la carpeta de Release:

1. Navega a: `bin\x64\Release\net8.0-windows10.0.19041.0\win10-x64\`
2. Copia **toda la carpeta** `win10-x64` completa
3. La carpeta contiene:
   - `FilaVirtual.App.exe` (el ejecutable principal)
   - Todas las DLLs necesarias
   - Recursos (fuentes, imágenes, iconos)
   - `seed.json` (datos iniciales)

### Regenerar el archivo ZIP

Si necesitas crear un nuevo ZIP después de hacer cambios:

```powershell
powershell -Command "Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('bin\x64\Release\net8.0-windows10.0.19041.0\win10-x64', 'FilaVirtual-MVP-v1.0.zip')"
```

## Archivo Distribuible

📦 **FilaVirtual-MVP-v1.0.zip** (34.71 MB)

Este archivo contiene todo lo necesario para ejecutar la aplicación.

## Requisitos del Sistema

- **Sistema Operativo**: Windows 10 versión 19041 o superior (Windows 10 May 2020 Update)
- **Arquitectura**: 64-bit (x64)
- **Espacio en disco**: ~100 MB (descomprimido)
- **Permisos**: No requiere permisos de administrador

## Instalación en el Equipo Destino

1. Descomprime el ZIP (si usaste la Opción 2) o copia la carpeta completa
2. Ejecuta `FilaVirtual.App.exe`
3. La aplicación creará automáticamente la base de datos SQLite local en la primera ejecución

## Notas Importantes

- **No eliminar archivos**: Todos los archivos DLL y recursos son necesarios para el funcionamiento correcto
- **Base de datos**: Se creará automáticamente en `%LOCALAPPDATA%\FilaVirtual.App`
- **Datos iniciales**: El menú se carga desde `seed.json` incluido en la carpeta

## Estructura de Archivos Críticos

```
win10-x64/
├── FilaVirtual.App.exe         # Ejecutable principal
├── FilaVirtual.App.dll         # Biblioteca principal
├── seed.json                    # Datos iniciales del menú
├── OpenSans-*.ttf              # Fuentes
├── *.dll                        # Dependencias (MAUI, SQLite, etc.)
└── [múltiples carpetas de idiomas]
```

## Compilar Nueva Versión

Para generar un nuevo ejecutable después de hacer cambios:

```powershell
# Compilar y publicar en modo Release
dotnet build -c Release

# O publicar con optimizaciones
dotnet publish -c Release -p:PublishReadyToRun=true
```

## Solución de Problemas

### La aplicación no inicia
- Verifica que tienes Windows 10 versión 19041 o superior
- Asegúrate de que copiaste **todos** los archivos de la carpeta

### Error de base de datos
- Elimina la carpeta `%LOCALAPPDATA%\FilaVirtual.App` y ejecuta nuevamente

### Falta el menú
- Verifica que `seed.json` esté en la misma carpeta que el ejecutable

---

**Versión**: 1.0  
**Fecha**: Octubre 2025  
**Plataforma**: .NET MAUI 8.0 - Windows

