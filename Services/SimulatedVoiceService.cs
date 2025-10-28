namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Servicio de voz simulado para VMs sin micrófono
    /// Permite probar la funcionalidad sin hardware de audio
    /// </summary>
    public class SimulatedVoiceService : ISpeechRecognitionService
    {
        public event EventHandler<string>? SpeechRecognized;
        public event EventHandler<string>? ErrorOccurred;

        public bool IsListening { get; private set; }

        /// <summary>
        /// Inicia el reconocimiento simulado
        /// </summary>
        public async Task StartListeningAsync()
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("[SimulatedVoice] Iniciando reconocimiento simulado...");
                
                IsListening = true;
                System.Diagnostics.Debug.WriteLine("[SimulatedVoice] ✅ Reconocimiento simulado iniciado");
                
                // Simular reconocimiento después de 2 segundos
                _ = Task.Delay(2000).ContinueWith(_ => {
                    MainThread.BeginInvokeOnMainThread(() => {
                        // Simular diferentes pedidos para testing
                        var pedidosSimulados = new[]
                        {
                            "quiero un café",
                            "dame dos medialunas",
                            "pido agua y jugo",
                            "quiero tres cafés y una medialuna",
                            "dame un sandwich"
                        };
                        
                        var pedidoAleatorio = pedidosSimulados[Random.Shared.Next(pedidosSimulados.Length)];
                        System.Diagnostics.Debug.WriteLine($"[SimulatedVoice] Simulando pedido: {pedidoAleatorio}");
                        SpeechRecognized?.Invoke(this, pedidoAleatorio);
                    });
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SimulatedVoice Error] {ex.Message}");
                ErrorOccurred?.Invoke(this, $"Error en simulación: {ex.Message}");
                IsListening = false;
            }
            
            await Task.CompletedTask;
        }

        /// <summary>
        /// Detiene el reconocimiento simulado
        /// </summary>
        public async Task StopListeningAsync()
        {
            try
            {
                IsListening = false;
                System.Diagnostics.Debug.WriteLine("[SimulatedVoice] Reconocimiento simulado detenido");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SimulatedVoice Error] Error al detener: {ex.Message}");
            }
            
            await Task.CompletedTask;
        }
    }
}

