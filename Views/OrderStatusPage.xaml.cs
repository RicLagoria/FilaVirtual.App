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
            
            // Iniciar auto-actualización cada 10 segundos (menos agresivo)
            _autoRefreshTimer = new System.Timers.Timer(10000);
            _autoRefreshTimer.Elapsed += async (s, e) =>
            {
                // Solo actualizar si la página está realmente visible
                if (IsVisible && BindingContext is OrderStatusVM vm && vm.Pedido != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await vm.CargarPedidoAsync(vm.Pedido.OrderId);
                    });
                }
            };
            _autoRefreshTimer.Start();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
            // Detener auto-actualización al salir
            _autoRefreshTimer?.Stop();
            _autoRefreshTimer?.Dispose();
        }

        private async void OnVolverAlMenuClicked(object sender, EventArgs e)
        {
            // Detener auto-refresh antes de navegar
            _autoRefreshTimer?.Stop();
            _autoRefreshTimer?.Dispose();
            
            // Volver al menú principal
            await Shell.Current.GoToAsync("//MenuPage");
        }
    }
}


