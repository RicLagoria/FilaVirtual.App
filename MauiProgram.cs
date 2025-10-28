using Microsoft.Extensions.Logging;
using FilaVirtual.App.Services;
using FilaVirtual.App.ViewModels;
using FilaVirtual.App.Views;
using CommunityToolkit.Maui;

namespace FilaVirtual.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Inicializar SQLite - CRÍTICO para que funcione en todas las plataformas
            SQLitePCL.Batteries_V2.Init();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() // Toast notifications
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Registrar servicios
            builder.Services.AddSingleton<IStorage, SQLiteStorage>();
            builder.Services.AddSingleton<IMenuService, LocalMenuService>();
            builder.Services.AddSingleton<IOrderService, LocalOrderService>();
            builder.Services.AddSingleton<IQueueService, LocalQueueService>();
            builder.Services.AddSingleton<INotificationService, LocalNotificationService>();
            builder.Services.AddSingleton<ICartNotificationService, CartNotificationService>();
            builder.Services.AddSingleton<IAudioService, WindowsAudioService>();
            
            // Registrar servicios de reconocimiento de voz
            // Usar servicio optimizado para VM (más compatible)
            System.Diagnostics.Debug.WriteLine("[MauiProgram] Usando servicio optimizado para VM");
            builder.Services.AddSingleton<ISpeechRecognitionService, VMSpeechRecognitionService>();
            
            // Registrar servicio de interpretación de voz
            // Opción 1: Simple (pattern matching) - sin dependencias externas
            // Opción 2: Gemini Pro (IA) - requiere GEMINI_API_KEY
            var geminiApiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY");
            if (!string.IsNullOrEmpty(geminiApiKey))
            {
                System.Diagnostics.Debug.WriteLine("[MauiProgram] Usando Gemini Pro para interpretación de voz");
                builder.Services.AddSingleton<IVoiceOrderService, GeminiVoiceOrderService>();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[MauiProgram] Usando interpretación simple (sin IA)");
                builder.Services.AddSingleton<IVoiceOrderService, SimpleVoiceOrderService>();
            }

            // Registrar ViewModels
            builder.Services.AddSingleton<CartVM>(); // Singleton para mantener estado del carrito
            builder.Services.AddTransient<MenuVM>();
            builder.Services.AddTransient<OrderStatusVM>();
            builder.Services.AddTransient<OperatorVM>();

            // Registrar Views
            builder.Services.AddTransient<MenuPage>();
            builder.Services.AddTransient<CartPage>();
            builder.Services.AddTransient<OrderStatusPage>();
            builder.Services.AddTransient<OperatorPage>();

            return builder.Build();
        }

        /// <summary>
        /// Detecta si la aplicación está ejecutándose en una máquina virtual
        /// Usa métodos simples para evitar dependencias complejas
        /// </summary>
        private static bool IsRunningInVM()
        {
            try
            {
                // Método simple: verificar variables de entorno comunes de VM
                var vmwareTools = Environment.GetEnvironmentVariable("VMWARE_TOOLS_INSTALLER");
                var virtualBox = Environment.GetEnvironmentVariable("VBOX_INSTALL_PATH");
                var hyperV = Environment.GetEnvironmentVariable("HYPERV_VM");
                
                if (!string.IsNullOrEmpty(vmwareTools) || 
                    !string.IsNullOrEmpty(virtualBox) || 
                    !string.IsNullOrEmpty(hyperV))
                {
                    System.Diagnostics.Debug.WriteLine("[VM Detection] Detectada VM por variables de entorno");
                    return true;
                }
                
                // Verificar nombre de computadora (método simple)
                var computerName = Environment.MachineName?.ToLower();
                if (computerName != null && (
                    computerName.Contains("vm") ||
                    computerName.Contains("virtual") ||
                    computerName.Contains("test")
                ))
                {
                    System.Diagnostics.Debug.WriteLine($"[VM Detection] Posible VM por nombre: {computerName}");
                    return true;
                }
                
                // Por defecto, asumir que NO es VM para evitar problemas
                System.Diagnostics.Debug.WriteLine("[VM Detection] No detectada VM - usando servicio estándar");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[VM Detection] Error detectando VM: {ex.Message}");
                return false; // Asumir que no es VM si hay error
            }
        }
    }
}
