using FilaVirtual.App.ViewModels;

namespace FilaVirtual.App.Views
{
    public partial class OrderStatusPage : ContentPage
    {
        private System.Timers.Timer? _autoRefreshTimer;

        public OrderStatusPage(OrderStatusVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            // Solo iniciar el timer si no existe uno ya activo
            if (_autoRefreshTimer == null)
            {
                // Iniciar auto-actualización cada 10 segundos
                _autoRefreshTimer = new System.Timers.Timer(10000);
                _autoRefreshTimer.Elapsed += async (s, e) =>
                {
                    // Solo actualizar si la página está realmente visible
                    if (BindingContext is OrderStatusVM vm && vm.Pedido != null)
                    {
                        try
                        {
                            await MainThread.InvokeOnMainThreadAsync(async () =>
                            {
                                await vm.CargarPedidoAsync(vm.Pedido.OrderId);
                            });
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error en auto-refresh: {ex.Message}");
                        }
                    }
                };
                _autoRefreshTimer.Start();
                System.Diagnostics.Debug.WriteLine("Timer de auto-refresh iniciado");
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            // Detener y limpiar el timer completamente
            DetenerTimer();
            System.Diagnostics.Debug.WriteLine("Timer de auto-refresh detenido");
        }

        private void DetenerTimer()
        {
            if (_autoRefreshTimer != null)
            {
                _autoRefreshTimer.Stop();
                _autoRefreshTimer.Dispose();
                _autoRefreshTimer = null;
            }
        }

        private async void OnVolverAlMenuClicked(object sender, EventArgs e)
        {
            // Detener timer antes de navegar
            DetenerTimer();
            
            // Hacer pop para volver al stack anterior (CartPage), luego ir al menú
            // Esto limpia la OrderStatusPage del stack de navegación
            await Shell.Current.Navigation.PopAsync();
            
            // Navegar al tab del menú
            await Shell.Current.GoToAsync("//MenuPage");
        }
    }
}


