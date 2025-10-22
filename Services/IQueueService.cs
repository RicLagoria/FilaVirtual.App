using FilaVirtual.App.Models;

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Interfaz para el servicio de gestión de cola priorizada
    /// Prioridad: ACC > EMB > DOC > STD
    /// Dentro de cada prioridad: FIFO (por CreatedAt)
    /// </summary>
    public interface IQueueService
    {
        /// <summary>
        /// Obtiene la cola completa ordenada por prioridad y FIFO
        /// </summary>
        Task<List<Order>> ObtenerColaOrdenadaAsync();

        /// <summary>
        /// Obtiene pedidos en cola (estado EnCola) ordenados por prioridad
        /// </summary>
        Task<List<Order>> ObtenerPedidosEnColaAsync();

        /// <summary>
        /// Obtiene pedidos en preparación ordenados por prioridad
        /// </summary>
        Task<List<Order>> ObtenerPedidosEnPreparacionAsync();

        /// <summary>
        /// Obtiene pedidos listos
        /// </summary>
        Task<List<Order>> ObtenerPedidosListosAsync();

        /// <summary>
        /// Obtiene la posición de un pedido en la cola según su prioridad
        /// </summary>
        Task<int> ObtenerPosicionEnColaAsync(string orderId);

        /// <summary>
        /// Mueve un pedido a preparación
        /// </summary>
        Task PrepararPedidoAsync(string orderId);

        /// <summary>
        /// Marca un pedido como listo
        /// </summary>
        Task MarcarPedidoListoAsync(string orderId);
    }
}


