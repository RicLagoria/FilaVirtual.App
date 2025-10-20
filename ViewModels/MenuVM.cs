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

        [ObservableProperty]
        private bool _estaCargando;

        [ObservableProperty]
        private string _mensajeError = string.Empty;

        /// <summary>
        /// Ítems del menú agrupados por categoría
        /// </summary>
        public ObservableCollection<GrupoMenu> MenuAgrupado { get; } = new();

        public MenuVM(IMenuService menuService, CartVM cartVM)
        {
            _menuService = menuService;
            _cartVM = cartVM;
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
                var menuPorCategoria = await _menuService.ObtenerMenuPorCategoriaAsync();

                // Actualizar la colección
                MenuAgrupado.Clear();
                foreach (var grupo in menuPorCategoria)
                {
                    MenuAgrupado.Add(new GrupoMenu(grupo.Key, grupo.Value));
                }
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

