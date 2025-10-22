using FilaVirtual.App.Models;

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Implementación del servicio de órdenes usando SQLite
    /// </summary>
    public class LocalOrderService : IOrderService
    {
        private readonly IStorage _storage;

        public LocalOrderService(IStorage storage)
        {
            _storage = storage;
        }

        public async Task<Order> CrearPedidoAsync(Order order, List<OrderItem> items)
        {
            // Insertar el pedido
            await _storage.InsertarAsync(order);
            
            // El ID se genera automáticamente, necesitamos recuperarlo
            var orderInsertado = await ObtenerPedidoPorOrderIdAsync(order.OrderId);
            
            if (orderInsertado != null)
            {
                // Asignar el OrderId a cada item y guardarlos
                foreach (var item in items)
                {
                    item.OrderId = orderInsertado.Id;
                    await _storage.InsertarAsync(item);
                }
            }

            return orderInsertado ?? order;
        }

        public async Task<Order?> ObtenerPedidoPorOrderIdAsync(string orderId)
        {
            var pedidos = await _storage.ObtenerTodosAsync<Order>();
            return pedidos.FirstOrDefault(p => p.OrderId == orderId);
        }

        public async Task<List<OrderItem>> ObtenerItemsPedidoAsync(int orderId)
        {
            var todosLosItems = await _storage.ObtenerTodosAsync<OrderItem>();
            return todosLosItems.Where(i => i.OrderId == orderId).ToList();
        }

        public async Task<int> ActualizarEstadoPedidoAsync(int orderId, EstadoPedido nuevoEstado)
        {
            var order = await _storage.ObtenerPorIdAsync<Order>(orderId);
            if (order != null)
            {
                order.Estado = nuevoEstado;
                order.UpdatedAt = DateTime.Now;
                return await _storage.ActualizarAsync(order);
            }
            return 0;
        }

        public async Task<List<Order>> ObtenerTodosPedidosAsync()
        {
            return await _storage.ObtenerTodosAsync<Order>();
        }

        public async Task<List<Order>> ObtenerPedidosPorEstadoAsync(EstadoPedido estado)
        {
            var pedidos = await ObtenerTodosPedidosAsync();
            return pedidos.Where(p => p.Estado == estado).ToList();
        }
    }
}


