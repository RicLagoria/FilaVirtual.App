namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Interfaz para el servicio de almacenamiento SQLite
    /// </summary>
    public interface IStorage
    {
        /// <summary>
        /// Inicializa la base de datos y crea las tablas necesarias
        /// </summary>
        Task InicializarAsync();

        /// <summary>
        /// Verifica si la base de datos ya tiene datos
        /// </summary>
        Task<bool> TieneDatosAsync<T>() where T : new();

        /// <summary>
        /// Inserta un elemento en la base de datos
        /// </summary>
        Task<int> InsertarAsync<T>(T item);

        /// <summary>
        /// Inserta múltiples elementos en la base de datos
        /// </summary>
        Task<int> InsertarTodosAsync<T>(IEnumerable<T> items);

        /// <summary>
        /// Obtiene todos los elementos de una tabla
        /// </summary>
        Task<List<T>> ObtenerTodosAsync<T>() where T : new();

        /// <summary>
        /// Obtiene un elemento por su ID
        /// </summary>
        Task<T?> ObtenerPorIdAsync<T>(int id) where T : new();

        /// <summary>
        /// Actualiza un elemento existente
        /// </summary>
        Task<int> ActualizarAsync<T>(T item);

        /// <summary>
        /// Elimina un elemento
        /// </summary>
        Task<int> EliminarAsync<T>(T item);
    }
}

