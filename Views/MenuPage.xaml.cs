using FilaVirtual.App.ViewModels;

namespace FilaVirtual.App.Views
{
    public partial class MenuPage : ContentPage
    {
        public MenuPage(MenuVM viewModel)
        {
            try
            {
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FilaVirtual_App_Log.txt");
                File.AppendAllText(logPath, $"[{DateTime.Now}] Iniciando MenuPage...\n");
                
                InitializeComponent();
                File.AppendAllText(logPath, $"[{DateTime.Now}] MenuPage InitializeComponent completado\n");
                
                BindingContext = viewModel;
                File.AppendAllText(logPath, $"[{DateTime.Now}] MenuPage BindingContext asignado\n");
            }
            catch (Exception ex)
            {
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FilaVirtual_App_Error.txt");
                File.WriteAllText(logPath, $"Error en MenuPage constructor: {ex}\n\nStackTrace: {ex.StackTrace}");
                throw;
            }
        }

        protected override async void OnAppearing()
        {
            try
            {
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FilaVirtual_App_Log.txt");
                File.AppendAllText(logPath, $"[{DateTime.Now}] MenuPage OnAppearing iniciado\n");
                
                base.OnAppearing();

                // Cargar el menú y limpiar carrito cuando aparece la página
                if (BindingContext is MenuVM vm)
                {
                    File.AppendAllText(logPath, $"[{DateTime.Now}] Llamando vm.OnAppearingAsync()\n");
                    await vm.OnAppearingAsync();
                    File.AppendAllText(logPath, $"[{DateTime.Now}] vm.OnAppearingAsync() completado\n");
                    File.AppendAllText(logPath, $"[{DateTime.Now}] MenuPage OnAppearing completado - APLICACIÓN LISTA\n");
                }
                else
                {
                    File.AppendAllText(logPath, $"[{DateTime.Now}] BindingContext no es MenuVM\n");
                }
            }
            catch (Exception ex)
            {
                var logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "FilaVirtual_App_Error.txt");
                File.WriteAllText(logPath, $"Error en MenuPage OnAppearing: {ex}\n\nStackTrace: {ex.StackTrace}");
                throw;
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

