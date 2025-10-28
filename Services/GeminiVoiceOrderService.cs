using System.Text.Json;
using System.Text;

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Implementación del servicio de interpretación de pedidos por voz usando Gemini Pro
    /// Más económico que GPT y con excelente rendimiento
    /// </summary>
    public class GeminiVoiceOrderService : IVoiceOrderService
    {
        private readonly HttpClient _httpClient;
        private readonly SimpleVoiceOrderService _fallbackService;
        private readonly string _apiKey;
        private readonly string _apiUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent";

        public GeminiVoiceOrderService()
        {
            // Configurar Gemini Pro
            _apiKey = Environment.GetEnvironmentVariable("GEMINI_API_KEY") 
                     ?? throw new InvalidOperationException("GEMINI_API_KEY no configurada");
            
            _httpClient = new HttpClient();
            _fallbackService = new SimpleVoiceOrderService();
        }

        /// <summary>
        /// Interpreta el texto reconocido usando Gemini Pro con fallback inteligente
        /// </summary>
        public async Task<List<VoiceOrderItem>> InterpretarPedidoAsync(string textoVoz)
        {
            if (string.IsNullOrWhiteSpace(textoVoz))
                return new List<VoiceOrderItem>();

            try
            {
                System.Diagnostics.Debug.WriteLine($"[Gemini] Interpretando: '{textoVoz}'");

                // Crear prompt optimizado para Gemini
                var prompt = CrearPrompt(textoVoz);
                
                // Llamar a Gemini Pro con timeout
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                var response = await LlamarGeminiAPIAsync(prompt, cts.Token);
                
                if (string.IsNullOrEmpty(response))
                {
                    System.Diagnostics.Debug.WriteLine($"[Gemini] ⚠️ Respuesta vacía, usando fallback");
                    return await _fallbackService.InterpretarPedidoAsync(textoVoz);
                }

                System.Diagnostics.Debug.WriteLine($"[Gemini] Respuesta: {response}");

                // Parsear respuesta JSON
                var items = ParsearRespuesta(response);
                
                if (items.Count > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"[Gemini] ✅ Items parseados: {items.Count}");
                    return items;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"[Gemini] ⚠️ No se pudieron parsear items, usando fallback");
                    return await _fallbackService.InterpretarPedidoAsync(textoVoz);
                }
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine($"[Gemini] ⏰ Timeout, usando fallback");
                return await _fallbackService.InterpretarPedidoAsync(textoVoz);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Gemini Error] {ex.Message}, usando fallback");
                return await _fallbackService.InterpretarPedidoAsync(textoVoz);
            }
        }

        /// <summary>
        /// Llama a la API de Gemini Pro usando HTTP client
        /// </summary>
        private async Task<string> LlamarGeminiAPIAsync(string prompt, CancellationToken cancellationToken)
        {
            try
            {
                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.1,
                        maxOutputTokens = 150
                    }
                };

                var json = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var url = $"{_apiUrl}?key={_apiKey}";
                var response = await _httpClient.PostAsync(url, content, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var geminiResponse = JsonSerializer.Deserialize<GeminiResponse>(responseContent);
                    
                    if (geminiResponse?.candidates?.Length > 0)
                    {
                        return geminiResponse.candidates[0].content.parts[0].text;
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    System.Diagnostics.Debug.WriteLine($"[Gemini API Error] {response.StatusCode}: {errorContent}");
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Gemini API Error] {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Crea el prompt optimizado para Gemini Pro
        /// </summary>
        private string CrearPrompt(string textoVoz)
        {
            return $@"Eres un asistente de cantina universitaria. Convierte el siguiente pedido a JSON.

PRODUCTOS DISPONIBLES:
- Espresso, Café con Leche, Capuccino
- Agua 500 ml, Jugo de frutas  
- Medialuna, Tostada, JyQ (jamón y queso)

INSTRUCCIONES:
1. Identifica productos y cantidades del texto
2. Si no hay cantidad específica, asume 1
3. Devuelve SOLO JSON válido
4. Usa nombres exactos de productos

EJEMPLOS:
Input: 'quiero dos cafés'
Output: {{""items"":[{{""producto"":""Café con Leche"",""cantidad"":2}}]}}

Input: 'una medialuna y agua'
Output: {{""items"":[{{""producto"":""Medialuna"",""cantidad"":1}},{{""producto"":""Agua 500 ml"",""cantidad"":1}}]}}

PEDIDO A INTERPRETAR: ""{textoVoz}""

RESPUESTA (solo JSON):";
        }

        /// <summary>
        /// Parsea la respuesta JSON de Gemini
        /// </summary>
        private List<VoiceOrderItem> ParsearRespuesta(string respuesta)
        {
            try
            {
                // Limpiar respuesta (remover markdown si existe)
                var jsonLimpio = respuesta.Trim();
                if (jsonLimpio.StartsWith("```json"))
                {
                    jsonLimpio = jsonLimpio.Substring(7);
                }
                if (jsonLimpio.EndsWith("```"))
                {
                    jsonLimpio = jsonLimpio.Substring(0, jsonLimpio.Length - 3);
                }
                jsonLimpio = jsonLimpio.Trim();

                // Parsear JSON
                using var doc = JsonDocument.Parse(jsonLimpio);
                var root = doc.RootElement;
                
                if (root.TryGetProperty("items", out var itemsArray))
                {
                    var resultado = new List<VoiceOrderItem>();
                    
                    foreach (var item in itemsArray.EnumerateArray())
                    {
                        if (item.TryGetProperty("producto", out var producto) &&
                            item.TryGetProperty("cantidad", out var cantidad))
                        {
                            resultado.Add(new VoiceOrderItem
                            {
                                NombreProducto = producto.GetString() ?? "",
                                Cantidad = cantidad.GetInt32(),
                                Confianza = 0.9f
                            });
                        }
                    }
                    
                    return resultado;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Gemini Parse Error] {ex.Message}");
            }

            return new List<VoiceOrderItem>();
        }
    }

    /// <summary>
    /// Modelo para la respuesta de la API de Gemini
    /// </summary>
    public class GeminiResponse
    {
        public GeminiCandidate[] candidates { get; set; } = Array.Empty<GeminiCandidate>();
    }

    public class GeminiCandidate
    {
        public GeminiContent content { get; set; } = new();
    }

    public class GeminiContent
    {
        public GeminiPart[] parts { get; set; } = Array.Empty<GeminiPart>();
    }

    public class GeminiPart
    {
        public string text { get; set; } = string.Empty;
    }
}

