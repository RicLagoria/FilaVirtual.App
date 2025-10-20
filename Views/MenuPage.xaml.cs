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

            // Cargar el menú cuando aparece la página
            if (BindingContext is MenuVM vm)
            {
                await vm.CargarMenuAsync();
            }
        }
    }
}

