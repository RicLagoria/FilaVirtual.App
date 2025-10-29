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

        [ObservableProperty]
        private string _estadoMic = string.Empty;

        [ObservableProperty]
        private Color _colorEstado = Colors.Gray;

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
                     ISpeechRecognitionService speechService)
        {
            _menuService = menuService;
            _cartVM = cartVM;
            _speechService = speechService;
            
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
                // Detener micrófono
                await _speechService.StopListeningAsync();
                EstaEscuchando = false;
                TextoEscuchado = string.Empty;
                EstadoMic = "Micrófono detenido";
                ColorEstado = Colors.Gray;
                System.Diagnostics.Debug.WriteLine("[MenuVM] Micrófono detenido");
            }
            else
            {
                // Iniciar micrófono
                EstadoMic = "Iniciando...";
                ColorEstado = Colors.Orange;
                TextoEscuchado = "Configurando...";
                
                try
                {
                    // Iniciar reconocimiento
                    await _speechService.StartListeningAsync();
                    
                    // Actualizar estado
                    EstaEscuchando = _speechService.IsListening;
                    
                    if (EstaEscuchando)
                    {
                        TextoEscuchado = "🎤 Escuchando... di tu pedido";
                        EstadoMic = "Escuchando";
                        ColorEstado = Colors.Green;
                        System.Diagnostics.Debug.WriteLine("[MenuVM] ✅ Micrófono activado");
                    }
                    else
                    {
                        TextoEscuchado = "❌ Error al activar micrófono";
                        EstadoMic = "Error";
                        ColorEstado = Colors.Red;
                        System.Diagnostics.Debug.WriteLine("[MenuVM] ❌ Error al activar micrófono");
                    }
                }
                catch (Exception ex)
                {
                    EstaEscuchando = false;
                    TextoEscuchado = $"❌ Error: {ex.Message}";
                    EstadoMic = "Error";
                    ColorEstado = Colors.Red;
                    System.Diagnostics.Debug.WriteLine($"[MenuVM] ❌ Excepción: {ex.Message}");
                }
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
                // Buscar productos mencionados en el texto
                var productosEncontrados = BuscarProductosEnTexto(texto.ToLower());
                
                if (productosEncontrados.Count == 0)
                {
                    TextoEscuchado = "❌ No encontré productos del menú en tu pedido";
                    return;
                }

                // Agregar productos al carrito
                int itemsAgregados = 0;
                foreach (var producto in productosEncontrados)
                {
                    _cartVM.AgregarItem(producto);
                    itemsAgregados++;
                    System.Diagnostics.Debug.WriteLine($"[MenuVM] Agregado: {producto.Nombre}");
                }

                // Mostrar confirmación
                TextoEscuchado = $"✅ {itemsAgregados} producto(s) agregados al carrito";
                
                // Mostrar confirmación visual
                await MostrarConfirmacionAgregado($"{itemsAgregados} producto(s)");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[MenuVM] Error al procesar voz: {ex.Message}");
                TextoEscuchado = "❌ Error al procesar pedido";
            }
        }

        /// <summary>
        /// Busca productos del menú mencionados en el texto reconocido
        /// </summary>
        private List<MenuItemModel> BuscarProductosEnTexto(string texto)
        {
            var productosEncontrados = new List<MenuItemModel>();
            
            // Mapeo de palabras clave a productos del menú
            var mapaProductos = new Dictionary<string, List<string>>
            {
                { "Espresso", new List<string> { "espresso", "expreso", "café espresso" } },
                { "Café con Leche", new List<string> { "café con leche", "cortado", "café cortado", "café" } },
                { "Capuccino", new List<string> { "capuccino", "capuchino", "cappuccino" } },
                { "Agua 500 ml", new List<string> { "agua", "agua mineral" } },
                { "Jugo de frutas", new List<string> { "jugo", "jugo de frutas", "juguito" } },
                { "Medialuna", new List<string> { "medialuna", "medialunas", "croissant" } },
                { "Tostada", new List<string> { "tostada", "tostadas", "pan tostado" } },
                { "JyQ", new List<string> { "jamón y queso", "jamón queso", "sandwich", "sándwich", "jyq" } }
            };

            // Buscar cada producto en el texto
            foreach (var grupo in MenuAgrupado)
            {
                foreach (var item in grupo)
                {
                    // Verificar si el nombre del producto está en el texto
                    if (texto.Contains(item.Nombre.ToLower()))
                    {
                        if (!productosEncontrados.Any(p => p.Nombre == item.Nombre))
                        {
                            productosEncontrados.Add(item);
                        }
                        continue;
                    }

                    // Verificar palabras clave alternativas
                    if (mapaProductos.ContainsKey(item.Nombre))
                    {
                        var palabrasClave = mapaProductos[item.Nombre];
                        foreach (var palabra in palabrasClave)
                        {
                            if (texto.Contains(palabra))
                            {
                                if (!productosEncontrados.Any(p => p.Nombre == item.Nombre))
                                {
                                    productosEncontrados.Add(item);
                                }
                                break;
                            }
                        }
                    }
                }
            }

            return productosEncontrados;
        }

        /// <summary>
        /// Manejador del evento cuando ocurre un error en el reconocimiento
        /// </summary>
        private void OnSpeechError(object? sender, string error)
        {
            System.Diagnostics.Debug.WriteLine($"[MenuVM] Error de voz: {error}");
            TextoEscuchado = $"❌ Error: {error}";
            EstadoMic = "Error";
            ColorEstado = Colors.Red;
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

