using SQLite;
using FilaVirtual.App.Models;

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Implementación del servicio de almacenamiento usando SQLite
    /// </summary>
    public class SQLiteStorage : IStorage
    {
        private SQLiteAsyncConnection? _database;
        private readonly string _dbPath;

        public SQLiteStorage()
        {
            // Ruta de la base de datos en el almacenamiento local de la app
            _dbPath = Path.Combine(FileSystem.AppDataDirectory, "filavirtual.db3");
        }

        /// <summary>
        /// Obtiene la conexión a la base de datos, creándola si no existe
        /// </summary>
        private async Task<SQLiteAsyncConnection> ObtenerConexionAsync()
        {
            if (_database != null)
                return _database;

            _database = new SQLiteAsyncConnection(_dbPath);
            await InicializarAsync();
            return _database;
        }

        public async Task InicializarAsync()
        {
            if (_database == null)
                _database = new SQLiteAsyncConnection(_dbPath);

            // Crear tablas si no existen
            await _database.CreateTableAsync<MenuItemModel>();
            
            // Aquí se agregarán más tablas en próximos sprints:
            // await _database.CreateTableAsync<Order>();
            // await _database.CreateTableAsync<OrderItem>();
        }

        public async Task<bool> TieneDatosAsync<T>() where T : new()
        {
            var db = await ObtenerConexionAsync();
            var count = await db.Table<T>().CountAsync();
            return count > 0;
        }

        public async Task<int> InsertarAsync<T>(T item)
        {
            var db = await ObtenerConexionAsync();
            return await db.InsertAsync(item);
        }

        public async Task<int> InsertarTodosAsync<T>(IEnumerable<T> items)
        {
            var db = await ObtenerConexionAsync();
            return await db.InsertAllAsync(items);
        }

        public async Task<List<T>> ObtenerTodosAsync<T>() where T : new()
        {
            var db = await ObtenerConexionAsync();
            return await db.Table<T>().ToListAsync();
        }

        public async Task<T?> ObtenerPorIdAsync<T>(int id) where T : new()
        {
            var db = await ObtenerConexionAsync();
            return await db.FindAsync<T>(id);
        }

        public async Task<int> ActualizarAsync<T>(T item)
        {
            var db = await ObtenerConexionAsync();
            return await db.UpdateAsync(item);
        }

        public async Task<int> EliminarAsync<T>(T item)
        {
            var db = await ObtenerConexionAsync();
            return await db.DeleteAsync(item);
        }
    }
}

