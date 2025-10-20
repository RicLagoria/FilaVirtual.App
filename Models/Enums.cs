namespace FilaVirtual.App.Models
{
    /// <summary>
    /// Estados posibles de un pedido en el sistema
    /// </summary>
    public enum EstadoPedido
    {
        EnCola,
        EnPreparacion,
        Listo
    }

    /// <summary>
    /// Niveles de prioridad para la cola
    /// ACC > EMB > DOC > STD
    /// </summary>
    public enum TipoPrioridad
    {
        ACC = 1,  // Accesibilidad (máxima prioridad)
        EMB = 2,  // Embarazadas
        DOC = 3,  // Docentes
        STD = 4   // Estudiantes (estándar)
    }

    /// <summary>
    /// Categorías del menú
    /// </summary>
    public enum CategoriaMenu
    {
        Cafe,
        Bebidas,
        Pasteleria,
        Sandwiches
    }
}

