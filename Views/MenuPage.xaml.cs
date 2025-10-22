using FilaVirtual.App.ViewModels;

namespace FilaVirtual.App.Views
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage(MenuVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Cargar el menú y limpiar carrito cuando aparece la página
            if (BindingContext is MenuVM vm)
            {
                await vm.OnAppearingAsync();
            }
        }

        /// <summary>
        /// Acceso al panel de operador mediante botón discreto
        /// </summary>
        private async void OnAccesoOperadorTapped(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[MenuPage] Acceso a panel de operador activado");
            
            try
            {
                // Navegar al panel de operador
                await Shell.Current.GoToAsync("OperatorPage");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MenuPage] Error al navegar: {ex.Message}");
                await DisplayAlert("Error", "No se pudo acceder al panel de operador", "OK");
            }
        }
    }
}

