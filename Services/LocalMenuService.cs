using FilaVirtual.App.Models;
using System.Text.Json;

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Implementación del servicio de menú usando almacenamiento local SQLite
    /// </summary>
    public class LocalMenuService : IMenuService
    {
        private readonly IStorage _storage;

        public LocalMenuService(IStorage storage)
        {
            _storage = storage;
        }

        public async Task CargarDatosInicialesAsync()
        {
            // Verificar si ya hay datos en la base de datos
            var tieneDatos = await _storage.TieneDatosAsync<MenuItemModel>();
            
            if (!tieneDatos)
            {
                // Cargar datos desde seed.json
                var seedData = await CargarSeedDataAsync();
                if (seedData != null && seedData.Count > 0)
                {
                    await _storage.InsertarTodosAsync(seedData);
                }
            }
        }

        public async Task<List<MenuItemModel>> ObtenerMenuCompletoAsync()
        {
            return await _storage.ObtenerTodosAsync<MenuItemModel>();
        }

        public async Task<Dictionary<string, List<MenuItemModel>>> ObtenerMenuPorCategoriaAsync()
        {
            var items = await ObtenerMenuCompletoAsync();
            return items
                .Where(i => i.Disponible)
                .GroupBy(i => i.Categoria)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.Nombre).ToList());
        }

        public async Task<MenuItemModel?> ObtenerItemPorIdAsync(int id)
        {
            return await _storage.ObtenerPorIdAsync<MenuItemModel>(id);
        }

        public async Task<List<string>> ObtenerCategoriasAsync()
        {
            var items = await ObtenerMenuCompletoAsync();
            return items
                .Where(i => i.Disponible)
                .Select(i => i.Categoria)
                .Distinct()
                .OrderBy(c => c)
                .ToList();
        }

        /// <summary>
        /// Carga los datos de seed.json embebido en el proyecto
        /// </summary>
        private async Task<List<MenuItemModel>> CargarSeedDataAsync()
        {
            try
            {
                using var stream = await FileSystem.OpenAppPackageFileAsync("seed.json");
                using var reader = new StreamReader(stream);
                var json = await reader.ReadToEndAsync();

                var seedData = JsonSerializer.Deserialize<SeedData>(json);
                return seedData?.Menu ?? new List<MenuItemModel>();
            }
            catch (Exception ex)
            {
                // Log del error (en producción usaríamos ILogger)
                System.Diagnostics.Debug.WriteLine($"Error al cargar seed data: {ex.Message}");
                return new List<MenuItemModel>();
            }
        }

        /// <summary>
        /// Clase auxiliar para deserializar el archivo seed.json
        /// </summary>
        private class SeedData
        {
            public List<MenuItemModel> Menu { get; set; } = new();
        }
    }
}

