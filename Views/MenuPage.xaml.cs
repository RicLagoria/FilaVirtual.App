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
        /// Acceso oculto al panel de operador mediante triple-tap
        /// </summary>
        private async void OnAccesoOperadorTapped(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[MenuPage] Acceso a panel de operador activado");
            
            // Navegar al panel de operador
            await Shell.Current.GoToAsync("OperatorPage");
        }
    }
}

