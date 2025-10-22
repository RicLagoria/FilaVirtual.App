namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Implementación del servicio de notificaciones
    /// Para MVP usa DisplayAlert, en futuro se puede agregar toast y sonido
    /// </summary>
    public class LocalNotificationService : INotificationService
    {
        public async Task NotificarPedidoListoAsync(string orderId)
        {
            await MostrarNotificacionAsync(
                "¡Pedido Listo!",
                $"Tu pedido {orderId} está listo para retirar."
            );
        }

        public async Task MostrarNotificacionAsync(string titulo, string mensaje)
        {
            // Verificar que estamos en el hilo principal
            if (Application.Current?.MainPage != null)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert(titulo, mensaje, "OK");
                });
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[Notification] {titulo}: {mensaje}");
            }
        }
    }
}


