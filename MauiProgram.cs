using Microsoft.Extensions.Logging;
using FilaVirtual.App.Services;
using FilaVirtual.App.ViewModels;
using FilaVirtual.App.Views;

namespace FilaVirtual.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            // Inicializar SQLite - CRÍTICO para que funcione en todas las plataformas
            SQLitePCL.Batteries_V2.Init();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            // Registrar servicios
            builder.Services.AddSingleton<IStorage, SQLiteStorage>();
            builder.Services.AddSingleton<IMenuService, LocalMenuService>();
            builder.Services.AddSingleton<ICartNotificationService, CartNotificationService>();

            // Registrar ViewModels
            builder.Services.AddSingleton<CartVM>(); // Singleton para mantener estado del carrito
            builder.Services.AddTransient<MenuVM>();

            // Registrar Views
            builder.Services.AddTransient<MenuPage>();
            builder.Services.AddTransient<CartPage>();

            return builder.Build();
        }
    }
}
