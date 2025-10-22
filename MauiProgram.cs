using Microsoft.Extensions.Logging;
using FilaVirtual.App.Services;
using FilaVirtual.App.ViewModels;
using FilaVirtual.App.Views;
using CommunityToolkit.Maui;

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
                .UseMauiCommunityToolkit() // Toast notifications
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
            builder.Services.AddSingleton<IOrderService, LocalOrderService>();
            builder.Services.AddSingleton<IQueueService, LocalQueueService>();
            builder.Services.AddSingleton<INotificationService, LocalNotificationService>();
            builder.Services.AddSingleton<ICartNotificationService, CartNotificationService>();
            
            // Registrar servicios de reconocimiento de voz
            builder.Services.AddSingleton<ISpeechRecognitionService, WindowsSpeechRecognitionService>();
            builder.Services.AddSingleton<IVoiceOrderService, SimpleVoiceOrderService>();

            // Registrar ViewModels
            builder.Services.AddSingleton<CartVM>(); // Singleton para mantener estado del carrito
            builder.Services.AddTransient<MenuVM>();
            builder.Services.AddTransient<OrderStatusVM>();
            builder.Services.AddTransient<OperatorVM>();

            // Registrar Views
            builder.Services.AddTransient<MenuPage>();
            builder.Services.AddTransient<CartPage>();
            builder.Services.AddTransient<OrderStatusPage>();
            builder.Services.AddTransient<OperatorPage>();

            return builder.Build();
        }
    }
}
