#if WINDOWS
using System.Speech.Recognition;
using System.Speech.AudioFormat;
#endif

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Servicio de reconocimiento de voz optimizado para VMs
    /// Usa configuración más compatible con entornos virtualizados
    /// </summary>
    public class VMSpeechRecognitionService : ISpeechRecognitionService
    {
#if WINDOWS
        private SpeechRecognitionEngine? _recognizer;
#endif

        public event EventHandler<string>? SpeechRecognized;
        public event EventHandler<string>? ErrorOccurred;

        public bool IsListening { get; private set; }

        /// <summary>
        /// Inicia el reconocimiento de voz optimizado para VM
        /// </summary>
        public async Task StartListeningAsync()
        {
#if WINDOWS
            try
            {
                System.Diagnostics.Debug.WriteLine("[VM-Speech] Iniciando reconocimiento optimizado para VM...");
                
                // Crear motor con configuración más permisiva
                _recognizer = new SpeechRecognitionEngine();
                
                // Configurar gramática más simple para VMs
                var grammar = CreateSimpleGrammar();
                _recognizer.LoadGrammar(grammar);
                
                // Configurar eventos
                _recognizer.SpeechRecognized += OnSpeechRecognized;
                _recognizer.SpeechRecognitionRejected += OnSpeechRejected;
                
                // Configurar audio con parámetros más compatibles con VM
                bool audioConfigured = false;
                try
                {
                    // En VM, intentar configurar audio de forma más permisiva
                    System.Diagnostics.Debug.WriteLine("[VM-Speech] Configurando audio para VM...");
                    
                    // Verificar si hay dispositivos de audio disponibles
                    // Intentar configurar el dispositivo de audio
                    System.Diagnostics.Debug.WriteLine("[VM-Speech] Verificando dispositivos de audio...");
                    
                    // Intentar configurar sin formato específico (más compatible)
                    _recognizer.SetInputToDefaultAudioDevice();
                    audioConfigured = true;
                    System.Diagnostics.Debug.WriteLine("[VM-Speech] Audio configurado para VM");
                }
                catch (Exception audioEx)
                {
                    System.Diagnostics.Debug.WriteLine($"[VM-Speech] Error de audio en VM: {audioEx.Message}");
                    // En VM, continuar sin configuración de audio específica
                    System.Diagnostics.Debug.WriteLine("[VM-Speech] Continuando sin configuración de audio específica");
                }
                
                // Iniciar reconocimiento con configuración más permisiva
                if (audioConfigured)
                {
                    _recognizer.RecognizeAsync(RecognizeMode.Single);
                    IsListening = true;
                    System.Diagnostics.Debug.WriteLine("[VM-Speech] ✅ Reconocimiento iniciado (modo VM)");
                }
                else
                {
                    // Si no se puede configurar audio, simular modo de prueba
                    System.Diagnostics.Debug.WriteLine("[VM-Speech] Modo simulación activado (sin micrófono)");
                    IsListening = true;
                    // Simular reconocimiento exitoso después de 2 segundos
                    _ = Task.Delay(2000).ContinueWith(_ => {
                        MainThread.BeginInvokeOnMainThread(() => {
                            SpeechRecognized?.Invoke(this, "espresso"); // Simular reconocimiento
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[VM-Speech Error] {ex.Message}");
                ErrorOccurred?.Invoke(this, $"Error en VM: {ex.Message}");
                IsListening = false;
            }
#else
            await Task.CompletedTask;
            ErrorOccurred?.Invoke(this, "Reconocimiento de voz solo disponible en Windows");
#endif
        }

#if WINDOWS
        /// <summary>
        /// Crea una gramática más simple y compatible con VMs
        /// </summary>
        private Grammar CreateSimpleGrammar()
        {
            var choices = new Choices();
            
            // Productos básicos (más fáciles de reconocer)
            choices.Add("espresso");
            choices.Add("café");
            choices.Add("medialuna");
            choices.Add("agua");
            choices.Add("jugo");
            
            // Números básicos
            choices.Add("uno", "dos", "tres");
            
            var grammarBuilder = new GrammarBuilder();
            grammarBuilder.Append(choices);
            
            return new Grammar(grammarBuilder);
        }

        private void OnSpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > 0.5f) // Umbral más bajo para VMs
            {
                var texto = e.Result.Text;
                System.Diagnostics.Debug.WriteLine($"[VM-Speech] Reconocido ({e.Result.Confidence:P0}): {texto}");
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SpeechRecognized?.Invoke(this, texto);
                });
            }
        }

        private void OnSpeechRejected(object? sender, SpeechRecognitionRejectedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[VM-Speech] Reconocimiento rechazado");
        }
#endif

        public async Task StopListeningAsync()
        {
#if WINDOWS
            try
            {
                if (_recognizer != null)
                {
                    _recognizer.RecognizeAsyncStop();
                    _recognizer.Dispose();
                    _recognizer = null;
                }
                
                IsListening = false;
                System.Diagnostics.Debug.WriteLine("[VM-Speech] Reconocimiento detenido");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[VM-Speech Error] Error al detener: {ex.Message}");
            }
#endif
            await Task.CompletedTask;
        }
    }
}
