using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilaVirtual.App.Models;
using FilaVirtual.App.Services;
using QRCoder;
using System.Text.Json;

namespace FilaVirtual.App.ViewModels
{
    /// <summary>
    /// ViewModel para la página de estado del pedido
    /// </summary>
    [QueryProperty(nameof(OrderIdParam), "orderId")]
    public partial class OrderStatusVM : ObservableObject
    {
        private readonly IOrderService _orderService;

        [ObservableProperty]
        private bool _estaCargando;

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        [ObservableProperty]
        private Order? _pedido;

        [ObservableProperty]
        private List<OrderItem> _items = new();

        [ObservableProperty]
        private ImageSource? _imagenQR;

        [ObservableProperty]
        private string _orderIdParam = string.Empty;

        [ObservableProperty]
        private string _estadoTexto = string.Empty;

        [ObservableProperty]
        private Color _estadoColor = Colors.Gray;

        public OrderStatusVM(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Se ejecuta cuando cambia el OrderId desde la navegación
        /// </summary>
        partial void OnOrderIdParamChanged(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _ = CargarPedidoAsync(value);
            }
        }

        /// <summary>
        /// Carga los datos del pedido
        /// </summary>
        public async Task CargarPedidoAsync(string orderId)
        {
            try
            {
                EstaCargando = true;
                MensajeError = string.Empty;

                // Obtener el pedido
                Pedido = await _orderService.ObtenerPedidoPorOrderIdAsync(orderId);

                if (Pedido == null)
                {
                    MensajeError = $"No se encontró el pedido {orderId}";
                    return;
                }

                // Obtener los items del pedido
                Items = await _orderService.ObtenerItemsPedidoAsync(Pedido.Id);

                // Generar código QR
                GenerarCodigoQR(orderId);

                // Actualizar estado visual
                ActualizarEstadoVisual();
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar el pedido: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error en OrderStatusVM.CargarPedidoAsync: {ex}");
            }
            finally
            {
                EstaCargando = false;
            }
        }

        /// <summary>
        /// Marca el pedido como pagado (demo)
        /// </summary>
        [RelayCommand]
        public async Task MarcarComoPagadoAsync()
        {
            if (Pedido == null) return;

            try
            {
                EstaCargando = true;

                // Actualizar estado a En Preparación
                await _orderService.ActualizarEstadoPedidoAsync(Pedido.Id, EstadoPedido.EnPreparacion);
                
                // Recargar el pedido
                await CargarPedidoAsync(Pedido.OrderId);
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al marcar como pagado: {ex.Message}";
            }
            finally
            {
                EstaCargando = false;
            }
        }

        /// <summary>
        /// Genera el código QR del pedido
        /// </summary>
        private void GenerarCodigoQR(string orderId)
        {
            try
            {
                if (Pedido == null) return;

                // Crear payload del QR
                var qrPayload = new
                {
                    orderId = Pedido.OrderId,
                    total = Pedido.Total,
                    timestamp = Pedido.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                    items = Items.Count
                };

                var qrJson = JsonSerializer.Serialize(qrPayload);

                // Generar QR usando QRCoder
                using var qrGenerator = new QRCodeGenerator();
                using var qrCodeData = qrGenerator.CreateQrCode(qrJson, QRCodeGenerator.ECCLevel.Q);
                using var qrCode = new PngByteQRCode(qrCodeData);
                var qrBytes = qrCode.GetGraphic(20);

                // Convertir a ImageSource
                ImagenQR = ImageSource.FromStream(() => new MemoryStream(qrBytes));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al generar QR: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza el texto y color del estado según el estado actual
        /// </summary>
        private void ActualizarEstadoVisual()
        {
            if (Pedido == null) return;

            switch (Pedido.Estado)
            {
                case EstadoPedido.EnCola:
                    EstadoTexto = "En Cola";
                    EstadoColor = Color.FromArgb("#FF9800"); // Naranja
                    break;
                case EstadoPedido.EnPreparacion:
                    EstadoTexto = "En Preparación";
                    EstadoColor = Color.FromArgb("#2196F3"); // Azul
                    break;
                case EstadoPedido.Listo:
                    EstadoTexto = "Listo para Retirar";
                    EstadoColor = Color.FromArgb("#4CAF50"); // Verde
                    break;
            }
        }
    }
}


