using SQLite;

namespace FilaVirtual.App.Models
{
    /// <summary>
    /// Representa un ítem dentro de un pedido
    /// </summary>
    [Table("OrderItems")]
    public class OrderItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        /// <summary>
        /// ID del pedido al que pertenece este ítem
        /// </summary>
        [NotNull]
        public int OrderId { get; set; }

        /// <summary>
        /// ID del ítem del menú
        /// </summary>
        [NotNull]
        public int MenuItemId { get; set; }

        /// <summary>
        /// Nombre del ítem (snapshot al momento del pedido)
        /// </summary>
        [NotNull, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Precio unitario al momento del pedido (en centavos)
        /// </summary>
        public decimal PrecioUnitario { get; set; }

        /// <summary>
        /// Cantidad del ítem
        /// </summary>
        public int Cantidad { get; set; } = 1;

        /// <summary>
        /// Subtotal (PrecioUnitario * Cantidad)
        /// </summary>
        [Ignore]
        public decimal Subtotal => PrecioUnitario * Cantidad;

        /// <summary>
        /// Formato de precio unitario con cultura es-AR
        /// </summary>
        [Ignore]
        public string PrecioUnitarioFormateado => PrecioUnitario.ToString("C");

        /// <summary>
        /// Formato de subtotal con cultura es-AR
        /// </summary>
        [Ignore]
        public string SubtotalFormateado => Subtotal.ToString("C");
    }
}

