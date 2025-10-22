using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Implementación del servicio de notificaciones
    /// Usa toast notifications y sonido del sistema
    /// </summary>
    public class LocalNotificationService : INotificationService
    {
        public async Task NotificarPedidoListoAsync(string orderId)
        {
            // Mostrar toast
            await MostrarNotificacionAsync(
                "¡Pedido Listo! 🎉",
                $"Tu pedido {orderId} está listo para retirar."
            );
            
            // Reproducir sonido del sistema (beep)
            ReproducirSonidoNotificacion();
        }

        public async Task MostrarNotificacionAsync(string titulo, string mensaje)
        {
            try
            {
                // Crear toast con estilo personalizado
                var textoCompleto = $"{titulo}\n{mensaje}";
                
                var toast = Toast.Make(
                    textoCompleto,
                    ToastDuration.Long,
                    14
                );

                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await toast.Show();
                });
                
                System.Diagnostics.Debug.WriteLine($"[Toast] {titulo}: {mensaje}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Notification Error] {ex.Message}");
                
                // Fallback a DisplayAlert si el toast falla
                if (Application.Current?.MainPage != null)
                {
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        await Application.Current.MainPage.DisplayAlert(titulo, mensaje, "OK");
                    });
                }
            }
        }

        /// <summary>
        /// Reproduce un sonido de notificación usando beep del sistema
        /// </summary>
        private void ReproducirSonidoNotificacion()
        {
            try
            {
                // En Windows, usar el beep del sistema
#if WINDOWS
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // Beep corto (frecuencia 800Hz, duración 200ms)
                    System.Console.Beep(800, 200);
                    System.Diagnostics.Debug.WriteLine("[Sound] Beep reproducido");
                });
#else
                System.Diagnostics.Debug.WriteLine("[Sound] Sonido no soportado en esta plataforma");
#endif
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Sound Error] {ex.Message}");
            }
        }
    }
}


