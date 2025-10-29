#if WINDOWS
using System.Speech.Recognition;
using System.Globalization;
#endif

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Servicio de reconocimiento de voz simple y directo para Windows 11
    /// Sin detección de VM ni configuraciones complejas
    /// </summary>
    public class SimpleSpeechService : ISpeechRecognitionService
    {
#if WINDOWS
        private SpeechRecognitionEngine? _recognizer;
#endif

        public event EventHandler<string>? SpeechRecognized;
        public event EventHandler<string>? ErrorOccurred;

        public bool IsListening { get; private set; }

        /// <summary>
        /// Inicia el reconocimiento de voz de forma simple
        /// </summary>
        public async Task StartListeningAsync()
        {
#if WINDOWS
            try
            {
                System.Diagnostics.Debug.WriteLine("[SimpleSpeech] Iniciando reconocimiento de voz...");
                
                // Ejecutar la inicialización en un hilo separado para evitar bloquear la UI
                await Task.Run(() =>
                {
                    // Detectar idioma disponible para reconocimiento de voz
                    var culture = GetAvailableSpeechCulture();
                    System.Diagnostics.Debug.WriteLine($"[SimpleSpeech] Usando idioma: {culture.Name}");
                    
                    // Crear motor de reconocimiento con idioma detectado
                    _recognizer = new SpeechRecognitionEngine(culture);
                    
                    // Configurar gramática simple
                    var grammar = CreateSimpleGrammar();
                    _recognizer.LoadGrammar(grammar);
                    
                    // Configurar eventos
                    _recognizer.SpeechRecognized += OnSpeechRecognized;
                    _recognizer.SpeechRecognitionRejected += OnSpeechRejected;
                    
                    // Configurar audio
                    _recognizer.SetInputToDefaultAudioDevice();
                });
                
                // Iniciar reconocimiento en el hilo principal
                if (_recognizer != null)
                {
                    _recognizer.RecognizeAsync(RecognizeMode.Multiple);
                }
                
                IsListening = true;
                System.Diagnostics.Debug.WriteLine("[SimpleSpeech] ✅ Reconocimiento iniciado correctamente");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SimpleSpeech] Error: {ex.Message}");
                ErrorOccurred?.Invoke(this, $"Error al iniciar micrófono: {ex.Message}");
                IsListening = false;
            }
#else
            await Task.CompletedTask;
            ErrorOccurred?.Invoke(this, "Reconocimiento de voz solo disponible en Windows");
#endif
        }

#if WINDOWS
        /// <summary>
        /// Detecta un idioma disponible para reconocimiento de voz
        /// </summary>
        private CultureInfo GetAvailableSpeechCulture()
        {
            // Lista de idiomas preferidos en orden de prioridad
            var preferredCultures = new[]
            {
                "es-AR", // Español Argentina (preferido)
                "es-ES", // Español España
                "es-MX", // Español México
                "es",    // Español genérico
                "en-US", // Inglés Estados Unidos (fallback)
                "en"     // Inglés genérico (último recurso)
            };

            foreach (var cultureName in preferredCultures)
            {
                try
                {
                    var culture = new CultureInfo(cultureName);
                    // Intentar crear un reconocedor temporal para verificar disponibilidad
                    using (var testRecognizer = new SpeechRecognitionEngine(culture))
                    {
                        System.Diagnostics.Debug.WriteLine($"[SimpleSpeech] Idioma {cultureName} disponible");
                        return culture;
                    }
                }
                catch (ArgumentException)
                {
                    System.Diagnostics.Debug.WriteLine($"[SimpleSpeech] Idioma {cultureName} no disponible");
                    continue;
                }
            }

            // Si ningún idioma específico funciona, usar el idioma por defecto del sistema
            System.Diagnostics.Debug.WriteLine("[SimpleSpeech] Usando idioma por defecto del sistema");
            return CultureInfo.CurrentCulture;
        }

        /// <summary>
        /// Crea una gramática simple con productos del menú
        /// </summary>
        private Grammar CreateSimpleGrammar()
        {
            var choices = new Choices();
            
            // Productos del menú
            choices.Add("espresso", "expreso", "café");
            choices.Add("café con leche", "cortado");
            choices.Add("capuccino", "capuchino");
            choices.Add("agua", "agua mineral");
            choices.Add("jugo", "jugo de frutas");
            choices.Add("medialuna", "medialunas");
            choices.Add("tostada", "tostadas");
            choices.Add("sándwich", "sandwich", "jamón y queso");
            
            // Números
            choices.Add("uno", "una", "un", "1");
            choices.Add("dos", "2");
            choices.Add("tres", "3");
            choices.Add("cuatro", "4");
            choices.Add("cinco", "5");
            
            // Palabras de conexión
            choices.Add("quiero", "quisiera", "dame", "pido", "quería");
            choices.Add("y", "con", "más");
            
            var grammarBuilder = new GrammarBuilder();
            // Usar el mismo idioma que el reconocedor
            grammarBuilder.Culture = _recognizer?.RecognizerInfo?.Culture ?? CultureInfo.CurrentCulture;
            grammarBuilder.Append(choices);
            
            return new Grammar(grammarBuilder);
        }

        private void OnSpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > 0.6f)
            {
                var texto = e.Result.Text;
                System.Diagnostics.Debug.WriteLine($"[SimpleSpeech] Reconocido ({e.Result.Confidence:P0}): {texto}");
                
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SpeechRecognized?.Invoke(this, texto);
                });
            }
        }

        private void OnSpeechRejected(object? sender, SpeechRecognitionRejectedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[SimpleSpeech] Reconocimiento rechazado");
        }
#endif

        /// <summary>
        /// Detiene el reconocimiento de voz
        /// </summary>
        public async Task StopListeningAsync()
        {
#if WINDOWS
            try
            {
                await Task.Run(() =>
                {
                    if (_recognizer != null)
                    {
                        _recognizer.RecognizeAsyncStop();
                        _recognizer.Dispose();
                        _recognizer = null;
                    }
                });
                
                IsListening = false;
                System.Diagnostics.Debug.WriteLine("[SimpleSpeech] Reconocimiento detenido");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[SimpleSpeech] Error al detener: {ex.Message}");
            }
#else
            await Task.CompletedTask;
#endif
        }
    }
}
