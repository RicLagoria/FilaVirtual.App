using SQLite;

namespace FilaVirtual.App.Models
{
    /// <summary>
    /// Representa un pedido en el sistema
    /// </summary>
    [Table("Orders")]
    public class Order
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// ID único del pedido (para QR y seguimiento)
        /// </summary>
        [NotNull, MaxLength(50)]
        public string OrderId { get; set; } = string.Empty;

        /// <summary>
        /// Estado actual del pedido
        /// </summary>
        public EstadoPedido Estado { get; set; } = EstadoPedido.EnCola;

        /// <summary>
        /// Tipo de prioridad del cliente
        /// </summary>
        public TipoPrioridad TipoPrioridad { get; set; } = TipoPrioridad.STD;

        /// <summary>
        /// Total del pedido en centavos
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Fecha y hora de creación del pedido
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Fecha y hora de última actualización
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        /// <summary>
        /// Formato de total con cultura es-AR
        /// </summary>
        [Ignore]
        public string TotalFormateado => Total.ToString("C");

        /// <summary>
        /// Formato de fecha y hora
        /// </summary>
        [Ignore]
        public string FechaFormateada => CreatedAt.ToString("dd/MM/yyyy HH:mm");
    }
}

