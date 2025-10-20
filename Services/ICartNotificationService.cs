namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Servicio para notificar cambios en el carrito
    /// </summary>
    public interface ICartNotificationService
    {
        /// <summary>
        /// Registra la referencia al tab del carrito
        /// </summary>
        void RegistrarCarritoTab(object carritoTab);

        /// <summary>
        /// Actualiza el título del tab del carrito con la cantidad de items
        /// </summary>
        void ActualizarTituloCarrito(int cantidadItems);
    }
}
