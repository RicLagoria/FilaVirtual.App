namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Interfaz para el servicio de interpretación de pedidos por voz
    /// </summary>
    public interface IVoiceOrderService
    {
        /// <summary>
        /// Interpreta un texto reconocido y extrae los items del pedido
        /// </summary>
        /// <param name="textoVoz">Texto reconocido por el servicio de voz</param>
        /// <returns>Lista de items del pedido interpretados</returns>
        Task<List<VoiceOrderItem>> InterpretarPedidoAsync(string textoVoz);
    }

    /// <summary>
    /// Representa un item del pedido reconocido por voz
    /// </summary>
    public class VoiceOrderItem
    {
        /// <summary>
        /// Nombre del producto reconocido
        /// </summary>
        public string NombreProducto { get; set; } = string.Empty;

        /// <summary>
        /// Cantidad solicitada (por defecto 1)
        /// </summary>
        public int Cantidad { get; set; } = 1;

        /// <summary>
        /// Nivel de confianza de la interpretación (0.0 a 1.0)
        /// </summary>
        public float Confianza { get; set; } = 1.0f;
    }
}

