using FilaVirtual.App.Services;
using FilaVirtual.App.Views;

namespace FilaVirtual.App
{
    public partial class AppShell : Shell
    {
        public AppShell(ICartNotificationService cartNotificationService)
        {
            InitializeComponent();
            
            // Registrar rutas de navegación
            Routing.RegisterRoute("OrderStatusPage", typeof(OrderStatusPage));
            Routing.RegisterRoute("OperatorPage", typeof(OperatorPage));
            
            // Registrar el tab del carrito inmediatamente
            cartNotificationService.RegistrarCarritoTab(CarritoTab);
            
            // Inicializar con el estado correcto (0 items)
            cartNotificationService.ActualizarTituloCarrito(0);
        }
    }
}
