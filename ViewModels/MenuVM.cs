using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FilaVirtual.App.Models;
using FilaVirtual.App.Services;
using System.Collections.ObjectModel;

namespace FilaVirtual.App.ViewModels
{
    /// <summary>
    /// ViewModel para la página del menú
    /// Muestra los ítems del menú agrupados por categoría
    /// </summary>
    public partial class MenuVM : ObservableObject
    {
        private readonly IMenuService _menuService;
        private readonly CartVM _cartVM;
        private readonly ISpeechRecognitionService _speechService;
        private readonly IVoiceOrderService _voiceOrderService;

        [ObservableProperty]
        private bool _estaCargando;

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(MostrarMensajeSinResultados))]
        private string _textoBusqueda = string.Empty;

        [ObservableProperty]
        private string _mensajeSinResultados = string.Empty;

        [ObservableProperty]
        private bool _estaEscuchando;

        [ObservableProperty]
        private string _textoEscuchado = string.Empty;

        /// <summary>
        /// Ítems del menú agrupados por categoría
        /// </summary>
        public ObservableCollection<GrupoMenu> MenuAgrupado { get; } = new();

        /// <summary>
        /// Colección original sin filtrar (para restaurar después de búsqueda)
        /// </summary>
        private Dictionary<string, List<MenuItemModel>> _menuCompleto = new();

        /// <summary>
        /// Indica si se debe mostrar el mensaje de sin resultados
        /// </summary>
        public bool MostrarMensajeSinResultados => !string.IsNullOrWhiteSpace(TextoBusqueda) && MenuAgrupado.Count == 0;

        public MenuVM(IMenuService menuService, CartVM cartVM, 
                     ISpeechRecognitionService speechService, 
                     IVoiceOrderService voiceOrderService)
        {
            _menuService = menuService;
            _cartVM = cartVM;
            _speechService = speechService;
            _voiceOrderService = voiceOrderService;
            
            // Suscribirse a cambios en el texto de búsqueda
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TextoBusqueda))
                {
                    FiltrarMenu();
                }
            };

            // Suscribirse a eventos de reconocimiento de voz
            _speechService.SpeechRecognized += OnSpeechRecognized;
            _speechService.ErrorOccurred += OnSpeechError;
        }

        /// <summary>
        /// Se ejecuta cuando la página aparece
        /// </summary>
        [RelayCommand]
        public async Task OnAppearingAsync()
        {
            // Limpiar carrito al volver al menú (asegurar estado limpio)
            _cartVM.LimpiarCarrito();
            await CargarMenuAsync();
        }

        /// <summary>
        /// Carga los datos del menú al aparecer la página
        /// </summary>
        [RelayCommand]
        public async Task CargarMenuAsync()
        {
            try
            {
                EstaCargando = true;
                MensajeError = string.Empty;

                // Obtener menú agrupado por categoría (ahora usa datos hardcodeados)
                _menuCompleto = await _menuService.ObtenerMenuPorCategoriaAsync();

                // Actualizar la colección con todos los items
                ActualizarMenuAgrupado(_menuCompleto);
            }
            catch (Exception ex)
            {
                MensajeError = $"Error al cargar el menú: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error en MenuVM.CargarMenuAsync: {ex}");
            }
            finally
            {
                EstaCargando = false;
            }
        }

        /// <summary>
        /// Filtra el menú basándose en el texto de búsqueda
        /// </summary>
        private void FiltrarMenu()
        {
            if (string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                // Si no hay texto de búsqueda, mostrar todo el menú
                ActualizarMenuAgrupado(_menuCompleto);
                MensajeSinResultados = string.Empty;
                return;
            }

            // Filtrar items que coincidan con el texto de búsqueda
            var textoMinuscula = TextoBusqueda.ToLower().Trim();
            var menuFiltrado = new Dictionary<string, List<MenuItemModel>>();

            foreach (var categoria in _menuCompleto)
            {
                var itemsFiltrados = categoria.Value
                    .Where(item => 
                        item.Nombre.ToLower().Contains(textoMinuscula) ||
                        item.Categoria.ToLower().Contains(textoMinuscula))
                    .ToList();

                if (itemsFiltrados.Any())
                {
                    menuFiltrado[categoria.Key] = itemsFiltrados;
                }
            }

            // Actualizar la colección
            ActualizarMenuAgrupado(menuFiltrado);

            // Actualizar mensaje si no hay resultados
            if (menuFiltrado.Count == 0)
            {
                MensajeSinResultados = $"No se encontraron productos que coincidan con '{TextoBusqueda}'";
            }
            else
            {
                MensajeSinResultados = string.Empty;
            }
        }

        /// <summary>
        /// Actualiza la colección MenuAgrupado con el diccionario proporcionado
        /// </summary>
        private void ActualizarMenuAgrupado(Dictionary<string, List<MenuItemModel>> menuPorCategoria)
        {
            MenuAgrupado.Clear();
            foreach (var grupo in menuPorCategoria)
            {
                MenuAgrupado.Add(new GrupoMenu(grupo.Key, grupo.Value));
            }
        }

        /// <summary>
        /// Agrega un ítem al carrito
        /// </summary>
        [RelayCommand]
        private async Task AgregarAlCarrito(MenuItemModel item)
        {
            if (item == null) return;

            _cartVM.AgregarItem(item);
            System.Diagnostics.Debug.WriteLine($"Ítem agregado al carrito: {item.Nombre} - {item.PrecioFormateado}");

            // Mostrar confirmación visual
            await MostrarConfirmacionAgregado(item.Nombre);
        }

        /// <summary>
        /// Muestra confirmación visual de item agregado
        /// </summary>
        private async Task MostrarConfirmacionAgregado(string nombreItem)
        {
            // Mostrar confirmación en el mensaje de error (temporal)
            MensajeError = $"✓ {nombreItem} agregado al carrito";
            
            // Limpiar mensaje después de 2 segundos
            await Task.Delay(2000);
            MensajeError = string.Empty;
            
            System.Diagnostics.Debug.WriteLine($"✓ {nombreItem} agregado al carrito");
        }

        /// <summary>
        /// Activa o desactiva el micrófono para pedidos por voz
        /// </summary>
        [RelayCommand]
        public async Task ToggleMicAsync()
        {
            if (EstaEscuchando)
            {
                await _speechService.StopListeningAsync();
                EstaEscuchando = false;
                TextoEscuchado = string.Empty;
                System.Diagnostics.Debug.WriteLine("[MenuVM] Micrófono detenido");
            }
            else
            {
                await _speechService.StartListeningAsync();
                EstaEscuchando = true;
                TextoEscuchado = "🎤 Escuchando... di tu pedido";
                System.Diagnostics.Debug.WriteLine("[MenuVM] Micrófono activado");
            }
        }

        /// <summary>
        /// Manejador del evento cuando se reconoce voz
        /// </summary>
        private async void OnSpeechRecognized(object? sender, string texto)
        {
            System.Diagnostics.Debug.WriteLine($"[MenuVM] Voz reconocida: {texto}");
            TextoEscuchado = $"Escuché: \"{texto}\"";

            try
            {
                // Interpretar el pedido
                var items = await _voiceOrderService.InterpretarPedidoAsync(texto);

                if (items.Count == 0)
                {
                    TextoEscuchado = "❌ No entendí el pedido. Intenta de nuevo.";
                    return;
                }

                // Agregar items al carrito
                int itemsAgregados = 0;
                foreach (var voiceItem in items)
                {
                    // Buscar el producto en el menú
                    MenuItemModel? menuItem = null;
                    
                    foreach (var grupo in MenuAgrupado)
                    {
                        menuItem = grupo.FirstOrDefault(m =>
                            m.Nombre.Equals(voiceItem.NombreProducto, StringComparison.OrdinalIgnoreCase) ||
                            m.Nombre.Contains(voiceItem.NombreProducto, StringComparison.OrdinalIgnoreCase)
                        );
                        
                        if (menuItem != null) break;
                    }

                    if (menuItem != null)
                    {
                        // Agregar la cantidad solicitada
                        for (int i = 0; i < voiceItem.Cantidad; i++)
                        {
                            _cartVM.AgregarItem(menuItem);
                        }
                        
                        itemsAgregados++;
                        System.Diagnostics.Debug.WriteLine($"[MenuVM] Agregado: {voiceItem.Cantidad}x {menuItem.Nombre}");
                    }
                }

                // Mostrar confirmación
                if (itemsAgregados > 0)
                {
                    TextoEscuchado = $"✅ {itemsAgregados} producto(s) agregados al carrito";
                    
                    // Opcional: Mostrar alert
                    await MainThread.InvokeOnMainThreadAsync(async () =>
                    {
                        var mensaje = string.Join("\n", items.Select(i => $"• {i.Cantidad}x {i.NombreProducto}"));
                        await Application.Current!.MainPage!.DisplayAlert(
                            "✅ Agregado al Carrito",
                            mensaje,
                            "OK"
                        );
                    });
                }
                else
                {
                    TextoEscuchado = "❌ No encontré ese producto en el menú";
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MenuVM] Error al procesar voz: {ex.Message}");
                TextoEscuchado = "❌ Error al procesar pedido";
            }
        }

        /// <summary>
        /// Manejador del evento cuando ocurre un error en el reconocimiento
        /// </summary>
        private void OnSpeechError(object? sender, string error)
        {
            System.Diagnostics.Debug.WriteLine($"[MenuVM] Error de voz: {error}");
            TextoEscuchado = $"❌ Error: {error}";
            EstaEscuchando = false;
        }
    }

    /// <summary>
    /// Clase auxiliar para agrupar ítems del menú por categoría
    /// </summary>
    public class GrupoMenu : ObservableCollection<MenuItemModel>
    {
        public string Categoria { get; set; }
        public int CantidadItems => Count;

        public GrupoMenu(string categoria, IEnumerable<MenuItemModel> items) : base(items)
        {
            Categoria = categoria;
        }
    }
}

