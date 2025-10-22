using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilaVirtual.App.Models;
using FilaVirtual.App.Services;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace FilaVirtual.App.ViewModels
{
    /// <summary>
    /// ViewModel para la página del carrito
    /// Maneja la lógica del carrito y confirmación de pedido
    /// </summary>
    public partial class CartVM : ObservableObject
    {
        private readonly ICartNotificationService _cartNotificationService;
        private readonly IOrderService _orderService;

        [ObservableProperty]
        private bool _estaCargando;

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        [ObservableProperty]
        private decimal _total;

        [ObservableProperty]
        private int _cantidadItems;

        [ObservableProperty]
        private bool _tieneItems;

        [ObservableProperty]
        private int _cantidadProductos;

        [ObservableProperty]
        private TipoPrioridad _prioridadSeleccionada = TipoPrioridad.STD;

        /// <summary>
        /// Items del carrito
        /// </summary>
        public ObservableCollection<CartItem> ItemsCarrito { get; } = new();

        /// <summary>
        /// Lista de prioridades disponibles para el Picker
        /// </summary>
        public List<TipoPrioridad> PrioridadesDisponibles { get; } = new()
        {
            TipoPrioridad.ACC,
            TipoPrioridad.EMB,
            TipoPrioridad.DOC,
            TipoPrioridad.STD
        };

        /// <summary>
        /// Total formateado con cultura es-AR
        /// </summary>
        [ObservableProperty]
        private string _totalFormateado = "$0";

        public CartVM(ICartNotificationService cartNotificationService, IOrderService orderService)
        {
            _cartNotificationService = cartNotificationService;
            _orderService = orderService;
            
            // Suscribirse a cambios en la colección para actualizar totales
            ItemsCarrito.CollectionChanged += (s, e) =>
            {
                ActualizarTotales();
                
                // Suscribirse a cambios de cantidad en cada item
                if (e.NewItems != null)
                {
                    foreach (CartItem item in e.NewItems)
                    {
                        item.PropertyChanged += (sender, args) =>
                        {
                            if (args.PropertyName == nameof(CartItem.Cantidad))
                            {
                                ActualizarTotales();
                            }
                        };
                    }
                }
            };
        }

        /// <summary>
        /// Limpia completamente el carrito
        /// </summary>
        public void LimpiarCarrito()
        {
            ItemsCarrito.Clear();
            PrioridadSeleccionada = TipoPrioridad.STD; // Resetear a prioridad por defecto
            MensajeError = string.Empty;
            ActualizarTotales();
            System.Diagnostics.Debug.WriteLine("Carrito limpiado completamente");
        }

        /// <summary>
        /// Agrega un ítem al carrito
        /// </summary>
        [RelayCommand]
        public void AgregarItem(MenuItemModel item)
        {
            if (item == null) return;

            var itemExistente = ItemsCarrito.FirstOrDefault(i => i.MenuItemId == item.Id);
            
            if (itemExistente != null)
            {
                // Si ya existe, incrementar cantidad
                itemExistente.Cantidad++;
            }
            else
            {
                // Si no existe, agregar nuevo
                ItemsCarrito.Add(new CartItem
                {
                    MenuItemId = item.Id,
                    Nombre = item.Nombre,
                    PrecioUnitario = item.Precio,
                    Cantidad = 1
                });
            }

            ActualizarTotales();
        }

        /// <summary>
        /// Quita un ítem del carrito
        /// </summary>
        [RelayCommand]
        public void QuitarItem(CartItem item)
        {
            if (item == null) return;

            ItemsCarrito.Remove(item);
            ActualizarTotales();
        }

        /// <summary>
        /// Incrementa la cantidad de un ítem
        /// </summary>
        [RelayCommand]
        public void IncrementarCantidad(CartItem item)
        {
            if (item == null) return;

            item.Cantidad++;
            ActualizarTotales();
        }

        /// <summary>
        /// Decrementa la cantidad de un ítem
        /// </summary>
        [RelayCommand]
        public void DecrementarCantidad(CartItem item)
        {
            if (item == null) return;

            if (item.Cantidad > 1)
            {
                item.Cantidad--;
            }
            else
            {
                // Si cantidad es 1, quitar del carrito
                QuitarItem(item);
            }

            ActualizarTotales();
        }

        /// <summary>
        /// Confirma el pedido y genera QR
        /// </summary>
        [RelayCommand]
        public async Task ConfirmarPedidoAsync()
        {
            if (ItemsCarrito.Count == 0)
            {
                MensajeError = "El carrito está vacío";
                return;
            }

            try
            {
                EstaCargando = true;
                MensajeError = string.Empty;

                // Generar OrderId único
                var orderId = GenerarOrderId();
                
                // Crear Order con la prioridad seleccionada
                var order = new Order
                {
                    OrderId = orderId,
                    Estado = EstadoPedido.EnCola,
                    TipoPrioridad = PrioridadSeleccionada,
                    Total = Total,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                };

                // Crear OrderItems
                var orderItems = ItemsCarrito.Select(cartItem => new OrderItem
                {
                    OrderId = 0, // Se asignará al insertar
                    MenuItemId = cartItem.MenuItemId,
                    Nombre = cartItem.Nombre,
                    PrecioUnitario = cartItem.PrecioUnitario,
                    Cantidad = cartItem.Cantidad
                }).ToList();

                // Generar payload para QR
                var qrPayload = new
                {
                    orderId = order.OrderId,
                    total = order.Total,
                    timestamp = order.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ss"),
                    items = orderItems.Count
                };

                var qrJson = JsonSerializer.Serialize(qrPayload);
                
                // Persistir el pedido en SQLite
                var orderCreado = await _orderService.CrearPedidoAsync(order, orderItems);
                
                System.Diagnostics.Debug.WriteLine($"Pedido confirmado: {orderCreado.OrderId}");
                System.Diagnostics.Debug.WriteLine($"QR Payload: {qrJson}");

                // Limpiar carrito completamente
                LimpiarCarrito();

                // Navegar a la página de estado del pedido
                await Shell.Current.GoToAsync($"OrderStatusPage?orderId={orderCreado.OrderId}");
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al confirmar pedido: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error en CartVM.ConfirmarPedidoAsync: {ex}");
            }
            finally
            {
                EstaCargando = false;
            }
        }

        /// <summary>
        /// Actualiza los totales del carrito
        /// </summary>
        private void ActualizarTotales()
        {
            Total = ItemsCarrito.Sum(i => i.Subtotal);
            CantidadItems = ItemsCarrito.Sum(i => i.Cantidad);
            CantidadProductos = ItemsCarrito.Count; // Cantidad de productos únicos
            TieneItems = CantidadItems > 0;
            TotalFormateado = Total.ToString("C");
            
            // Actualizar título del tab con la cantidad total de items
            _cartNotificationService.ActualizarTituloCarrito(CantidadItems);
        }

        /// <summary>
        /// Genera un OrderId único
        /// </summary>
        private string GenerarOrderId()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(1000, 9999);
            return $"ORD-{timestamp}-{random}";
        }
    }

    /// <summary>
    /// Representa un ítem en el carrito (no persistido)
    /// </summary>
    public partial class CartItem : ObservableObject
    {
        public int MenuItemId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public decimal PrecioUnitario { get; set; }

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(Subtotal), nameof(SubtotalFormateado))]
        private int _cantidad = 1;

        public decimal Subtotal => Cantidad * PrecioUnitario;
        public string PrecioUnitarioFormateado => PrecioUnitario.ToString("C");
        public string SubtotalFormateado => Subtotal.ToString("C");
    }
}

