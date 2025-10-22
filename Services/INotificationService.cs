namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Interfaz para el servicio de notificaciones
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Notifica al cliente que su pedido está listo
        /// </summary>
        Task NotificarPedidoListoAsync(string orderId);

        /// <summary>
        /// Muestra una notificación general
        /// </summary>
        Task MostrarNotificacionAsync(string titulo, string mensaje);
    }
}


