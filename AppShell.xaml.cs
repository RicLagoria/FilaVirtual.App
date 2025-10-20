using FilaVirtual.App.Services;

namespace FilaVirtual.App
{
    public partial class AppShell : Shell
    {
        public AppShell(ICartNotificationService cartNotificationService)
        {
            InitializeComponent();
            
            // Registrar el tab del carrito inmediatamente
            cartNotificationService.RegistrarCarritoTab(CarritoTab);
            
            // Inicializar con el estado correcto (0 items)
            cartNotificationService.ActualizarTituloCarrito(0);
        }
    }
}
