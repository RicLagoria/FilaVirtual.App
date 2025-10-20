using System.Globalization;

namespace FilaVirtual.App
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            // Configurar cultura es-AR para toda la aplicación
            var culture = new CultureInfo("es-AR");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            MainPage = new AppShell();
        }
    }
}
