using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilaVirtual.App.Models;
using FilaVirtual.App.Services;
using System.Collections.ObjectModel;

namespace FilaVirtual.App.ViewModels
{
    /// <summary>
    /// ViewModel para la vista del operador
    /// Gestiona la cola de pedidos y cambios de estado
    /// </summary>
    public partial class OperatorVM : ObservableObject
    {
        private readonly IQueueService _queueService;
        private readonly IOrderService _orderService;

        [ObservableProperty]
        private bool _estaCargando;

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        /// <summary>
        /// Pedidos en cola (estado: EnCola)
        /// </summary>
        public ObservableCollection<Order> PedidosEnCola { get; } = new();

        /// <summary>
        /// Pedidos en preparación (estado: EnPreparacion)
        /// </summary>
        public ObservableCollection<Order> PedidosEnPreparacion { get; } = new();

        /// <summary>
        /// Pedidos listos (estado: Listo)
        /// </summary>
        public ObservableCollection<Order> PedidosListos { get; } = new();

        public OperatorVM(IQueueService queueService, IOrderService orderService)
        {
            _queueService = queueService;
            _orderService = orderService;
        }

        /// <summary>
        /// Carga todos los pedidos agrupados por estado
        /// </summary>
        [RelayCommand]
        public async Task CargarPedidosAsync()
        {
            try
            {
                EstaCargando = true;
                MensajeError = string.Empty;

                // Obtener pedidos por estado (ya ordenados por prioridad)
                var enCola = await _queueService.ObtenerPedidosEnColaAsync();
                var enPreparacion = await _queueService.ObtenerPedidosEnPreparacionAsync();
                var listos = await _queueService.ObtenerPedidosListosAsync();

                // Actualizar colecciones
                ActualizarColeccion(PedidosEnCola, enCola);
                ActualizarColeccion(PedidosEnPreparacion, enPreparacion);
                ActualizarColeccion(PedidosListos, listos);
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar pedidos: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error en OperatorVM.CargarPedidosAsync: {ex}");
            }
            finally
            {
                EstaCargando = false;
            }
        }

        /// <summary>
        /// Mueve un pedido de En Cola a En Preparación
        /// </summary>
        [RelayCommand]
        public async Task PrepararPedidoAsync(Order pedido)
        {
            if (pedido == null) return;

            try
            {
                await _queueService.PrepararPedidoAsync(pedido.OrderId);
                await CargarPedidosAsync(); // Recargar para reflejar cambios
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al preparar pedido: {ex.Message}";
            }
        }

        /// <summary>
        /// Marca un pedido como Listo para retirar
        /// </summary>
        [RelayCommand]
        public async Task MarcarListoAsync(Order pedido)
        {
            if (pedido == null) return;

            try
            {
                await _queueService.MarcarPedidoListoAsync(pedido.OrderId);
                await CargarPedidosAsync(); // Recargar para reflejar cambios
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al marcar como listo: {ex.Message}";
            }
        }

        /// <summary>
        /// Actualiza una colección observable sin perder referencias
        /// </summary>
        private void ActualizarColeccion(ObservableCollection<Order> coleccion, List<Order> nuevosItems)
        {
            coleccion.Clear();
            foreach (var item in nuevosItems)
            {
                coleccion.Add(item);
            }
        }
    }
}


