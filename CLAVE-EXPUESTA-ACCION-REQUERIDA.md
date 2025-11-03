# ⚠️ ACCIÓN REQUERIDA: Clave API Expuesta

## 🚨 Problema Detectado

Se encontró una clave API de Google Gemini Pro expuesta en:
- **Archivo:** `CONFIGURACION-GEMINI.md`
- **Commit:** `25f2cad8cc323c8970edcb16e18f23e3e25795b2`
- **Clave parcial:** `AIzaSyDwDPIC...`

Esta clave está ahora en el historial de Git y potencialmente comprometida.

## 🔥 Acciones Inmediatas (URGENTE)

### 1. Revocar la Clave Expuesta AHORA

1. Ve a [Google AI Studio](https://aistudio.google.com/app/apikey)
2. Busca la clave que termina en `...MODVE2NSzA`
3. Haz clic en el ícono de eliminar (🗑️)
4. Confirma la revocación

### 2. Generar Nueva Clave

1. En la misma página, haz clic en "Create API Key"
2. Selecciona el proyecto
3. Copia la nueva clave
4. **NO la pegues en ningún archivo del repositorio**

### 3. Configurar la Nueva Clave de Forma Segura

```bash
# Usa User Secrets (recomendado)
dotnet user-secrets set "GeminiApiKey" "TU_NUEVA_CLAVE"

# O el script automatizado
.\Configurar-Gemini.ps1
```

### 4. Limpiar el Historial de Git (Opcional pero Recomendado)

⚠️ **Advertencia:** Esto reescribirá el historial de Git. Si trabajas en equipo, coordina con tu equipo primero.

```bash
# Opción 1: BFG Repo-Cleaner (más rápido)
# Descargar BFG de https://rtyley.github.io/bfg-repo-cleaner/
java -jar bfg.jar --replace-text passwords.txt

# Opción 2: git filter-branch (manual)
git filter-branch --tree-filter 'find . -name "CONFIGURACION-GEMINI.md" -exec sed -i "s/AIzaSyDwDPIC-v27c8urAgEhFuTe_MODVE2NSzA/[REMOVED]/g" {} \;' HEAD

# Después de cualquiera de los métodos:
git push --force --all
```

**Nota:** Si el repositorio es público o tiene múltiples colaboradores, considera:
- Notificar a todos los colaboradores
- Cambiar el origen remoto
- Crear un nuevo repositorio limpio

## ✅ Verificación

### Confirmar que la Clave ha sido Revocada

```bash
# Intentar hacer una solicitud con la clave antigua (debe fallar)
curl "https://generativelanguage.googleapis.com/v1beta/models?key=AIzaSyDwDPIC-v27c8urAgEhFuTe_MODVE2NSzA"
```

Deberías ver un error 403 o "API key not valid".

### Confirmar que la Nueva Clave Funciona

```bash
# Verificar que la nueva clave esté configurada
dotnet user-secrets list

# O
echo $env:GEMINI_API_KEY
```

### Verificar que No Queden Claves en el Código

```bash
# Buscar patrones de claves API
git grep -i "AIzaSyDwDPIC"
git grep -E "AIza[a-zA-Z0-9_-]{35}"
```

## 📊 Evaluar el Impacto

### Revisar Uso de la API

1. Ve a [Google Cloud Console](https://console.cloud.google.com/)
2. Selecciona tu proyecto
3. Ve a "APIs & Services" > "Credentials"
4. Revisa el uso de la API en los últimos días
5. Busca actividad sospechosa o inusual

### Verificar Costos

1. En Google Cloud Console, ve a "Billing"
2. Revisa si hay cargos inesperados
3. Establece alertas de facturación si aún no lo has hecho

## 🛡️ Prevención Futura

### 1. Configurar Pre-commit Hook

Crea `.git/hooks/pre-commit`:

```bash
#!/bin/bash
# Verificar que no se hagan commit de claves API

if git diff --cached | grep -E "AIza[a-zA-Z0-9_-]{35}"; then
    echo "❌ ERROR: Se detectó una posible clave API en los cambios"
    echo "Por favor, remueve la clave antes de hacer commit"
    exit 1
fi

exit 0
```

Hazlo ejecutable:
```bash
chmod +x .git/hooks/pre-commit
```

### 2. Usar git-secrets

```bash
# Instalar git-secrets
# Windows (con Scoop)
scoop install git-secrets

# Configurar
git secrets --install
git secrets --register-aws
git secrets --add 'AIza[a-zA-Z0-9_-]{35}'

# Escanear historial
git secrets --scan-history
```

### 3. Configurar .gitignore

Ya está configurado, pero verifica:

```bash
# Verificar que estos patrones estén en .gitignore
cat .gitignore | grep -E "(secrets\.json|\.env|\.key)"
```

## 📝 Checklist de Remediación

- [ ] Clave antigua revocada en Google AI Studio
- [ ] Nueva clave generada
- [ ] Nueva clave configurada con User Secrets
- [ ] Verificado que la nueva clave funciona
- [ ] Historial de Git limpiado (opcional)
- [ ] Uso de API revisado (sin actividad sospechosa)
- [ ] Pre-commit hooks configurados
- [ ] `.gitignore` actualizado
- [ ] Equipo notificado (si aplica)
- [ ] Este archivo eliminado después de completar todas las acciones

## 🗑️ Después de Completar

Una vez que hayas completado todas las acciones:

```bash
# Eliminar este archivo
rm CLAVE-EXPUESTA-ACCION-REQUERIDA.md

# Hacer commit de los cambios de seguridad
git add .
git commit -m "security: implementar gestión segura de claves API con User Secrets"
```

## 📞 Soporte

Si tienes dudas o necesitas ayuda:

- [Documentación de .NET User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets)
- [Google AI Studio](https://aistudio.google.com/)
- [GitHub Security Best Practices](https://docs.github.com/en/code-security)

---

**Fecha de detección:** Noviembre 3, 2025
**Estado:** ⚠️ PENDIENTE DE REMEDIACIÓN

