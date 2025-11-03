# Guía de Seguridad - FilaVirtual.App

## 🔐 Gestión de Claves API (Gemini Pro)

### ¿Por qué es Importante?

Las claves API son credenciales sensibles que **NUNCA** deben ser:
- Hardcodeadas en el código fuente
- Incluidas en archivos de configuración versionados
- Compartidas en repositorios públicos
- Publicadas en la documentación

### Métodos Recomendados

#### 1. User Secrets (Desarrollo Local) ⭐ RECOMENDADO

User Secrets es la forma más segura de gestionar secretos durante el desarrollo:

```bash
# Inicializar (solo una vez)
dotnet user-secrets init

# Guardar la API key
dotnet user-secrets set "GeminiApiKey" "AIzaSy..."

# Verificar secretos guardados
dotnet user-secrets list

# Eliminar un secreto
dotnet user-secrets remove "GeminiApiKey"

# Limpiar todos los secretos
dotnet user-secrets clear
```

**Ubicación de los secretos:**
- Windows: `%APPDATA%\Microsoft\UserSecrets\<user-secrets-id>\secrets.json`
- Linux/Mac: `~/.microsoft/usersecrets/<user-secrets-id>/secrets.json`

**Ventajas:**
✅ Fuera del repositorio (no se hace commit)
✅ Específico por proyecto
✅ Fácil de usar
✅ Compatible con .NET

#### 2. Variables de Entorno (Producción)

Para entornos de producción o CI/CD:

```powershell
# Sesión actual (temporal)
$env:GEMINI_API_KEY="TU_CLAVE_AQUI"

# Permanente (requiere reiniciar terminal)
[Environment]::SetEnvironmentVariable("GEMINI_API_KEY", "TU_CLAVE_AQUI", "User")

# Verificar
echo $env:GEMINI_API_KEY
```

**Ventajas:**
✅ Compatible con todos los sistemas
✅ Fácil de configurar en servidores/CI/CD
✅ No requiere código adicional

#### 3. Azure Key Vault (Producción Enterprise)

Para aplicaciones en producción con requisitos de seguridad avanzados:

```csharp
// Conectar con Azure Key Vault
var client = new SecretClient(
    new Uri("https://tu-keyvault.vault.azure.net/"),
    new DefaultAzureCredential()
);

// Obtener secreto
KeyVaultSecret secret = await client.GetSecretAsync("GeminiApiKey");
string apiKey = secret.Value;
```

**Ventajas:**
✅ Máxima seguridad
✅ Auditoría integrada
✅ Rotación automática de claves
✅ Control de acceso granular

### Usar la API Key en el Código

```csharp
// En tu servicio
public class GeminiSpeechService
{
    private readonly string _apiKey;

    public GeminiSpeechService(IConfiguration configuration)
    {
        // Prioridad: User Secrets > Variables de Entorno > App Settings
        _apiKey = configuration["GeminiApiKey"] 
                  ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY")
                  ?? throw new InvalidOperationException("API Key no configurada");
    }
}
```

### Script Automatizado

Usa el script `Configurar-Gemini.ps1` que configurará automáticamente:
1. User Secrets (desarrollo)
2. Variable de entorno (sesión)
3. Variable de entorno permanente (opcional)

```powershell
.\Configurar-Gemini.ps1
```

## 🚨 Qué NO Hacer

### ❌ NUNCA Hardcodear Claves

```csharp
// ❌ MAL - Clave expuesta en el código
var apiKey = "AIzaSy-EJEMPLO-ESTO-NO-ES-UNA-CLAVE-REAL";

// ✅ BIEN - Obtener de configuración segura
var apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
```

### ❌ NUNCA Incluir en appsettings.json

```json
// ❌ MAL - appsettings.json está versionado
{
  "GeminiApiKey": "AIzaSy..."
}

// ✅ BIEN - Usar appsettings.Development.json (en .gitignore)
// o mejor aún, User Secrets
```

### ❌ NUNCA Hacer Commit de Secretos

Si accidentalmente hiciste commit de una clave:

```bash
# 1. Revocar la clave inmediatamente en Google AI Studio
# 2. Generar una nueva clave
# 3. Limpiar el historial de Git (contactar a un senior)
```

## 🔍 Verificar Seguridad

### Escanear Repositorio

```bash
# Buscar posibles claves expuestas
git grep -i "AIza"
git grep -i "api.*key"
git grep -i "secret"
```

### Antes de Cada Commit

```bash
# Verificar qué se va a commitear
git diff --staged

# Revisar archivo por archivo
git status
```

## 📋 Checklist de Seguridad

Antes de hacer push:

- [ ] No hay claves API en el código
- [ ] No hay claves API en archivos de configuración versionados
- [ ] `.gitignore` incluye archivos sensibles
- [ ] User Secrets configurado correctamente
- [ ] Variables de entorno documentadas (sin valores reales)
- [ ] README actualizado sin claves expuestas

## 🔄 Rotación de Claves

### Cada 90 días (recomendado):

1. Generar nueva clave en Google AI Studio
2. Actualizar User Secrets: `dotnet user-secrets set "GeminiApiKey" "NUEVA_CLAVE"`
3. Actualizar variables de entorno en servidores
4. Verificar que la aplicación funcione
5. Revocar clave antigua

## 📞 En Caso de Exposición

Si una clave se expuso públicamente:

1. **INMEDIATO:** Revocar la clave en [Google AI Studio](https://aistudio.google.com/)
2. Generar nueva clave
3. Actualizar en todos los entornos
4. Revisar logs para uso no autorizado
5. Notificar al equipo

## 📚 Referencias

- [.NET User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets)
- [Azure Key Vault](https://azure.microsoft.com/services/key-vault/)
- [Google AI Studio](https://aistudio.google.com/)
- [OWASP Secrets Management](https://owasp.org/www-community/vulnerabilities/Use_of_hard-coded_password)

---

**Última actualización:** Noviembre 2025

