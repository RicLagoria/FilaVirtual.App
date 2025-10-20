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
            System.Diagnostics.Debug.WriteLine("=== INICIO CargarDatosInicialesAsync ===");
            
            // Verificar si ya hay datos en la base de datos
            var tieneDatos = await _storage.TieneDatosAsync<MenuItemModel>();
            System.Diagnostics.Debug.WriteLine($"TieneDatos: {tieneDatos}");
            
            if (!tieneDatos)
            {
                System.Diagnostics.Debug.WriteLine("No hay datos, cargando seed...");
                // Cargar datos desde seed.json
                var seedData = await CargarSeedDataAsync();
                System.Diagnostics.Debug.WriteLine($"SeedData cargado: {seedData?.Count ?? 0} items");
                
                if (seedData != null && seedData.Count > 0)
                {
                    await _storage.InsertarTodosAsync(seedData);
                    System.Diagnostics.Debug.WriteLine("Datos insertados en BD");
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Ya hay datos en BD, saltando carga inicial");
            }
            
            System.Diagnostics.Debug.WriteLine("=== FIN CargarDatosInicialesAsync ===");
        }

        public async Task<List<MenuItemModel>> ObtenerMenuCompletoAsync()
        {
            return await _storage.ObtenerTodosAsync<MenuItemModel>();
        }

        public Task<Dictionary<string, List<MenuItemModel>>> ObtenerMenuPorCategoriaAsync()
        {
            // Por ahora, usar datos hardcodeados para asegurar que funcione
            // En Sprint 3 se implementará la carga desde SQLite con await
            var items = CrearDatosEjemplo();
            
            var resultado = items
                .Where(i => i.Disponible)
                .GroupBy(i => i.Categoria)
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key, g => g.OrderBy(i => i.Nombre).ToList());
            
            return Task.FromResult(resultado);
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
                // Intentar primero con FileSystem.OpenAppPackageFileAsync
                try
                {
                    using var stream = await FileSystem.OpenAppPackageFileAsync("seed.json");
                    using var reader = new StreamReader(stream);
                    var json = await reader.ReadToEndAsync();
                    var seedData = JsonSerializer.Deserialize<SeedData>(json);
                    return seedData?.Menu ?? new List<MenuItemModel>();
                }
                catch
                {
                    // Fallback: leer directamente desde el directorio de la aplicación
                    var appDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    var seedPath = Path.Combine(appDirectory, "seed.json");
                    
                    if (File.Exists(seedPath))
                    {
                        var json = await File.ReadAllTextAsync(seedPath);
                        var seedData = JsonSerializer.Deserialize<SeedData>(json);
                        return seedData?.Menu ?? new List<MenuItemModel>();
                    }
                    
                    // Si no existe, crear datos de ejemplo directamente
                    return CrearDatosEjemplo();
                }
            }
            catch (Exception ex)
            {
                // Log del error (en producción usaríamos ILogger)
                System.Diagnostics.Debug.WriteLine($"Error al cargar seed data: {ex.Message}");
                return CrearDatosEjemplo();
            }
        }

        /// <summary>
        /// Crea datos de ejemplo directamente en código como fallback
        /// </summary>
        private List<MenuItemModel> CrearDatosEjemplo()
        {
            return new List<MenuItemModel>
            {
                new MenuItemModel { Id = 1, Categoria = "Café", Nombre = "Espresso", Precio = 900, Disponible = true },
                new MenuItemModel { Id = 2, Categoria = "Café", Nombre = "Café con Leche", Precio = 1200, Disponible = true },
                new MenuItemModel { Id = 3, Categoria = "Café", Nombre = "Capuccino", Precio = 1300, Disponible = true },
                new MenuItemModel { Id = 4, Categoria = "Café", Nombre = "Café Americano", Precio = 1000, Disponible = true },
                new MenuItemModel { Id = 5, Categoria = "Bebidas", Nombre = "Agua 500 ml", Precio = 700, Disponible = true },
                new MenuItemModel { Id = 6, Categoria = "Bebidas", Nombre = "Agua con gas 500 ml", Precio = 750, Disponible = true },
                new MenuItemModel { Id = 7, Categoria = "Bebidas", Nombre = "Jugo de frutas", Precio = 1100, Disponible = true },
                new MenuItemModel { Id = 8, Categoria = "Bebidas", Nombre = "Gaseosa lata", Precio = 950, Disponible = true },
                new MenuItemModel { Id = 9, Categoria = "Pastelería", Nombre = "Medialuna", Precio = 500, Disponible = true },
                new MenuItemModel { Id = 10, Categoria = "Pastelería", Nombre = "Tostada", Precio = 900, Disponible = true },
                new MenuItemModel { Id = 11, Categoria = "Pastelería", Nombre = "Croissant", Precio = 1200, Disponible = true },
                new MenuItemModel { Id = 12, Categoria = "Pastelería", Nombre = "Alfajor", Precio = 800, Disponible = true },
                new MenuItemModel { Id = 13, Categoria = "Sándwiches", Nombre = "JyQ", Precio = 2200, Disponible = true },
                new MenuItemModel { Id = 14, Categoria = "Sándwiches", Nombre = "Milanesa", Precio = 2800, Disponible = true },
                new MenuItemModel { Id = 15, Categoria = "Sándwiches", Nombre = "Vegetariano", Precio = 2400, Disponible = true }
            };
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

