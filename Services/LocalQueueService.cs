using FilaVirtual.App.Models;

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Implementación del servicio de cola priorizada
    /// Prioridad: ACC (1) > EMB (2) > DOC (3) > STD (4)
    /// Dentro de cada prioridad: FIFO por CreatedAt
    /// </summary>
    public class LocalQueueService : IQueueService
    {
        private readonly IOrderService _orderService;
        private readonly INotificationService _notificationService;

        public LocalQueueService(IOrderService orderService, INotificationService notificationService)
        {
            _orderService = orderService;
            _notificationService = notificationService;
        }

        public async Task<List<Order>> ObtenerColaOrdenadaAsync()
        {
            var pedidos = await _orderService.ObtenerTodosPedidosAsync();
            
            return pedidos
                .OrderBy(p => (int)p.TipoPrioridad)  // Prioridad: ACC(1) < STD(4)
                .ThenBy(p => p.CreatedAt)            // FIFO dentro de cada prioridad
                .ToList();
        }

        public async Task<List<Order>> ObtenerPedidosEnColaAsync()
        {
            var pedidos = await _orderService.ObtenerPedidosPorEstadoAsync(EstadoPedido.EnCola);
            
            return pedidos
                .OrderBy(p => (int)p.TipoPrioridad)
                .ThenBy(p => p.CreatedAt)
                .ToList();
        }

        public async Task<List<Order>> ObtenerPedidosEnPreparacionAsync()
        {
            var pedidos = await _orderService.ObtenerPedidosPorEstadoAsync(EstadoPedido.EnPreparacion);
            
            return pedidos
                .OrderBy(p => (int)p.TipoPrioridad)
                .ThenBy(p => p.CreatedAt)
                .ToList();
        }

        public async Task<List<Order>> ObtenerPedidosListosAsync()
        {
            return await _orderService.ObtenerPedidosPorEstadoAsync(EstadoPedido.Listo);
        }

        public async Task<int> ObtenerPosicionEnColaAsync(string orderId)
        {
            var cola = await ObtenerPedidosEnColaAsync();
            var posicion = cola.FindIndex(p => p.OrderId == orderId);
            return posicion >= 0 ? posicion + 1 : 0; // Retorna 1-based index
        }

        public async Task PrepararPedidoAsync(string orderId)
        {
            var order = await _orderService.ObtenerPedidoPorOrderIdAsync(orderId);
            if (order != null)
            {
                await _orderService.ActualizarEstadoPedidoAsync(order.Id, EstadoPedido.EnPreparacion);
            }
        }

        public async Task MarcarPedidoListoAsync(string orderId)
        {
            var order = await _orderService.ObtenerPedidoPorOrderIdAsync(orderId);
            if (order != null)
            {
                await _orderService.ActualizarEstadoPedidoAsync(order.Id, EstadoPedido.Listo);
                
                // Enviar notificación al cliente
                await _notificationService.NotificarPedidoListoAsync(order.OrderId);
            }
        }
    }
}


