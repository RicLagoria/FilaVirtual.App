using System.Globalization;
using FilaVirtual.App.Services;

namespace FilaVirtual.App
{
    public partial class App : Application
    {
        public App(ICartNotificationService cartNotificationService)
        {
            try
            {
                // Log de inicio
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FilaVirtual_App_Log.txt");
                File.AppendAllText(logPath, $"[{DateTime.Now}] Iniciando App...\n");
                
                InitializeComponent();
                File.AppendAllText(logPath, $"[{DateTime.Now}] InitializeComponent completado\n");

                // Configurar cultura es-AR para toda la aplicación
                var culture = new CultureInfo("es-AR");
                CultureInfo.DefaultThreadCurrentCulture = culture;
                CultureInfo.DefaultThreadCurrentUICulture = culture;
                File.AppendAllText(logPath, $"[{DateTime.Now}] Cultura configurada\n");

                MainPage = new AppShell(cartNotificationService);
                File.AppendAllText(logPath, $"[{DateTime.Now}] AppShell creado\n");
            }
            catch (Exception ex)
            {
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FilaVirtual_App_Error.txt");
                File.WriteAllText(logPath, $"Error en App: {ex}\n\nStackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
