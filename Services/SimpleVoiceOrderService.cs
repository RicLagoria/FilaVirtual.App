using System.Text.RegularExpressions;

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Implementación simple del servicio de interpretación de pedidos por voz
    /// Usa pattern matching para identificar productos y cantidades
    /// </summary>
    public class SimpleVoiceOrderService : IVoiceOrderService
    {
        /// <summary>
        /// Interpreta el texto reconocido y extrae los items del pedido
        /// </summary>
        public async Task<List<VoiceOrderItem>> InterpretarPedidoAsync(string textoVoz)
        {
            var items = new List<VoiceOrderItem>();
            
            if (string.IsNullOrWhiteSpace(textoVoz))
                return items;

            var texto = textoVoz.ToLower().Trim();
            System.Diagnostics.Debug.WriteLine($"[VoiceOrder] Interpretando: '{texto}'");

            // Extraer cantidad del texto (por defecto 1)
            int cantidad = ExtraerCantidad(texto);

            // Detectar productos del menú
            var productosDetectados = DetectarProductos(texto);

            // Crear items del pedido
            foreach (var producto in productosDetectados)
            {
                items.Add(new VoiceOrderItem
                {
                    NombreProducto = producto,
                    Cantidad = cantidad,
                    Confianza = 0.9f
                });
                
                System.Diagnostics.Debug.WriteLine($"[VoiceOrder] Detectado: {cantidad}x {producto}");
            }

            return await Task.FromResult(items);
        }

        /// <summary>
        /// Extrae la cantidad del texto reconocido
        /// </summary>
        private int ExtraerCantidad(string texto)
        {
            // Buscar números escritos
            if (texto.Contains("dos") || texto.Contains("2")) return 2;
            if (texto.Contains("tres") || texto.Contains("3")) return 3;
            if (texto.Contains("cuatro") || texto.Contains("4")) return 4;
            if (texto.Contains("cinco") || texto.Contains("5")) return 5;
            if (texto.Contains("seis") || texto.Contains("6")) return 6;

            // Buscar números con regex
            var match = Regex.Match(texto, @"\b(\d+)\b");
            if (match.Success && int.TryParse(match.Value, out int numero))
            {
                return numero;
            }

            // Por defecto 1
            return 1;
        }

        /// <summary>
        /// Detecta qué productos del menú se mencionan en el texto
        /// </summary>
        private List<string> DetectarProductos(string texto)
        {
            var productos = new List<string>();

            // Mapeo de palabras clave a productos del menú
            var mapaProductos = new Dictionary<string, List<string>>
            {
                { "Espresso", new List<string> { "espresso", "expreso", "café espresso" } },
                { "Café con Leche", new List<string> { "café con leche", "cortado", "café cortado" } },
                { "Capuccino", new List<string> { "capuccino", "capuchino", "cappuccino" } },
                { "Agua 500 ml", new List<string> { "agua", "agua mineral" } },
                { "Jugo de frutas", new List<string> { "jugo", "jugo de frutas", "juguito" } },
                { "Medialuna", new List<string> { "medialuna", "medialunas", "croissant" } },
                { "Tostada", new List<string> { "tostada", "tostadas", "pan tostado" } },
                { "JyQ", new List<string> { "jamón y queso", "jamón queso", "sandwich", "sándwich", "jyq" } }
            };

            // Buscar cada producto en el texto
            foreach (var kvp in mapaProductos)
            {
                var nombreProducto = kvp.Key;
                var palabrasClave = kvp.Value;

                foreach (var palabra in palabrasClave)
                {
                    if (texto.Contains(palabra))
                    {
                        if (!productos.Contains(nombreProducto))
                        {
                            productos.Add(nombreProducto);
                            break; // Evitar duplicados
                        }
                    }
                }
            }

            return productos;
        }
    }
}

