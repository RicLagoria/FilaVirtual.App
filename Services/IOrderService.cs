using FilaVirtual.App.Models;

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Interfaz para el servicio de gestión de pedidos
    /// </summary>
    public interface IOrderService
    {
        /// <summary>
        /// Crea un nuevo pedido con sus items
        /// </summary>
        Task<Order> CrearPedidoAsync(Order order, List<OrderItem> items);

        /// <summary>
        /// Obtiene un pedido por su OrderId
        /// </summary>
        Task<Order?> ObtenerPedidoPorOrderIdAsync(string orderId);

        /// <summary>
        /// Obtiene los items de un pedido
        /// </summary>
        Task<List<OrderItem>> ObtenerItemsPedidoAsync(int orderId);

        /// <summary>
        /// Actualiza el estado de un pedido
        /// </summary>
        Task<int> ActualizarEstadoPedidoAsync(int orderId, EstadoPedido nuevoEstado);

        /// <summary>
        /// Obtiene todos los pedidos
        /// </summary>
        Task<List<Order>> ObtenerTodosPedidosAsync();

        /// <summary>
        /// Obtiene pedidos por estado
        /// </summary>
        Task<List<Order>> ObtenerPedidosPorEstadoAsync(EstadoPedido estado);
    }
}


