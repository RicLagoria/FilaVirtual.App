using FilaVirtual.App.Models;

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Interfaz para el servicio de menú
    /// </summary>
    public interface IMenuService
    {
        /// <summary>
        /// Carga los datos iniciales del menú desde seed.json si la BD está vacía
        /// </summary>
        Task CargarDatosInicialesAsync();

        /// <summary>
        /// Obtiene todos los ítems del menú disponibles
        /// </summary>
        Task<List<MenuItemModel>> ObtenerMenuCompletoAsync();

        /// <summary>
        /// Obtiene los ítems del menú agrupados por categoría
        /// </summary>
        Task<Dictionary<string, List<MenuItemModel>>> ObtenerMenuPorCategoriaAsync();

        /// <summary>
        /// Obtiene un ítem específico del menú por su ID
        /// </summary>
        Task<MenuItemModel?> ObtenerItemPorIdAsync(int id);

        /// <summary>
        /// Obtiene todas las categorías disponibles
        /// </summary>
        Task<List<string>> ObtenerCategoriasAsync();
    }
}

