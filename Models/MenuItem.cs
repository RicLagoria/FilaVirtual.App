using SQLite;

namespace FilaVirtual.App.Models
{
    /// <summary>
    /// Representa un ítem del menú de la cantina
    /// </summary>
    [Table("MenuItems")]
    public class MenuItemModel
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [NotNull, MaxLength(50)]
        public string Categoria { get; set; } = string.Empty;

        [NotNull, MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [NotNull]
        public decimal Precio { get; set; }

        /// <summary>
        /// Indica si el ítem está disponible para la venta
        /// </summary>
        public bool Disponible { get; set; } = true;

        /// <summary>
        /// Formato de precio con cultura es-AR
        /// </summary>
        [Ignore]
        public string PrecioFormateado => Precio.ToString("C");
    }
}

