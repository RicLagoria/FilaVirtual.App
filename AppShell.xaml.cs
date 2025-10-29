using FilaVirtual.App.Services;
using FilaVirtual.App.Views;

namespace FilaVirtual.App
{
    public partial class AppShell : Shell
    {
        public AppShell(ICartNotificationService cartNotificationService)
        {
            try
            {
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FilaVirtual_App_Log.txt");
                File.AppendAllText(logPath, $"[{DateTime.Now}] Iniciando AppShell...\n");
                
                InitializeComponent();
                File.AppendAllText(logPath, $"[{DateTime.Now}] AppShell InitializeComponent completado\n");
                
                // Registrar rutas de navegación
                Routing.RegisterRoute("OrderStatusPage", typeof(OrderStatusPage));
                Routing.RegisterRoute("OperatorPage", typeof(OperatorPage));
                File.AppendAllText(logPath, $"[{DateTime.Now}] Rutas de navegación registradas\n");
                
                // Registrar el tab del carrito inmediatamente
                cartNotificationService.RegistrarCarritoTab(CarritoTab);
                File.AppendAllText(logPath, $"[{DateTime.Now}] CarritoTab registrado\n");
                
                // Inicializar con el estado correcto (0 items)
                cartNotificationService.ActualizarTituloCarrito(0);
                File.AppendAllText(logPath, $"[{DateTime.Now}] AppShell inicializado correctamente\n");
            }
            catch (Exception ex)
            {
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FilaVirtual_App_Error.txt");
                File.WriteAllText(logPath, $"Error en AppShell: {ex}\n\nStackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
