using FilaVirtual.App.ViewModels;

namespace FilaVirtual.App.Views
{
    public partial class CartPage : ContentPage
    {
        public CartPage(CartVM viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
