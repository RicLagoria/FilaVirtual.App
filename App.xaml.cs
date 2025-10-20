using System.Globalization;
using FilaVirtual.App.Services;

namespace FilaVirtual.App
{
    public partial class App : Application
    {
        public App(ICartNotificationService cartNotificationService)
        {
            InitializeComponent();

            // Configurar cultura es-AR para toda la aplicación
            var culture = new CultureInfo("es-AR");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            MainPage = new AppShell(cartNotificationService);
        }
    }
}
