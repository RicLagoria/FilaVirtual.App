#if WINDOWS
using System.Speech.Recognition;
#endif

namespace FilaVirtual.App.Services
{
    /// <summary>
    /// Implementación del servicio de reconocimiento de voz usando Windows Speech Recognition
    /// Solo funciona en Windows 10/11
    /// </summary>
    public class WindowsSpeechRecognitionService : ISpeechRecognitionService
    {
#if WINDOWS
        private SpeechRecognitionEngine? _recognizer;
#endif

        public event EventHandler<string>? SpeechRecognized;
        public event EventHandler<string>? ErrorOccurred;

        public bool IsListening { get; private set; }

        /// <summary>
        /// Inicia el reconocimiento de voz
        /// </summary>
        public async Task StartListeningAsync()
        {
#if WINDOWS
            try
            {
                // Crear el motor de reconocimiento con cultura español Argentina
                _recognizer = new SpeechRecognitionEngine(
                    new System.Globalization.CultureInfo("es-AR")
                );

                // Configurar gramática con productos del menú
                Choices menuItems = new Choices();
                
                // Productos del menú
                menuItems.Add("espresso", "expreso");
                menuItems.Add("café con leche", "cortado");
                menuItems.Add("capuccino", "capuchino");
                menuItems.Add("agua", "agua mineral");
                menuItems.Add("jugo", "jugo de frutas");
                menuItems.Add("medialuna", "medialunas");
                menuItems.Add("tostada", "tostadas");
                menuItems.Add("sándwich", "sandwich", "jamón y queso");

                // Números para cantidades
                Choices cantidades = new Choices();
                cantidades.Add("uno", "una", "un", "1");
                cantidades.Add("dos", "2");
                cantidades.Add("tres", "3");
                cantidades.Add("cuatro", "4");
                cantidades.Add("cinco", "5");

                // Palabras de conexión
                Choices conectores = new Choices();
                conectores.Add("quiero", "quisiera", "dame", "pido", "quería");
                conectores.Add("y", "con", "más");

                // Construir gramática: [conector] [cantidad] [producto]
                GrammarBuilder gb = new GrammarBuilder();
                gb.Culture = new System.Globalization.CultureInfo("es-AR");
                
                // Patrón flexible: permite varias formas de pedir
                gb.AppendWildcard(); // Permite palabras extra al inicio
                gb.Append(new Choices(menuItems, cantidades)); // Acepta productos o cantidades
                gb.AppendWildcard(); // Permite palabras extra al final

                Grammar grammar = new Grammar(gb);
                _recognizer.LoadGrammar(grammar);

                // También cargar gramática de dictado para mayor flexibilidad
                _recognizer.LoadGrammar(new DictationGrammar());

                // Configurar eventos
                _recognizer.SpeechRecognized += OnSpeechRecognized;
                _recognizer.SpeechRecognitionRejected += OnSpeechRejected;

                // Configurar entrada de audio (micrófono por defecto)
                _recognizer.SetInputToDefaultAudioDevice();

                // Iniciar reconocimiento asíncrono continuo
                _recognizer.RecognizeAsync(RecognizeMode.Multiple);

                IsListening = true;
                System.Diagnostics.Debug.WriteLine("[Speech] Reconocimiento de voz iniciado");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Speech Error] {ex.Message}");
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
        /// Manejador cuando se reconoce voz exitosamente
        /// </summary>
        private void OnSpeechRecognized(object? sender, SpeechRecognizedEventArgs e)
        {
            // Solo aceptar si la confianza es alta (>70%)
            if (e.Result.Confidence > 0.7f)
            {
                var texto = e.Result.Text;
                System.Diagnostics.Debug.WriteLine($"[Speech] Reconocido ({e.Result.Confidence:P0}): {texto}");
                
                // Disparar evento en el hilo principal
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    SpeechRecognized?.Invoke(this, texto);
                });
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"[Speech] Baja confianza ({e.Result.Confidence:P0}): {e.Result.Text}");
            }
        }

        /// <summary>
        /// Manejador cuando se rechaza el reconocimiento
        /// </summary>
        private void OnSpeechRejected(object? sender, SpeechRecognitionRejectedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("[Speech] Reconocimiento rechazado - no se entendió claramente");
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
                if (_recognizer != null)
                {
                    _recognizer.RecognizeAsyncStop();
                    _recognizer.Dispose();
                    _recognizer = null;
                }
                
                IsListening = false;
                System.Diagnostics.Debug.WriteLine("[Speech] Reconocimiento de voz detenido");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Speech Error] Error al detener: {ex.Message}");
            }
#endif
            await Task.CompletedTask;
        }
    }
}

