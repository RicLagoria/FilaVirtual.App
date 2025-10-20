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

            // Registrar ViewModels
            builder.Services.AddTransient<MenuVM>();

            // Registrar Views
            builder.Services.AddTransient<MenuPage>();

            return builder.Build();
        }
    }
}
