using FilaVirtual.App.ViewModels;

namespace FilaVirtual.App.Views
{
    public partial class OperatorPage : ContentPage
    {
        public OperatorPage(OperatorVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // Cargar pedidos cuando aparece la página
            if (BindingContext is OperatorVM vm)
            {
                await vm.CargarPedidosAsync();
            }
        }
    }
}


