using FilaVirtual.App.ViewModels;

namespace FilaVirtual.App.Views
{
    public partial class OrderStatusPage : ContentPage
    {
        public OrderStatusPage(OrderStatusVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        private async void OnVolverAlMenuClicked(object sender, EventArgs e)
        {
            // Volver al menú principal
            await Shell.Current.GoToAsync("//MenuPage");
        }
    }
}


