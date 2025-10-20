using FilaVirtual.App.Views;

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Implementación del servicio de notificaciones del carrito
    /// </summary>
    public class CartNotificationService : ICartNotificationService
    {
        private ShellContent? _carritoTab;
        private const string CARRITO_ROUTE = "CartPage";

        /// <summary>
        /// Registra la referencia al tab del carrito
        /// </summary>
        public void RegistrarCarritoTab(object carritoTab)
        {
            _carritoTab = carritoTab as ShellContent;
        }

        public void ActualizarTituloCarrito(int cantidadItems)
        {
            // Actualizar en el hilo principal - SIEMPRE mostrar el contador
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var nuevoTitulo = $"🛒 ({cantidadItems})";
                
                // Método 1: Usar la referencia directa
                if (_carritoTab != null)
                {
                    _carritoTab.Title = nuevoTitulo;
                }
                
                // Método 2: Buscar el tab en Shell.Current como fallback
                // Esto asegura que el título se actualice incluso si la referencia directa falla
                if (Shell.Current != null)
                {
                    var tabBar = Shell.Current.Items.FirstOrDefault() as TabBar;
                    if (tabBar != null)
                    {
                        var carritoShellContent = tabBar.Items.FirstOrDefault(item => item.Route == CARRITO_ROUTE);
                        if (carritoShellContent != null)
                        {
                            carritoShellContent.Title = nuevoTitulo;
                        }
                    }
                }
            });
        }
    }
}
